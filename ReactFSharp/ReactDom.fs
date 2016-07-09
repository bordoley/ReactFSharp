namespace React

open FSharp.Control.Reactive
open ImmutableCollections
open System
open System.Reactive.Subjects

type [<ReferenceEquality>] internal ReactDOMNode = 
  | ReactStatefulDOMNode of ReactStatefulDOMNode
  | ReactLazyDOMNode of ReactLazyDOMNode
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

and [<ReferenceEquality>] internal ReactLazyDOMNode = {
  element: ReactLazyElement
  child: ReactDOMNode
}

and [<ReferenceEquality>] internal ReactNativeDOMNode = {
  element: ReactNativeElement
}

and [<ReferenceEquality>] internal ReactNativeDOMNodeGroup = {
  element: ReactNativeElementGroup
  children: IImmutableMap<string, ReactDOMNode>
}

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module internal ReactDom =
  let render (element: ReactElement) =
    let rec updateChildrenWith elements nodes =
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
      
      | (ReactLazyDOMNode node, ReactLazyElement ele)
          when Object.ReferenceEquals(node.element.Component, ele.Component) && node.element.Props = ele.Props -> tree

      | (ReactLazyDOMNode node, ReactLazyElement ele)
          when Object.ReferenceEquals(node.element.Component, ele.Component) ->
            ReactLazyDOMNode {
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
          match tree with
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

          | ReactLazyElement ele -> ReactLazyDOMNode {
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