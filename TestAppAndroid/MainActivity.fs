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
    let focusEvent = new Event<unit>()

    Components.LinearLayout {
      props =
        { LinearLayoutProps.Default with
            orientation = (int) Orientation.Vertical
        }
      children = ImmutableVector.createUnsafe
        [|
          Components.Toolbar {
            props = 
              { ToolbarProps.Default with
                  layoutParameters = fillWidthWrapHeightLayoutParams
                  subTitle = "a subtitle"
                  title = "React FSharp App"
              }
            children = ImmutableMap.empty ()
          }

          Components.Button {
            TextViewProps.Default with
              backgroundColor = Color.Black
              enabled = true
              layoutParameters = fillWidthWrapHeightLayoutParams
              text = "Click on me to increment"
              onClick = props.onClick
           }

          Components.Button {
            TextViewProps.Default with
              backgroundColor = Color.Black
              enabled = true
              layoutParameters = fillWidthWrapHeightLayoutParams
              text = "Move focus"
              onClick = Func<unit, unit>(focusEvent.Trigger)
           }

          Components.TextView {
            TextViewProps.Default with
              enabled = true
              focusable = true
              focusableInTouchMode = true
              layoutParameters = fillWidthWrapHeightLayoutParams
              text = sprintf "count %i" props.count
          }

          Components.EditText {
            EditTextProps.Default with
              enabled = true
              focusable = true
              focusableInTouchMode = true
              layoutParameters = fillWidthWrapHeightLayoutParams
              requestFocus = focusEvent.Publish
              text = "I should have focus"
          }
        |]
    }
  )

  let MyStatefulComponent () =
    let scheduler = System.Reactive.Concurrency.Scheduler.TaskPool

    let reducer state _ = state + 1

    let shouldUpdate old updated = true

    let actions = new Event<unit>()
    let onClick = Func<unit, unit>(actions.Trigger)

    let render (props: unit, state: int) = MyComponent {
      onClick = onClick
      count = state
    }

    let actions = actions.Publish |> Observable.observeOn scheduler

    let StatefulComponent = ReactComponent.stateReducing render reducer shouldUpdate 0 actions

    StatefulComponent ()

  override this.OnCreate (bundle) =
    base.OnCreate (bundle)

    let onError exn =
      Console.WriteLine (exn.ToString ())
      this.RunOnUiThread (fun () -> raise exn)

    let views =
      ImmutableMap.create
        [|
          Button.viewProvider
          EditText.viewProvider
          FrameLayout.viewProvider
          GridView.viewProvider
          ImageView.viewProvider
          LinearLayout.viewProvider
          //ListView.viewProvider
          RatingBar.viewProvider
          RelativeLayout.viewProvider
          Space.viewProvider
          SwipeRefreshLayout.viewProvider
          TextView.viewProvider
          Toolbar.viewProvider
          ViewPager.viewProvider
        |]

    let updateView = function
      | Some (view: View) ->
          this.SetContentView (view)
      | None -> this.SetContentView null

    let subscription = 
      MyStatefulComponent ()
      |> ReactView.render views this 
      |> Observable.subscribeWithError updateView onError

    ()
