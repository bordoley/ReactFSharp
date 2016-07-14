namespace React

open FSharp.Control.Reactive
open ImmutableCollections
open System
open System.Reactive.Concurrency
open System.Reactive.Linq

type ReactElement =
  | ReactStatefulElement of ReactStatefulElement
  | ReactLazyElement of ReactLazyElement
  | ReactNativeElement of ReactNativeElement
  | ReactNoneElement

and ReactStatefulElement = {
  Id: obj
  Component: Func<IObservable<obj>, IObservable<ReactElement>>
  Props: obj
}

and ReactLazyElement = {
  Id: obj
  Component: Func<obj, ReactElement>
  Props:obj
}

and ReactNativeElement = {
    Name: string
    Props: obj
    Children: IImmutableMap<string, ReactElement>
  }

and ReactStatefulComponent<'Props> = IObservable<'Props> -> IObservable<ReactElement>

and ReactComponent<'Props> = 'Props -> ReactElement

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ReactComponent =
  let makeLazy (f: ReactComponent<'Props>): ReactComponent<'Props> =
    let castedComp (props : obj) = f (props :?> 'Props)

    let f props = ReactLazyElement {
      Id = f :> obj
      Component = Func<obj, ReactElement>(castedComp)
      Props = props
    }

    f

  let fromStatefulComponent (comp: ReactStatefulComponent<'Props>): ReactComponent<'Props> =
    let castedComp (propsStream: IObservable<obj>) =
      propsStream |> Observable.cast<'Props> |> comp

    let f props = ReactStatefulElement {
      Id = comp :> obj
      Component = Func<IObservable<obj>, IObservable<ReactElement>> castedComp
      Props = props
    }

    f


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