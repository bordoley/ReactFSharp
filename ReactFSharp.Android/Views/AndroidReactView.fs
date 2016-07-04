namespace React.Android.Views

open Android.Content
open Android.Views
open Android.Widget
open ImmutableCollections
open React
open React.Android
open System
open System.Reactive.Concurrency
open System.Reactive.Subjects

module FSXObservable = FSharp.Control.Reactive.Observable

type IAndroidReactView =
    abstract member View: View with get

type StatefulAndroidReactView =
  {
    id: obj
    dispose: unit -> unit
    view: View
  }

  interface IAndroidReactView with
    member this.View = this.view

  interface IReactStatefulView with
    member this.Dispose () = this.dispose ()
    member this.Id = this.id

type AndroidReactView =
  {
    dispose: unit -> unit
    name: string
    getProps: unit -> obj
    setProps: obj -> unit
    view: View
  }

  interface IAndroidReactView with
    member this.View = this.view

  interface IReactView with
    member this.Dispose () = this.dispose ()
    member this.Name = this.name
    member this.Props
      with get () = this.getProps ()
       and set props = this.setProps props

type AndroidReactViewGroup =
  {
    dispose: unit -> unit
    name: string
    view: View
    getChildren: unit -> IImmutableMap<string, ReactView>
    setChildren: IImmutableMap<string, ReactView> -> unit
    getProps: unit -> obj
    setProps: obj -> unit
  }

  interface IAndroidReactView with
    member this.View = this.view

  interface IReactViewGroup with
    member this.Children
      with get() = this.getChildren ()
       and set children = this.setChildren children
    member this.Dispose () = this.dispose ()
    member this.Name = this.name
    member this.Props
      with get () = this.getProps ()
       and set props = this.setProps props

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module AndroidReactView =
  let getView = function
    | ReactView child -> Some (child :?> IAndroidReactView).View
    | ReactStatefulView child -> Some (child :?> IAndroidReactView).View
    | ReactViewGroup child -> Some (child :?> IAndroidReactView).View
    | _ -> None

  let private updateView (view: FrameLayout) prevView nextView =
    view.RemoveAllViews ()
    prevView |> ReactView.dispose

    match getView nextView with
    | Some child ->
        view.AddView child
        view.LayoutParameters <- child.LayoutParameters
    | _ -> ()

    nextView

  let private statefulViewProvider (context: Android.Content.Context) (id: obj, state: IObservable<ReactView>) =
    let view = new FrameLayout(context);
    view.LayoutParameters <- new ViewGroup.LayoutParams(-2, -2)

    let subscription =
      state
      |> Observable.scan (updateView view) ReactViewNone
      |> FSXObservable.last
      |> Observable.subscribe ReactView.dispose

    let dispose () =
      subscription.Dispose()
      view.Dispose()

    ReactStatefulView {
      id = id
      dispose = dispose
      view = view
    }

  let private createReactViewInternal<'view, 'props when 'view :> View>
      (name: string)
      (viewProvider: Context -> 'view)
      (setProps: 'view -> 'props -> unit)
      (disposeView: 'view -> unit)
      (context: Context)
      (initialProps: obj) : AndroidReactView =

    let initialProps = (initialProps :?>'props)

    let view = viewProvider(context)

    let propsSubject = new BehaviorSubject<'props>(initialProps);

    let propsUpdaterSubscription =
      propsSubject
      |> FSXObservable.iter (setProps view)
      |> Observable.subscribe (fun _ -> ())

    let setProps (props: obj) =
      let props = (props :?> 'props)
      propsSubject.OnNext props

    let dispose () =
      propsUpdaterSubscription.Dispose()
      disposeView view

    {
      dispose = dispose
      name = name
      getProps = fun () -> propsSubject.Value :> obj
      setProps = setProps
      view = view
    }

  let createView<'view, 'props when 'view :> View>
      (name: string)
      (viewProvider: Context -> 'view)
      (setProps: 'view -> 'props -> unit)
      (disposeView: 'view -> unit)
      (context: Context)
      (initialProps: obj) =
    ReactView (
      createReactViewInternal
        name
        viewProvider
        setProps
        disposeView
        context
        initialProps
    )

  let createViewGroup<'view, 'props when 'view :> ViewGroup>
      (name: string)
      (viewProvider: Context -> 'view)
      (setProps: 'view -> 'props -> unit)
      (updateChildren: 'view -> IImmutableMap<string, ReactView> -> IImmutableMap<string, ReactView> -> unit)
      (disposeView: 'view -> unit)
      (context: Context)
      (initialProps: obj) =

    let reactView =
      createReactViewInternal
        name
        viewProvider
        setProps
        disposeView
        context
        initialProps

    let childrenSubject = new BehaviorSubject<IImmutableMap<string, ReactView>>(ImmutableMap.empty ());

    let childrenUpdaterSubscription =
      childrenSubject
      |> FSXObservable.bufferCountSkip 2 1
      |> FSXObservable.iter (fun values -> updateChildren (reactView.view :?> 'view) values.[0] values.[1])
      |> Observable.subscribe (fun _ -> ())

    let dispose () =
      childrenUpdaterSubscription.Dispose ()
      reactView.dispose ()

    ReactViewGroup {
      dispose = dispose
      name = name
      getProps = reactView.getProps
      setProps = reactView.setProps
      view = reactView.view
      getChildren = fun () -> childrenSubject.Value
      setChildren = childrenSubject.OnNext
    }

  let render = ReactView.render Scheduler.mainLoopScheduler

  let createViewProvider (context: Context) (viewMap: IImmutableMap<string, Context -> obj -> ReactView>) : ReactView.ViewProvider = {
    createView = fun name ->
      (viewMap |> ImmutableMap.get name) context
    createStatefulView = statefulViewProvider context
  }