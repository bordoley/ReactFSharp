namespace React

open FSharp.Control.Reactive
open ImmutableCollections
open System
open System.Reactive.Linq
open System.Reactive.Disposables
open System.Reactive.Concurrency

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
  let private dispose (view: ReactView) = (view :> IDisposable).Dispose()

  let rec bindToNativeViewContainer (update: Option<obj> -> unit) (reactView: ReactView): IDisposable =
    match reactView with
    | ReactStatefulView statefulView ->
        let reducer (subscription: IDisposable) (reactView: ReactView) =
          subscription.Dispose()
          reactView |> bindToNativeViewContainer update

        let subscription =
          statefulView.State
          |> Observable.scanInit Disposable.Empty reducer 
          |> Observable.last
          |> Observable.subscribe (fun view -> view.Dispose())

        subscription
    | ReactView view ->
        Some (view.View) |> update
        Disposable.Empty
    | ReactViewGroup view ->
        Some (view.View) |> update
        Disposable.Empty
    | ReactViewNone ->
        None |> update
        Disposable.Empty

  let updateChildren
       (setViewAtIndex: int -> Option<obj> -> unit)
       (removeViewAtIndex: int -> unit)
       (oldChildren: IImmutableMap<string, IDisposable>)
       (newChildren: IImmutableMap<string, ReactView>): IImmutableMap<string, IDisposable> =

    let updateSubscription (index: int) = function
      | ((prevKey, _) as prev, (nextKey, _)) when prevKey = nextKey ->
          prev
      | ((_, prevSubscription: IDisposable), (nextKey, nextView))->
          prevSubscription.Dispose ()
          (nextKey, nextView |> bindToNativeViewContainer (setViewAtIndex index))

    let updateAndSubscribe (index: int) = function
      | (Some prev, Some next) ->
          removeViewAtIndex index
          (prev, next) |> updateSubscription index
      | (None, Some (key, view)) ->
          (key, view |> bindToNativeViewContainer (setViewAtIndex index))
      | _ -> failwith "this can never happen"

    let result =
      if oldChildren.Count >= newChildren.Count then
        // Remove children at the tail
        for i = newChildren.Count to oldChildren.Count - 1
          do removeViewAtIndex i

        Seq.zip oldChildren newChildren
        |> Seq.mapi updateSubscription

      else
        Seq.zipAll oldChildren newChildren
        |> Seq.mapi updateAndSubscribe

    ImmutableMap.create result

  let render
      (scheduler: IScheduler)
      (createView: string -> obj -> ReactView)
      (element: ReactElement) =
    let rec updateWith (dom: ReactDOMNode) (view: ReactView) =
      match (dom, view) with
      | (ReactNoneDOMNode, _) ->
          ReactViewNone

      | (ReactStatelessDOMNode node, _) ->
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
          when node.element.Name = reactView.Name ->
            reactView.Props <- node.element.Props
            view

      | (ReactNativeDOMNodeGroup node, ReactViewGroup viewWithChildren)
          when node.element.Name = viewWithChildren.Name ->
            let children =
              node.children 
              |> ImmutableMap.map (
                fun key node ->
                  updateWith node
                   <| match viewWithChildren.Children |> ImmutableMap.tryGet key with
                      | Some childView -> childView
                      | None -> ReactViewNone
                ) 
              |> ImmutableMap.create

            let oldChildren = viewWithChildren.Children

            // Update the props after adding the children. On android this is needed
            // to support the BaselineAlignedChildIndex property
            viewWithChildren.Children <- children
            viewWithChildren.Props <- node.element.Props

            for (name, view) in oldChildren do
              match children |> ImmutableMap.tryGet name with
              | Some _ -> ()
              | None -> dispose view

            view

      | (node, view) ->
          view |> dispose

          let (name, props) =
            match node with
            | ReactNativeDOMNode node -> (node.element.Name, node.element.Props)
            | ReactNativeDOMNodeGroup node -> (node.element.Name, node.element.Props)
            | _ -> failwith "node must be a ReactNativeDomNode ReactNativeDOMNodeGroup"

          let view = createView name props

          match (node, view) with
          | (ReactNativeDOMNode node, ReactView _) -> ()
          | (ReactNativeDOMNodeGroup node, ReactViewGroup view) ->
              view.Children <-
                node.children 
                |> ImmutableMap.map (fun _ node -> ReactViewNone |> updateWith node) 
                |> ImmutableMap.create
          | _ -> failwith "node/view mismatch"

          view

    let dom = ReactDom.render element
    updateWith dom ReactViewNone

