namespace React.Android.Widget

open Android.Animation
open Android.Content
open Android.Content.Res
open Android.Graphics
open Android.Graphics.Drawables
open Android.Support.V4.View
open Android.Support.V7.Widget
open Android.Views
open Android.Widget
open ImmutableCollections
open React
open React.Android
open React.Android.Views
open System

type IToolbarProps =
  inherit IViewGroupProps

  abstract member SubTitle: string
  abstract member Title: string

type ToolbarProps =
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

    // Toolbar Props
    subTitle: string
    title: string
  }

  interface IToolbarProps with
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

    // Toolbar Props
    member this.SubTitle = this.subTitle
    member this.Title = this.title

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module private ToolbarProps =
  let internal defaultProps = {
    // View Props
    accessibilityLiveRegion = ViewGroupProps.Default.accessibilityLiveRegion
    accessibilityTraversalAfter = ViewGroupProps.Default.accessibilityTraversalAfter
    accessibilityTraversalBefore = ViewGroupProps.Default.accessibilityTraversalBefore
    activated = ViewGroupProps.Default.activated
    alpha = ViewGroupProps.Default.alpha
    background = ViewGroupProps.Default.background
    clickable = ViewGroupProps.Default.clickable
    contentDescription = ViewGroupProps.Default.contentDescription
    contextClickable = ViewGroupProps.Default.contextClickable
    drawingCacheEnabled = ViewGroupProps.Default.drawingCacheEnabled
    drawingCacheQuality = ViewGroupProps.Default.drawingCacheQuality
    elevation = ViewGroupProps.Default.elevation
    enabled = ViewGroupProps.Default.enabled
    fadingEdgeLength = ViewGroupProps.Default.fadingEdgeLength
    filterTouchesWhenObscured = ViewGroupProps.Default.filterTouchesWhenObscured
    fitsSystemWindows = ViewGroupProps.Default.fitsSystemWindows
    focusable = ViewGroupProps.Default.focusable
    focusableInTouchMode = ViewGroupProps.Default.focusableInTouchMode
    hapticFeedbackEnabled = ViewGroupProps.Default.hapticFeedbackEnabled
    horizontalFadingEdgeEnabled = ViewGroupProps.Default.horizontalFadingEdgeEnabled
    horizontalScrollBarEnabled = ViewGroupProps.Default.horizontalScrollBarEnabled
    id = ViewGroupProps.Default.id
    importantForAccessibility = ViewGroupProps.Default.importantForAccessibility
    isScrollContainer = ViewGroupProps.Default.isScrollContainer
    keepScreenOn = ViewGroupProps.Default.keepScreenOn
    labelFor = ViewGroupProps.Default.labelFor
    layerPaint = ViewGroupProps.Default.layerPaint
    layerType = ViewGroupProps.Default.layerType
    layoutDirection = ViewGroupProps.Default.layoutDirection
    layoutParameters = ViewGroupProps.Default.layoutParameters
    longClickable = ViewGroupProps.Default.longClickable
    minHeight = ViewGroupProps.Default.minHeight
    minWidth = ViewGroupProps.Default.minWidth
    nestedScrollingEnabled = ViewGroupProps.Default.nestedScrollingEnabled
    nextFocusDownId = ViewGroupProps.Default.nextFocusDownId
    nextFocusForwardId = ViewGroupProps.Default.nextFocusForwardId
    nextFocusLeftId = ViewGroupProps.Default.nextFocusLeftId
    nextFocusRightId = ViewGroupProps.Default.nextFocusRightId
    nextFocusUpId = ViewGroupProps.Default.nextFocusUpId
    onClick = ViewGroupProps.Default.onClick
    onCreateContextMenu = ViewGroupProps.Default.onCreateContextMenu
    onDrag = ViewGroupProps.Default.onDrag
    onFocusChange = ViewGroupProps.Default.onFocusChange
    onGenericMotion = ViewGroupProps.Default.onGenericMotion
    onHover = ViewGroupProps.Default.onHover
    onKey = ViewGroupProps.Default.onKey
    onLongClick = ViewGroupProps.Default.onLongClick
    onSystemUiVisibilityChange = ViewGroupProps.Default.onSystemUiVisibilityChange
    onTouch = ViewGroupProps.Default.onTouch
    //outlineProvider = ViewGroupProps.Default.outlineProvider
    overScrollBy = ViewGroupProps.Default.overScrollBy
    overScrollMode = ViewGroupProps.Default.overScrollMode
    paddingBottom = ViewGroupProps.Default.paddingBottom
    paddingEnd = ViewGroupProps.Default.paddingEnd
    paddingStart = ViewGroupProps.Default.paddingStart
    paddingTop = ViewGroupProps.Default.paddingTop
    pivotX = ViewGroupProps.Default.pivotX
    pivotY = ViewGroupProps.Default.pivotY
    requestFocus = ViewGroupProps.Default.requestFocus
    rotation = ViewGroupProps.Default.rotation
    rotationX = ViewGroupProps.Default.rotationX
    rotationY = ViewGroupProps.Default.rotationY
    scaleX = ViewGroupProps.Default.scaleX
    scaleY = ViewGroupProps.Default.scaleY
    //scrollBarDefaultDelayBeforeFade = ViewGroupProps.Default.scrollBarDefaultDelayBeforeFade
    //scrollBarFadeDuration = ViewGroupProps.Default.scrollBarDefaultDelayBeforeFade
    //scrollBarFadingEnabled = ViewGroupProps.Default.scrollBarFadingEnabled
    //scrollBarSize = ViewGroupProps.Default.scrollBarSize
    scrollBarStyle = ViewGroupProps.Default.scrollBarStyle
    scrollBy = ViewGroupProps.Default.scrollBy
    scrollTo = ViewGroupProps.Default.scrollTo
    selected = ViewGroupProps.Default.selected
    soundEffectsEnabled = ViewGroupProps.Default.soundEffectsEnabled
    //stateListAnimator = ViewGroupProps.Default.stateListAnimator
    //systemUiVisibility =  ViewGroupProps.Default.systemUiVisibility
    //textAlignment = ViewGroupProps.Default.textAlignment
    //textDirection = ViewGroupProps.Default.textDirection
    transitionName = ViewGroupProps.Default.transitionName
    translationX = ViewGroupProps.Default.translationX
    translationY = ViewGroupProps.Default.translationY
    translationZ = ViewGroupProps.Default.translationZ
    verticalFadingEdgeEnabled = ViewGroupProps.Default.verticalFadingEdgeEnabled
    verticalScrollBarEnabled = ViewGroupProps.Default.verticalScrollBarEnabled
    //verticalScrollbarPosition = ViewGroupProps.Default.verticalScrollbarPosition
    visibility = ViewGroupProps.Default.visibility

    // Toolbar Props
    subTitle = ""
    title = ""
  }

type ToolbarProps with
  static member Default = ToolbarProps.defaultProps

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Toolbar =
  let private name = typeof<Android.Support.V7.Widget.Toolbar>.FullName

  let setProps (onError: Exception -> unit) (view: Android.Support.V7.Widget.Toolbar) (props: IToolbarProps) =
    view.Subtitle <- props.SubTitle
    view.Title <- props.Title
    ViewGroup.setProps onError view props

  let private createView context =
    let viewGroupProvider () = new Android.Support.V7.Widget.Toolbar (context)
    View.create name viewGroupProvider setProps

  let viewProvider = (name, createView)

  let internal reactComponent
      (props: IToolbarProps)
      (children: IImmutableMap<int, ReactElement>) = ReactNativeElement {
    Name = name
    Props = props
    Children = children
  }