namespace TestAppAndroid

open System

open Android.App
open Android.Content
open Android.OS
open Android.Runtime
open Android.Views
open Android.Widget

open React
open React.Android
open React.Android.Views
open React.Android.Widget

module FSXObservable = FSharp.Control.Reactive.Observable

module Views =
  let StatefulButton = ReactStatefulComponent (fun props ->
    let action = new Event<unit>()
     
    let reducer (state, _) = state + 1
  
    let render (props, state: int) = 
      Components.Button >>= 
        { TextView.defaultProps with
            layoutParameters = new FrameLayout.LayoutParams(new ViewGroup.LayoutParams(-1, -2))
            text = sprintf "count %i" state
            onClick = Some action.Trigger
        }

    let actions = 
      action.Publish
      |> FSXObservable.observeOn (System.Reactive.Concurrency.NewThreadScheduler())

    ReactComponent.stateReducing render reducer 0 actions props
  )
      
  let MyComponent = ReactStatelessComponent (fun (props: unit) -> 
    Components.LinearLayout >>= {
      props = 
        { LinearLayout.defaultProps with
            layoutParameters = new LinearLayout.LayoutParams(-1, -1, (float32 0.0))
            orientation = Android.Widget.Orientation.Vertical
        }
      children = %% 
        [| 
          ("child1", StatefulButton >>= ())
          ("child2", StatefulButton >>= ())
          ("child3", StatefulButton >>= ()) 
        |]
    }
  )

[<Activity (Label = "ReactFSharp", MainLauncher = true)>]
type MainActivity () =
  inherit Activity ()

  override this.OnCreate (bundle) =
    base.OnCreate (bundle)

    let viewProvider = AndroidReactView.createViewProvider (this :> Context) Widgets.views
    let render = AndroidReactView.render viewProvider

    let element = Views.MyComponent >>= ()
    let view = render element

    match AndroidReactView.getView view with
    | Some view -> this.SetContentView view
    | None -> failwith "ugghh"
