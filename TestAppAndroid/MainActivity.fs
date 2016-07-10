namespace TestAppAndroid

open Android.App
open Android.Content
open Android.Graphics
open Android.OS
open Android.Runtime
open Android.Support.V7.Widget
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

  let fillWidthWrapHeightLayoutParams = new LinearLayoutCompat.LayoutParams(-1, -2)

  let MyComponent = ReactComponent.makeLazy (fun (props: MyComponentProps) ->
    Components.LinearLayout {
      props =
        { LinearLayoutProps.Default with
            orientation = (int) Orientation.Vertical
        }
      children =
        [|
          ( "Toolbar",
            Components.Toolbar {
              props = 
                { ToolbarProps.Default with
                    layoutParameters = fillWidthWrapHeightLayoutParams
                    subTitle = "a subtitle"
                    title = "React FSharp App"
                }
              children = ImmutableMap.empty ()
            }
          )

          ( "button",
            Components.Button {
              TextViewProps.Default with
                backgroundColor = Color.Black
                enabled = true
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
      ImmutableMap.create
        [|
          Button.viewProvider
          LinearLayout.viewProvider
          TextView.viewProvider
          Toolbar.viewProvider
        |]

    let view = MyStatefulComponent |> ReactView.render views this

    let updateView = function
      | Some (view: View) ->
          this.SetContentView (view)
      | None -> this.SetContentView null

    let onError exn =
      Console.WriteLine (exn.ToString ())
      raise exn

    let viewSubscription = 
      view |> ReactView.observe |> Observable.subscribeWithError updateView onError
    ()
