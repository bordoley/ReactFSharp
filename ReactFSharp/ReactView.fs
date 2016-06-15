namespace React

open React
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

  abstract member Id: int with get

and IReactView =
  inherit IDisposable

  abstract member Name: string with get
  abstract member UpdateProps: obj -> unit

and IReactViewGroup =
  inherit IDisposable

  abstract member Name: string with get
  abstract member UpdateProps: obj -> unit
  abstract member Children: ReactChildren<ReactView> with get, set

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ReactView =
  type ViewProvider = {
    createView: string -> obj -> ReactView
    createStatefulView: (int * IObservable<ReactView>) -> ReactView
  }

  let dispose = function
    | ReactView view -> view.Dispose()
    | ReactStatefulView view -> view.Dispose()
    | ReactViewGroup view -> view.Dispose()
    | ReactViewNone -> ()

  let private removeChildren = function
    | ReactViewGroup view -> view.Children <- ReactChildren.empty
    | _ -> ()
 
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
          when (node.element.comp :> obj).GetHashCode() = statefulView.Id -> view

      | (ReactStatefulDOMNode node, _) ->
          view |> dispose

          let id = (node.element.comp :> obj).GetHashCode()

          let state = 
            node.state
            |> FSXObservable.observeOn scheduler
            |> Observable.scan 
                (fun view dom -> 
                  view |> updateWith dom) 
                ReactViewNone
            |> FSXObservable.distinctUntilChanged

          viewProvider.createStatefulView (id, state)

      | (ReactNativeDOMNode node, ReactView statelessView) 
          when node.element.name = statelessView.Name ->
            statelessView.UpdateProps node.element.props
            view

      | (ReactNativeDOMNodeGroup node, ReactViewGroup viewWithChildren) 
          when node.element.name = viewWithChildren.Name ->
            viewWithChildren.UpdateProps node.element.props

            let children =
              node.children |> ReactChildren.map (
                fun key node -> 
                  updateWith node 
                   <| match viewWithChildren.Children |> ReactChildren.tryFind key with
                      | Some childView -> childView
                      | None -> ReactViewNone
              )
          
            let oldChildren = viewWithChildren.Children
            viewWithChildren.Children <- children
         
            for (name, view) in oldChildren.nodes do
              match children |> React.ReactChildren.tryFind name with
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
                  node.children |> ReactChildren.map (
                    fun key node -> ReactViewNone |> updateWith node
                  )
                view.Children <- children
            | _ -> failwith "node/view mismatch"

          view

    let dom = ReactDom.render element 
    updateWith dom ReactViewNone

