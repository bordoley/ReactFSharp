namespace React.Android.Widget

open Android.Animation
open Android.Content
open Android.Content.Res
open Android.Graphics
open Android.Graphics.Drawables
open Android.Support.V4.View
open Android.Support.V4.Widget
open Android.Support.V7.Widget
open Android.Views
open Android.Widget
open ImmutableCollections
open React
open React.Android
open React.Android.Views
open System

type IFrameLayoutProps =
  inherit IViewGroupProps

type FrameLayoutProps =
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
  }

  interface IFrameLayoutProps with
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

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module private FrameLayoutProps = 
  let internal defaultProps = {
    // View Props
    AccessibilityLiveRegion = ViewGroupProps.Default.AccessibilityLiveRegion
    AccessibilityTraversalAfter = ViewGroupProps.Default.AccessibilityTraversalAfter
    AccessibilityTraversalBefore = ViewGroupProps.Default.AccessibilityTraversalBefore
    Activated = ViewGroupProps.Default.Activated
    Alpha = ViewGroupProps.Default.Alpha
    Background = ViewGroupProps.Default.Background
    Clickable = ViewGroupProps.Default.Clickable
    ContentDescription = ViewGroupProps.Default.ContentDescription
    ContextClickable = ViewGroupProps.Default.ContextClickable
    DrawingCacheEnabled = ViewGroupProps.Default.DrawingCacheEnabled
    DrawingCacheQuality = ViewGroupProps.Default.DrawingCacheQuality
    Elevation = ViewGroupProps.Default.Elevation
    Enabled = ViewGroupProps.Default.Enabled
    FadingEdgeLength = ViewGroupProps.Default.FadingEdgeLength
    FilterTouchesWhenObscured = ViewGroupProps.Default.FilterTouchesWhenObscured
    FitsSystemWindows = ViewGroupProps.Default.FitsSystemWindows
    Focusable = ViewGroupProps.Default.Focusable
    FocusableInTouchMode = ViewGroupProps.Default.FocusableInTouchMode
    HapticFeedbackEnabled = ViewGroupProps.Default.HapticFeedbackEnabled
    HorizontalFadingEdgeEnabled = ViewGroupProps.Default.HorizontalFadingEdgeEnabled
    HorizontalScrollBarEnabled = ViewGroupProps.Default.HorizontalScrollBarEnabled
    Id = ViewGroupProps.Default.Id
    ImportantForAccessibility = ViewGroupProps.Default.ImportantForAccessibility
    IsScrollContainer = ViewGroupProps.Default.IsScrollContainer
    KeepScreenOn = ViewGroupProps.Default.KeepScreenOn
    LabelFor = ViewGroupProps.Default.LabelFor
    LayerPaint = ViewGroupProps.Default.LayerPaint
    LayerType = ViewGroupProps.Default.LayerType
    LayoutDirection = ViewGroupProps.Default.LayoutDirection
    LayoutParameters = ViewGroupProps.Default.LayoutParameters
    LongClickable = ViewGroupProps.Default.LongClickable
    MinHeight = ViewGroupProps.Default.MinHeight
    MinWidth = ViewGroupProps.Default.MinWidth
    NestedScrollingEnabled = ViewGroupProps.Default.NestedScrollingEnabled
    NextFocusDownId = ViewGroupProps.Default.NextFocusDownId
    NextFocusForwardId = ViewGroupProps.Default.NextFocusForwardId
    NextFocusLeftId = ViewGroupProps.Default.NextFocusLeftId
    NextFocusRightId = ViewGroupProps.Default.NextFocusRightId
    NextFocusUpId = ViewGroupProps.Default.NextFocusUpId
    OnClick = ViewGroupProps.Default.OnClick
    OnCreateContextMenu = ViewGroupProps.Default.OnCreateContextMenu
    OnDrag = ViewGroupProps.Default.OnDrag
    OnFocusChange = ViewGroupProps.Default.OnFocusChange
    OnGenericMotion = ViewGroupProps.Default.OnGenericMotion
    OnHover = ViewGroupProps.Default.OnHover
    OnKey = ViewGroupProps.Default.OnKey
    OnLongClick = ViewGroupProps.Default.OnLongClick
    OnSystemUiVisibilityChange = ViewGroupProps.Default.OnSystemUiVisibilityChange
    OnTouch = ViewGroupProps.Default.OnTouch
    //OutlineProvider = ViewGroupProps.Default.OutlineProvider
    OverScrollBy = ViewGroupProps.Default.OverScrollBy
    OverScrollMode = ViewGroupProps.Default.OverScrollMode
    Padding = ViewGroupProps.Default.Padding
    PivotX = ViewGroupProps.Default.PivotX
    PivotY = ViewGroupProps.Default.PivotY
    RequestFocus = ViewGroupProps.Default.RequestFocus
    Rotation = ViewGroupProps.Default.Rotation
    RotationX = ViewGroupProps.Default.RotationX
    RotationY = ViewGroupProps.Default.RotationY
    ScaleX = ViewGroupProps.Default.ScaleX
    ScaleY = ViewGroupProps.Default.ScaleY
    //ScrollBarDefaultDelayBeforeFade = ViewGroupProps.Default.ScrollBarDefaultDelayBeforeFade
    //ScrollBarFadeDuration = ViewGroupProps.Default.ScrollBarDefaultDelayBeforeFade
    //ScrollBarFadingEnabled = ViewGroupProps.Default.ScrollBarFadingEnabled
    //ScrollBarSize = ViewGroupProps.Default.ScrollBarSize
    ScrollBarStyle = ViewGroupProps.Default.ScrollBarStyle
    ScrollBy = ViewGroupProps.Default.ScrollBy
    ScrollTo = ViewGroupProps.Default.ScrollTo
    Selected = ViewGroupProps.Default.Selected
    SoundEffectsEnabled = ViewGroupProps.Default.SoundEffectsEnabled
    //StateListAnimator = ViewGroupProps.Default.StateListAnimator
    //SystemUiVisibility =  ViewGroupProps.Default.SystemUiVisibility
    //TextAlignment = ViewGroupProps.Default.TextAlignment
    //TextDirection = ViewGroupProps.Default.TextDirection
    TransitionName = ViewGroupProps.Default.TransitionName
    TranslationX = ViewGroupProps.Default.TranslationX
    TranslationY = ViewGroupProps.Default.TranslationY
    TranslationZ = ViewGroupProps.Default.TranslationZ
    VerticalFadingEdgeEnabled = ViewGroupProps.Default.VerticalFadingEdgeEnabled
    VerticalScrollBarEnabled = ViewGroupProps.Default.VerticalScrollBarEnabled
    //VerticalScrollbarPosition = ViewGroupProps.Default.VerticalScrollbarPosition
    Visibility = ViewGroupProps.Default.Visibility
  }

type FrameLayoutProps with
  static member Default = FrameLayoutProps.defaultProps

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module FrameLayout =
  let private name = typeof<FrameLayout>.FullName

  let setProps (onError: Exception -> unit) (view: FrameLayout) (props: IFrameLayoutProps) =
    ViewGroup.setProps onError view props

  let private createView (context: Context) =
    let viewGroupProvider () = new FrameLayout (context)
    ViewGroup.create name viewGroupProvider setProps

  let viewProvider = (name, createView)

  let internal reactComponent
      (props: IFrameLayoutProps)
      (children: IImmutableMap<int, ReactElement>) = ReactNativeElement {
    Name = name
    Props = props
    Children = children
  }

