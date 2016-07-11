﻿namespace React

open FSharp.Control.Reactive
open ImmutableCollections
open System
open System.Reactive.Subjects

type [<ReferenceEquality>] ReactDOMNode =
  | ReactStatefulDOMNode of ReactStatefulDOMNode
  | ReactLazyDOMNode of ReactLazyDOMNode
  | ReactNativeDOMNode of ReactNativeDOMNode
  | ReactNoneDOMNode

and [<ReferenceEquality>] ReactStatefulDOMNode =
  {
    element: ReactStatefulElement
    id: obj
    updateProps: obj -> unit
    state: IObservable<ReactDOMNode>
    dispose: unit -> unit
  }
  interface IDisposable with
    member this.Dispose() = this.dispose ()

and [<ReferenceEquality>] ReactLazyDOMNode = {
  element: ReactLazyElement
  child: ReactDOMNode
}

and [<ReferenceEquality>] ReactNativeDOMNode = {
  element: ReactNativeElement
  children: IImmutableMap<string, ReactDOMNode>
}

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module internal ReactDom =
  let rec private updateWith (element: ReactElement) (tree: ReactDOMNode) =
    match (element, tree) with
    | (ReactStatefulElement ele, ReactStatefulDOMNode node)
          when Object.ReferenceEquals(node.element.Id, ele.Id)
            && node.element.Props = ele.Props ->
        tree

    | (ReactLazyElement ele, ReactLazyDOMNode node)
          when Object.ReferenceEquals(node.element.Id, ele.Id)
            && node.element.Props = ele.Props ->
        tree

    | (ReactNativeElement ele, ReactNativeDOMNode node)
          when node.element.Name = ele.Name
            && node.element.Props = ele.Props
            && node.element.Children = ele.Children ->
        tree


    | (ReactStatefulElement element, ReactStatefulDOMNode node)
          when Object.ReferenceEquals(node.element.Id, element.Id) ->
        node.updateProps element.Props
        ReactStatefulDOMNode { node with element = element }

    | (ReactLazyElement ele, ReactLazyDOMNode node)
          when Object.ReferenceEquals(node.element.Id, ele.Id) ->
        ReactLazyDOMNode {
          element = ele
          child = node.child |> updateWith (ele.Component ele.Props)
        }

    | (ReactNativeElement ele, ReactNativeDOMNode node)
          when node.element.Name = ele.Name ->
        ReactNativeDOMNode {
          element = ele
          children = node.children |> updateChildrenWith ele.Children
        }

    | (ReactNoneElement, _) ->
        ReactNoneDOMNode


    | (ele, tree) ->
        match tree with
        | ReactStatefulDOMNode node -> node.dispose ()
        | _ -> ()

        match ele with
        | ReactStatefulElement ele ->
            let props = new BehaviorSubject<obj>(ele.Props);
            let state =
              (ele.Component (props |> Observable.asObservable))
              |> Observable.scanInit ReactNoneDOMNode (fun dom ele -> dom |> updateWith ele)
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
            children = ImmutableMap.empty () |> updateChildrenWith ele.Children
          }

        | ReactNoneElement -> ReactNoneDOMNode

  and private updateChildrenWith elements nodes =
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

  let render (element: ReactElement) =
    updateWith element ReactNoneDOMNode
