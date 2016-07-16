namespace React.Android.Views

open Android.Animation
open Android.Content.Res
open Android.Graphics
open Android.Graphics.Drawables
open Android.Support.V4.View
open Android.Views
open ImmutableCollections
open React
open React.Android
open System
open System.Runtime.CompilerServices

type IViewGroupProps =
  inherit IViewProps

type ViewGroupProps =
  {
    // View Props
    accessibilityLiveRegion: int
    accessibilityTraversalBefore: int
    accessibilityTraversalAfter: int
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

  interface IViewGroupProps with
    // View Props
    member this.AccessibilityLiveRegion = this.accessibilityLiveRegion
    member this.AccessibilityTraversalBefore = this.accessibilityTraversalBefore
    member this.AccessibilityTraversalAfter = this.accessibilityTraversalAfter
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

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module private ViewGroupProps =
  let internal defaultProps = {
    // View Props
    accessibilityLiveRegion = ViewProps.Default.accessibilityLiveRegion
    accessibilityTraversalAfter = ViewProps.Default.accessibilityTraversalAfter
    accessibilityTraversalBefore = ViewProps.Default.accessibilityTraversalBefore
    activated = ViewProps.Default.activated
    alpha = ViewProps.Default.alpha
    background = ViewProps.Default.background
    clickable = ViewProps.Default.clickable
    contentDescription = ViewProps.Default.contentDescription
    contextClickable = ViewProps.Default.contextClickable
    drawingCacheEnabled = ViewProps.Default.drawingCacheEnabled
    drawingCacheQuality = ViewProps.Default.drawingCacheQuality
    elevation = ViewProps.Default.elevation
    enabled = ViewProps.Default.enabled
    fadingEdgeLength = ViewProps.Default.fadingEdgeLength
    filterTouchesWhenObscured = ViewProps.Default.filterTouchesWhenObscured
    fitsSystemWindows = ViewProps.Default.fitsSystemWindows
    focusable = ViewProps.Default.focusable
    focusableInTouchMode = ViewProps.Default.focusableInTouchMode
    hapticFeedbackEnabled = ViewProps.Default.hapticFeedbackEnabled
    horizontalFadingEdgeEnabled = ViewProps.Default.horizontalFadingEdgeEnabled
    horizontalScrollBarEnabled = ViewProps.Default.horizontalScrollBarEnabled
    id = ViewProps.Default.id
    importantForAccessibility = ViewProps.Default.importantForAccessibility
    isScrollContainer = ViewProps.Default.isScrollContainer
    keepScreenOn = ViewProps.Default.keepScreenOn
    labelFor = ViewProps.Default.labelFor
    layerPaint = ViewProps.Default.layerPaint
    layerType = ViewProps.Default.layerType
    layoutDirection = ViewProps.Default.layoutDirection
    layoutParameters = ViewProps.Default.layoutParameters
    longClickable = ViewProps.Default.longClickable
    minHeight = ViewProps.Default.minHeight
    minWidth = ViewProps.Default.minWidth
    nestedScrollingEnabled = ViewProps.Default.nestedScrollingEnabled
    nextFocusDownId = ViewProps.Default.nextFocusDownId
    nextFocusForwardId = ViewProps.Default.nextFocusForwardId
    nextFocusLeftId = ViewProps.Default.nextFocusLeftId
    nextFocusRightId = ViewProps.Default.nextFocusRightId
    nextFocusUpId = ViewProps.Default.nextFocusUpId
    onClick = ViewProps.Default.onClick
    onCreateContextMenu = ViewProps.Default.onCreateContextMenu
    onDrag = ViewProps.Default.onDrag
    onFocusChange = ViewProps.Default.onFocusChange
    onGenericMotion = ViewProps.Default.onGenericMotion
    onHover = ViewProps.Default.onHover
    onKey = ViewProps.Default.onKey
    onLongClick = ViewProps.Default.onLongClick
    onSystemUiVisibilityChange = ViewProps.Default.onSystemUiVisibilityChange
    onTouch = ViewProps.Default.onTouch
    //outlineProvider = ViewProps.Default.outlineProvider
    overScrollBy = ViewProps.Default.overScrollBy
    overScrollMode = ViewProps.Default.overScrollMode
    paddingBottom = ViewProps.Default.paddingBottom
    paddingEnd = ViewProps.Default.paddingEnd
    paddingStart = ViewProps.Default.paddingStart
    paddingTop = ViewProps.Default.paddingTop
    pivotX = ViewProps.Default.pivotX
    pivotY = ViewProps.Default.pivotY
    requestFocus = ViewProps.Default.requestFocus
    rotation = ViewProps.Default.rotation
    rotationX = ViewProps.Default.rotationX
    rotationY = ViewProps.Default.rotationY
    scaleX = ViewProps.Default.scaleX
    scaleY = ViewProps.Default.scaleY
    //scrollBarDefaultDelayBeforeFade = ViewProps.Default.scrollBarDefaultDelayBeforeFade
    //scrollBarFadeDuration = ViewProps.Default.scrollBarDefaultDelayBeforeFade
    //scrollBarFadingEnabled = ViewProps.Default.scrollBarFadingEnabled
    //scrollBarSize = ViewProps.Default.scrollBarSize
    scrollBarStyle = ViewProps.Default.scrollBarStyle
    scrollBy = ViewProps.Default.scrollBy
    scrollTo = ViewProps.Default.scrollTo
    selected = ViewProps.Default.selected
    soundEffectsEnabled = ViewProps.Default.soundEffectsEnabled
    //stateListAnimator = ViewProps.Default.stateListAnimator
    //systemUiVisibility =  ViewProps.Default.systemUiVisibility
    //textAlignment = ViewProps.Default.textAlignment
    //textDirection = ViewProps.Default.textDirection
    transitionName = ViewProps.Default.transitionName
    translationX = ViewProps.Default.translationX
    translationY = ViewProps.Default.translationY
    translationZ = ViewProps.Default.translationZ
    verticalFadingEdgeEnabled = ViewProps.Default.verticalFadingEdgeEnabled
    verticalScrollBarEnabled = ViewProps.Default.verticalScrollBarEnabled
    //verticalScrollbarPosition = ViewProps.Default.verticalScrollbarPosition
    visibility = ViewProps.Default.visibility
  }

type ViewGroupProps with
  static member Default = ViewGroupProps.defaultProps

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ViewGroup =
  let private removeAllViews (view: View) =
    let view = view :?> ViewGroup
    view.RemoveAllViews()

  let private addViews
      (viewGroup: View)
      (children: seq<View>) =
    let viewGroup = viewGroup :?> ViewGroup
    for child in children do
      viewGroup.AddView child
    (viewGroup :> View).Invalidate ()

  let create<'props, 'viewGroup when 'viewGroup :> ViewGroup>
      (name: string)
      (viewGroupProvider: unit -> 'viewGroup)
      (setProps: (Exception -> unit) -> 'viewGroup -> 'props -> IDisposable)
      (onError: Exception -> unit)
      (createNativeView: string (* view name *) -> obj (* initialProps *) -> IReactView<View>)
      (initialProps: obj) =

    let viewGroupProvider () = viewGroupProvider () :> View
    let setProps onError (view: View) props =
      setProps onError (view :?> 'viewGroup) props

    ReactView.createViewImmediatelyRenderingAllChildren
      Scheduler.mainLoopScheduler
      onError
      createNativeView
      removeAllViews
      addViews
      name
      viewGroupProvider
      (setProps onError)
      initialProps

  let setProps (onError: Exception -> unit) (view: ViewGroup) (props: IViewGroupProps) =
    View.setProps onError view props