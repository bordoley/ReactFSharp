﻿namespace TestAppAndroid

open Android.App
open Android.Content
open Android.Graphics
open Android.Graphics.Drawables
open Android.OS
open Android.Runtime
open Android.Support.V7.Widget
open Android.Views
open Android.Widget
open FSharp.Control.Reactive
open ImmutableCollections
open React
open React.Android
open React.Android.Views
open React.Android.Widget
open System

type MyComponentProps = {
  onClick: Action
  count: int
}

[<Activity (Label = "ReactFSharp", MainLauncher = true)>]
type MainActivity () =
  inherit Activity ()

  let fillWidthWrapHeightLayoutParams =
    Func<ViewGroup.LayoutParams>(fun () -> new LinearLayoutCompat.LayoutParams(-1, -2) :> ViewGroup.LayoutParams)

  let redDrawable =
    Func<Drawable>(fun () -> new ColorDrawable(Color.Red) :> Drawable)

  let focusEvent = new Event<unit>()
  let focusDownRequested =
    focusEvent.Publish |> Observable.map (fun _ -> FocusSearchDirection.Down)
  let requestFocusDown = Action focusEvent.Trigger

  let MyComponent = ReactComponent.makeLazy (fun (props: MyComponentProps) ->
    Components.LinearLayout {
      LinearLayoutProps.Default with
        orientation = (int) Orientation.Vertical
        Padding =
          {
            Top = 10
            Bottom = 10
            Start = 10
            End = 10
          }
    } ( ImmutableVector.createUnsafe
          [|
            Components.Toolbar {
              ToolbarProps.Default with
                LayoutParameters = fillWidthWrapHeightLayoutParams
                subTitle = "a subtitle"
                title = "React FSharp App"
            } (ImmutableMap.empty ())

            Components.Button {
              ButtonProps.Default with
                Activated = true
                Background = redDrawable
                Clickable = true
                Enabled = true
                LayoutParameters = fillWidthWrapHeightLayoutParams
                text = "Click on me to increment"
                OnClick = props.onClick
             }

            Components.Button {
              ButtonProps.Default with
                Activated = true
                Background = redDrawable
                Clickable = true
                Enabled = true
                LayoutParameters = fillWidthWrapHeightLayoutParams
                text = "Move focus"
                OnClick = requestFocusDown
             }

            Components.TextView {
              TextViewProps.Default with
                Enabled = true
                Focusable = true
                FocusableInTouchMode = true
                LayoutParameters = fillWidthWrapHeightLayoutParams
                text = sprintf "count %i" props.count
            }

            Components.EditText {
              EditTextProps.Default with
                Enabled = true
                Focusable = true
                FocusableInTouchMode = true
                LayoutParameters = fillWidthWrapHeightLayoutParams
                RequestFocus = focusDownRequested
                text = "I should have focus"
            }
          |]
        )
  )

  let MyStatefulComponent () =
    let scheduler = System.Reactive.Concurrency.Scheduler.TaskPool

    let reducer state _ = state + 1

    let shouldUpdate old updated = true

    let actions = new Event<unit>()
    let onClick = Action actions.Trigger

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
