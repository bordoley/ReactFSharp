﻿namespace React.Android.Views

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

  abstract member AddStatesFromChildren: bool
  abstract member ClipChildren: bool
  abstract member ClipToPadding: bool
  abstract member DescendantFocusability: DescendantFocusability
  abstract member MotionEventSplittingEnabled: bool
  //layoutAnimation
  abstract member LayoutMode: int
  // LayoutTransition
  abstract member PersistentDrawingCache: PersistentDrawingCaches
  abstract member TouchscreenBlocksFocus: bool
  abstract member TransitionGroup: bool

type ViewGroupProps =
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

    // ViewGroup Props
    AddStatesFromChildren: bool
    ClipChildren: bool
    ClipToPadding: bool
    DescendantFocusability: DescendantFocusability
    MotionEventSplittingEnabled: bool
    //layoutAnimation
    LayoutMode: int
    // LayoutTransition
    PersistentDrawingCache: PersistentDrawingCaches
    TouchscreenBlocksFocus: bool
    TransitionGroup: bool
  }

  interface IViewGroupProps with
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

    // ViewGroup Props
    member this.AddStatesFromChildren = this.AddStatesFromChildren
    member this.ClipChildren = this.ClipChildren
    member this.ClipToPadding = this.ClipToPadding
    member this.DescendantFocusability = this.DescendantFocusability
    member this.MotionEventSplittingEnabled = this.MotionEventSplittingEnabled
    //layoutAnimation
    member this.LayoutMode = this.LayoutMode
    // LayoutTransition
    member this.PersistentDrawingCache = this.PersistentDrawingCache
    member this.TouchscreenBlocksFocus = this.TouchscreenBlocksFocus
    member this.TransitionGroup = this.TransitionGroup

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module private ViewGroupProps =
  let internal defaultProps = {
    // View Props
    AccessibilityLiveRegion = ViewProps.Default.AccessibilityLiveRegion
    AccessibilityTraversalAfter = ViewProps.Default.AccessibilityTraversalAfter
    AccessibilityTraversalBefore = ViewProps.Default.AccessibilityTraversalBefore
    Activated = ViewProps.Default.Activated
    Alpha = ViewProps.Default.Alpha
    Background = ViewProps.Default.Background
    CameraDistance = ViewProps.Default.CameraDistance
    Clickable = ViewProps.Default.Clickable
    ClipBounds = ViewProps.Default.ClipBounds
    ClipToOutline = ViewProps.Default.ClipToOutline
    ContentDescription = ViewProps.Default.ContentDescription
    ContextClickable = ViewProps.Default.ContextClickable
    DrawingCacheBackgroundColor = ViewProps.Default.DrawingCacheBackgroundColor
    DrawingCacheEnabled = ViewProps.Default.DrawingCacheEnabled
    DrawingCacheQuality = ViewProps.Default.DrawingCacheQuality
    Elevation = ViewProps.Default.Elevation
    Enabled = ViewProps.Default.Enabled
    FadingEdgeLength = ViewProps.Default.FadingEdgeLength
    FilterTouchesWhenObscured = ViewProps.Default.FilterTouchesWhenObscured
    FitsSystemWindows = ViewProps.Default.FitsSystemWindows
    Focusable = ViewProps.Default.Focusable
    FocusableInTouchMode = ViewProps.Default.FocusableInTouchMode
    HapticFeedbackEnabled = ViewProps.Default.HapticFeedbackEnabled
    HorizontalFadingEdgeEnabled = ViewProps.Default.HorizontalFadingEdgeEnabled
    HorizontalScrollBarEnabled = ViewProps.Default.HorizontalScrollBarEnabled
    Id = ViewProps.Default.Id
    ImportantForAccessibility = ViewProps.Default.ImportantForAccessibility
    IsScrollContainer = ViewProps.Default.IsScrollContainer
    KeepScreenOn = ViewProps.Default.KeepScreenOn
    LabelFor = ViewProps.Default.LabelFor
    LayerPaint = ViewProps.Default.LayerPaint
    LayerType = ViewProps.Default.LayerType
    LayoutDirection = ViewProps.Default.LayoutDirection
    LayoutParameters = ViewProps.Default.LayoutParameters
    LongClickable = ViewProps.Default.LongClickable
    MinHeight = ViewProps.Default.MinHeight
    MinWidth = ViewProps.Default.MinWidth
    NestedScrollingEnabled = ViewProps.Default.NestedScrollingEnabled
    NextFocusDownId = ViewProps.Default.NextFocusDownId
    NextFocusForwardId = ViewProps.Default.NextFocusForwardId
    NextFocusLeftId = ViewProps.Default.NextFocusLeftId
    NextFocusRightId = ViewProps.Default.NextFocusRightId
    NextFocusUpId = ViewProps.Default.NextFocusUpId
    OnClick = ViewProps.Default.OnClick
    OnCreateContextMenu = ViewProps.Default.OnCreateContextMenu
    OnDrag = ViewProps.Default.OnDrag
    OnFocusChange = ViewProps.Default.OnFocusChange
    OnGenericMotion = ViewProps.Default.OnGenericMotion
    OnHover = ViewProps.Default.OnHover
    OnKey = ViewProps.Default.OnKey
    OnLongClick = ViewProps.Default.OnLongClick
    OnSystemUiVisibilityChange = ViewProps.Default.OnSystemUiVisibilityChange
    OnTouch = ViewProps.Default.OnTouch
    //OutlineProvider = ViewProps.Default.OutlineProvider
    OverScrollMode = ViewProps.Default.OverScrollMode
    Padding = ViewProps.Default.Padding
    Pivot = ViewProps.Default.Pivot
    RequestFocus = ViewProps.Default.RequestFocus
    Rotation = ViewProps.Default.Rotation
    Scale = ViewProps.Default.Scale
    ScrollBarDefaultDelayBeforeFade = ViewProps.Default.ScrollBarDefaultDelayBeforeFade
    ScrollBarFadeDuration = ViewProps.Default.ScrollBarDefaultDelayBeforeFade
    ScrollBarFadingEnabled = ViewProps.Default.ScrollBarFadingEnabled
    ScrollBarSize = ViewProps.Default.ScrollBarSize
    ScrollBarStyle = ViewProps.Default.ScrollBarStyle
    ScrollBy = ViewProps.Default.ScrollBy
    ScrollTo = ViewProps.Default.ScrollTo
    Selected = ViewProps.Default.Selected
    SoundEffectsEnabled = ViewProps.Default.SoundEffectsEnabled
    //StateListAnimator = ViewProps.Default.StateListAnimator
    SystemUiVisibility =  ViewProps.Default.SystemUiVisibility
    TextAlignment = ViewProps.Default.TextAlignment
    TextDirection = ViewProps.Default.TextDirection
    TransitionName = ViewProps.Default.TransitionName
    Translation = ViewProps.Default.Translation
    VerticalFadingEdgeEnabled = ViewProps.Default.VerticalFadingEdgeEnabled
    VerticalScrollBarEnabled = ViewProps.Default.VerticalScrollBarEnabled
    VerticalScrollbarPosition = ViewProps.Default.VerticalScrollbarPosition
    Visibility = ViewProps.Default.Visibility

    // ViewGroup Props
    AddStatesFromChildren = false
    ClipChildren = true
    ClipToPadding = true
    DescendantFocusability = DescendantFocusability.AfterDescendants
    MotionEventSplittingEnabled = false
    //layoutAnimation
    LayoutMode = -1
    // LayoutTransition
    PersistentDrawingCache = PersistentDrawingCaches.ScrollingCache
    TouchscreenBlocksFocus = false
    TransitionGroup = false
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
      (createNativeView: string (* view name *) -> IReactView<View>) =

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

  let setProps (onError: Exception -> unit) (view: ViewGroup) (props: IViewGroupProps) =
    view.SetAddStatesFromChildren props.AddStatesFromChildren
    view.SetClipChildren props.ClipChildren
    view.SetClipToPadding props.ClipToPadding
    view.DescendantFocusability <- props.DescendantFocusability

    ViewGroupCompat.SetMotionEventSplittingEnabled(
      view,
      props.MotionEventSplittingEnabled
    )
    //layoutAnimation

    ViewGroupCompat.SetLayoutMode(view, props.LayoutMode)

    // LayoutTransition
    view.PersistentDrawingCache <- props.PersistentDrawingCache

    if Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop then
      view.TouchscreenBlocksFocus <- props.TouchscreenBlocksFocus
   
    ViewGroupCompat.SetTransitionGroup(view, props.TransitionGroup)
    View.setProps onError view props