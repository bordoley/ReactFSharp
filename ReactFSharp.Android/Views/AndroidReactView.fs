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

type AndroidReactView =
  {
    dispose: unit -> unit
    name: string
    getProps: unit -> obj
    setProps: obj -> unit
    view: View
  }

  interface IReactView with
    member this.Dispose () = this.dispose ()
    member this.Name = this.name
    member this.Props
      with get () = this.getProps ()
       and set props = this.setProps props
    member this.View = this.view :> obj

type AndroidReactViewGroup =
  {
    dispose: unit -> unit
    name: string
    view: ViewGroup
    getChildren: unit -> IImmutableMap<string, ReactView>
    setChildren: IImmutableMap<string, ReactView> -> unit
    getProps: unit -> obj
    setProps: obj -> unit
  }

  interface IReactViewGroup with
    member this.Children
      with get() = this.getChildren ()
       and set children = this.setChildren children
    member this.Dispose () = this.dispose ()
    member this.Name = this.name
    member this.Props
      with get () = this.getProps ()
       and set props = this.setProps props
    member this.View = this.view :> obj

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module AndroidReactView =
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

  // FIXME: I think this can generified and reused for iOS
  let private updateChildren
       (viewGroup: ViewGroup)
       (emptyView: unit -> View)
       (oldChildren: IImmutableMap<string, IDisposable>)
       (newChildren: IImmutableMap<string, ReactView>): IImmutableMap<string, IDisposable> =

     let setNativeViewAtIndex (index: int) (viewGroup: ViewGroup) (view: Option<obj>) =
       if index < viewGroup.ChildCount then
         viewGroup.RemoveViewAt index
       match view with
       | Some view  -> viewGroup.AddView(view :?> View, index)
       | None _ -> viewGroup.AddView(emptyView (), index)

     let updateSubscription (index: int) = function
       | ((prevKey, _) as prev, (nextKey, _)) when prevKey = nextKey ->
           prev
       | ((_, prevSubscription: IDisposable), (nextKey, nextView))->
           prevSubscription.Dispose ()
           (nextKey, nextView |> ReactView.updateNativeView (setNativeViewAtIndex index viewGroup))

     let updateAndSubscribe (index: int) = function
       | (Some prev, Some next) ->
           viewGroup.RemoveViewAt index
           (prev, next) |> updateSubscription index
       | (None, Some (key, view)) ->
           (key, view |> ReactView.updateNativeView (setNativeViewAtIndex index viewGroup))
       | _ -> failwith "this can never happen"

     let result =
       if oldChildren.Count >= newChildren.Count then
         // Remove children at the tail
         for i = newChildren.Count to oldChildren.Count - 1
           do viewGroup.RemoveViewAt i

         Seq.zip oldChildren newChildren
         |> Seq.mapi updateSubscription

       else
         Seq.zipAll oldChildren newChildren
         |> Seq.mapi updateAndSubscribe

     ImmutableMap.create result

  let createViewGroup<'view, 'props when 'view :> ViewGroup>
      (name: string)
      (viewProvider: Context -> 'view)
      (setProps: 'view -> 'props -> unit)
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

    let emptyView () = (new Android.Widget.Space(context)) :> View
    let viewGroup = ((reactView :> IReactView).View) :?> ViewGroup
    let updateChildren = updateChildren viewGroup emptyView

    let childrenSubject = new BehaviorSubject<IImmutableMap<string, ReactView>>(ImmutableMap.empty ());

    let childrenUpdaterSubscription =
      childrenSubject
      |> FSXObservable.fold updateChildren (ImmutableMap.empty ())
      |> Observable.subscribe (fun map ->
            map
            |> ImmutableMap.values
            |> Seq.iter (fun subscription -> subscription.Dispose())
        )

    let dispose () =
      childrenUpdaterSubscription.Dispose ()
      reactView.dispose ()

    ReactViewGroup {
      dispose = dispose
      name = name
      getProps = reactView.getProps
      setProps = reactView.setProps
      view = viewGroup
      getChildren = fun () -> childrenSubject.Value
      setChildren = childrenSubject.OnNext
    }

  let render
      (nativeViews: IImmutableMap<string, Context -> obj -> ReactView>)
      (context: Context)
      (element: ReactElement) =

    let createView name = (nativeViews |> ImmutableMap.get name) context
    ReactView.render Scheduler.mainLoopScheduler createView element
