namespace React

open System
open System.Reactive.Subjects

module FSXObservable = FSharp.Control.Reactive.Observable

type [<ReferenceEquality>] ReactDOMNode = 
  | ReactStatefulDOMNode of ReactStatefulDOMNode
  | ReactStatelessDOMNode of ReactStatelessDOMNode
  | ReactNativeDOMNode of ReactNativeDOMNode
  | ReactNativeDOMNodeGroup of ReactNativeDOMNodeGroup
  | ReactNoneDOMNode

and [<ReferenceEquality>] ReactStatefulDOMNode = 
  {
    element: ReactStatefulElement
    updateProps: obj -> unit
    state: IObservable<ReactDOMNode>
    dispose: unit -> unit 
  } 
  interface IDisposable with
    member this.Dispose() = this.dispose ()

and [<ReferenceEquality>] ReactStatelessDOMNode = {
  element: ReactStatelessElement
  child: ReactDOMNode
}

and [<ReferenceEquality>] ReactNativeDOMNode = {
  element: ReactNativeElement
}

and [<ReferenceEquality>] ReactNativeDOMNodeGroup = {
  element: ReactNativeElementGroup
  children: ReactChildren<ReactDOMNode>
}

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ReactDom =
  let rec private updateChildrenWith (elements: ReactChildren<ReactElement>) (nodes: ReactChildren<ReactDOMNode>) =
    let keys = elements |> ReactChildren.keys

    keys 
    |> Seq.map (fun key ->
        let element = elements |> ReactChildren.find key

        let node =
          match (nodes |> ReactChildren.tryFind key) with
          | None -> ReactNoneDOMNode |> updateWith element
          | Some node -> node |> updateWith element
        (key, node)
      ) 
    |> Seq.toArray
    |> ReactChildren.create

  and updateWith (element: ReactElement) (tree: ReactDOMNode) = 
    match (tree, element) with
    | (ReactStatefulDOMNode node, ReactStatefulElement ele) 
        when Object.ReferenceEquals(node.element.comp, ele.comp) && node.element.props = ele.props -> tree

    | (ReactStatefulDOMNode node, ReactStatefulElement ele) 
        when Object.ReferenceEquals(node.element.comp, ele.comp) ->
          let element: ReactStatefulElement = {
            comp = node.element.comp
            props = ele.props
          }

          node.updateProps ele.props

          ReactStatefulDOMNode {
            element = element
            updateProps = node.updateProps
            state = node.state
            dispose = node.dispose
          }
    
    | (ReactStatelessDOMNode node, ReactStatelessElement ele)
        when Object.ReferenceEquals(node.element.comp, ele.comp) && node.element.props = ele.props -> tree

    | (ReactStatelessDOMNode node, ReactStatelessElement ele)
        when Object.ReferenceEquals(node.element.comp, ele.comp) ->
          ReactStatelessDOMNode {
            element = ele
            child = node.child |> updateWith (ele.comp ele.props)
          }
    
    | (ReactNativeDOMNode node, ReactNativeElement ele) when node.element.name = ele.name ->
        ReactNativeDOMNode {
          element = ele
        }     

    | (ReactNativeDOMNodeGroup node, ReactNativeElementGroup ele) when node.element.name = ele.name ->
        ReactNativeDOMNodeGroup {
          element = ele
          children = node.children |> updateChildrenWith ele.children
        }     
     
    | (tree, ele) ->
        do match tree with
           | ReactStatefulDOMNode node -> node.dispose ()
           | _ -> ()

        match ele with
        | ReactStatefulElement ele ->
          let subject = new Subject<obj>();
          let state = 
            (ele.comp (subject |> FSXObservable.asObservable))
            |> Observable.scan reducer ReactNoneDOMNode
            |> FSXObservable.multicast (new BehaviorSubject<ReactDOMNode>(ReactNoneDOMNode))
          
          let connection = state.Connect()
              
          let dispose () =
            subject.OnCompleted()
            connection.Dispose()
           
          let updateProps = subject.OnNext

          updateProps ele.props

          ReactStatefulDOMNode {
            element = ele
            updateProps = updateProps
            state = state |> FSXObservable.asObservable
            dispose = dispose
          }

        | ReactStatelessElement ele -> ReactStatelessDOMNode {
            element = ele
            child = ReactNoneDOMNode |> updateWith (ele.comp ele.props)
          }

        | ReactNativeElement ele -> ReactNativeDOMNode {
            element = ele
          }

        | ReactNativeElementGroup ele -> ReactNativeDOMNodeGroup {
            element = ele 
            children = ReactChildren.empty |> updateChildrenWith ele.children
          }

        | ReactNoneElement -> ReactNoneDOMNode

  and private reducer dom ele = dom |> updateWith ele
