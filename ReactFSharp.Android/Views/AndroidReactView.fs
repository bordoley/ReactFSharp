namespace React.Android.Views

open Android.Content
open Android.Views
open Android.Widget
open ImmutableCollections
open React
open React.Android
open System
open System.Reactive.Concurrency

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
    updateProps: obj -> unit
    view: View
  }

  interface IAndroidReactView with
    member this.View = this.view

  interface IReactView with
    member this.Dispose () = this.dispose ()
    member this.Name = this.name
    member this.UpdateProps props = this.updateProps props

type AndroidReactViewGroup =
  {
    dispose: unit -> unit
    name: string
    updateProps: obj -> unit
    view: View
    getChildren: unit -> IImmutableMap<string, ReactView> 
    setChildren: IImmutableMap<string, ReactView> -> unit
  }

  interface IAndroidReactView with
    member this.View = this.view

  interface IReactViewGroup with
    member this.Dispose () = this.dispose ()
    member this.Name = this.name
    member this.UpdateProps props = this.updateProps props
    member this.Children 
      with get() = this.getChildren () 
       and set children = this.setChildren children

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module AndroidReactView =
  let getView = function
    | ReactView child -> Some (child :?> IAndroidReactView).View
    | ReactStatefulView child -> Some (child :?> IAndroidReactView).View
    | ReactViewGroup child -> Some (child :?> IAndroidReactView).View
    | _ -> None

  let private statefulViewProvider (context: Android.Content.Context) (id: obj, state: IObservable<ReactView>) = 
    let view = new FrameLayout(context);
    view.LayoutParameters <- new ViewGroup.LayoutParams(-2, -2)

    let subscription = 
      state
      |> Observable.scan (
          fun prevView nextView ->
            view.RemoveAllViews ()
            prevView |> ReactView.dispose

            match getView nextView with
            | Some child -> 
                view.AddView child 
                view.LayoutParameters <- child.LayoutParameters
            | _ -> ()

            nextView
          )
          ReactViewNone
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

 
  let render = ReactView.render Scheduler.mainLoopScheduler

  let createViewProvider (context: Context) (viewMap: IImmutableMap<string, Context -> obj -> ReactView>) : ReactView.ViewProvider = {
    createView = fun name ->
      (viewMap |> ImmutableMap.get name) context
    createStatefulView = statefulViewProvider context
  }