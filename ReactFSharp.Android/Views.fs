namespace React.Android.Views

open Android.Content
open Android.Views
open Android.Widget
open React
open React.Android
open System

module FSXObservable = FSharp.Control.Reactive.Observable

type AndroidReactView =
    abstract member View: View with get

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module AndroidReactView =
  let getView = function
    | ReactView child -> Some (child :?> AndroidReactView).View
    | ReactStatefulView child -> Some (child :?> AndroidReactView).View
    | ReactViewGroup child -> Some (child :?> AndroidReactView).View
    | _ -> None

  let private statefulViewProvider (context: Android.Content.Context) (id:int, state: IObservable<ReactView>) = 
     let view = new FrameLayout(context);
     view.LayoutParameters <- new ViewGroup.LayoutParams(-2, -2)

     let subscription = 
        state
        |> Observable.scan
            (fun prevView nextView ->
              view.RemoveAllViews ()
              prevView |> ReactView.dispose

              match getView nextView with
              | Some child -> view.AddView child 
              | _ -> ()

              nextView
            )
            ReactViewNone
        |> FSXObservable.last
        |> Observable.subscribe ReactView.dispose

     ReactStatefulView { new obj()
       interface IReactStatefulView with
         member this.Id = id
         member this.State = state
       interface IDisposable with
         member this.Dispose () = 
           subscription.Dispose()
           view.Dispose()
       interface AndroidReactView with
         member this.View = (view :> View)
     }

  let render = ReactView.render Scheduler.mainLoopScheduler

  let createViewProvider (context: Context) (viewMap: Map<string, Context -> obj -> ReactView>) : ReactView.ViewProvider = {
    createView = fun name ->
      (viewMap |> Map.find name) context
    createStatefulView = statefulViewProvider context
  }