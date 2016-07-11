namespace React.Android.Views

open Android.Content
open Android.Views
open ImmutableCollections
open React
open React.Android
open System
open System.Reactive.Concurrency

type AndroidViewCreator = (Exception -> unit) (* onError *) ->
                            (ReactDOMNode -> ReactView<View> -> ReactView<View>) (* updateWith *) ->
                            obj (* initialProps *) ->
                            IReactNativeView<View>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ReactView =
  let render
      (nativeViews: IImmutableMap<string, Context -> AndroidViewCreator>)
      (context: Context)
      (element: ReactElement) =

    let nativeViews =
      nativeViews
      |> Seq.map (fun (name, f) -> (name, (f context)))
      |> ImmutableMap.create

    let createView onError viewCreator name props =
      (nativeViews |> ImmutableMap.get name) onError viewCreator props

    ReactView.render Scheduler.mainLoopScheduler createView element
