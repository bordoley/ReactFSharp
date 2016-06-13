namespace React.Android

open System
open System.Reactive.Disposables
open React

module FSXObservable = FSharp.Control.Reactive.Observable

module AndroidNativeView =
  open Android.Views

  type AndroidNativeView =
    abstract member View: View with get

  let getView = function
    | ReactStatelessView view -> (view :?> AndroidNativeView).View
    | ReactStatefulView view -> (view :?> AndroidNativeView).View
    | ReactViewWithChild view -> (view :?> AndroidNativeView).View
    | ReactViewWithChildren view -> (view :?> AndroidNativeView).View

module NativeWidget =
  open Android.Views
  open Android.Widget
  open AndroidNativeView

  type ViewGroupLayoutParams = 
    struct
      val width: int
      val height: int
      new(width: int, height: int) = { width = width; height = height }
    end
 
  type LinearLayoutLayoutParams = 
    struct
      val width: int
      val height: int
      val weight: float32
      new(width: int, height: int, weight: float32) = { width = width; height = height; weight = weight }
    end

  type LinearLayoutProps = {
    layoutParams: LinearLayoutLayoutParams
    orientation: Orientation
  } 

  type ButtonProps = {
    layoutParams: ViewGroupLayoutParams
    text: string
    onClick: Option<unit -> unit>
  }

  let statefulViewProvider (context: Android.Content.Context) (id:int, state: IObservable<Option<ReactView>>) = 
     let view = new FrameLayout(context);
     view.LayoutParameters <- new ViewGroup.LayoutParams(-2, -2)

     let subscription = 
        state
        |> Observable.scan
            (fun prevView nextView ->
              view.RemoveAllViews ()
              prevView |> ReactView.dispose

              match nextView with
              | None -> ()
              | Some childView ->
                  view.AddView (getView childView)

              nextView
            )
            None
        |> FSXObservable.last
        |> Observable.subscribe (fun view -> view |> ReactView.dispose)

     ReactStatefulView { new obj()
       interface ReactStatefulView with
         member this.Id = id
         member this.State = state
       interface IDisposable with
         member this.Dispose () = 
           subscription.Dispose()
           view.Dispose()
       interface AndroidNativeView with
         member this.View = (view :> View)
     }

  let viewProvider context =
    let Button (props: obj) = 
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

      ReactStatelessView { new obj()
        interface ReactStatelessView with
          member this.Name = "Android.Widget.Button"
          member this.UpdateProps props = updateProps props
        interface IDisposable with
          member this.Dispose () = 
            currentProps := None
            view.Dispose()
        interface AndroidNativeView with
          member this.View = (view :> View)
      }

    let LinearLayout (props: obj) =
      let view = new LinearLayout(context)

      let updateProps (props: obj) =
        let props = (props :?> LinearLayoutProps)
        view.LayoutParameters <- new LinearLayout.LayoutParams(props.layoutParams.width, props.layoutParams.height, props.layoutParams.weight)
        view.Orientation <- props.orientation

      updateProps props

      let children = ref ReactChildren.empty

      ReactViewWithChildren { new obj()
          interface ReactViewWithChildren with 
            member this.Name = "Android.Widget.LinearLayout"
            member this.UpdateProps props = updateProps props
            member this.Children 
              with get () = !children
              and set (value) =
                let oldChildren = !children

                ReactChildren.iteri2optional (
                  fun prev next indx -> 
                    match (prev, next) with
                    | (Some (prevKey, prevChild), Some(nextKey, nextChild)) when prevKey = nextKey -> ()
                    | (Some (prevKey, prevChild), Some(nextKey, nextChild)) ->
                        let newChildView = getView nextChild

                        view.RemoveViewAt indx
                        view.AddView(newChildView, indx)
                        ()

                    | (Some (_, prevChild), None) ->
                        view.RemoveView (getView prevChild)

                    | (None, Some(_, nextChild)) ->
                        view.AddView (getView nextChild)

                    | (None, None) -> ()

                ) value oldChildren

                children := value
          interface IDisposable with
            member this.Dispose () = view.Dispose()
          interface AndroidNativeView with
            member this.View = (view :> View)
      }

    Map.empty
    |> Map.add "Android.Widget.Button" Button
    |> Map.add "Android.Widget.LinearLayout" LinearLayout


module Widget =
  type LinearLayoutComponentProps = {
    props: NativeWidget.LinearLayoutProps
    children: ReactChildren<ReactElement>
  }

  let LinearLayout = ReactStatelessComponent (fun (props: LinearLayoutComponentProps) -> ReactNativeElementWithChildren {
    name = "Android.Widget.LinearLayout"
    props = props.props
    children = props.children
  })

  let Button = ReactStatelessComponent (fun (props: NativeWidget.ButtonProps) -> ReactNativeElement {
    name = "Android.Widget.Button"
    props = props
  })


