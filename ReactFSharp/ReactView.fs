namespace React

open FSharp.Control.Reactive
open ImmutableCollections
open System
open System.Reactive.Concurrency
open System.Reactive.Disposables
open System.Reactive.Linq
open System.Reactive.Subjects

type [<ReferenceEquality>] ReactView =
  | ReactStatefulView of IReactStatefulView
  | ReactView of IReactView
  | ReactViewGroup of IReactViewGroup
  | ReactViewNone

  interface IDisposable with
    member this.Dispose() =
      match this with
      | ReactView view -> view.Dispose()
      | ReactStatefulView view -> view.Dispose()
      | ReactViewGroup view -> view.Dispose()
      | ReactViewNone -> ()

and IReactStatefulView =
  inherit IDisposable

  abstract member Id: obj
  abstract member State: IObservable<ReactView>

and IReactView =
  inherit IDisposable

  abstract member Name: string with get
  abstract member Props: obj with get, set
  abstract member View: obj

and IReactViewGroup =
  inherit IReactView

  abstract member Children: IImmutableMap<string, ReactView> with get, set

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ReactView =
  open FSharp.Control.Reactive.Observable

  let private dispose (disposable: IDisposable) = disposable.Dispose()

  let rec private observe<'view when 'view :> IDisposable> (reactView: ReactView): IObservable<Option<'view>> =
    match reactView with
    | ReactStatefulView statefulView ->
        let mapper (reactView: ReactView)=
          observe reactView

        statefulView.State
        |> Observable.flatmap mapper
    | ReactView view ->
        Some (view.View :?> 'view) |> Observable.single
    | ReactViewGroup view ->
        Some (view.View :?> 'view) |> Observable.single
    | ReactViewNone ->
        None |> Observable.single

  let private createViewInternal<'view, 'props when 'view :> IDisposable>
      (name: string)
      (viewProvider: unit -> 'view)
      (setProps: 'view -> 'props -> IDisposable) =
    let createReactView (initialProps: obj) =
      let initialProps = (initialProps :?> 'props)
      let view = viewProvider ()

      let propsSubject = new BehaviorSubject<'props>(initialProps)
      let onException = new BehaviorSubject<Option<Exception>>(None)

      let propsUpdaterSubscription =
        propsSubject
        |> Observable.map (setProps view)
        |> Observable.scanInit
            Disposable.Empty
            (fun acc next -> acc |> dispose; next)
        |> Observable.iterError
            (fun _ -> ())
            (Some >> onException.OnNext)
        |> Observable.last
        |> Observable.subscribe dispose

      let assertValidState () =
        match onException.Value with
        | Some exn -> raise (AggregateException exn)
        | _ -> ()

      { new IReactView with
          member this.Dispose () =
            propsSubject.OnCompleted ()
            onException.OnCompleted()
            propsUpdaterSubscription.Dispose()
            view.Dispose()
          member this.Name = name
          member this.Props
            with get () =
              propsSubject.Value :> obj
            and set props =
              let props = (props :?> 'props)
              propsSubject.OnNext props
              assertValidState ()

          member this.View = view :> obj
      }
    createReactView

  let createView<'view, 'props when 'view :> IDisposable>
      (name: string)
      (viewProvider: unit -> 'view)
      (setProps: 'view -> 'props -> IDisposable) =
    let createReactView (initialProps: obj) =
      ReactView <| createViewInternal name viewProvider setProps initialProps
    createReactView

  let createViewGroup<'view, 'viewGroup, 'props when 'view :> IDisposable and 'viewGroup :> IDisposable>
      (onError: Exception -> unit)
      (name: string)
      (viewGroupProvider: unit -> 'viewGroup)
      (setProps: 'viewGroup -> 'props -> IDisposable)
      (setViewAtIndex: 'viewGroup -> int -> Option<'view> -> unit)
      (removeViewAtIndex: 'viewGroup -> int -> unit) =

    let createReactViewGroup (initialProps: obj) =
      let reactView = createViewInternal name viewGroupProvider setProps initialProps
      let viewGroup = reactView.View :?> 'viewGroup

      let setViewAtIndex = setViewAtIndex viewGroup
      let removeViewAtIndex = removeViewAtIndex viewGroup

      let childrenSubject = new BehaviorSubject<IImmutableMap<string, ReactView>>(ImmutableMap.empty ())
      let onException = new BehaviorSubject<Option<Exception>>(None)

      let updateChildren (acc: IImmutableMap<ReactView, IDisposable>) (newMap: IImmutableMap<string, ReactView>) =
        // Remove children at the tail
        if acc.Count > newMap.Count then
          for i = (acc.Count - 1) downto (newMap.Count - 1)
            do removeViewAtIndex i

        let subscriber index view =
          match acc |> ImmutableMap.tryGet view with
          | Some disposable -> (view, disposable)
          | None ->
              let subscription =
                view |> observe |> Observable.subscribeWithError (setViewAtIndex index) onError
              (view, subscription)

        let newAcc = newMap |> ImmutableMap.values |> Seq.mapi subscriber |> ImmutableMap.create

        let viewDisposer (reactView, subscription) =
          if newAcc |> ImmutableMap.containsKey reactView |> not then
            dispose subscription

        acc |> Seq.iter viewDisposer

        newAcc

      let childrenUpdaterSubscription =
        childrenSubject
        |> Observable.fold updateChildren (ImmutableMap.empty ())
        |> Observable.subscribeWithError
            (ImmutableMap.values >> Seq.iter dispose)
            (Some >> onException.OnNext)

      let assertValidState () =
        match onException.Value with
        | Some exn -> raise exn
        | _ -> ()

      ReactViewGroup {
        new IReactViewGroup with
          member this.Children
            with get() = childrenSubject.Value

             and set children =
               childrenSubject.OnNext children
               assertValidState ()

          member this.Dispose () =
            childrenSubject.OnCompleted()
            onException.OnCompleted()
            childrenUpdaterSubscription.Dispose ()
            reactView.Dispose ()

          member this.Name = name

          member this.Props
            with get () = reactView.Props
             and set props = reactView.Props <- props

          member this.View = reactView.View
      }

    createReactViewGroup

  let render<'view when 'view :> IDisposable>
      (scheduler: IScheduler)
      (createView: (Exception -> unit) -> string (* view name *) -> obj (* props *) -> ReactView)
      (element: ReactElement) =

    let rec updateWith (onError: Exception -> unit) (dom: ReactDOMNode) (view: ReactView) =
      let updateWith = updateWith onError
      let createView = createView onError

      match (dom, view) with
      | (ReactNoneDOMNode, _) ->
          ReactViewNone

      | (ReactLazyDOMNode node, _) ->
          view |> updateWith node.child

      | (ReactStatefulDOMNode node, ReactStatefulView statefulView)
          when node.id = statefulView.Id -> view

      | (ReactStatefulDOMNode node, _) ->
          view |> dispose

          let id = node.id

          let state =
            node.state
            |> Observable.observeOn scheduler
            |> Observable.scanInit
                ReactViewNone
                (fun view dom -> view |> updateWith dom)
            |> Observable.distinctUntilChanged
            |> Observable.replayBuffer 1

          let subscription = state.Connect ()

          ReactStatefulView {
            new IReactStatefulView with
              member this.Dispose () = subscription.Dispose ()
              member this.Id = id
              member this.State = state :> IObservable<ReactView>
          }

      | (ReactNativeDOMNode node, ReactView reactView)
            when node.Name = reactView.Name
              && node.Props = reactView.Props ->
          view

      | (ReactNativeDOMNode node, ReactView reactView)
            when node.Name = reactView.Name ->
          reactView.Props <- node.Props
          view

      | (ReactNativeDOMNode node, _) ->
          view |> dispose
          createView node.Name node.Props

      // FIXME: Right now we don't have a data model that allows using a quick comparison
      // to assert the children haven't changed.
      | (ReactNativeDOMNodeGroup node, ReactViewGroup viewGroup)
            when node.element.Name = viewGroup.Name ->
          let oldChildren = viewGroup.Children

          let newChildren =
            node.children
            |> ImmutableMap.map (
              fun key node ->
                let currentChildViewForKey =
                  match viewGroup.Children |> ImmutableMap.tryGet key with
                  | Some childView -> childView
                  | None -> ReactViewNone

                currentChildViewForKey |> updateWith node
              )
            |> ImmutableMap.create

          // Update the props after adding the children. On android this is needed
          // to support the BaselineAlignedChildIndex property
          viewGroup.Children <- newChildren

          if node.element.Props <> viewGroup.Props then
            viewGroup.Props <- node.element.Props

          for (name, view) in oldChildren do
            match newChildren |> ImmutableMap.tryGet name with
            | Some _ -> ()
            | None -> dispose view

          view
      | (ReactNativeDOMNodeGroup node, _) ->
          view |> dispose
          let view = createView node.element.Name node.element.Props
          let viewGroup =
            match view with
            | ReactViewGroup viewGroup -> viewGroup
            | _ -> failwith "view is not a viewGroup"
          viewGroup.Children <-
            node.children
            |> ImmutableMap.map (fun _ node -> ReactViewNone |> updateWith node)
            |> ImmutableMap.create
          view

    let subscribe (observer: IObserver<Option<'view>>) =
      let dom = ReactDom.render element

      updateWith observer.OnError dom ReactViewNone
      |> observe<'view>
      |> Observable.subscribeObserver observer

    Observable.Create(subscribe)

