namespace React

open FSharp.Control.Reactive
open ImmutableCollections
open System
open System.Collections.Generic
open System.Reactive.Concurrency
open System.Reactive.Disposables
open System.Reactive.Linq
open System.Reactive.Subjects
open System.Runtime.CompilerServices

type IReactView<'view when 'view :> IDisposable> =
  inherit IDisposable

  abstract member Name: string with get
  abstract member Props: obj with get, set
  abstract member View: 'view
  abstract member Children: IImmutableMap<int, ReactDOMNode> with get, set

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ReactView =
  let private dispose (disposable: IDisposable) = disposable.Dispose()

  let updateNativeView
      (createNativeView: string (* view name *) -> obj (* initialProps *) -> IReactView<'view>)
      (view: Option<IReactView<'view>>)
      (dom: Option<ReactNativeDOMNode>): Option<IReactView<'view>> =
    match (view, dom) with
    | (Some reactView, Some dom)
          when dom.element.Name = reactView.Name ->
        if dom.element.Props <> reactView.Props then
          reactView.Props <- dom.element.Props
        if dom.children <> reactView.Children then 
          reactView.Children <- dom.children

        view
    | (_, Some dom) ->
        view |> Option.iter dispose

        let newReactView = createNativeView dom.element.Name dom.element.Props
        newReactView.Children <- dom.children
        Some newReactView
    | _ ->
        view |> Option.iter dispose
        None
  
  let render<'view when 'view :> IDisposable>
      (scheduler: IScheduler)
      (createNativeView:
        (Exception -> unit) (* onError *) ->
        (string (* view name *) -> obj (* initialProps *) -> IReactView<'view>) ->
        string (* view name *) ->
        obj (* initialProps *) ->
        IReactView<'view>
      )
      (element: ReactElement): IObservable<Option<'view>>=

    let curryCreateNativeView onError =
      let createNativeViewRef =
        let dummy (viewName: string) (initialProps: obj): IReactView<'view> = failwith "oops"
        ref dummy

      let createNativeView (viewName: string) (initialProps: obj) =
          createNativeView onError (!createNativeViewRef) viewName initialProps

      createNativeViewRef := createNativeView
      createNativeView

    let subscribe (observer: IObserver<Option<'view>>) =
      let createNativeView = curryCreateNativeView observer.OnError
      let updateNativeView = updateNativeView createNativeView

      ReactDom.render element
      |> ReactDom.observe
      |> Observable.observeOn scheduler
      |> Observable.scanInit None updateNativeView
      |> Observable.map (Option.map (fun x -> x.View))
      |> Observable.subscribeObserver observer

    Observable.Create(subscribe)

  let createView<'view, 'props when 'view :> IDisposable>
      (name: string)
      (view: 'view)
      (setProps: 'props -> IDisposable)
      (setChildren: IImmutableMap<int, ReactDOMNode> -> unit)
      (onDispose: unit -> unit)
      (initialProps: obj) : IReactView<'view> =

    let initialProps = (initialProps :?> 'props)

    let propsSubject = new BehaviorSubject<'props>(initialProps)
    let childrenSubject = new BehaviorSubject<IImmutableMap<int, ReactDOMNode>>(ImmutableMap.empty ())
    let errors = new BehaviorSubject<Option<Exception>>(None)

    let propsSubscription =
      propsSubject
      |> Observable.distinctUntilChanged
      |> Observable.map setProps
      |> Observable.scanInit
          Disposable.Empty
          (fun acc next -> acc |> dispose; next)
      |> Observable.last
      |> Observable.subscribeWithError dispose (Some >> errors.OnNext)

    let throwIfErrors () =
      match errors.Value with
      | Some exn -> raise (AggregateException exn)
      | _ -> ()

    {
      new IReactView<'view> with
        member this.Dispose () =
          onDispose ()
          propsSubject.OnCompleted ()
          propsSubscription.Dispose ()
          view.Dispose ()
        member this.Name = name
        member this.Props
          with get () = propsSubject.Value :> obj
           and set props = 
             propsSubject.OnNext (props :?> 'props)
             throwIfErrors ()
        member this.View = view
        member this.Children
          with get () = childrenSubject.Value
           and set value =
             setChildren value
    }

  let createViewWithoutChildren<'view, 'props when 'view :> IDisposable>
      (name: string)
      (viewProvider: unit -> 'view)
      (setProps: 'view -> 'props -> IDisposable)
      (initialProps: obj) =
    let view = viewProvider ()
    createView name view (setProps view) ignore ignore initialProps

  let createViewImmediatelyRenderingAllChildren<'view, 'props when 'view :> IDisposable>
      (scheduler: IScheduler)
      (onError: Exception -> unit)
      (createNativeView: string (* view name *) -> obj (* initialProps *) -> IReactView<'view>)
      (removeAllViews: 'view -> unit)
      (addViews: 'view (* parent *) -> seq<'view> (* children *)-> unit)
      (name: string)
      (viewProvider: unit -> 'view)
      (setProps: 'view -> 'props -> IDisposable)
      (initialProps: obj) =

    let view = viewProvider ()
    let updateNativeView = updateNativeView createNativeView

    let reactViewCache: IPersistentMap<int, IReactView<'view>> ref = ref (PersistentMap.empty ())

    let cleanseCache (currentKeys: IImmutableSet<int>) =
      let currentViewCache = reactViewCache.contents
      let viewCacheWithoutRemovedKeys =
        currentViewCache
        |> Seq.fold
            (fun (acc: ITransientMap<int, IReactView<'view>>) (key, reactView) ->
              if not (currentKeys |> ImmutableSet.contains key) then
                reactView.Dispose ()
                acc.Remove key
              else acc
            )
            (currentViewCache.Mutate ())
        |> TransientMap.persist
      reactViewCache := viewCacheWithoutRemovedKeys

    let addToCache (key: int) (view: IReactView<'view>) =
      reactViewCache := reactViewCache.contents |> PersistentMap.put key view

    let setChildrenSubject = new Subject<IImmutableMap<int, ReactDOMNode>>()
    let setChildrenSubscription =
      setChildrenSubject
      |> Observable.flatmap (
          Seq.map (
            fun (key, dom) ->
              ReactDom.observe dom |> Observable.map (fun nativeDomNode -> (key, nativeDomNode))
          ) >> Observable.combineLatestSeq
        )
      |> Observable.observeOn scheduler
      |> Observable.map (
          Seq.map (
            fun (key, dom) ->
              let currentReactView = reactViewCache.contents |> ImmutableMap.tryGet key
              let newReactView = updateNativeView currentReactView dom

              match newReactView with
              | Some reactView -> addToCache key reactView
              | None -> ()

              (key, newReactView)
          )
          >> Seq.filter (fun (_, dom) -> dom |> Option.isSome)
          >> Seq.map (fun (key, dom) -> (key,  Option.get dom))
          >> ImmutableMap.create
        )
      |> Observable.distinctUntilChanged
      |> Observable.iter (ImmutableMap.keySet >> cleanseCache)
      |> Observable.map (ImmutableMap.values >> Seq.map (fun reactView -> reactView.View))
      |> Observable.iter (
          fun childViews ->
            removeAllViews view
            addViews view childViews
          )
      |> Observable.subscribeWithError ignore onError

    let onDispose () =
      setChildrenSubject.OnCompleted ()
      setChildrenSubscription.Dispose ()
      reactViewCache.contents
      |> ImmutableMap.values
      |> Seq.iter dispose

    createView name view (setProps view) setChildrenSubject.OnNext onDispose initialProps
