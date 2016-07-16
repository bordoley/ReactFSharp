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

type IEditTextProps =
  inherit ITextViewProps

type EditTextProps =
  {
    // View Props
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
    Padding: ViewPadding
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

    // TextView Props
    text: string
  }

  interface IEditTextProps with
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
    member this.Padding = this.Padding
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

    // TextView Props
    member this.Text = this.text

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module private EditTextProps =
  let internal defaultProps = {
    // View Props
    AccessibilityLiveRegion = TextViewProps.Default.AccessibilityLiveRegion
    AccessibilityTraversalAfter = TextViewProps.Default.AccessibilityTraversalAfter
    AccessibilityTraversalBefore = TextViewProps.Default.AccessibilityTraversalBefore
    Activated = TextViewProps.Default.Activated
    Alpha = TextViewProps.Default.Alpha
    Background = TextViewProps.Default.Background
    Clickable = TextViewProps.Default.Clickable
    ContentDescription = TextViewProps.Default.ContentDescription
    ContextClickable = TextViewProps.Default.ContextClickable
    DrawingCacheEnabled = TextViewProps.Default.DrawingCacheEnabled
    DrawingCacheQuality = TextViewProps.Default.DrawingCacheQuality
    Elevation = TextViewProps.Default.Elevation
    Enabled = TextViewProps.Default.Enabled
    FadingEdgeLength = TextViewProps.Default.FadingEdgeLength
    FilterTouchesWhenObscured = TextViewProps.Default.FilterTouchesWhenObscured
    FitsSystemWindows = TextViewProps.Default.FitsSystemWindows
    Focusable = TextViewProps.Default.Focusable
    FocusableInTouchMode = TextViewProps.Default.FocusableInTouchMode
    HapticFeedbackEnabled = TextViewProps.Default.HapticFeedbackEnabled
    HorizontalFadingEdgeEnabled = TextViewProps.Default.HorizontalFadingEdgeEnabled
    HorizontalScrollBarEnabled = TextViewProps.Default.HorizontalScrollBarEnabled
    Id = TextViewProps.Default.Id
    ImportantForAccessibility = TextViewProps.Default.ImportantForAccessibility
    IsScrollContainer = TextViewProps.Default.IsScrollContainer
    KeepScreenOn = TextViewProps.Default.KeepScreenOn
    LabelFor = TextViewProps.Default.LabelFor
    LayerPaint = TextViewProps.Default.LayerPaint
    LayerType = TextViewProps.Default.LayerType
    LayoutDirection = TextViewProps.Default.LayoutDirection
    LayoutParameters = TextViewProps.Default.LayoutParameters
    LongClickable = TextViewProps.Default.LongClickable
    MinHeight = TextViewProps.Default.MinHeight
    MinWidth = TextViewProps.Default.MinWidth
    NestedScrollingEnabled = TextViewProps.Default.NestedScrollingEnabled
    NextFocusDownId = TextViewProps.Default.NextFocusDownId
    NextFocusForwardId = TextViewProps.Default.NextFocusForwardId
    NextFocusLeftId = TextViewProps.Default.NextFocusLeftId
    NextFocusRightId = TextViewProps.Default.NextFocusRightId
    NextFocusUpId = TextViewProps.Default.NextFocusUpId
    OnClick = TextViewProps.Default.OnClick
    OnCreateContextMenu = TextViewProps.Default.OnCreateContextMenu
    OnDrag = TextViewProps.Default.OnDrag
    OnFocusChange = TextViewProps.Default.OnFocusChange
    OnGenericMotion = TextViewProps.Default.OnGenericMotion
    OnHover = TextViewProps.Default.OnHover
    OnKey = TextViewProps.Default.OnKey
    OnLongClick = TextViewProps.Default.OnLongClick
    OnSystemUiVisibilityChange = TextViewProps.Default.OnSystemUiVisibilityChange
    OnTouch = TextViewProps.Default.OnTouch
    //OutlineProvider = TextViewProps.Default.OutlineProvider
    OverScrollBy = TextViewProps.Default.OverScrollBy
    OverScrollMode = TextViewProps.Default.OverScrollMode
    Padding = TextViewProps.Default.Padding
    PivotX = TextViewProps.Default.PivotX
    PivotY = TextViewProps.Default.PivotY
    RequestFocus = TextViewProps.Default.RequestFocus
    Rotation = TextViewProps.Default.Rotation
    RotationX = TextViewProps.Default.RotationX
    RotationY = TextViewProps.Default.RotationY
    ScaleX = TextViewProps.Default.ScaleX
    ScaleY = TextViewProps.Default.ScaleY
    //ScrollBarDefaultDelayBeforeFade = TextViewProps.Default.ScrollBarDefaultDelayBeforeFade
    //ScrollBarFadeDuration = TextViewProps.Default.ScrollBarDefaultDelayBeforeFade
    //ScrollBarFadingEnabled = TextViewProps.Default.ScrollBarFadingEnabled
    //ScrollBarSize = TextViewProps.Default.ScrollBarSize
    ScrollBarStyle = TextViewProps.Default.ScrollBarStyle
    ScrollBy = TextViewProps.Default.ScrollBy
    ScrollTo = TextViewProps.Default.ScrollTo
    Selected = TextViewProps.Default.Selected
    SoundEffectsEnabled = TextViewProps.Default.SoundEffectsEnabled
    //StateListAnimator = TextViewProps.Default.StateListAnimator
    //SystemUiVisibility =  TextViewProps.Default.SystemUiVisibility
    //TextAlignment = TextViewProps.Default.TextAlignment
    //TextDirection = TextViewProps.Default.TextDirection
    TransitionName = TextViewProps.Default.TransitionName
    TranslationX = TextViewProps.Default.TranslationX
    TranslationY = TextViewProps.Default.TranslationY
    TranslationZ = TextViewProps.Default.TranslationZ
    VerticalFadingEdgeEnabled = TextViewProps.Default.VerticalFadingEdgeEnabled
    VerticalScrollBarEnabled = TextViewProps.Default.VerticalScrollBarEnabled
    //VerticalScrollbarPosition = TextViewProps.Default.VerticalScrollbarPosition
    Visibility = TextViewProps.Default.Visibility

    // TextView
    text = ""
  }

type EditTextProps with
  static member Default = EditTextProps.defaultProps

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module EditText =
  let private name = typeof<AppCompatEditText>.FullName

  let setProps (onError: Exception -> unit) (view: EditText) (props: IEditTextProps) =
    // FIXME: hack
    view.SetTextColor Color.Blue
    TextView.setProps onError view props

  let private createView (context: Context) =
    let viewProvider () = new AppCompatEditText (context) :> EditText
    View.create name viewProvider setProps

  let viewProvider = (name, createView)

  let internal reactComponent (props: IEditTextProps) = ReactNativeElement {
    Name = name
    Props = props
    Children = ImmutableVector.empty ()
  }