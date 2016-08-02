namespace React.Android

open Android.Content
open Android.Views
open ImmutableCollections
open React
open System
open System.Reactive.Concurrency

type AndroidViewCreator = (Exception -> unit) (* onError *) ->
                            (string (* view name *) -> IReactView<View>) ->
                            IReactView<View>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ReactView =
  let render
      (nativeViews: IImmutableMap<string, Context -> AndroidViewCreator>)
      (context: Context)
      (element: ReactElement) =

    let createNativeView onError viewCreator name =
      let createAndroidView = (nativeViews |> ImmutableMap.get name) context
      createAndroidView onError viewCreator

    ReactView.render Scheduler.mainLoopScheduler createNativeView element
