namespace React.Android.Views

open Android.Animation
open Android.Content.Res
open Android.Graphics
open Android.Graphics.Drawables
open Android.Support.V4.View
open Android.Views
open FSharp.Control.Reactive
open React
open React.Android
open System
open System.Reactive.Disposables
open System.Runtime.CompilerServices

type ViewPadding = {
  Bottom: int
  End: int
  Start: int
  Top: int
}

type IViewProps =
  abstract member AccessibilityLiveRegion: int
  abstract member AccessibilityTraversalAfter: int
  abstract member AccessibilityTraversalBefore: int
  abstract member Activated: bool
  abstract member Alpha: float32
  abstract member Background: Func<Drawable>
  abstract member CameraDistance: Single
  abstract member Clickable: bool
  abstract member ClipBounds: Func<Option<Rect>>
  abstract member ClipToOutline: bool
  abstract member ContextClickable: bool
  abstract member ContentDescription: string
  abstract member DrawingCacheBackgroundColor: Color
  abstract member DrawingCacheEnabled: bool
  abstract member DrawingCacheQuality: DrawingCacheQuality
  abstract member Elevation: Single
  abstract member Enabled: bool
  abstract member FadingEdgeLength:int
  abstract member FitsSystemWindows: bool
  abstract member FilterTouchesWhenObscured: bool
  abstract member Focusable: bool
  abstract member FocusableInTouchMode: bool
  abstract member HapticFeedbackEnabled: bool
  abstract member HorizontalFadingEdgeEnabled: bool
  abstract member HorizontalScrollBarEnabled: bool
  abstract member Id: int
  abstract member ImportantForAccessibility: int
  abstract member IsScrollContainer: bool
  abstract member KeepScreenOn: bool
  abstract member LabelFor: int
  abstract member LayerPaint: Func<Paint>
  abstract member LayerType: int
  abstract member LayoutDirection: int
  abstract member LayoutParameters: Func<ViewGroup.LayoutParams>
  abstract member LongClickable: bool
  abstract member MinHeight: int
  abstract member MinWidth: int
  abstract member NestedScrollingEnabled: bool
  abstract member NextFocusDownId: int
  abstract member NextFocusForwardId: int
  abstract member NextFocusLeftId: int
  abstract member NextFocusRightId: int
  abstract member NextFocusUpId: int
  abstract member OnClick: Action
  abstract member OnCreateContextMenu: Action<IContextMenu, IContextMenuContextMenuInfo>
  abstract member OnDrag: Func<DragEvent, bool>
  abstract member OnFocusChange: Action<bool>
  abstract member OnGenericMotion: Func<MotionEvent, bool>
  abstract member OnHover: Func<MotionEvent, bool>
  abstract member OnKey: Func<Keycode, KeyEvent, bool>
  abstract member OnLongClick: Func<bool>
  abstract member OnSystemUiVisibilityChange: Action<StatusBarVisibility>
  abstract member OnTouch: Func<MotionEvent, bool>
  //abstract member OutlineProvider: ViewOutlineProvider
  abstract member OverScrollMode: int
  abstract member Padding: ViewPadding
  abstract member Pivot: Single (* X *) * Single (* Y *)
  abstract member RequestFocus: IObservable<FocusSearchDirection>
  abstract member Rotation: Single (* Pivot *) * Single (* X *) * Single (* Y *)
  abstract member Scale: Single (* X *) * Single (* Y *)
  abstract member ScrollBarDefaultDelayBeforeFade: int32
  abstract member ScrollBarFadeDuration: int32
  abstract member ScrollBarFadingEnabled: bool
  abstract member ScrollBarSize: int
  abstract member ScrollBarStyle: ScrollbarStyles
  abstract member ScrollBy: IObservable<int * int>
  abstract member ScrollTo: IObservable<int * int>
  abstract member Selected: bool
  abstract member SoundEffectsEnabled: bool
  //abstract member StateListAnimator: StateListAnimator
  abstract member SystemUiVisibility: StatusBarVisibility
  abstract member TextAlignment: TextAlignment
  abstract member TextDirection: TextDirection
  abstract member TransitionName: string
  abstract member Translation: Single (* X *) * Single (* Y *) * Single (* Z *)
  abstract member VerticalFadingEdgeEnabled: bool
  abstract member VerticalScrollBarEnabled: bool
  abstract member VerticalScrollbarPosition: ScrollbarPosition
  abstract member Visibility: ViewStates

type ViewProps =
  {
    AccessibilityLiveRegion: int
    AccessibilityTraversalAfter: int
    AccessibilityTraversalBefore: int
    Activated: bool
    Alpha: float32
    Background: Func<Drawable>
    CameraDistance: Single
    Clickable: bool
    ClipBounds: Func<Option<Rect>>
    ClipToOutline: bool
    ContextClickable: bool
    ContentDescription: string
    DrawingCacheBackgroundColor: Color
    DrawingCacheEnabled: bool
    DrawingCacheQuality: DrawingCacheQuality
    Elevation: Single
    Enabled: bool
    FadingEdgeLength:int
    FitsSystemWindows: bool
    FilterTouchesWhenObscured: bool
    Focusable: bool
    FocusableInTouchMode: bool
    HapticFeedbackEnabled: bool
    HorizontalFadingEdgeEnabled: bool
    HorizontalScrollBarEnabled: bool
    Id: int
    ImportantForAccessibility: int
    IsScrollContainer: bool
    KeepScreenOn: bool
    LabelFor: int
    LayerPaint: Func<Paint>
    LayerType: int
    LayoutDirection: int
    LayoutParameters: Func<ViewGroup.LayoutParams>
    LongClickable: bool
    MinHeight: int
    MinWidth: int
    NestedScrollingEnabled: bool
    NextFocusDownId: int
    NextFocusForwardId: int
    NextFocusLeftId: int
    NextFocusRightId: int
    NextFocusUpId: int
    OnClick: Action
    OnCreateContextMenu: Action<IContextMenu, IContextMenuContextMenuInfo>
    OnDrag: Func<DragEvent, bool>
    OnFocusChange: Action<bool>
    OnGenericMotion: Func<MotionEvent, bool>
    OnHover: Func<MotionEvent, bool>
    OnKey: Func<Keycode, KeyEvent, bool>
    OnLongClick: Func<bool>
    OnSystemUiVisibilityChange: Action<StatusBarVisibility>
    OnTouch: Func<MotionEvent, bool>
    //OutlineProvider: ViewOutlineProvider
    OverScrollMode: int
    Padding: ViewPadding
    Pivot: Single * Single
    RequestFocus: IObservable<FocusSearchDirection>
    Rotation: Single * Single * Single
    Scale: Single * Single
    ScrollBarDefaultDelayBeforeFade: int32
    ScrollBarFadeDuration: int32
    ScrollBarFadingEnabled: bool
    ScrollBarSize: int
    ScrollBarStyle: ScrollbarStyles
    ScrollBy: IObservable<int * int>
    ScrollTo: IObservable<int * int>
    Selected: bool
    SoundEffectsEnabled: bool
    //StateListAnimator: StateListAnimator
    SystemUiVisibility: StatusBarVisibility
    TextAlignment: TextAlignment
    TextDirection: TextDirection
    TransitionName: string
    Translation: Single * Single * Single
    VerticalFadingEdgeEnabled: bool
    VerticalScrollBarEnabled: bool
    VerticalScrollbarPosition: ScrollbarPosition
    Visibility: ViewStates
  }

  interface IViewProps with
    // View Props
    member this.AccessibilityLiveRegion = this.AccessibilityLiveRegion
    member this.AccessibilityTraversalAfter = this.AccessibilityTraversalAfter
    member this.AccessibilityTraversalBefore = this.AccessibilityTraversalBefore
    member this.Activated = this.Activated
    member this.Alpha = this.Alpha
    member this.Background = this.Background
    member this.CameraDistance = this.CameraDistance
    member this.Clickable = this.Clickable
    member this.ClipBounds = this.ClipBounds
    member this.ClipToOutline = this.ClipToOutline
    member this.ContentDescription = this.ContentDescription
    member this.ContextClickable = this.ContextClickable
    member this.DrawingCacheBackgroundColor = this.DrawingCacheBackgroundColor
    member this.DrawingCacheEnabled = this.DrawingCacheEnabled
    member this.DrawingCacheQuality = this.DrawingCacheQuality
    member this.Elevation = this.Elevation
    member this.Enabled = this.Enabled
    member this.FadingEdgeLength = this.FadingEdgeLength
    member this.FitsSystemWindows = this.FitsSystemWindows
    member this.FilterTouchesWhenObscured = this.FilterTouchesWhenObscured
    member this.Focusable = this.Focusable
    member this.FocusableInTouchMode = this.FocusableInTouchMode
    member this.HapticFeedbackEnabled = this.HapticFeedbackEnabled
    member this.HorizontalFadingEdgeEnabled = this.HorizontalFadingEdgeEnabled
    member this.HorizontalScrollBarEnabled = this.HorizontalScrollBarEnabled
    member this.Id = this.Id
    member this.ImportantForAccessibility = this.ImportantForAccessibility
    member this.IsScrollContainer = this.IsScrollContainer
    member this.KeepScreenOn = this.KeepScreenOn
    member this.LabelFor = this.LabelFor
    member this.LayerPaint = this.LayerPaint
    member this.LayerType = this.LayerType
    member this.LayoutDirection = this.LayoutDirection
    member this.LayoutParameters = this.LayoutParameters
    member this.LongClickable = this.LongClickable
    member this.MinHeight = this.MinHeight
    member this.MinWidth = this.MinWidth
    member this.NestedScrollingEnabled = this.NestedScrollingEnabled
    member this.NextFocusDownId = this.NextFocusDownId
    member this.NextFocusForwardId = this.NextFocusForwardId
    member this.NextFocusLeftId = this.NextFocusLeftId
    member this.NextFocusRightId = this.NextFocusRightId
    member this.NextFocusUpId = this.NextFocusUpId
    member this.OnClick = this.OnClick
    member this.OnCreateContextMenu = this.OnCreateContextMenu
    member this.OnDrag = this.OnDrag
    member this.OnFocusChange = this.OnFocusChange
    member this.OnGenericMotion = this.OnGenericMotion
    member this.OnHover = this.OnHover
    member this.OnKey = this.OnKey
    member this.OnLongClick = this.OnLongClick
    member this.OnSystemUiVisibilityChange = this.OnSystemUiVisibilityChange
    member this.OnTouch = this.OnTouch
    //member this.OutlineProvider = this.OutlineProvider
    member this.OverScrollMode = this.OverScrollMode
    member this.Padding = this.Padding
    member this.Pivot = this.Pivot
    member this.RequestFocus = this.RequestFocus
    member this.Rotation = this.Rotation
    member this.Scale = this.Scale
    member this.ScrollBarDefaultDelayBeforeFade = this.ScrollBarDefaultDelayBeforeFade
    member this.ScrollBarFadeDuration = this.ScrollBarFadeDuration
    member this.ScrollBarFadingEnabled = this.ScrollBarFadingEnabled
    member this.ScrollBarSize = this.ScrollBarSize
    member this.ScrollBarStyle = this.ScrollBarStyle
    member this.ScrollBy = this.ScrollBy
    member this.ScrollTo = this.ScrollTo
    member this.Selected = this.Selected
    member this.SoundEffectsEnabled = this.SoundEffectsEnabled
    //member this.StateListAnimator = this.StateListAnimator
    member this.SystemUiVisibility = this.SystemUiVisibility
    member this.TextAlignment = this.TextAlignment
    member this.TextDirection = this.TextDirection
    member this.TransitionName = this.TransitionName
    member this.Translation = this.Translation
    member this.VerticalFadingEdgeEnabled = this.VerticalFadingEdgeEnabled
    member this.VerticalScrollBarEnabled = this.VerticalScrollBarEnabled
    member this.VerticalScrollbarPosition = this.VerticalScrollbarPosition
    member this.Visibility = this.Visibility

// This is a hack around the F# compiler. We want to ensure that
// the default event handlers are values so that they can be effectively
// cached and don't break record equality
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module private ViewProps =
  let private defaultBackground = Func<Drawable>(fun () -> new ColorDrawable(Color.White) :> Drawable)

  let private defaultClipBounds = Func<Option<Rect>>(fun () -> None)

  let private defaultLayerPaint = Func<Paint>(fun () -> new Paint ())

  let private defaultLayoutParameters = Func<ViewGroup.LayoutParams> (fun () -> new ViewGroup.LayoutParams(-1, -1))

  let private defaultOnClick =
    Action(fun () -> ())

  let private defaultOnCreateContextMenu =
    Action<IContextMenu, IContextMenuContextMenuInfo> (fun _ _ -> ())

  let private defaultOnDrag =
    Func<DragEvent, bool> (fun _ -> false)

  let private defaultOnFocusChange =
    Action<bool>(ignore)

  let private defaultOnGenericMotion =
    Func<MotionEvent, bool>(fun _ -> false)

  let private defaultOnHover =
    Func<MotionEvent, bool>(fun _ -> false)

  let private defaultOnKey =
    Func<Keycode, KeyEvent, bool>(fun _ _ -> false)

  let private defaultOnLongClick =
    Func<bool>(fun _ -> false)

  let private defaultOnSystemUiVisibilityChange =
    Action<StatusBarVisibility>(ignore)

  let private defaultOnTouch =
    Func<MotionEvent, bool>(fun _ -> false)

  let private defaultPadding = {
    Bottom = 0
    End = 0
    Start = 0
    Top = 0
  }

  let private defaultPivot = (0.0f, 0.0f)

  let private defaultRequestFocus = Observable.empty<FocusSearchDirection>

  let private defaultRotation = (0.0f, 0.0f, 0.0f)

  let private defaultScale = (1.0f, 1.0f)

  let private defaultScrollBy = Observable.empty<int * int>

  let private defaultScrollTo = Observable.empty<int * int>

  //let private defaultStateListAnimator = new StateListAnimator()

  let private defaultTranslation = (0.0f, 0.0f, 0.0f)

  let internal defaultProps = {
    AccessibilityLiveRegion = ViewCompat.AccessibilityLiveRegionNone
    AccessibilityTraversalAfter = View.NoId
    AccessibilityTraversalBefore = View.NoId
    Activated = false
    Alpha = 1.0f
    Background = defaultBackground
    CameraDistance = 0.0f
    Clickable = false
    ClipBounds = defaultClipBounds
    ClipToOutline = false
    ContentDescription = ""
    ContextClickable = false
    DrawingCacheBackgroundColor = Unchecked.defaultof<Color>
    DrawingCacheEnabled = false
    DrawingCacheQuality = DrawingCacheQuality.Auto
    Elevation = 0.0f
    Enabled = false
    FadingEdgeLength = 0
    FilterTouchesWhenObscured = false
    FitsSystemWindows = true
    Focusable = false
    FocusableInTouchMode = false
    HapticFeedbackEnabled = false
    HorizontalFadingEdgeEnabled = false
    HorizontalScrollBarEnabled = false
    Id = View.NoId
    ImportantForAccessibility = ViewCompat.ImportantForAccessibilityAuto
    IsScrollContainer = false
    KeepScreenOn = false
    LabelFor = View.NoId
    LayerPaint = defaultLayerPaint
    LayerType = ViewCompat.LayerTypeNone
    LayoutDirection = ViewCompat.LayoutDirectionInherit
    LayoutParameters = defaultLayoutParameters
    LongClickable = false
    MinHeight = 0
    MinWidth = 0
    NestedScrollingEnabled = false
    NextFocusDownId = View.NoId
    NextFocusForwardId = View.NoId
    NextFocusLeftId = View.NoId
    NextFocusRightId = View.NoId
    NextFocusUpId = View.NoId
    OnClick = defaultOnClick
    OnCreateContextMenu = defaultOnCreateContextMenu
    OnDrag = defaultOnDrag
    OnFocusChange = defaultOnFocusChange
    OnGenericMotion = defaultOnGenericMotion
    OnHover = defaultOnHover
    OnKey = defaultOnKey
    OnLongClick = defaultOnLongClick
    OnSystemUiVisibilityChange = defaultOnSystemUiVisibilityChange
    OnTouch = defaultOnTouch
    //OutlineProvider = ViewOutlineProvider.Background
    OverScrollMode = ViewCompat.OverScrollIfContentScrolls
    Padding = defaultPadding
    Pivot = defaultPivot
    RequestFocus = defaultRequestFocus
    Rotation = defaultRotation
    Scale = defaultScale
    ScrollBarDefaultDelayBeforeFade = ViewConfiguration.ScrollDefaultDelay
    ScrollBarFadeDuration = ViewConfiguration.ScrollBarFadeDuration
    ScrollBarFadingEnabled = true
    ScrollBarSize = 10
    ScrollBarStyle = ScrollbarStyles.InsideOverlay
    ScrollBy = defaultScrollBy
    ScrollTo = defaultScrollTo
    Selected = false
    SoundEffectsEnabled = true
    //StateListAnimator = defaultStateListAnimator
    SystemUiVisibility =  StatusBarVisibility.Visible
    TextAlignment = TextAlignment.Inherit
    TextDirection = TextDirection.Inherit
    TransitionName = ""
    Translation = defaultTranslation
    VerticalFadingEdgeEnabled = false
    VerticalScrollBarEnabled = false
    VerticalScrollbarPosition = ScrollbarPosition.Default
    Visibility = ViewStates.Visible
  }

type ViewProps with
  static member Default = ViewProps.defaultProps

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module View =
  type private OnClickListener (onClick: Action) =
    inherit Java.Lang.Object ()

    interface View.IOnClickListener with
      member this.OnClick view = onClick.Invoke ()

  type private OnCreateContextMenuListener (onCreateContextMenu: Action<IContextMenu, IContextMenuContextMenuInfo>) =
    inherit Java.Lang.Object ()

    interface View.IOnCreateContextMenuListener with
      member this.OnCreateContextMenu (menu, view, info) = onCreateContextMenu.Invoke(menu, info)

  type private OnDragListener (onDrag: Func<DragEvent, bool>) =
    inherit Java.Lang.Object ()

    interface Android.Views.View.IOnDragListener with
      member this.OnDrag (view, motionEvent) = onDrag.Invoke(motionEvent)

  type private OnFocusChangeListener (onFocusChange: Action<bool>) =
    inherit Java.Lang.Object ()

    interface Android.Views.View.IOnFocusChangeListener with
      member this.OnFocusChange (view, hasFocus) = onFocusChange.Invoke hasFocus

  type private OnGenericMotionListener (onGenericMotion: Func<MotionEvent, bool>) =
    inherit Java.Lang.Object ()

    interface Android.Views.View.IOnGenericMotionListener with
      member this.OnGenericMotion (view, motionEvent) = onGenericMotion.Invoke(motionEvent)

  type private OnHoverListener (onHover: Func<MotionEvent, bool>) =
    inherit Java.Lang.Object ()

    interface Android.Views.View.IOnHoverListener with
      member this.OnHover (view, motionEvent) = onHover.Invoke(motionEvent)

  type private OnKeyListener (onKey: Func<Keycode, KeyEvent, bool>) =
    inherit Java.Lang.Object ()

    interface View.IOnKeyListener with
      member this.OnKey (view, keyCode, keyEvent) = onKey.Invoke(keyCode, keyEvent)

  type private OnLongClickListener (onLongClick: Func<bool>) =
    inherit Java.Lang.Object ()

    interface View.IOnLongClickListener with
      member this.OnLongClick view = onLongClick.Invoke ()

  type private OnSystemUiVisibilityChangeListener (onSystemUiVisibilityChange: Action<StatusBarVisibility>) =
    inherit Java.Lang.Object ()

    interface View.IOnSystemUiVisibilityChangeListener with
      member this.OnSystemUiVisibilityChange(sbv) = onSystemUiVisibilityChange.Invoke(sbv)

  type private OnTouchListener (onTouch: Func<MotionEvent, bool>) =
    inherit Java.Lang.Object ()

    interface Android.Views.View.IOnTouchListener with
      member this.OnTouch (view, motionEvent) = onTouch.Invoke(motionEvent)

  let private viewPropsCache =
    new ConditionalWeakTable<View, IViewProps>()

  let setProps (onError: Exception -> unit) (view: View) (props: IViewProps): IDisposable =
    // Prevent Android from saving view state and trying to refresh it
    view.SaveEnabled <- false
    ViewCompat.SetSaveFromParentEnabled(view, false)

    let cachedProps = viewPropsCache.TryGetValue view

    match cachedProps with
    | (true, cachedProps) when cachedProps.OnClick = props.OnClick -> ()
    | _ -> view.SetOnClickListener (new OnClickListener (props.OnClick))

    match cachedProps with
    | (true, cachedProps) when cachedProps.OnCreateContextMenu = props.OnCreateContextMenu -> ()
    | _ -> view.SetOnCreateContextMenuListener (new OnCreateContextMenuListener (props.OnCreateContextMenu))

    match cachedProps with
    | (true, cachedProps)
          when cachedProps.OnDrag = props.OnDrag
            || Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.Honeycomb ->
        ()
    | _ -> view.SetOnDragListener (new OnDragListener (props.OnDrag))

    match cachedProps with
    | (true, cachedProps) when cachedProps.OnFocusChange = props.OnFocusChange -> ()
    | _ -> view.OnFocusChangeListener <- (new OnFocusChangeListener(props.OnFocusChange))

    match cachedProps with
    | (true, cachedProps)
          when cachedProps.OnGenericMotion = props.OnGenericMotion
            || Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.HoneycombMr1 ->
        ()
    | _ -> view.SetOnGenericMotionListener (new OnGenericMotionListener (props.OnGenericMotion))

    match cachedProps with
    | (true, cachedProps)
          when cachedProps.OnDrag = props.OnDrag
            || Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.IceCreamSandwich ->
        ()
    | _ -> view.SetOnGenericMotionListener (new OnGenericMotionListener (props.OnGenericMotion))

    match cachedProps with
    | (true, cachedProps)
          when cachedProps.OnHover = props.OnHover
            || Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.IceCreamSandwich ->
        ()
    | _ -> view.SetOnHoverListener (new OnHoverListener (props.OnHover))

    match cachedProps with
    | (true, cachedProps) when cachedProps.OnKey = props.OnKey -> ()
    | _ -> view.SetOnKeyListener (new OnKeyListener(props.OnKey))

    match cachedProps with
    | (true, cachedProps) when cachedProps.OnLongClick = props.OnLongClick -> ()
    | _ -> view.SetOnLongClickListener (new OnLongClickListener(props.OnLongClick))

    match cachedProps with
    | (true, cachedProps)
          when cachedProps.OnSystemUiVisibilityChange = props.OnSystemUiVisibilityChange
            || Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.Honeycomb ->
        ()
    | _ ->
        view.SetOnSystemUiVisibilityChangeListener (
          new OnSystemUiVisibilityChangeListener(props.OnSystemUiVisibilityChange)
        )

    match cachedProps with
    | (true, cachedProps) when cachedProps.OnTouch = props.OnTouch -> ()
    | _ -> view.SetOnTouchListener(new OnTouchListener(props.OnTouch))

    ViewCompat.SetAccessibilityLiveRegion(view, props.AccessibilityLiveRegion)

    if Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.LollipopMr1 then
      view.AccessibilityTraversalAfter <- props.AccessibilityTraversalAfter
      view.AccessibilityTraversalBefore <- props.AccessibilityTraversalBefore

    ViewCompat.SetActivated(view, props.Activated)
    ViewCompat.SetAlpha(view, props.Alpha)

    // Drawable can't be reused, so instead we weak cache the Func delegate
    // used to create the background and reference compare.
    match cachedProps with
    | (true, cachedProps) when cachedProps.Background = props.Background -> ()
    | _ -> view.Background <- props.Background.Invoke ()

    if Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.HoneycombMr1 then
      view.SetCameraDistance props.CameraDistance

    view.Clickable <- props.Clickable

    match cachedProps with
    | (true, cachedProps) when cachedProps.ClipBounds = props.ClipBounds -> ()
    | _ ->
        ViewCompat.SetClipBounds(
          view,
          match (props.ClipBounds.Invoke ()) with | Some x -> x | _ -> null
        )

    match cachedProps with
    | (true, cachedProps)
          // Need to avoid calling ClipToOutline multiple times when not re-rendering
          // to avoid android damage the view's bounds.
          // https://github.com/android/platform_frameworks_base/blob/master/core/java/android/view/View.java#L12048
          when cachedProps.ClipToOutline = props.ClipToOutline
            || Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.Lollipop ->
        ()
    | _ ->
        view.ClipToOutline <- props.ClipToOutline

    view.ContentDescription <- props.ContentDescription

    if Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M then
      view.ContextClickable <- props.ContextClickable

    view.DrawingCacheBackgroundColor <- props.DrawingCacheBackgroundColor
    view.DrawingCacheEnabled <- props.DrawingCacheEnabled
    view.DrawingCacheQuality <- props.DrawingCacheQuality
    ViewCompat.SetElevation(view,props.Elevation)
    view.Enabled <- props.Enabled
    view.SetFadingEdgeLength props.FadingEdgeLength
    ViewCompat.SetFitsSystemWindows(view, props.FitsSystemWindows)

    if Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Gingerbread then
      view.FilterTouchesWhenObscured <- props.FilterTouchesWhenObscured

    view.Focusable <- props.Focusable
    view.FocusableInTouchMode <- props.FocusableInTouchMode
    view.HapticFeedbackEnabled <- props.HapticFeedbackEnabled
    view.HorizontalFadingEdgeEnabled <- props.HorizontalFadingEdgeEnabled
    view.HorizontalScrollBarEnabled <- props.HorizontalScrollBarEnabled
    view.Id <- props.Id
    ViewCompat.SetImportantForAccessibility(view, props.ImportantForAccessibility)
    view.SetScrollContainer(props.IsScrollContainer)
    view.KeepScreenOn <- props.KeepScreenOn
    ViewCompat.SetLabelFor(view, props.LabelFor)

    match cachedProps with
    | (true, cachedProps)
          when cachedProps.LayerType = props.LayerType
            && cachedProps.LayerPaint = props.LayerPaint -> ()
    | _ -> ViewCompat.SetLayerType(view, props.LayerType, props.LayerPaint.Invoke ())

    ViewCompat.SetLayoutDirection(view, props.LayoutDirection)

    match cachedProps with
    | (true, cachedProps) when cachedProps.LayoutParameters = props.LayoutParameters -> ()
    | _ -> view.LayoutParameters <- props.LayoutParameters.Invoke ()

    view.LongClickable <- props.LongClickable
    view.SetMinimumHeight props.MinHeight
    view.SetMinimumWidth props.MinWidth
    ViewCompat.SetNestedScrollingEnabled(view, props.NestedScrollingEnabled)
    view.NextFocusDownId <- props.NextFocusDownId

    if Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Honeycomb then
      view.NextFocusForwardId <- props.NextFocusForwardId

    view.NextFocusLeftId <- props.NextFocusLeftId
    view.NextFocusRightId <- props.NextFocusRightId
    view.NextFocusUpId <- props.NextFocusRightId
    //view.OutlineProvider < props.OutlineProvider

    ViewCompat.SetOverScrollMode(view, props.OverScrollMode)
    ViewCompat.SetPaddingRelative(
      view,
      props.Padding.Start,
      props.Padding.Top,
      props.Padding.End,
      props.Padding.Bottom
    )

    let (pivotX, pivotY) = props.Pivot
    ViewCompat.SetPivotX(view, pivotX)
    ViewCompat.SetPivotY(view, pivotY)

    let requestFocusSubcription =
      if props.RequestFocus <> ViewProps.Default.RequestFocus then
        props.RequestFocus
        |> Observable.observeOn Scheduler.mainLoopScheduler
        |> Observable.iter (view.RequestFocus >> ignore)
        |> Observable.subscribeWithError ignore onError
      else Disposable.Empty

    let (rotationPivot, rotationX, rotationY) = props.Rotation
    ViewCompat.SetRotation(view, rotationPivot)
    ViewCompat.SetRotationX(view, rotationX)
    ViewCompat.SetRotationY(view, rotationY)

    let (scaleX, scaleY) = props.Scale
    ViewCompat.SetScaleX(view, scaleX)
    ViewCompat.SetScaleY(view, scaleY)

    if Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.JellyBean then
      view.ScrollBarDefaultDelayBeforeFade <- props.ScrollBarDefaultDelayBeforeFade
      view.ScrollBarFadeDuration <- props.ScrollBarFadeDuration
      view.ScrollBarSize <- props.ScrollBarSize

    view.ScrollBarStyle <- props.ScrollBarStyle
    view.Selected <- props.Selected

    let scrollBySubcription =
      if props.ScrollBy <> ViewProps.Default.ScrollBy then
        props.ScrollBy
        |> Observable.observeOn Scheduler.mainLoopScheduler
        |> Observable.iter view.ScrollBy
        |> Observable.subscribeWithError ignore onError
      else Disposable.Empty

    let scrollToSubcription =
      if props.ScrollTo <> ViewProps.Default.ScrollTo then
        props.ScrollTo
        |> Observable.observeOn Scheduler.mainLoopScheduler
        |> Observable.iter view.ScrollTo
        |> Observable.subscribeWithError ignore onError
      else Disposable.Empty

    view.Selected <- props.Selected
    view.SoundEffectsEnabled <- props.SoundEffectsEnabled
    //view.StateListAnimator <- props.StateListAnimator

    if Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Honeycomb then
      view.SystemUiVisibility <- props.SystemUiVisibility

    if Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.JellyBeanMr1 then
      view.TextAlignment <- props.TextAlignment
      view.TextDirection <- props.TextDirection

    ViewCompat.SetTransitionName(view, props.TransitionName)

    let (translationX, translationY, translationZ) = props.Translation
    ViewCompat.SetTranslationX(view, translationX)
    ViewCompat.SetTranslationY(view, translationY)
    ViewCompat.SetTranslationZ(view, translationZ)

    view.VerticalFadingEdgeEnabled <- props.VerticalFadingEdgeEnabled
    view.VerticalScrollBarEnabled <- props.VerticalScrollBarEnabled

    if Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Honeycomb then
      view.VerticalScrollbarPosition <- props.VerticalScrollbarPosition

    view.Visibility <- props.Visibility

    viewPropsCache.Remove(view) |> ignore
    viewPropsCache.Add(view, props)

    Disposables.compose [|
        requestFocusSubcription
        scrollBySubcription
        scrollToSubcription
      |]

  let create<'props, 'view when 'view :> View>
      (name: string)
      (viewProvider: unit -> 'view)
      (setProps: (Exception -> unit) -> 'view -> 'props -> IDisposable)
      (onError: Exception -> unit)
      (createNativeView: string (* view name *) -> obj (* initialProps *) -> IReactView<View>)
      (initialProps: obj) =

    let viewProvider () = viewProvider () :> View
    let setProps onError (view: View) props = setProps onError (view :?> 'view) props

    ReactView.createViewWithoutChildren
      name
      viewProvider
      (setProps onError)
      initialProps
