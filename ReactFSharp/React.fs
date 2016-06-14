namespace React

open System

module FSXObservable = FSharp.Control.Reactive.Observable

type [<ReferenceEquality>] ReactElement = 
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

  [<CompiledName("CreateChildren")>]
  let (~%%) (children: array<string * ReactElement>) = ReactChildren.create children
