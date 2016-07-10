namespace React.Android.Views

open Android.Content
open Android.Views
open ImmutableCollections
open React
open React.Android
open System
open System.Reactive.Concurrency

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ReactView =
  let render
      (nativeViews: IImmutableMap<string, Context -> (Exception -> unit) -> obj -> ReactView>)
      (context: Context)
      (element: ReactElement) =

    let createView onError name =
      (nativeViews |> ImmutableMap.get name) context onError

    ReactView.render Scheduler.mainLoopScheduler createView element
