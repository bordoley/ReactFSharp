namespace React

open FSharp.Control.Reactive
open ImmutableCollections
open System

type ReactElement =
  | ReactStatefulElement of ReactStatefulElement
  | ReactLazyElement of ReactLazyElement
  | ReactNativeElement of ReactNativeElement
  | ReactNoneElement

and [<AbstractClass>] ReactStatefulElement internal () =
  abstract member Id: obj

  abstract member Evaluate: IObservable<obj> -> IObservable<ReactElement>

  abstract member Props: obj

  override this.Equals (that: obj) =
    match that with
    | :? ReactStatefulElement as that ->
      (this :> IEquatable<ReactStatefulElement>).Equals that
    | _ -> false

  override this.GetHashCode () =
    (hash this.Id * 31) + (hash this.Props)

  interface IEquatable<ReactStatefulElement> with
    member this.Equals (that: ReactStatefulElement) =
      Object.ReferenceEquals(this.Id, that.Id) &&
      this.Props = that.Props

and private ReactStatefulElement<'Props> internal (comp: ReactStatefulComponent<'Props>,
                                                   props: 'Props
                                                  ) =
  inherit ReactStatefulElement ()

  override this.Id = comp :> obj

  override this.Evaluate (propChanges: IObservable<obj>) =
    propChanges
    |> Observable.cast<'Props>
    |> Observable.startWith [props]
    |> Observable.distinctUntilChanged
    |> comp

  override this.Props = props :> obj

and [<AbstractClass>] ReactLazyElement internal () =
  abstract member Id: obj

  abstract member Evaluate: unit -> ReactElement

  abstract member Props: obj

  override this.Equals (that: obj) =
    match that with
    | :? ReactStatefulElement as that ->
      (this :> IEquatable<ReactLazyElement>).Equals that
    | _ -> false

  override this.GetHashCode () =
    (hash this.Id * 31) + (hash this.Props)

  interface IEquatable<ReactLazyElement> with
    member this.Equals (that: ReactLazyElement) =
      Object.ReferenceEquals(this.Id, that.Id) &&
      this.Props = that.Props

and private ReactLazyElement<'Props> (comp: ReactComponent<'Props>,
                                      props: 'Props
                                     ) =
  inherit ReactLazyElement ()

  override this.Id = comp :> obj

  override this.Evaluate () = comp props

  override this.Props = props :> obj

and ReactNativeElement = {
    Name: string
    Props: obj
    Children: IImmutableMap<int, ReactElement>
  }

and ReactStatefulComponent<'Props> = IObservable<'Props> -> IObservable<ReactElement>

and ReactComponent<'Props> = 'Props -> ReactElement

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ReactComponent =
  let makeLazy (f: ReactComponent<'Props>): ReactComponent<'Props> =
    let f props =
      ReactLazyElement<'Props>(f, props) :> ReactLazyElement
      |> ReactElement.ReactLazyElement

    f

  let fromStatefulComponent (comp: ReactStatefulComponent<'Props>): ReactComponent<'Props> =
    let f props =
      ReactStatefulElement<'Props> (comp, props) :> ReactStatefulElement
      |> ReactElement.ReactStatefulElement
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
