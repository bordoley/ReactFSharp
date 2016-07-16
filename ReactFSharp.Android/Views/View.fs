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

type IViewProps =
  abstract member AccessibilityLiveRegion: int
  abstract member AccessibilityTraversalAfter: int
  abstract member AccessibilityTraversalBefore: int
  abstract member Activated: bool
  abstract member Alpha: float32
  abstract member Background: Func<Drawable>
  abstract member Clickable: bool
  abstract member ContextClickable: bool
  abstract member ContentDescription: string
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
  abstract member OverScrollBy: IObservable<int * int * int * int * int * int * int * int * bool>
  abstract member OverScrollMode: int
  abstract member PaddingBottom: int
  abstract member PaddingEnd: int
  abstract member PaddingStart: int
  abstract member PaddingTop: int
  abstract member PivotX: Single
  abstract member PivotY: Single
  abstract member RequestFocus: IObservable<FocusSearchDirection>
  abstract member Rotation: Single
  abstract member RotationX: Single
  abstract member RotationY: Single
  abstract member ScaleX: Single
  abstract member ScaleY: Single
  //abstract member ScrollBarDefaultDelayBeforeFade: int32
  //abstract member ScrollBarFadeDuration: int32
  //abstract member ScrollBarFadingEnabled: bool
  //abstract member ScrollBarSize: int
  abstract member ScrollBarStyle: ScrollbarStyles
  abstract member ScrollBy: IObservable<int * int>
  abstract member ScrollTo: IObservable<int * int>
  abstract member Selected: bool
  abstract member SoundEffectsEnabled: bool
  //abstract member StateListAnimator: StateListAnimator
  //abstract member SystemUiVisibility: StatusBarVisibility
  //abstract member TextAlignment: TextAlignment
  //abstract member TextDirection: TextDirection
  abstract member TransitionName: string
  abstract member TranslationX: Single
  abstract member TranslationY: Single
  abstract member TranslationZ: Single
  abstract member VerticalFadingEdgeEnabled: bool
  abstract member VerticalScrollBarEnabled: bool
  //abstract member VerticalScrollbarPosition: ScrollbarPosition
  abstract member Visibility: ViewStates

type ViewProps =
  {
    AccessibilityLiveRegion: int
    AccessibilityTraversalAfter: int
    AccessibilityTraversalBefore: int
    Activated: bool
    Alpha: float32
    Background: Func<Drawable>
    Clickable: bool
    ContextClickable: bool
    ContentDescription: string
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
    OverScrollBy: IObservable<int * int * int * int * int * int * int * int * bool>
    OverScrollMode: int
    PaddingBottom: int
    PaddingEnd: int
    PaddingStart: int
    PaddingTop: int
    PivotX: Single
    PivotY: Single
    RequestFocus: IObservable<FocusSearchDirection>
    Rotation: Single
    RotationX: Single
    RotationY: Single
    ScaleX: Single
    ScaleY: Single
    //ScrollBarDefaultDelayBeforeFade: int32
    //ScrollBarFadeDuration: int32
    //ScrollBarFadingEnabled: bool
    //ScrollBarSize: int
    ScrollBarStyle: ScrollbarStyles
    ScrollBy: IObservable<int * int>
    ScrollTo: IObservable<int * int>
    Selected: bool
    SoundEffectsEnabled: bool
    //StateListAnimator: StateListAnimator
    //SystemUiVisibility: StatusBarVisibility
    //TextAlignment: TextAlignment
    //TextDirection: TextDirection
    TransitionName: string
    TranslationX: Single
    TranslationY: Single
    TranslationZ: Single
    VerticalFadingEdgeEnabled: bool
    VerticalScrollBarEnabled: bool
    //VerticalScrollbarPosition: ScrollbarPosition
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
    member this.Clickable = this.Clickable
    member this.ContentDescription = this.ContentDescription
    member this.ContextClickable = this.ContextClickable
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
    member this.OverScrollBy = this.OverScrollBy
    member this.OverScrollMode = this.OverScrollMode
    member this.PaddingBottom = this.PaddingBottom
    member this.PaddingEnd = this.PaddingEnd
    member this.PaddingStart = this.PaddingStart
    member this.PaddingTop = this.PaddingTop
    member this.PivotX = this.PivotX
    member this.PivotY = this.PivotY
    member this.RequestFocus = this.RequestFocus
    member this.Rotation = this.Rotation
    member this.RotationX= this.RotationX
    member this.RotationY= this.RotationY
    member this.ScaleX = this.ScaleX
    member this.ScaleY = this.ScaleY
    //member this.ScrollBarDefaultDelayBeforeFade = this.ScrollBarDefaultDelayBeforeFade
    //member this.ScrollBarFadeDuration = this.ScrollBarFadeDuration
    //member this.ScrollBarFadingEnabled = this.ScrollBarFadingEnabled
    //member this.ScrollBarSize = this.ScrollBarSize
    member this.ScrollBarStyle = this.ScrollBarStyle
    member this.ScrollBy = this.ScrollBy
    member this.ScrollTo = this.ScrollTo
    member this.Selected = this.Selected
    member this.SoundEffectsEnabled = this.SoundEffectsEnabled
    //member this.StateListAnimator = this.StateListAnimator
    //member this.SystemUiVisibility = this.SystemUiVisibility
    //member this.TextAlignment = this.TextAlignment
    //member this.TextDirection = this.TextDirection
    member this.TransitionName = this.TransitionName
    member this.TranslationX = this.TranslationX
    member this.TranslationY = this.TranslationY
    member this.TranslationZ = this.TranslationY
    member this.VerticalFadingEdgeEnabled = this.VerticalFadingEdgeEnabled
    member this.VerticalScrollBarEnabled = this.VerticalScrollBarEnabled
    //member this.VerticalScrollbarPosition = this.VerticalScrollbarPosition
    member this.Visibility = this.Visibility

// This is a hack around the F# compiler. We want to ensure that
// the default event handlers are values so that they can be effectively
// cached and don't break record equality
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module private ViewProps =
  let private defaultBackground = Func<Drawable>(fun () -> new ColorDrawable(Color.White) :> Drawable)

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

  let private defaultOverScrollBy = Observable.empty<int * int * int * int * int * int * int * int * bool>

  let private defaultRequestFocus = Observable.empty<FocusSearchDirection>

  let private defaultScrollBy = Observable.empty<int * int>

  let private defaultScrollTo = Observable.empty<int * int>

  //let private defaultStateListAnimator = new StateListAnimator()

  //let private defaultViewConfiguration = new ViewConfiguration()

  let internal defaultProps = {
    AccessibilityLiveRegion = ViewCompat.AccessibilityLiveRegionNone
    AccessibilityTraversalAfter = View.NoId
    AccessibilityTraversalBefore = View.NoId
    Activated = false
    Alpha = 1.0f
    Background = defaultBackground
    Clickable = false
    ContentDescription = ""
    ContextClickable = false
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
    OverScrollBy = defaultOverScrollBy
    OverScrollMode = ViewCompat.OverScrollIfContentScrolls
    PaddingBottom = 0
    PaddingEnd = 0
    PaddingStart = 0
    PaddingTop = 0
    PivotX = 0.0f
    PivotY = 0.0f
    RequestFocus = defaultRequestFocus
    Rotation = 0.0f
    RotationX = 0.0f
    RotationY = 0.0f
    ScaleX = 1.0f
    ScaleY = 1.0f
    //ScrollBarDefaultDelayBeforeFade = ViewConfiguration.ScrollDefaultDelay
    //ScrollBarFadeDuration = ViewConfiguration.ScrollBarFadeDuration
    //ScrollBarFadingEnabled = true
    //ScrollBarSize = defaultViewConfiguration.ScaledScrollBarSize
    ScrollBarStyle = ScrollbarStyles.InsideOverlay
    ScrollBy = defaultScrollBy
    ScrollTo = defaultScrollTo
    Selected = false
    SoundEffectsEnabled = true
    //StateListAnimator = defaultStateListAnimator
    //SystemUiVisibility =  StatusBarVisibility.Visible
    //TextAlignment = TextAlignment.Inherit
    //TextDirection = TextDirection.Inherit
    TransitionName = ""
    TranslationX = 0.0f
    TranslationY = 0.0f
    TranslationZ = 0.0f
    VerticalFadingEdgeEnabled = false
    VerticalScrollBarEnabled = false
    //VerticalScrollbarPosition = ScrollbarPosition.Default
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

    //view.SetOnCreateContextMenuListener (OnCreateContextMenuListener.Create props.OnCreateContextMenu)
    //view.SetOnDragListener (OnDragListener.Create props.OnDrag)

    match cachedProps with
    | (true, cachedProps) when cachedProps.OnFocusChange = props.OnFocusChange -> ()
    | _ -> view.OnFocusChangeListener <- (new OnFocusChangeListener(props.OnFocusChange))

    //view.SetOnGenericMotionListener (OnGenericMotionListener.Create props.OnGenericMotion)
    //view.SetOnHoverListener (OnHoverListener.Create props.OnHover)

    match cachedProps with
    | (true, cachedProps) when cachedProps.OnKey = props.OnKey -> ()
    | _ -> view.SetOnKeyListener (new OnKeyListener(props.OnKey))

    match cachedProps with
    | (true, cachedProps) when cachedProps.OnLongClick = props.OnLongClick -> ()
    | _ -> view.SetOnLongClickListener (new OnLongClickListener(props.OnLongClick))

    //view.SetOnSystemUiVisibilityChangeListener (
    //  OnSystemUiVisibilityChangeListener.Create props.OnSystemUiVisibilityChange
    //)
    match cachedProps with
    | (true, cachedProps) when cachedProps.OnTouch = props.OnTouch -> ()
    | _ -> view.SetOnTouchListener(new OnTouchListener(props.OnTouch))

    ViewCompat.SetAccessibilityLiveRegion(view, props.AccessibilityLiveRegion)
    //abstract member AccessibilityTraversalAfter: int
    //abstract member AccessibilityTraversalBefore: int
    ViewCompat.SetActivated(view, props.Activated)
    ViewCompat.SetAlpha(view, props.Alpha)

    // Drawable can't be reused, so instead we weak cache the Func delegate
    // used to create the background and reference compare.
    match cachedProps with
    | (true, cachedProps) when cachedProps.Background = props.Background -> ()
    | _ -> view.Background <- props.Background.Invoke ()

    view.Clickable <- props.Clickable
    view.ContentDescription <- props.ContentDescription
    //view.ContextClickable <- props.ContextClickable
    view.DrawingCacheEnabled <- props.DrawingCacheEnabled
    view.DrawingCacheQuality <- props.DrawingCacheQuality
    ViewCompat.SetElevation(view,props.Elevation)
    view.Enabled <- props.Enabled
    view.SetFadingEdgeLength props.FadingEdgeLength
    ViewCompat.SetFitsSystemWindows(view, props.FitsSystemWindows)
    //view.FilterTouchesWhenObscured <- props.FilterTouchesWhenObscured
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
    //view.NextFocusForwardId <- props.NextFocusForwardId
    view.NextFocusLeftId <- props.NextFocusLeftId
    view.NextFocusRightId <- props.NextFocusRightId
    view.NextFocusUpId <- props.NextFocusRightId
    //view.OutlineProvider < props.OutlineProvider
    //abstract member OverScrollBy: IObservable<int * int * int * int * int * int * int * int * bool>
    ViewCompat.SetOverScrollMode(view, props.OverScrollMode)
    ViewCompat.SetPaddingRelative(
      view,
      props.PaddingStart,
      props.PaddingTop,
      props.PaddingEnd,
      props.PaddingBottom
    )
    ViewCompat.SetPivotX(view, props.PivotX)
    ViewCompat.SetPivotY(view, props.PivotY)

    let requestFocusSubcription =
      if props.RequestFocus <> ViewProps.Default.RequestFocus then
        props.RequestFocus
        |> Observable.observeOn Scheduler.mainLoopScheduler
        |> Observable.iter (view.RequestFocus >> ignore)
        |> Observable.subscribeWithError ignore onError
      else Disposable.Empty

    ViewCompat.SetRotation(view, props.Rotation)
    ViewCompat.SetRotationX(view, props.RotationX)
    ViewCompat.SetRotationY(view, props.RotationY)

    ViewCompat.SetScaleX(view, props.ScaleX)
    ViewCompat.SetScaleY(view, props.ScaleY)

    //view.ScrollBarDefaultDelayBeforeFade <- props.ScrollBarDefaultDelayBeforeFade
    //view.ScrollBarFadeDuration <- props.ScrollBarFadeDuration
    //view.ScrollBarSize <- props.ScrollBarSize
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
    //view.SystemUiVisibility <- props.SystemUiVisibility
    //view.TextAlignment <- props.TextAlignment
    //view.TextDirection <- props.TextDirection
    ViewCompat.SetTransitionName(view, props.TransitionName)
    ViewCompat.SetTranslationX(view, props.TranslationX)
    ViewCompat.SetTranslationY(view, props.TranslationY)
    ViewCompat.SetTranslationZ(view, props.TranslationZ)
    view.VerticalFadingEdgeEnabled <- props.VerticalFadingEdgeEnabled
    view.VerticalScrollBarEnabled <- props.VerticalScrollBarEnabled
    //view.VerticalScrollbarPosition <- props.VerticalScrollbarPosition
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
