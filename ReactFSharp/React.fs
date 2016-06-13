namespace React

open System
open System.Collections.Generic
open System.Reactive.Subjects

module FSXObservable = FSharp.Control.Reactive.Observable

type [<ReferenceEquality>] ReactChildren<'a> = {
  keyMap: Map<string, int>
  nodes: array<(string * 'a)>
}

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ReactChildren =
  let empty = {
    keyMap = Map.empty
    nodes = [||]
  }

  let find (key: string) (children: ReactChildren<'a>) =
    let index = children.keyMap |> Map.find key
    let (key, value) = children.nodes.[index]
    value

  let tryFind (key: string) (children: ReactChildren<'a>) =
    let index = children.keyMap |> Map.tryFind key

    match index with 
    | None -> None
    | Some index -> 
        let (key, value) = children.nodes.[index]
        Some value

  let keys (children: ReactChildren<'a>) = 
    let keys = 
      children.keyMap 
      |> Map.fold (
        fun keyArray key index -> 
          Array.set keyArray index key
          keyArray
      ) (Array.create children.nodes.Length "")

    keys :> seq<string>
  

  let private createUnsafe (nodes: array<string * 'a>) =
    let (_, keyMap) = 
      nodes
      |> Array.fold (
        fun (index, keyMap) (key, _) -> (index + 1, keyMap |> Map.add key index)
      ) (0, Map.empty)
    {
      keyMap = keyMap
      nodes = nodes
    }

  let create (nodes: array<string * 'a>) =
    let nodes = Array.copy nodes 
    createUnsafe nodes

  let map (f: string -> 'T -> 'U) (children: ReactChildren<'T>) = 
    let nodes = children.nodes |> Array.map (fun (k, v) -> (k, f k v))
    createUnsafe nodes

  let filter (f: string -> 'T -> bool) (children: ReactChildren<'T>) = 
    let nodes = children.nodes |> Array.filter (fun (k, v) -> f k v)
    createUnsafe nodes

  let iteri2optional (f: Option<string*'a> -> Option<string*'b> -> int -> unit) (b: ReactChildren<'b>) (a: ReactChildren<'a>) =
     let length = Math.Max(a.nodes.Length, b.nodes.Length)

     for i = 0 to length do
       let valA = if i < a.nodes.Length then Some a.nodes.[i] else None
       let valB = if i < b.nodes.Length then Some b.nodes.[i] else None

       f valA valB i

type [<ReferenceEquality>] ReactElement = 
  | ReactStatefulElement of ReactStatefulElement
  | ReactStatelessElement of ReactStatelessElement
  | ReactNativeElement of ReactNativeElement
  | ReactNativeElementWithChild of ReactNativeElementWithChild
  | ReactNativeElementWithChildren of ReactNativeElementWithChildren
  | ReactNoneElement

and [<ReferenceEquality>] ReactStatefulElement = { 
  comp: ReactStatefulComponent<obj> 
  props: obj
}

and [<ReferenceEquality>] ReactStatelessElement = { 
  comp: ReactStatelessComponent<obj> 
  props: obj
}

and [<ReferenceEquality>] ReactNativeElement = {
  name: string
  props: obj
}

and [<ReferenceEquality>] ReactNativeElementWithChild = {
  name: string
  props: obj
  child: ReactElement
}

and [<ReferenceEquality>] ReactNativeElementWithChildren = {
  name: string
  props: obj
  children: ReactChildren<ReactElement>
}

and [<ReferenceEquality>] ReactComponent<'Props> = 
  | ReactStatefulComponent of ReactStatefulComponent<'Props>
  | ReactStatelessComponent of ReactStatelessComponent<'Props>
  | ReactNoneComponent

and ReactStatefulComponent<'Props> = (IObservable<'Props> -> IObservable<ReactElement>)

and ReactStatelessComponent<'Props> = ('Props -> ReactElement)

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ReactComponent =
  let stateReducing 
      (render: ('Props * 'State) -> ReactElement)
      (reducer: ('State * 'Action) -> 'State)
      (initialState: 'State)
      (actions: IObservable<'Action>)
      (props: IObservable<'Props>) =

    
    let state = 
      actions     
      |> Observable.scan (fun acc action -> reducer (acc, action)) initialState  
      |> FSXObservable.startWith [initialState]
    
    FSXObservable.combineLatest props state 
    |> Observable.map render

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ReactElement = 
  let create (props : 'Props) (comp : ReactComponent<'Props>) = 
    match comp with
    | ReactStatefulComponent comp -> 
       let comp = fun (propsStream : IObservable<obj>) -> 
         let propsStream = propsStream |> Observable.map (fun props -> props :?> 'Props)
         comp propsStream
        
       ReactStatefulElement {
         comp = comp
         props = props    
       }

    | ReactStatelessComponent comp -> 
        let comp = fun (props : obj) -> comp (props :?> 'Props)

        ReactStatelessElement {
          comp = comp
          props = props
        }

    | ReactNoneComponent -> ReactNoneElement

[<AutoOpen>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Operators = 
  [<CompiledName("CreateElement")>]
  let (>>=) (comp : ReactComponent<'Props>) (props : 'Props) = ReactElement.create props comp
  let (~%%) (children: array<string * ReactElement>) = ReactChildren.create children

type [<ReferenceEquality>] ReactDOMNode = 
  | ReactStatefulDOMNode of ReactStatefulDOMNode
  | ReactStatelessDOMNode of ReactStatelessDOMNode
  | ReactNativeDOMNode of ReactNativeDOMNode
  | ReactNativeDOMNodeWithChild of ReactNativeDOMNodeWithChild
  | ReactNativeDOMNodeWithChildren of ReactNativeDOMNodeWithChildren
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

and [<ReferenceEquality>] ReactNativeDOMNodeWithChild = {
  element: ReactNativeElementWithChild
  child: ReactDOMNode
}

and [<ReferenceEquality>] ReactNativeDOMNodeWithChildren = {
  element: ReactNativeElementWithChildren
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

    | (ReactNativeDOMNodeWithChild node, ReactNativeElementWithChild ele) when node.element.name = ele.name ->
        ReactNativeDOMNodeWithChild {
          element = ele
          child = node.child |> updateWith ele.child
        }     
    | (ReactNativeDOMNodeWithChildren node, ReactNativeElementWithChildren ele) when node.element.name = ele.name ->
        ReactNativeDOMNodeWithChildren {
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
        | ReactNativeElementWithChild ele -> ReactNativeDOMNodeWithChild {
            element = ele
            child = ReactNoneDOMNode |> updateWith ele.child 
          }
        | ReactNativeElementWithChildren ele -> ReactNativeDOMNodeWithChildren {
            element = ele 
            children = ReactChildren.empty |> updateChildrenWith ele.children
          }

        | ReactNoneElement -> ReactNoneDOMNode

  and private reducer = fun dom ele -> dom |> updateWith ele