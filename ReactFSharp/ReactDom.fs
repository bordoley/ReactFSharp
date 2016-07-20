namespace React

open FSharp.Control.Reactive
open ImmutableCollections
open System
open System.Reactive.Subjects

type ReactDOMNode =
  | ReactStatefulDOMNode of ReactStatefulDOMNode
  | ReactLazyDOMNode of ReactLazyDOMNode
  | ReactNativeDOMNode of ReactNativeDOMNode
  | ReactNoneDOMNode

and ReactStatefulDOMNode internal (id: obj,
                                   updateProps: obj -> unit,
                                   state: IObservable<ReactDOMNode>
                                  ) =
  let state = 
    state |> Observable.publishInitial ReactNoneDOMNode

  let connection = state.Connect()

  member this.Id = id

  member this.UpdateProps props =
    updateProps props

  interface IObservable<ReactDOMNode> with
    member this.Subscribe observer =
      state.Subscribe observer

  interface IDisposable with
    member this.Dispose() =
      connection.Dispose()

and ReactLazyDOMNode = {
  Element: ReactLazyElement
  Value: ReactDOMNode
}

and ReactNativeDOMNode = {
  Element: ReactNativeElement
  Children: IImmutableMap<int, ReactDOMNode>
}

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ReactDom =
  let private dispose (disposable: IDisposable) = disposable.Dispose()

  let rec observe<'view> (dom: ReactDOMNode): IObservable<Option<ReactNativeDOMNode>> =
    match dom with
    | ReactStatefulDOMNode dom ->
        dom |> Observable.flatmap observe
    | ReactLazyDOMNode dom ->
       observe dom.Value
    | ReactNativeDOMNode dom ->
        Some dom |> Observable.single
    | ReactNoneDOMNode -> None |> Observable.single

  let rec private updateWith (element: ReactElement) (tree: ReactDOMNode) =
    match (element, tree) with
    | (ReactLazyElement ele, ReactLazyDOMNode node)
          when ele = node.Element ->
        tree

    | (ReactNativeElement ele, ReactNativeDOMNode node)
          when ele = node.Element ->
        tree

    | (ReactStatefulElement element, ReactStatefulDOMNode node)
          when Object.ReferenceEquals(node.Id, element.Id) ->
        node.UpdateProps element.Props
        tree

    | (ReactLazyElement ele, ReactLazyDOMNode node)
          when Object.ReferenceEquals(node.Element.Id, ele.Id) ->
        ReactLazyDOMNode {
          Element = ele
          Value = node.Value |> updateWith (ele.Evaluate ())
        }

    | (ReactNativeElement ele, ReactNativeDOMNode node)
          when node.Element.Name = ele.Name ->
        ReactNativeDOMNode {
          Element = ele
          Children = node.Children |> updateChildrenWith ele.Children
        }

    | (ReactNoneElement, _) ->
        ReactNoneDOMNode


    | (ele, tree) ->
        match tree with
        | ReactStatefulDOMNode node ->  dispose node
        | _ -> ()

        match ele with
        | ReactStatefulElement ele ->
            let propsChanges = new Subject<obj>()

            let state =
              propsChanges 
              |> Observable.asObservable
              |> ele.Evaluate
              |> Observable.scanInit ReactNoneDOMNode (fun dom ele -> dom |> updateWith ele)
              |> Observable.distinctUntilChanged

            ReactDOMNode.ReactStatefulDOMNode
            <| new ReactStatefulDOMNode(ele.Id, propsChanges.OnNext, state)

        | ReactLazyElement ele -> ReactLazyDOMNode {
            Element = ele
            Value = ReactNoneDOMNode |> updateWith (ele.Evaluate ())
          }

        | ReactNativeElement ele -> ReactNativeDOMNode {
            Element = ele
            Children = ImmutableMap.empty () |> updateChildrenWith ele.Children
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

  let internal render (element: ReactElement) =
    updateWith element ReactNoneDOMNode
