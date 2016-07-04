namespace React

open ImmutableCollections
open System
open System.Reactive.Linq
open System.Reactive.Disposables
open System.Reactive.Concurrency

module FSXObservable = FSharp.Control.Reactive.Observable

type [<ReferenceEquality>] ReactView = 
  | ReactStatefulView of IReactStatefulView
  | ReactView of IReactView
  | ReactViewGroup of IReactViewGroup
  | ReactViewNone

and IReactStatefulView = 
  inherit IDisposable

  abstract member Id: obj

and IReactView =
  inherit IDisposable

  abstract member Name: string with get
  abstract member Props: obj with get, set

and IReactViewGroup =
  inherit IDisposable

  abstract member Name: string with get
  abstract member Props: obj with get, set
  abstract member Children: IImmutableMap<string, ReactView> with get, set

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ReactView =
  type ViewProvider = {
    createView: string -> obj -> ReactView
    createStatefulView: (obj * IObservable<ReactView>) -> ReactView
  }

  let dispose = function
    | ReactView view -> view.Dispose()
    | ReactStatefulView view -> view.Dispose()
    | ReactViewGroup view -> view.Dispose()
    | ReactViewNone -> ()
 
  let render
      (scheduler: IScheduler)
      (viewProvider: ViewProvider) 
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
            |> FSXObservable.observeOn scheduler
            |> Observable.scan 
                (fun view dom -> 
                  view |> updateWith dom) 
                ReactViewNone
            |> FSXObservable.distinctUntilChanged

          viewProvider.createStatefulView (id, state)

      | (ReactNativeDOMNode node, ReactView reactView) 
          when node.element.name = reactView.Name ->
            reactView.Props <- node.element.props
            view

      | (ReactNativeDOMNodeGroup node, ReactViewGroup viewWithChildren) 
          when node.element.name = viewWithChildren.Name ->
            let children =
              node.children |> ImmutableMap.map (
                fun key node -> 
                  updateWith node 
                   <| match viewWithChildren.Children |> ImmutableMap.tryGet key with
                      | Some childView -> childView
                      | None -> ReactViewNone
              ) |> ImmutableMap.create
          
            let oldChildren = viewWithChildren.Children
           
            // Update the props after adding the children. On android this is needed
            // to support the BaselineAlignedChildIndex property
            viewWithChildren.Children <- children
            viewWithChildren.Props <- node.element.props

            for (name, view) in oldChildren do
              match children |> ImmutableMap.tryGet name with
              | Some _ -> ()
              | None -> dispose view

            view

      | (node, view) -> 
          view |> dispose

          let (name, props) =
            match node with
            | ReactNativeDOMNode node -> (node.element.name, node.element.props)
            | ReactNativeDOMNodeGroup node -> (node.element.name, node.element.props)
            | _ -> failwith "node must be a ReactNativeDomNode ReactNativeDOMNodeGroup"

          let view = viewProvider.createView name props

          do 
            match (node, view) with
            | (ReactNativeDOMNode node, ReactView _) -> ()
            | (ReactNativeDOMNodeGroup node, ReactViewGroup view) ->
                let children =
                  node.children |> ImmutableMap.map (
                    fun key node -> ReactViewNone |> updateWith node
                  ) |> ImmutableMap.create
                view.Children <- children
            | _ -> failwith "node/view mismatch"

          view

    let dom = ReactDom.render element 
    updateWith dom ReactViewNone

