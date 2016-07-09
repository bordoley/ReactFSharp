namespace TestAppAndroid

open Android.App
open Android.Content
open Android.Graphics
open Android.OS
open Android.Runtime
open Android.Views
open Android.Widget
open FSharp.Control.Reactive
open ImmutableCollections
open React
open React.Android.Views
open React.Android.Widget
open System

type MyComponentProps = {
  onClick: Func<unit, unit>
  count: int
}

[<Activity (Label = "ReactFSharp", MainLauncher = true)>]
type MainActivity () =
  inherit Activity ()

  let fillWidthWrapHeightLayoutParams = new LinearLayout.LayoutParams(-1, -2)

  let MyComponent = ReactComponent.makeLazy (fun (props: MyComponentProps) ->
    Components.LinearLayout {
      props =
        { LinearLayoutProps.Default with
            orientation = Orientation.Vertical
        }
      children =
        [|
          ( "Toolbar",
            Components.Toolbar {
              ToolbarProps.Default with
                layoutParameters = fillWidthWrapHeightLayoutParams
                subTitle = "a subtitle"
                title = "React FSharp App"
            }
          )

          ( "button",
            Components.Button {
              TextViewProps.Default with
                layoutParameters = fillWidthWrapHeightLayoutParams
                text = "Click on me to increment"
                onClick = props.onClick
             }
          )

          ( "textView",
            Components.TextView {
              TextViewProps.Default with
                clickable = false
                layoutParameters = fillWidthWrapHeightLayoutParams
                text = sprintf "count %i" props.count
            }
          )
        |]
    }
  )

  let MyStatefulComponent =
    let reducer state _ = state + 1

    let shouldUpdate old updated = true

    let actions = new Event<unit>()
    let onClick = Func<unit, unit>(actions.Trigger)

    let render (props: unit, state: int) = MyComponent {
      onClick = onClick
      count = state
    }

    let actions =
      actions.Publish
      |> Observable.observeOn (System.Reactive.Concurrency.NewThreadScheduler())

    let StatefulComponent = ReactComponent.stateReducing render reducer shouldUpdate 0 actions

    StatefulComponent ()

  override this.OnCreate (bundle) =
    base.OnCreate (bundle)

    let views =
      PersistentMap.create
        [|
          (Button.name, Button.createView)
          (LinearLayout.name, LinearLayout.createView)
          (TextView.name, TextView.createView)
          (Toolbar.name, Toolbar.createView)
        |]

    let view = MyStatefulComponent |> ReactView.render views this

    let updateView = function
      | Some (view: obj) ->
          this.SetContentView (view :?> View)
      | None -> this.SetContentView null

    let viewSubscription = view |> ReactView.bindToNativeViewContainer updateView
    ()
