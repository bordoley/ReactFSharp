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

type IButtonProps =
  inherit ITextViewProps

type ButtonProps =
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

    // TextView Props
    text: string
  }

  interface IButtonProps with
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

    // TextView Props
    member this.Text = this.text

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module private ButtonProps =
  let internal defaultProps = {
    // View Props
    accessibilityLiveRegion = TextViewProps.Default.accessibilityLiveRegion
    accessibilityTraversalAfter = TextViewProps.Default.accessibilityTraversalAfter
    accessibilityTraversalBefore = TextViewProps.Default.accessibilityTraversalBefore
    activated = TextViewProps.Default.activated
    alpha = TextViewProps.Default.alpha
    background = TextViewProps.Default.background
    clickable = TextViewProps.Default.clickable
    contentDescription = TextViewProps.Default.contentDescription
    contextClickable = TextViewProps.Default.contextClickable
    drawingCacheEnabled = TextViewProps.Default.drawingCacheEnabled
    drawingCacheQuality = TextViewProps.Default.drawingCacheQuality
    elevation = TextViewProps.Default.elevation
    enabled = TextViewProps.Default.enabled
    fadingEdgeLength = TextViewProps.Default.fadingEdgeLength
    filterTouchesWhenObscured = TextViewProps.Default.filterTouchesWhenObscured
    fitsSystemWindows = TextViewProps.Default.fitsSystemWindows
    focusable = TextViewProps.Default.focusable
    focusableInTouchMode = TextViewProps.Default.focusableInTouchMode
    hapticFeedbackEnabled = TextViewProps.Default.hapticFeedbackEnabled
    horizontalFadingEdgeEnabled = TextViewProps.Default.horizontalFadingEdgeEnabled
    horizontalScrollBarEnabled = TextViewProps.Default.horizontalScrollBarEnabled
    id = TextViewProps.Default.id
    importantForAccessibility = TextViewProps.Default.importantForAccessibility
    isScrollContainer = TextViewProps.Default.isScrollContainer
    keepScreenOn = TextViewProps.Default.keepScreenOn
    labelFor = TextViewProps.Default.labelFor
    layerPaint = TextViewProps.Default.layerPaint
    layerType = TextViewProps.Default.layerType
    layoutDirection = TextViewProps.Default.layoutDirection
    layoutParameters = TextViewProps.Default.layoutParameters
    longClickable = TextViewProps.Default.longClickable
    minHeight = TextViewProps.Default.minHeight
    minWidth = TextViewProps.Default.minWidth
    nestedScrollingEnabled = TextViewProps.Default.nestedScrollingEnabled
    nextFocusDownId = TextViewProps.Default.nextFocusDownId
    nextFocusForwardId = TextViewProps.Default.nextFocusForwardId
    nextFocusLeftId = TextViewProps.Default.nextFocusLeftId
    nextFocusRightId = TextViewProps.Default.nextFocusRightId
    nextFocusUpId = TextViewProps.Default.nextFocusUpId
    onClick = TextViewProps.Default.onClick
    onCreateContextMenu = TextViewProps.Default.onCreateContextMenu
    onDrag = TextViewProps.Default.onDrag
    onFocusChange = TextViewProps.Default.onFocusChange
    onGenericMotion = TextViewProps.Default.onGenericMotion
    onHover = TextViewProps.Default.onHover
    onKey = TextViewProps.Default.onKey
    onLongClick = TextViewProps.Default.onLongClick
    onSystemUiVisibilityChange = TextViewProps.Default.onSystemUiVisibilityChange
    onTouch = TextViewProps.Default.onTouch
    //outlineProvider = TextViewProps.Default.outlineProvider
    overScrollBy = TextViewProps.Default.overScrollBy
    overScrollMode = TextViewProps.Default.overScrollMode
    paddingBottom = TextViewProps.Default.paddingBottom
    paddingEnd = TextViewProps.Default.paddingEnd
    paddingStart = TextViewProps.Default.paddingStart
    paddingTop = TextViewProps.Default.paddingTop
    pivotX = TextViewProps.Default.pivotX
    pivotY = TextViewProps.Default.pivotY
    requestFocus = TextViewProps.Default.requestFocus
    rotation = TextViewProps.Default.rotation
    rotationX = TextViewProps.Default.rotationX
    rotationY = TextViewProps.Default.rotationY
    scaleX = TextViewProps.Default.scaleX
    scaleY = TextViewProps.Default.scaleY
    //scrollBarDefaultDelayBeforeFade = TextViewProps.Default.scrollBarDefaultDelayBeforeFade
    //scrollBarFadeDuration = TextViewProps.Default.scrollBarDefaultDelayBeforeFade
    //scrollBarFadingEnabled = TextViewProps.Default.scrollBarFadingEnabled
    //scrollBarSize = TextViewProps.Default.scrollBarSize
    scrollBarStyle = TextViewProps.Default.scrollBarStyle
    scrollBy = TextViewProps.Default.scrollBy
    scrollTo = TextViewProps.Default.scrollTo
    selected = TextViewProps.Default.selected
    soundEffectsEnabled = TextViewProps.Default.soundEffectsEnabled
    //stateListAnimator = TextViewProps.Default.stateListAnimator
    //systemUiVisibility =  TextViewProps.Default.systemUiVisibility
    //textAlignment = TextViewProps.Default.textAlignment
    //textDirection = TextViewProps.Default.textDirection
    transitionName = TextViewProps.Default.transitionName
    translationX = TextViewProps.Default.translationX
    translationY = TextViewProps.Default.translationY
    translationZ = TextViewProps.Default.translationZ
    verticalFadingEdgeEnabled = TextViewProps.Default.verticalFadingEdgeEnabled
    verticalScrollBarEnabled = TextViewProps.Default.verticalScrollBarEnabled
    //verticalScrollbarPosition = TextViewProps.Default.verticalScrollbarPosition
    visibility = TextViewProps.Default.visibility

    // TextView
    text = ""
  }

type ButtonProps with
  static member Default = ButtonProps.defaultProps

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Button =
  let private name = typeof<AppCompatButton>.FullName

  let setProps (onError: Exception -> unit) (view: Button) (props: IButtonProps) =
    TextView.setProps onError view props

  let private createView (context: Context) =
    let viewProvider () =  new AppCompatButton (context) :> Button
    View.create name viewProvider setProps

  let viewProvider = (name, createView)

  let internal reactComponent (props: IButtonProps) = ReactNativeElement {
    Name = name
    Props = props
    Children = ImmutableVector.empty ()
  }