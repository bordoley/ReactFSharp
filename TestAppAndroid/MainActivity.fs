namespace TestAppAndroid

open System

open Android.App
open Android.Content
open Android.Graphics
open Android.OS
open Android.Runtime
open Android.Views
open Android.Widget

open ImmutableCollections

open React
open React.Android.Views
open React.Android.Widget

module FSXObservable = FSharp.Control.Reactive.Observable

type MyComponentProps = {
  onClick: unit -> unit
  count: int
  layoutParameters: ViewGroup.LayoutParams
}

[<Activity (Label = "ReactFSharp", MainLauncher = true)>]
type MainActivity () =
  inherit Activity ()

  let MyComponent = ReactStatelessComponent (fun (props: MyComponentProps) ->
    Components.LinearLayout >>= {
      props = 
        LinearLayoutProps (
          layoutParameters = props.layoutParameters,
          orientation = Android.Widget.Orientation.Vertical
        )
      children = %% 
        [|
          ("Toolbar", 
            Components.Toolbar 
            >>= ToolbarProps (
                  layoutParameters = new LinearLayout.LayoutParams(-1, -2),
                  subTitle = "a subtitle",
                  title = "React FSharp App"
                )
          )

          ("button", 
            Components.Button 
            >>= TextViewProps (
                  layoutParameters = new LinearLayout.LayoutParams(-1, -2),
                  text = "Click on me to increment",
                  onClick = props.onClick
                 )
          )

          ("textView", 
            Components.TextView 
            >>= TextViewProps (
                  clickable = false,
                  layoutParameters = new LinearLayout.LayoutParams(-1, -1),
                  text = sprintf "count %i" props.count
                )
          )
        |]
    }
  )

  let MyStatefulComponent = ReactStatelessComponent (fun (props: unit) ->
    let reducer state _ = state + 1

    let shouldUpdate old updated = true

    let render 
      (
        (onClick, layoutParameters): ((unit -> unit) * FrameLayout.LayoutParams), 
        state: int
      ) = MyComponent >>= {
        onClick = onClick
        count = state
        layoutParameters = layoutParameters
      }
  
    let action = new Event<unit>()

    let actions = 
      action.Publish
      |> FSXObservable.observeOn (System.Reactive.Concurrency.NewThreadScheduler())

    let StatefulComponent = ReactStatefulComponent.create render reducer shouldUpdate 0 actions

    StatefulComponent >>= (action.Trigger, new FrameLayout.LayoutParams(-1, -1))
  )

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

    let element = MyStatefulComponent >>= ()
    let view = element |> ReactView.render views this

    let updateView = function
      | Some (view: obj) ->
          this.SetContentView (view :?> View)
      | None -> this.SetContentView null

    let viewSubscription = view |> ReactView.bindToNativeViewContainer updateView 
    ()
