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
  let render
      (nativeViews: IImmutableMap<string, Context -> obj -> ReactView>)
      (context: Context)
      (element: ReactElement) =

    let createView name = (nativeViews |> ImmutableMap.get name) context
    ReactView.render Scheduler.mainLoopScheduler createView element
