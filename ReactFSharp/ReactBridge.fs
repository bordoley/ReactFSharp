namespace React

open React
open System
open System.Reactive.Linq
open System.Reactive.Disposables
open System.Reactive.Concurrency

module FSXObservable = FSharp.Control.Reactive.Observable

type [<ReferenceEquality>] ReactView = 
  | ReactStatelessView of ReactStatelessView
  | ReactStatefulView of ReactStatefulView
  | ReactViewWithChild of ReactViewWithChild
  | ReactViewWithChildren of ReactViewWithChildren

and ReactStatelessView =
  inherit IDisposable

  abstract member Name: string with get
  abstract member UpdateProps: obj -> unit

and ReactStatefulView = 
  inherit IDisposable

  abstract member Id: int with get
  abstract member State: IObservable<Option<ReactView>> with get

and ReactViewWithChild =
  inherit IDisposable

  abstract member Name: string with get
  abstract member UpdateProps: obj -> unit
  abstract member Child: Option<ReactView> with get, set

and ReactViewWithChildren =
  inherit IDisposable

  abstract member Name: string with get
  abstract member UpdateProps: obj -> unit
  abstract member Children: ReactChildren<ReactView> with get, set

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ReactView =
  let private mapAndFlatten f children = 
    children
    |> ReactChildren.map f
    |> ReactChildren.filter(
      fun key -> function
        | Some _ -> true
        | None -> false
      )
    |> ReactChildren.map (fun key node -> node |> Option.get)

  let private disposeView = function
    | ReactStatelessView view -> view.Dispose()
    | ReactStatefulView view -> view.Dispose()
    | ReactViewWithChild view -> view.Dispose()
    | ReactViewWithChildren view -> view.Dispose()

  let dispose = function
    | Some view -> view |> disposeView
    | None -> ()

  let private removeChildren = function
    | ReactViewWithChildren view -> view.Children <- ReactChildren.empty
    | ReactViewWithChild view -> view.Child <- Option.None
    | _ -> ()
 
  let rec updateWith
      (scheduler: IScheduler) 
      (nativeViews: Map<string, obj-> ReactView>) 
      (createStatefulView: (int * IObservable<Option<ReactView>>) -> ReactView) 
      (dom: ReactDOMNode) 
      (view: Option<ReactView>) =
    let updateWith = updateWith scheduler nativeViews createStatefulView

    match (dom, view) with
    | (ReactNoneDOMNode, _) -> 
        None
      
    | (ReactStatelessDOMNode node, _) -> 
        view |> updateWith node.child

    | (ReactStatefulDOMNode node, Some (ReactStatefulView statefulView)) 
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
              Option.None
          |> FSXObservable.distinctUntilChanged

        Some (createStatefulView (id, state))

    | (ReactNativeDOMNode node, Some (ReactStatelessView statelessView)) when node.element.name = statelessView.Name ->
        statelessView.UpdateProps node.element.props
        view

    | (ReactNativeDOMNodeWithChild node, Some (ReactViewWithChild viewWithChild)) when node.element.name = viewWithChild.Name ->
        viewWithChild.Child <- viewWithChild.Child |> updateWith node.child
        viewWithChild.UpdateProps node.element.props

        view

    | (ReactNativeDOMNodeWithChildren node, Some(ReactViewWithChildren viewWithChildren)) when node.element.name = viewWithChildren.Name ->
        viewWithChildren.UpdateProps node.element.props

        let children =
          node.children |> mapAndFlatten (
            fun key node -> 
              let childView = viewWithChildren.Children |> ReactChildren.tryFind key
              childView |> updateWith node
          )
        
        let oldChildren = viewWithChildren.Children
        viewWithChildren.Children <- children
       
        for (name, view) in oldChildren.nodes do
          match children |> React.ReactChildren.tryFind name with
          | Some _ -> ()
          | None -> disposeView view

        view

    | (node, view) -> 
        view |> dispose

        let (name, props) =
          match node with
          | ReactNativeDOMNode node -> (node.element.name, node.element.props)
          | ReactNativeDOMNodeWithChild node -> (node.element.name, node.element.props)
          | ReactNativeDOMNodeWithChildren node -> (node.element.name, node.element.props)
          | _ -> failwith "node is not a ReactNativeDomNode type"

        let viewCreator = nativeViews |> Map.find name

        let view = viewCreator props

        do 
          match (node, view) with
          | (ReactNativeDOMNode node, ReactStatelessView _) -> ()
          | (ReactNativeDOMNodeWithChild node, ReactViewWithChild view) -> 
              let child = view.Child |> updateWith node.child
              view.Child <- child
          | (ReactNativeDOMNodeWithChildren node, ReactViewWithChildren view) ->
              let children =
                node.children |> mapAndFlatten (
                  fun key node -> None |> updateWith node
                )
              view.Children <- children
          | _ -> failwith "node/view mismatch"
        Some view
