namespace TestAppAndroid

open System

open Android.App
open Android.Content
open Android.Graphics
open Android.OS
open Android.Runtime
open Android.Views
open Android.Widget

open React
open React.Android
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
        { LinearLayout.defaultProps with
            layoutParameters = props.layoutParameters
            orientation = Android.Widget.Orientation.Vertical
        }
      children = %% 
        [|
          ("Toolbar", Components.Toolbar >>= {
              Toolbar.defaultProps with
                layoutParameters = new LinearLayout.LayoutParams(-1, -2)
                subTitle = "a subtitle"
                title = "React FSharp App"
            })

          ("button", Components.Button >>= {
              TextView.defaultProps with
                layoutParameters = new LinearLayout.LayoutParams(-1, -2)
                text = "Click on me to increment"
                onClick = Some props.onClick
            })

          ("textView", Components.TextView >>= {
              TextView.defaultProps with
                clickable = false
                layoutParameters = new LinearLayout.LayoutParams(-1, -1)
                text = sprintf "count %i" props.count
            })
        |]
    }
  )

  let MyStatefulComponent = 
    let reducer state _ = state + 1

    let shouldUpdate old updated = true

    let action = new Event<unit>()

    let render (props, state: int) = MyComponent >>= {
      onClick = action.Trigger
      count = state
      layoutParameters = new FrameLayout.LayoutParams(-1, -1)
    }
  
    let actions = 
      action.Publish
      |> FSXObservable.observeOn (System.Reactive.Concurrency.NewThreadScheduler())

    ReactStatefulComponent
      (ReactComponent.stateReducing render reducer shouldUpdate 0 actions)

  override this.OnCreate (bundle) =
    base.OnCreate (bundle)

    let viewProvider = AndroidReactView.createViewProvider (this :> Context) Widgets.views
    let render = AndroidReactView.render viewProvider

    let element = MyStatefulComponent >>= ()
    let view = render element

    match AndroidReactView.getView view with
    | Some view -> this.SetContentView view
    | None -> failwith "ugghh"
