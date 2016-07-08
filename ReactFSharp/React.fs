namespace React

open FSharp.Control.Reactive
open ImmutableCollections
open System
open System.Reactive.Linq

type ReactElementChildren = IImmutableMap<string, ReactElement>

and [<ReferenceEquality>] ReactElement = 
  | ReactStatefulElement of ReactStatefulElement
  | ReactStatelessElement of ReactStatelessElement
  | ReactNativeElement of ReactNativeElement
  | ReactNativeElementGroup of ReactNativeElementGroup
  | ReactNoneElement

and ReactStatefulElement private (comp: ReactStatefulComponent<obj>, props: obj) = 
  static member internal Create<'Props>(comp: ReactStatefulComponent<'Props>, 
                                        props: 'Props
                                       ): ReactElement =
    let comp (propsStream : IObservable<obj>) = 
      propsStream |> Observable.cast<'Props> |> comp
     
    ReactElement.ReactStatefulElement <| ReactStatefulElement(comp, props)    

  member this.Component = comp
  member this.Props = props

and ReactStatelessElement private (comp: ReactStatelessComponent<obj>, props: obj) = 
  static member internal Create<'Props>(comp: ReactStatelessComponent<'Props>, 
                                        props: 'Props
                                       ): ReactElement =
    
    let comp (props : obj) = comp (props :?> 'Props)
    ReactElement.ReactStatelessElement <| ReactStatelessElement(comp, props :> obj)

  member this.Component = comp
  member this.Props = props

and [<ReferenceEquality>] ReactNativeElement = {
    Name: string
    Props: obj
  }

and [<ReferenceEquality>] ReactNativeElementGroup = {
    Name: string 
    Props: obj
    Children: ReactElementChildren
  }

and [<ReferenceEquality>] ReactComponent<'Props> = 
  | ReactStatefulComponent of ReactStatefulComponent<'Props>
  | ReactStatelessComponent of ReactStatelessComponent<'Props>
  | ReactNoneComponent

and ReactStatefulComponent<'Props> = (IObservable<'Props> -> IObservable<ReactElement>)

and ReactStatelessComponent<'Props> = ('Props -> ReactElement)

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ReactStatefulComponent =
  let create 
      (render: ('Props * 'State) -> ReactElement)
      (reducer: 'State -> 'Action -> 'State)
      (shouldUpdate: ('Props * 'State) -> ('Props * 'State) -> bool)
      (initialState: 'State) 
      (actions: IObservable<'Action>) =
    let statefulComponent (props: IObservable<'Props>) =
      let state = 
        actions     
        |> Observable.scanInit initialState reducer 
        |> Observable.startWith [initialState]

      let updatedPropsAndState =
        Observable.combineLatest props state
        |> Observable.bufferCountSkip 2 1
        |> Observable.map (fun list -> (list.[0], list.[1]))
        |> Observable.filter (fun (prev, next) -> shouldUpdate prev next)
        |> Observable.map (fun (_, next) -> next)

      let elements = 
        props
        |> Observable.first 
        |> Observable.map (fun props -> (props, initialState))
        |> Observable.concat updatedPropsAndState
        |> Observable.map render
        
      elements
    ReactStatefulComponent statefulComponent

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ReactElement = 
  let create (props : 'Props) (comp : ReactComponent<'Props>): ReactElement = 
    match comp with
    | ReactStatefulComponent comp -> 
         (ReactStatefulElement.Create(comp, props))

    | ReactStatelessComponent comp -> 
        ReactStatelessElement.Create(comp, props)

    | ReactNoneComponent -> ReactNoneElement

[<AutoOpen>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Operators = 
  [<CompiledName("CreateElement")>]
  let (>>=) (comp : ReactComponent<'Props>) (props: 'Props) = ReactElement.create props comp

  [<CompiledName("CreateChildren")>]
  let (~%%) (children: seq<string * ReactElement>) = ImmutableMap.create children
