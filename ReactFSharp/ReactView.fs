namespace React

open FSharp.Control.Reactive
open ImmutableCollections
open System
open System.Reactive.Concurrency
open System.Reactive.Disposables
open System.Reactive.Linq
open System.Reactive.Subjects
open System.Runtime.CompilerServices

type [<ReferenceEquality>] ReactView<'view> =
  | ReactStatefulView of IReactStatefulView<'view>
  | ReactNativeView of IReactNativeView<'view>
  | ReactViewNone

  interface IDisposable with
    member this.Dispose() =
      match this with
      | ReactNativeView view -> view.Dispose()
      | ReactStatefulView view -> view.Dispose()
      | ReactViewNone -> ()

and IReactStatefulView<'view> =
  inherit IDisposable

  abstract member Id: obj
  abstract member State: IObservable<ReactView<'view>>

and IReactNativeView<'view> =
  inherit IDisposable

  abstract member Name: string with get
  abstract member Props: obj with get, set
  abstract member View: 'view
  abstract member Children: IImmutableMap<string, ReactDOMNode> with get, set

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ReactView =
  let private dispose (disposable: IDisposable) = disposable.Dispose()

  let rec private observe<'view> (reactView: ReactView<'view>): IObservable<Option<'view>> =
    match reactView with
    | ReactStatefulView statefulView ->
        let mapper (reactView: ReactView<'view>)=
          observe reactView

        statefulView.State
        |> Observable.flatmap mapper
    | ReactNativeView view ->
        Some view.View |> Observable.single
    | ReactViewNone ->
        None |> Observable.single

  let render<'view when 'view :> IDisposable>
      (scheduler: IScheduler)
      (createNativeView:
        (Exception -> unit) (* onError *) ->
        (ReactDOMNode -> ReactView<'view> -> ReactView<'view>) (* updateWith *) ->
        string (* view name *) ->
        obj (* initialProps *) ->
        IReactNativeView<'view>
      )
      (element: ReactElement) =

    let rec updateWith (onError: Exception -> unit) (dom: ReactDOMNode) (view: ReactView<'view>) =
      let updateWith = updateWith onError
      let createNativeView = createNativeView onError updateWith

      match (dom, view) with
      | (ReactNoneDOMNode, _) ->
          ReactViewNone

      | (ReactLazyDOMNode node, _) ->
          view |> updateWith node.child

      | (ReactStatefulDOMNode node, ReactStatefulView statefulView)
            when node.id = statefulView.Id ->
          view

      | (ReactStatefulDOMNode node, _) ->
          view |> dispose

          let id = node.id

          let state =
            node.state
            |> Observable.observeOn scheduler
            |> Observable.scanInit
                ReactViewNone
                (fun view dom -> view |> updateWith dom)
            |> Observable.distinctUntilChangedCompare EqualityComparer.referenceEquality
            |> Observable.replayBuffer 1

          let subscription = state.Connect ()

          ReactStatefulView {
            new IReactStatefulView<'view> with
              member this.Dispose () = subscription.Dispose ()
              member this.Id = id
              member this.State = state :> IObservable<ReactView<'view>>
          }

      | (ReactNativeDOMNode node, ReactNativeView reactView)
            when node.element.Name = reactView.Name
              && node.element.Props = reactView.Props
              && node.children = reactView.Children ->
          view

      | (ReactNativeDOMNode node, ReactNativeView reactView)
            when node.element.Name = reactView.Name
              && node.children = reactView.Children ->
          reactView.Props <- node.element.Props
          view

      | (ReactNativeDOMNode node, ReactNativeView reactView)
            when node.element.Name = reactView.Name ->
          reactView.Props <- node.element.Props
          reactView.Children <- node.children
          view

      | (ReactNativeDOMNode node, _) ->
          view |> dispose
          let view = createNativeView node.element.Name node.element.Props
          view.Children <- node.children
          ReactNativeView view

    let subscribe (observer: IObserver<Option<'view>>) =
      let dom = ReactDom.render element

      updateWith observer.OnError dom ReactViewNone
      |> observe<'view>
      |> Observable.subscribeObserver observer

    Observable.Create(subscribe)

  let createView<'view, 'props>
      (name: string)
      (viewProvider: unit -> 'view)
      (setProps: 'view -> 'props -> IDisposable)
      (setChildren: 'view -> IImmutableMap<string, ReactDOMNode> -> IDisposable)
      (initialProps: obj) : IReactNativeView<'view> =

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
      new IReactNativeView<'view> with
        member this.Dispose () =
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
    createView name viewProvider setProps (fun _ _ -> Disposable.Empty) initialProps

  let createViewImmediatelyRenderingAllChildren<'view, 'props when 'view :not struct>
      (onError: Exception -> unit)
      (viewChildrenCache: ConditionalWeakTable<'view, IImmutableMap<string, ReactView<'view>>>)
      (updateViewWith: ReactDOMNode -> ReactView<'view> -> ReactView<'view>)
      (removeAllViews: 'view -> unit)
      (setViewAtIndex: 'view -> int -> Option<'view> -> unit)
      (name: string)
      (viewProvider: unit -> 'view)
      (setProps: 'view -> 'props -> IDisposable)
      (initialProps: obj) =

    let setChildren (view: 'view) (domNodeChildren: IImmutableMap<string, ReactDOMNode>) =
      let currentViewChildren =
        viewChildrenCache.GetValue(view, fun view -> ImmutableMap.empty ())

      let updateView (key, domNode) =
        let view =
          match currentViewChildren |> ImmutableMap.tryGet key with
          | Some view -> view |> updateViewWith domNode
          | None -> ReactViewNone |> updateViewWith domNode
        (key, view)

      let newViewChildren =
        domNodeChildren
        |> Seq.map updateView
        |> ImmutableMap.create

      viewChildrenCache.Remove (view) |> ignore
      viewChildrenCache.Add (view, newViewChildren)
      view |> removeAllViews

      let observeViewAtIndex index reactView =
        observe reactView
        |> Observable.iter (fun viewAtIndex ->
            setViewAtIndex view  index viewAtIndex)
        |> Observable.subscribeWithError (fun _ -> ()) onError

      newViewChildren
      |> ImmutableMap.values
      |> Seq.mapi observeViewAtIndex
      |> Seq.toArray
      |> Disposables.compose

    createView name viewProvider setProps setChildren initialProps