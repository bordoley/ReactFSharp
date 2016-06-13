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


module Views =
  open React.Android.NativeWidget
  open React.Android.Widget

  let MyButton = ReactStatelessComponent (fun (props: unit) ->
    Button >>= { 
      layoutParams = ViewGroupLayoutParams(300, 100)
      text = "just a test" 
      onClick = None
    }
  )

  let StatefulButton: ReactComponent<unit> = ReactStatefulComponent (fun props ->
    let action = new Event<unit>()
  
    let reducer (state, _) = state + 1
  
    let render (props, state: int) = Button >>= { 
      layoutParams = ViewGroupLayoutParams(300, 100)
      text = sprintf "count %i" state
      onClick = Some action.Trigger
    }

    ReactComponent.stateReducing render reducer 0 action.Publish props
  )
      

  let MyComponent = ReactStatelessComponent (fun (props: unit) -> 
    LinearLayout >>= {
      props = 
        {
          layoutParams = LinearLayoutLayoutParams(-1, -1,  (float32 0.0))
          orientation = Android.Widget.Orientation.Vertical
        }
      children = %% 
        [| 
          ("child1", MyButton >>= ())
          ("child2", StatefulButton >>= ())
          ("child3", StatefulButton >>= ()) 
        |]
    }
  )

[<Activity (Label = "ReactFSharp", MainLauncher = true, Icon = "@mipmap/icon")>]
type MainActivity () =
    inherit Activity ()

    override this.OnCreate (bundle) =
        base.OnCreate (bundle)

        let element = Views.MyComponent >>= ()
        let rootNode = ReactNoneDOMNode |> ReactDom.updateWith element
        let viewProvider = React.Android.NativeWidget.viewProvider this
        let statefulViewProvider = React.Android.NativeWidget.statefulViewProvider this
        let updateWith = ReactView.updateWith AndroidScheduler.scheduler viewProvider statefulViewProvider
        let layout = None |> updateWith rootNode

        match layout with
        | Some layout -> 
          let view = React.Android.AndroidNativeView.getView layout
          this.SetContentView view
        | None -> failwith "ugghh"
