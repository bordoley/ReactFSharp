namespace React.Android.Views

open Android.Content
open Android.Views
open FSharp.Control.Reactive
open ImmutableCollections
open React
open React.Android
open System
open System.Reactive.Concurrency
open System.Reactive.Subjects

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ReactView =
  let private createReactViewInternal<'view, 'props when 'view :> View>
      (name: string)
      (viewProvider: Context -> 'view)
      (setProps: 'view -> 'props -> unit)
      (disposeView: 'view -> unit)
      (context: Context)
      (initialProps: obj) : IReactView =

    let initialProps = (initialProps :?> 'props)

    let view = viewProvider(context)

    let propsSubject = new BehaviorSubject<'props>(initialProps);

    let propsUpdaterSubscription =
      propsSubject
      |> Observable.iter (setProps view)
      |> Observable.subscribe (fun _ -> ())

    { new IReactView with
        member this.Dispose () =
          propsUpdaterSubscription.Dispose()
          disposeView view
        member this.Name = name
        member this.Props
          with get () =
            propsSubject.Value :> obj
          and set props = 
            let props = (props :?> 'props)
            propsSubject.OnNext props
        member this.View = view :> obj
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
      (disposeView: 'view -> unit)
      (context: Context)
      (initialProps: obj) : ReactView =

    let reactView =
      createReactViewInternal
        name
        viewProvider
        setProps
        disposeView
        context
        initialProps

    let emptyView () = (new Android.Widget.Space(context)) :> View
    let viewGroup = reactView.View :?> ViewGroup

    let setViewAtIndex (index: int) (view: Option<obj>) =
      if index < viewGroup.ChildCount then
        viewGroup.RemoveViewAt index
      match view with
      | Some view  -> viewGroup.AddView(view :?> View, index)
      | None _ -> viewGroup.AddView(emptyView (), index)

    let removeViewAtIndex (index: int) =
      viewGroup.RemoveViewAt index

    let updateChildren = ReactView.updateChildren setViewAtIndex removeViewAtIndex

    let childrenSubject = new BehaviorSubject<IImmutableMap<string, ReactView>>(ImmutableMap.empty ());

    let childrenUpdaterSubscription =
      childrenSubject
      |> Observable.fold updateChildren (ImmutableMap.empty ())
      |> Observable.subscribe (ImmutableMap.values >> Seq.iter (fun subscription -> subscription.Dispose()))

    ReactViewGroup {
      new IReactViewGroup with
        member this.Children
          with get() = childrenSubject.Value
           and set children = childrenSubject.OnNext children
        member this.Dispose () =
          childrenUpdaterSubscription.Dispose ()
          reactView.Dispose ()
        member this.Name = name
        member this.Props
          with get () = reactView.Props
           and set props = reactView.Props <- props
        member this.View = reactView.View
    }

  let render
      (nativeViews: IImmutableMap<string, Context -> obj -> ReactView>)
      (context: Context)
      (element: ReactElement) =

    let createView name = (nativeViews |> ImmutableMap.get name) context
    ReactView.render Scheduler.mainLoopScheduler createView element
