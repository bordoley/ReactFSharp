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
  onClick: unit -> unit
  count: int
  layoutParameters: ViewGroup.LayoutParams
}

[<Activity (Label = "ReactFSharp", MainLauncher = true)>]
type MainActivity () =
  inherit Activity ()

  let toolbarLayoutParams = new LinearLayout.LayoutParams(-1, -2)

  let MyComponent = ReactComponent.makeLazy (fun (props: MyComponentProps) ->
    Components.LinearLayout {
      props = 
        { LinearLayoutProps.Default with
            layoutParameters = props.layoutParameters
            orientation = Android.Widget.Orientation.Vertical
        }
      children =
        [|
          ( "Toolbar", 
            Components.Toolbar {
              ToolbarProps.Default with
                layoutParameters = toolbarLayoutParams
                subTitle = "a subtitle"
                title = "React FSharp App"
            }
          )

          ( "button", 
            Components.Button {
              TextViewProps.Default with
                layoutParameters = new LinearLayout.LayoutParams(-1, -2)
                text = "Click on me to increment"
                onClick = props.onClick
             }
          )

          ( "textView", 
            Components.TextView {
              TextViewProps.Default with
                clickable = false
                layoutParameters = new LinearLayout.LayoutParams(-1, -1)
                text = sprintf "count %i" props.count
            }
          )
        |]
    }
  )

  let MyStatefulComponent =
    let reducer state _ = state + 1

    let shouldUpdate old updated = true

    let render 
      (
        (onClick, layoutParameters): ((unit -> unit) * FrameLayout.LayoutParams), 
        state: int
      ) = MyComponent {
        onClick = onClick
        count = state
        layoutParameters = layoutParameters
      }
  
    let action = new Event<unit>()

    let actions = 
      action.Publish
      |> Observable.observeOn (System.Reactive.Concurrency.NewThreadScheduler())

    let StatefulComponent = ReactComponent.stateReducing render reducer shouldUpdate 0 actions

    StatefulComponent (action.Trigger, new FrameLayout.LayoutParams(-1, -1))

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
