namespace React.Android.Widget

open Android.Views
open Android.Widget
open ImmutableCollections
open React
open React.Android.Views
open System
open System.Reactive.Disposables

module FSXObservable = FSharp.Control.Reactive.Observable

module internal WidgetImpl =
  let AndroidWidgetButton = "Android.Widget.Button"
  let AndroidWidgetLinearLayout = "Android.Widget.LinearLayout"

  let Button (context: Android.Content.Context) (props: obj) = 
    let view = new Button(context)

    let currentProps = ref None
    let onClickSubscription = ref Disposable.Empty

    let subscribeToOnClick onClick =
      let subscription = !onClickSubscription
      subscription.Dispose() 

      match onClick with
      | None -> ()
      | Some cb -> 
        let subscription = 
          FSXObservable.fromEventPattern "Click" view
          |> Observable.subscribe (fun _ -> cb ())
        onClickSubscription := subscription

    let updateProps (props: obj) =
      let props = (props :?> ButtonProps)
      let oldProps = !currentProps
      currentProps := Some props

      match oldProps with
      | Some oldProps ->
          if oldProps.layoutParams <> props.layoutParams then 
            view.LayoutParameters <- new ViewGroup.LayoutParams(props.layoutParams.width, props.layoutParams.height)
          if oldProps.text <> props.text then 
            view.Text <- props.text

          subscribeToOnClick props.onClick

      | None ->
          view.LayoutParameters <- new ViewGroup.LayoutParams(props.layoutParams.width, props.layoutParams.height)
          view.Text <- props.text
          subscribeToOnClick props.onClick

    updateProps props

    ReactView { new obj()
      interface IReactView with
        member this.Name = AndroidWidgetButton
        member this.UpdateProps props = updateProps props
      interface IDisposable with
        member this.Dispose () = 
          currentProps := None
          view.Dispose()
      interface AndroidReactView with
        member this.View = (view :> View)
    }

  let LinearLayout (context: Android.Content.Context) (props: obj) =
    let view = new LinearLayout(context)

    let updateProps (props: obj) =
      let props = (props :?> LinearLayoutProps)
      view.LayoutParameters <- new LinearLayout.LayoutParams(props.layoutParams.width, props.layoutParams.height, props.layoutParams.weight)
      view.Orientation <- props.orientation

    updateProps props

    let children = ref Collection.empty

    ReactViewGroup { new obj()
        interface IReactViewGroup with 
          member this.Name = AndroidWidgetLinearLayout
          member this.UpdateProps props = updateProps props
          member this.Children 
            with get () = !children
            and set (value) =
              let oldChildren = !children

              Seq.zipAll oldChildren value
              |> Seq.iteri (
                fun indx (prev, next) ->
                  match (prev, next) with
                  | (Some (prevKey, prevChild), Some(nextKey, nextChild)) when prevKey = nextKey -> ()
                  | (Some (prevKey, prevChild), Some(nextKey, nextChild)) ->
                      view.RemoveViewAt indx

                      match AndroidReactView.getView nextChild with
                      | Some child -> view.AddView (child , indx)
                      | None -> ()

                  | (Some (_, prevChild), None) ->
                      match AndroidReactView.getView prevChild with
                      | Some child -> view.RemoveView child
                      | None -> ()

                  | (None, Some(_, nextChild)) ->
                      match AndroidReactView.getView nextChild with
                      | Some child -> view.AddView child
                      | None -> ()

                  | (None, None) -> ()
                ) 

              children := value
        interface IDisposable with
          member this.Dispose () = view.Dispose()
        interface AndroidReactView with
          member this.View = (view :> View)
    }
