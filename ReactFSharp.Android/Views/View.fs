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
    accessibilityLiveRegion: int
    accessibilityTraversalAfter: int
    accessibilityTraversalBefore: int
    activated: bool
    alpha: float32
    background: Func<Drawable>
    clickable: bool
    contextClickable: bool
    contentDescription: string
    drawingCacheEnabled: bool
    drawingCacheQuality: DrawingCacheQuality
    elevation: Single
    enabled: bool
    fadingEdgeLength:int
    fitsSystemWindows: bool
    filterTouchesWhenObscured: bool
    focusable: bool
    focusableInTouchMode: bool
    hapticFeedbackEnabled: bool
    horizontalFadingEdgeEnabled: bool
    horizontalScrollBarEnabled: bool
    id: int
    importantForAccessibility: int
    isScrollContainer: bool
    keepScreenOn: bool
    labelFor: int
    layerPaint: Func<Paint>
    layerType: int
    layoutDirection: int
    layoutParameters: Func<ViewGroup.LayoutParams>
    longClickable: bool
    minHeight: int
    minWidth: int
    nestedScrollingEnabled: bool
    nextFocusDownId: int
    nextFocusForwardId: int
    nextFocusLeftId: int
    nextFocusRightId: int
    nextFocusUpId: int
    onClick: Action
    onCreateContextMenu: Action<IContextMenu, IContextMenuContextMenuInfo>
    onDrag: Func<DragEvent, bool>
    onFocusChange: Action<bool>
    onGenericMotion: Func<MotionEvent, bool>
    onHover: Func<MotionEvent, bool>
    onKey: Func<Keycode, KeyEvent, bool>
    onLongClick: Func<bool>
    onSystemUiVisibilityChange: Action<StatusBarVisibility>
    onTouch: Func<MotionEvent, bool>
    //outlineProvider: ViewOutlineProvider
    overScrollBy: IObservable<int * int * int * int * int * int * int * int * bool>
    overScrollMode: int
    paddingBottom: int
    paddingEnd: int
    paddingStart: int
    paddingTop: int
    pivotX: Single
    pivotY: Single
    requestFocus: IObservable<FocusSearchDirection>
    rotation: Single
    rotationX: Single
    rotationY: Single
    scaleX: Single
    scaleY: Single
    //scrollBarDefaultDelayBeforeFade: int32
    //scrollBarFadeDuration: int32
    //scrollBarFadingEnabled: bool
    //scrollBarSize: int
    scrollBarStyle: ScrollbarStyles
    scrollBy: IObservable<int * int>
    scrollTo: IObservable<int * int>
    selected: bool
    soundEffectsEnabled: bool
    //stateListAnimator: StateListAnimator
    //systemUiVisibility: StatusBarVisibility
    //textAlignment: TextAlignment
    //textDirection: TextDirection
    transitionName: string
    translationX: Single
    translationY: Single
    translationZ: Single
    verticalFadingEdgeEnabled: bool
    verticalScrollBarEnabled: bool
    //verticalScrollbarPosition: ScrollbarPosition
    visibility: ViewStates
  }

  interface IViewProps with
    // View Props
    member this.AccessibilityLiveRegion = this.accessibilityLiveRegion
    member this.AccessibilityTraversalAfter = this.accessibilityTraversalAfter
    member this.AccessibilityTraversalBefore = this.accessibilityTraversalBefore
    member this.Activated = this.activated
    member this.Alpha = this.alpha
    member this.Background = this.background
    member this.Clickable = this.clickable
    member this.ContentDescription = this.contentDescription
    member this.ContextClickable = this.contextClickable
    member this.DrawingCacheEnabled = this.drawingCacheEnabled
    member this.DrawingCacheQuality = this.drawingCacheQuality
    member this.Elevation = this.elevation
    member this.Enabled = this.enabled
    member this.FadingEdgeLength = this.fadingEdgeLength
    member this.FitsSystemWindows = this.fitsSystemWindows
    member this.FilterTouchesWhenObscured = this.filterTouchesWhenObscured
    member this.Focusable = this.focusable
    member this.FocusableInTouchMode = this.focusableInTouchMode
    member this.HapticFeedbackEnabled = this.hapticFeedbackEnabled
    member this.HorizontalFadingEdgeEnabled = this.horizontalFadingEdgeEnabled
    member this.HorizontalScrollBarEnabled = this.horizontalScrollBarEnabled
    member this.Id = this.id
    member this.ImportantForAccessibility = this.importantForAccessibility
    member this.IsScrollContainer = this.isScrollContainer
    member this.KeepScreenOn = this.keepScreenOn
    member this.LabelFor = this.labelFor
    member this.LayerPaint = this.layerPaint
    member this.LayerType = this.layerType
    member this.LayoutDirection = this.layoutDirection
    member this.LayoutParameters = this.layoutParameters
    member this.LongClickable = this.longClickable
    member this.MinHeight = this.minHeight
    member this.MinWidth = this.minWidth
    member this.NestedScrollingEnabled = this.nestedScrollingEnabled
    member this.NextFocusDownId = this.nextFocusDownId
    member this.NextFocusForwardId = this.nextFocusForwardId
    member this.NextFocusLeftId = this.nextFocusLeftId
    member this.NextFocusRightId = this.nextFocusRightId
    member this.NextFocusUpId = this.nextFocusUpId
    member this.OnClick = this.onClick
    member this.OnCreateContextMenu = this.onCreateContextMenu
    member this.OnDrag = this.onDrag
    member this.OnFocusChange = this.onFocusChange
    member this.OnGenericMotion = this.onGenericMotion
    member this.OnHover = this.onHover
    member this.OnKey = this.onKey
    member this.OnLongClick = this.onLongClick
    member this.OnSystemUiVisibilityChange = this.onSystemUiVisibilityChange
    member this.OnTouch = this.onTouch
    //member this.OutlineProvider = this.outlineProvider
    member this.OverScrollBy = this.overScrollBy
    member this.OverScrollMode = this.overScrollMode
    member this.PaddingBottom = this.paddingBottom
    member this.PaddingEnd = this.paddingEnd
    member this.PaddingStart = this.paddingStart
    member this.PaddingTop = this.paddingTop
    member this.PivotX = this.pivotX
    member this.PivotY = this.pivotY
    member this.RequestFocus = this.requestFocus
    member this.Rotation = this.rotation
    member this.RotationX= this.rotationX
    member this.RotationY= this.rotationY
    member this.ScaleX = this.scaleX
    member this.ScaleY = this.scaleY
    //member this.ScrollBarDefaultDelayBeforeFade = this.scrollBarDefaultDelayBeforeFade
    //member this.ScrollBarFadeDuration = this.scrollBarFadeDuration
    //member this.ScrollBarFadingEnabled = this.scrollBarFadingEnabled
    //member this.ScrollBarSize = this.scrollBarSize
    member this.ScrollBarStyle = this.scrollBarStyle
    member this.ScrollBy = this.scrollBy
    member this.ScrollTo = this.scrollTo
    member this.Selected = this.selected
    member this.SoundEffectsEnabled = this.soundEffectsEnabled
    //member this.StateListAnimator = this.stateListAnimator
    //member this.SystemUiVisibility = this.systemUiVisibility
    //member this.TextAlignment = this.textAlignment
    //member this.TextDirection = this.textDirection
    member this.TransitionName = this.transitionName
    member this.TranslationX = this.translationX
    member this.TranslationY = this.translationY
    member this.TranslationZ = this.translationY
    member this.VerticalFadingEdgeEnabled = this.verticalFadingEdgeEnabled
    member this.VerticalScrollBarEnabled = this.verticalScrollBarEnabled
    //member this.VerticalScrollbarPosition = this.verticalScrollbarPosition
    member this.Visibility = this.visibility

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
    accessibilityLiveRegion = ViewCompat.AccessibilityLiveRegionNone
    accessibilityTraversalAfter = View.NoId
    accessibilityTraversalBefore = View.NoId
    activated = false
    alpha = 1.0f
    background = defaultBackground
    clickable = false
    contentDescription = ""
    contextClickable = false
    drawingCacheEnabled = false
    drawingCacheQuality = DrawingCacheQuality.Auto
    elevation = 0.0f
    enabled = false
    fadingEdgeLength = 0
    filterTouchesWhenObscured = false
    fitsSystemWindows = true
    focusable = false
    focusableInTouchMode = false
    hapticFeedbackEnabled = false
    horizontalFadingEdgeEnabled = false
    horizontalScrollBarEnabled = false
    id = View.NoId
    importantForAccessibility = ViewCompat.ImportantForAccessibilityAuto
    isScrollContainer = false
    keepScreenOn = false
    labelFor = View.NoId
    layerPaint = defaultLayerPaint
    layerType = ViewCompat.LayerTypeNone
    layoutDirection = ViewCompat.LayoutDirectionInherit
    layoutParameters = defaultLayoutParameters
    longClickable = false
    minHeight = 0
    minWidth = 0
    nestedScrollingEnabled = false
    nextFocusDownId = View.NoId
    nextFocusForwardId = View.NoId
    nextFocusLeftId = View.NoId
    nextFocusRightId = View.NoId
    nextFocusUpId = View.NoId
    onClick = defaultOnClick
    onCreateContextMenu = defaultOnCreateContextMenu
    onDrag = defaultOnDrag
    onFocusChange = defaultOnFocusChange
    onGenericMotion = defaultOnGenericMotion
    onHover = defaultOnHover
    onKey = defaultOnKey
    onLongClick = defaultOnLongClick
    onSystemUiVisibilityChange = defaultOnSystemUiVisibilityChange
    onTouch = defaultOnTouch
    //outlineProvider = ViewOutlineProvider.Background
    overScrollBy = defaultOverScrollBy
    overScrollMode = ViewCompat.OverScrollIfContentScrolls
    paddingBottom = 0
    paddingEnd = 0
    paddingStart = 0
    paddingTop = 0
    pivotX = 0.0f
    pivotY = 0.0f
    requestFocus = defaultRequestFocus
    rotation = 0.0f
    rotationX = 0.0f
    rotationY = 0.0f
    scaleX = 1.0f
    scaleY = 1.0f
    //scrollBarDefaultDelayBeforeFade = ViewConfiguration.ScrollDefaultDelay
    //scrollBarFadeDuration = ViewConfiguration.ScrollBarFadeDuration
    //scrollBarFadingEnabled = true
    //scrollBarSize = defaultViewConfiguration.ScaledScrollBarSize
    scrollBarStyle = ScrollbarStyles.InsideOverlay
    scrollBy = defaultScrollBy
    scrollTo = defaultScrollTo
    selected = false
    soundEffectsEnabled = true
    //stateListAnimator = defaultStateListAnimator
    //systemUiVisibility =  StatusBarVisibility.Visible
    //textAlignment = TextAlignment.Inherit
    //textDirection = TextDirection.Inherit
    transitionName = ""
    translationX = 0.0f
    translationY = 0.0f
    translationZ = 0.0f
    verticalFadingEdgeEnabled = false
    verticalScrollBarEnabled = false
    //verticalScrollbarPosition = ScrollbarPosition.Default
    visibility = ViewStates.Visible
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
      if props.RequestFocus <> ViewProps.Default.requestFocus then
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
      if props.ScrollBy <> ViewProps.Default.scrollBy then
        props.ScrollBy
        |> Observable.observeOn Scheduler.mainLoopScheduler
        |> Observable.iter view.ScrollBy
        |> Observable.subscribeWithError ignore onError
      else Disposable.Empty

    let scrollToSubcription =
      if props.ScrollTo <> ViewProps.Default.scrollTo then
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
