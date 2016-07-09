namespace React

open FSharp.Control.Reactive
open ImmutableCollections
open System
open System.Reactive.Linq

type [<ReferenceEquality>] ReactElement = 
  | ReactStatefulElement of ReactStatefulElement
  | ReactLazyElement of ReactLazyElement
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

and ReactLazyElement private (comp: ReactComponent<obj>, props: obj) = 
  static member internal Create<'Props>(comp: ReactComponent<'Props>, 
                                        props: 'Props
                                       ): ReactElement =
    
    let comp (props : obj) = comp (props :?> 'Props)
    ReactElement.ReactLazyElement <| ReactLazyElement(comp, props :> obj)

  member this.Component = comp
  member this.Props = props

and [<ReferenceEquality>] ReactNativeElement = {
    Name: string
    Props: obj
  }

and [<ReferenceEquality>] ReactNativeElementGroup = {
    Name: string 
    Props: obj
    Children: IImmutableMap<string, ReactElement>
  }

and ReactStatefulComponent<'Props> = IObservable<'Props> -> IObservable<ReactElement>

and ReactComponent<'Props> = 'Props -> ReactElement

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ReactComponent =
  let makeLazy (f: ReactComponent<'Props>): ReactComponent<'Props> =
    fun props -> ReactLazyElement.Create(f, props)

  let fromStatefulComponent (comp: ReactStatefulComponent<'Props>): ReactComponent<'Props> =
    fun props -> ReactStatefulElement.Create(comp, props)

  let stateReducing
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

    fromStatefulComponent statefulComponent