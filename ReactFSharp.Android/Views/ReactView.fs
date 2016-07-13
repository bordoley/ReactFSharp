namespace React.Android.Views

open Android.Content
open Android.Views
open ImmutableCollections
open React
open React.Android
open System
open System.Reactive.Concurrency

type AndroidViewCreator = (Exception -> unit) (* onError *) ->
                            (string (* view name *) -> obj (* initialProps *) -> IReactView<View>) ->
                            obj (* initialProps *) ->
                            IReactView<View>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ReactView =
  let render
      (nativeViews: IImmutableMap<string, Context -> AndroidViewCreator>)
      (context: Context)
      (element: ReactElement) =

    let createNativeView onError viewCreator name (props: obj) =
      let createAndroidView = (nativeViews |> ImmutableMap.get name) context
      createAndroidView onError viewCreator props

    ReactView.render Scheduler.mainLoopScheduler createNativeView element
