namespace React

open ImmutableCollections
open System
open System.Reactive.Subjects

module FSXObservable = FSharp.Control.Reactive.Observable

type ReactDOMNodeChildren = IImmutableMap<string, ReactDOMNode>

and [<ReferenceEquality>] internal ReactDOMNode = 
  | ReactStatefulDOMNode of ReactStatefulDOMNode
  | ReactStatelessDOMNode of ReactStatelessDOMNode
  | ReactNativeDOMNode of ReactNativeDOMNode
  | ReactNativeDOMNodeGroup of ReactNativeDOMNodeGroup
  | ReactNoneDOMNode

and [<ReferenceEquality>] internal ReactStatefulDOMNode = 
  {
    element: ReactStatefulElement
    id: obj
    updateProps: obj -> unit
    state: IObservable<ReactDOMNode>
    dispose: unit -> unit 
  } 
  interface IDisposable with
    member this.Dispose() = this.dispose ()

and [<ReferenceEquality>] internal ReactStatelessDOMNode = {
  element: ReactStatelessElement
  child: ReactDOMNode
}

and [<ReferenceEquality>] internal ReactNativeDOMNode = {
  element: ReactNativeElement
}

and [<ReferenceEquality>] internal ReactNativeDOMNodeGroup = {
  element: ReactNativeElementGroup
  children: ReactDOMNodeChildren
}

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module internal ReactDom =
  let render (element: ReactElement) =
    let rec updateChildrenWith (elements: ReactElementChildren) (nodes: ReactDOMNodeChildren) =
      let keys = elements |> ImmutableMap.keys

      keys 
      |> Seq.map (fun key ->
          let element = elements |> ImmutableMap.get key

          let node =
            match (nodes |> ImmutableMap.tryGet key) with
            | None -> ReactNoneDOMNode |> updateWith element
            | Some node -> node |> updateWith element
          (key, node)
        ) 
      |> Seq.toArray
      |> ImmutableMap.create

    and updateWith (element: ReactElement) (tree: ReactDOMNode) = 
      match (tree, element) with
      | (ReactStatefulDOMNode node, ReactStatefulElement ele) 
          when Object.ReferenceEquals(node.element.comp, ele.comp) && node.element.props = ele.props -> tree

      | (ReactStatefulDOMNode node, ReactStatefulElement element) 
          when Object.ReferenceEquals(node.element.comp, element.comp) ->
            node.updateProps element.props

            ReactStatefulDOMNode {
              element = element
              id = node.id
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
              id = new obj()
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
              children = ImmutableMap.empty () |> updateChildrenWith ele.children
            }

          | ReactNoneElement -> ReactNoneDOMNode

    and reducer dom ele = dom |> updateWith ele

    updateWith element ReactNoneDOMNode 