namespace React

open ImmutableCollections
open System
open System.Reactive.Linq

module FSXObservable = FSharp.Control.Reactive.Observable

type ReactElementChildren = IImmutableMap<string, ReactElement>

and [<ReferenceEquality>] ReactElement = 
  | ReactStatefulElement of ReactStatefulElement
  | ReactStatelessElement of ReactStatelessElement
  | ReactNativeElement of ReactNativeElement
  | ReactNativeElementGroup of ReactNativeElementGroup
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

and [<ReferenceEquality>] ReactNativeElementGroup = {
  name: string
  props: obj
  children: ReactElementChildren
}

and [<ReferenceEquality>] ReactComponent<'Props> = 
  | ReactStatefulComponent of ReactStatefulComponent<'Props>
  | ReactStatelessComponent of ReactStatelessComponent<'Props>
  | ReactNoneComponent

and ReactStatefulComponent<'Props> = (IObservable<'Props> -> IObservable<ReactElement>)

and ReactStatelessComponent<'Props> = ('Props -> ReactElement)

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ReactComponent =
  open FSXObservable
  let stateReducing 
      (render: ('Props * 'State) -> ReactElement)
      (reducer: 'State -> 'Action -> 'State)
      (shouldUpdate: ('Props * 'State) -> ('Props * 'State) -> bool)
      (initialState: 'State) 
      (actions: IObservable<'Action>) =
    let statefulComponent (props: IObservable<'Props>) =
      let state = 
        actions     
        |> Observable.scan reducer initialState  
        |> FSXObservable.startWith [initialState]

      let updatedPropsAndState =
        FSXObservable.combineLatest props state
        |> FSXObservable.bufferCountSkip 2 1
        |> Observable.map (fun list -> (list.[0], list.[1]))
        |> Observable.filter (fun (o, n) -> shouldUpdate o n)
        |> Observable.map (fun (o, n) -> n)

      let elements = 
        props
        |> FSXObservable.first 
        |> FSXObservable.map (fun props -> (props, initialState))
        |> FSXObservable.concat updatedPropsAndState
        |> Observable.map render
        
      elements
    ReactStatefulComponent statefulComponent

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
  let (>>=) (comp : ReactComponent<'Props>) (props: 'Props) = ReactElement.create props comp

  [<CompiledName("CreateChildren")>]
  let (~%%) (children: seq<string * ReactElement>) = ImmutableMap.create children
