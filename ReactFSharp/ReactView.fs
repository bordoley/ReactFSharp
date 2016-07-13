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

type IReactView<'view> =
  inherit IDisposable

  abstract member Name: string with get
  abstract member Props: obj with get, set
  abstract member View: 'view
  abstract member Children: IImmutableMap<string, ReactDOMNode> with get, set

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
        if dom.element.Props = reactView.Props then 
          if dom.children = reactView.Children then ()
          else reactView.Children <- dom.children
        else reactView.Props <- dom.element.Props
        view
    | (_, Some dom) ->
        view |> Option.iter dispose

        let newReactView = createNativeView dom.element.Name dom.element.Props
        newReactView.Children <- dom.children
        Some newReactView
    | _ ->
        view |> Option.iter dispose
        None

  let render<'view>
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

  let createView<'view, 'props>
      (name: string)
      (viewProvider: unit -> 'view)
      (setProps: 'view -> 'props -> IDisposable)
      (setChildren: 'view -> IImmutableMap<string, ReactDOMNode> -> IDisposable)
      (onDispose: unit -> unit)
      (initialProps: obj) : IReactView<'view> =

    let initialProps = (initialProps :?> 'props)
    let view = viewProvider ()

    let propsSubject = new BehaviorSubject<'props>(initialProps)
    let childrenSubject = new BehaviorSubject<IImmutableMap<string, ReactDOMNode>>(ImmutableMap.empty ())
    let errors = new BehaviorSubject<Option<Exception>>(None)

    let propsSubscription =
      propsSubject
      |> Observable.distinctUntilChanged
      |> Observable.map (setProps view)
      |> Observable.scanInit
          Disposable.Empty
          (fun acc next -> acc |> dispose; next)
      |> Observable.iterError
          (fun _ -> ())
          (Some >> errors.OnNext)
      |> Observable.last
      |> Observable.subscribe dispose

    let childrenSubscription =
      childrenSubject
      |> Observable.distinctUntilChanged
      |> Observable.map (setChildren view)
      |> Observable.scanInit
          Disposable.Empty
          (fun acc next -> acc |> dispose; next)
      |> Observable.iterError
          (fun _ -> ())
          (Some >> errors.OnNext)
      |> Observable.last
      |> Observable.subscribe dispose

    let throwIfErrors () =
      match errors.Value with
      | Some exn -> raise (AggregateException exn)
      | _ -> ()

    {
      new IReactView<'view> with
        member this.Dispose () =
          onDispose ()
          propsSubject.OnCompleted ()
          childrenSubject.OnCompleted ()
          propsSubscription.Dispose ()
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
             childrenSubject.OnNext value
             throwIfErrors ()
    }

  let createViewWithoutChildren<'view, 'props>
      (name: string)
      (viewProvider: unit -> 'view)
      (setProps: 'view -> 'props -> IDisposable)
      (initialProps: obj) =
    createView name viewProvider setProps (fun _ _ -> Disposable.Empty) (fun () -> ()) initialProps

  let createViewImmediatelyRenderingAllChildren<'view, 'props when 'view :not struct>
      (scheduler: IScheduler)
      (onError: Exception -> unit)
      (createNativeView: string (* view name *) -> obj (* initialProps *) -> IReactView<'view>)
      (removeAllViews: 'view -> unit)
      (addViews: 'view (* parent *) -> seq<'view> (* children *)-> unit)
      (name: string)
      (viewProvider: unit -> 'view)
      (setProps: 'view -> 'props -> IDisposable)
      (initialProps: obj) =

    let updateNativeView = updateNativeView createNativeView

    let reactViewCache: IPersistentMap<string, IReactView<'view>> ref = ref (PersistentMap.empty ())

    let cleanseCache (currentKeys: IImmutableSet<string>) =
      let currentViewCache = reactViewCache.contents
      let viewCacheWithoutRemovedKeys =
        currentViewCache
        |> Seq.fold
            (fun (acc: ITransientMap<string, IReactView<'view>>) (key, reactView) -> 
              if not (currentKeys |> ImmutableSet.contains key) then
                reactView.Dispose ()
                acc.Remove key 
              else acc
            )
            (currentViewCache.Mutate ())
        |> TransientMap.persist
      reactViewCache := viewCacheWithoutRemovedKeys
    
    let updateCache (key: string) (view: IReactView<'view>) =
      reactViewCache := reactViewCache.contents |> PersistentMap.put key view

    let setChildren (view: 'view) (domNodeChildren: IImmutableMap<string, ReactDOMNode>) =
      domNodeChildren
      |> Seq.map (
          fun (key, dom) -> 
            ReactDom.observe dom |> Observable.map (fun nativeDomNode -> (key, nativeDomNode))
        )
      |> Observable.combineLatestSeq
      |> Observable.observeOn scheduler
      |> Observable.map (
          Seq.map (
            fun (key, dom) ->
              let currentReactView = reactViewCache.contents |> ImmutableMap.tryGet key
              let newReactView = updateNativeView currentReactView dom

              match newReactView with
              | Some reactView -> updateCache key reactView
              | None -> ()

              (key, newReactView)
          )
          >> Seq.filter (fun (_, dom) -> dom |> Option.isSome)
          >> Seq.map (fun (key, dom) -> (key,  Option.get dom))
          >> ImmutableMap.create
        )
      |> Observable.iter (ImmutableMap.keySet >> cleanseCache)
      |> Observable.map (ImmutableMap.values >> Seq.map (fun reactView -> reactView.View))
      |> Observable.map ImmutableVector.create
      |> Observable.distinctUntilChanged
      |> Observable.iter (
          fun childViews -> 
            removeAllViews view
            addViews view (childViews |> ImmutableMap.values)
          )
      |> Observable.subscribeWithError (fun _ -> ()) onError

    let onDispose () =
      reactViewCache.contents
      |> ImmutableMap.values
      |> Seq.iter (fun view -> view.Dispose ())

    createView name viewProvider setProps setChildren onDispose initialProps