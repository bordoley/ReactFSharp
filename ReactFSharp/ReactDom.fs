namespace React

open FSharp.Control.Reactive
open ImmutableCollections
open System
open System.Reactive.Subjects

type internal ReactDOMNodeChildren = IImmutableMap<string, ReactDOMNode>

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

      let keyToNodeMapper key =
        let element = elements |> ImmutableMap.get key

        let node =
          match (nodes |> ImmutableMap.tryGet key) with
          | None -> ReactNoneDOMNode |> updateWith element
          | Some node -> node |> updateWith element
        (key, node)

      keys 
      |> Seq.map keyToNodeMapper
      |> Seq.toArray
      |> ImmutableMap.create

    and updateWith (element: ReactElement) (tree: ReactDOMNode) = 
      match (tree, element) with
      | (ReactStatefulDOMNode node, ReactStatefulElement ele) 
          when Object.ReferenceEquals(node.element.Component, ele.Component) && node.element.Props = ele.Props -> tree

      | (ReactStatefulDOMNode node, ReactStatefulElement element) 
          when Object.ReferenceEquals(node.element.Component, element.Component) ->
            node.updateProps element.Props

            ReactStatefulDOMNode {
              element = element
              id = node.id
              updateProps = node.updateProps
              state = node.state
              dispose = node.dispose
            }
      
      | (ReactStatelessDOMNode node, ReactStatelessElement ele)
          when Object.ReferenceEquals(node.element.Component, ele.Component) && node.element.Props = ele.Props -> tree

      | (ReactStatelessDOMNode node, ReactStatelessElement ele)
          when Object.ReferenceEquals(node.element.Component, ele.Component) ->
            ReactStatelessDOMNode {
              element = ele
              child = node.child |> updateWith (ele.Component ele.Props)
            }
      
      | (ReactNativeDOMNode node, ReactNativeElement ele) when node.element.Name = ele.Name ->
          ReactNativeDOMNode {
            element = ele
          }     

      | (ReactNativeDOMNodeGroup node, ReactNativeElementGroup ele) when node.element.Name = ele.Name ->
          ReactNativeDOMNodeGroup {
            element = ele
            children = node.children |> updateChildrenWith ele.Children
          }     
       
      | (tree, ele) ->
          do match tree with
             | ReactStatefulDOMNode node -> node.dispose ()
             | _ -> ()

          match ele with
          | ReactStatefulElement ele ->
            let props = new BehaviorSubject<obj>(ele.Props);
            let state = 
              (ele.Component (props |> Observable.asObservable))
              |> Observable.scanInit ReactNoneDOMNode reducer 
              |> Observable.multicast (new BehaviorSubject<ReactDOMNode>(ReactNoneDOMNode))
            
            let connection = state.Connect()
                
            let dispose () =
              props.OnCompleted()
              connection.Dispose()

            ReactStatefulDOMNode {
              element = ele
              id = new obj()
              updateProps = props.OnNext
              state = state |> Observable.asObservable
              dispose = dispose
            }

          | ReactStatelessElement ele -> ReactStatelessDOMNode {
              element = ele
              child = ReactNoneDOMNode |> updateWith (ele.Component ele.Props)
            }

          | ReactNativeElement ele -> ReactNativeDOMNode {
              element = ele
            }

          | ReactNativeElementGroup ele -> ReactNativeDOMNodeGroup {
              element = ele 
              children = ImmutableMap.empty () |> updateChildrenWith ele.Children
            }

          | ReactNoneElement -> ReactNoneDOMNode

    and reducer dom ele = dom |> updateWith ele

    updateWith element ReactNoneDOMNode 