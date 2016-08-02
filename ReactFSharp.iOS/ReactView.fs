namespace React.iOS

open ImmutableCollections
open React
open System
open System.Reactive.Concurrency
open UIKit

type IOSViewCreator = (Exception -> unit) (* onError *) ->
                        (string (* view name *) -> IReactView<UIView>) ->
                        IReactView<UIView>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ReactView =
  let render
      (nativeViews: IImmutableMap<string, IOSViewCreator>)
      (element: ReactElement) =

    let createNativeView onError viewCreator name =
      let createIOSView = (nativeViews |> ImmutableMap.get name)
      createIOSView onError viewCreator

    ReactView.render Scheduler.mainLoopScheduler createNativeView element

