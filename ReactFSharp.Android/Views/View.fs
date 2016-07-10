namespace React.Android.Views

open Android.Content.Res
open Android.Graphics
open Android.Support.V4.View
open Android.Views
open FSharp.Control.Reactive
open React.Android
open System
open System.Reactive.Disposables
open System.Runtime.CompilerServices

[<Struct>]
type Padding =
  val start: int
  val top: int
  val end_: int
  val bottom: int

  new (start, top, end_, bottom) = { start = start; top = top; end_ = end_; bottom = bottom }

[<Struct>]
type Pivot =
  val x: Single
  val y: Single

  new (x, y) = { x = x; y = y }

[<Struct>]
type Translation =
  val x: Single
  val y: Single
  val z: Single

  new (x, y, z) = { x = x; y = y; z=z }

type IViewProps =
  abstract member AccessibilityLiveRegion: int
  //abstract member Activated: bool
  abstract member Alpha: float32
  abstract member BackgroundColor: Color
  abstract member BackgroundTintMode: PorterDuff.Mode
  abstract member Clickable: bool
  abstract member ContentDescription: string
  abstract member ContextClickable: bool
  abstract member Elevation: Single
  abstract member Enabled: bool
  abstract member FilterTouchesWhenObscured: bool
  abstract member Focusable: bool
  abstract member FocusableInTouchMode: bool
  abstract member HapticFeedbackEnabled: bool
  abstract member HorizontalFadingEdgeEnabled: bool
  abstract member HorizontalScrollBarEnabled: bool
  abstract member Id: int
  abstract member LayoutParameters: ViewGroup.LayoutParams
  abstract member OnClick: Func<unit, unit>
  abstract member OnCreateContextMenu: Func<IContextMenu, IContextMenuContextMenuInfo, unit>
  abstract member OnDrag: Func<DragEvent, bool>
  abstract member OnGenericMotion: Func<MotionEvent, bool>
  abstract member OnHover: Func<MotionEvent, bool>
  abstract member OnKey: Func<Keycode, KeyEvent, bool>
  abstract member OnLongClick: Func<unit, bool>
  abstract member OnSystemUiVisibilityChange: Func<StatusBarVisibility, unit>
  abstract member OnTouch: Func<MotionEvent, bool>
  abstract member Padding: Padding
  abstract member Pivot: Pivot

  // FIXME: There are a couple other variants of this property we might want to use instead.
  abstract member RequestFocus: IObservable<unit>
  abstract member ScrollBarSize: int
  abstract member ScrollBarStyle: ScrollbarStyles
  abstract member Selected: bool
  abstract member SoundEffectsEnabled: bool
  abstract member SystemUiVisibility: StatusBarVisibility
  abstract member TextAlignment: TextAlignment
  abstract member TextDirection: TextDirection
  abstract member TransitionName: string
  abstract member Translation: Translation
  abstract member VerticalFadingEdgeEnabled: bool
  abstract member VerticalScrollBarEnabled: bool
  abstract member VerticalScrollbarPosition: ScrollbarPosition
  abstract member Visibility: ViewStates

type ViewProps =
  {
    // View Props
    accessibilityLiveRegion: int
    alpha: float32
    backgroundColor: Color
    backgroundTintMode: PorterDuff.Mode
    clickable: bool
    contentDescription: string
    contextClickable: bool
    elevation: Single
    enabled: bool
    filterTouchesWhenObscured: bool
    focusable: bool
    focusableInTouchMode: bool
    hapticFeedbackEnabled: bool
    horizontalFadingEdgeEnabled: bool
    horizontalScrollBarEnabled: bool
    id: int
    layoutParameters: ViewGroup.LayoutParams
    onClick: Func<unit, unit>
    onCreateContextMenu: Func<IContextMenu, IContextMenuContextMenuInfo, unit>
    onDrag: Func<DragEvent, bool>
    onGenericMotion: Func<MotionEvent, bool>
    onHover: Func<MotionEvent, bool>
    onKey: Func<Keycode, KeyEvent, bool>
    onLongClick: Func<unit, bool>
    onSystemUiVisibilityChange: Func<StatusBarVisibility, unit>
    onTouch: Func<MotionEvent, bool>
    padding: Padding
    pivot: Pivot
    requestFocus: IObservable<unit>
    scrollBarSize: int
    scrollBarStyle: ScrollbarStyles
    selected: bool
    soundEffectsEnabled: bool
    systemUiVisibility: StatusBarVisibility
    textAlignment: TextAlignment
    textDirection: TextDirection
    transitionName: string
    translation: Translation
    verticalFadingEdgeEnabled: bool
    verticalScrollBarEnabled: bool
    verticalScrollbarPosition: ScrollbarPosition
    visibility: ViewStates
  }

  interface IViewProps with
    // View Props
    member this.AccessibilityLiveRegion = this.accessibilityLiveRegion
    member this.Alpha = this.alpha
    member this.BackgroundColor = this.backgroundColor
    member this.BackgroundTintMode = this.backgroundTintMode
    member this.Clickable = this.clickable
    member this.ContentDescription = this.contentDescription
    member this.ContextClickable = this.contextClickable
    member this.Elevation = this.elevation
    member this.Enabled = this.enabled
    member this.FilterTouchesWhenObscured = this.filterTouchesWhenObscured
    member this.Focusable = this.focusable
    member this.FocusableInTouchMode = this.focusableInTouchMode
    member this.HapticFeedbackEnabled = this.hapticFeedbackEnabled
    member this.HorizontalFadingEdgeEnabled = this.horizontalFadingEdgeEnabled
    member this.HorizontalScrollBarEnabled = this.horizontalScrollBarEnabled
    member this.Id = this.id
    member this.LayoutParameters = this.layoutParameters
    member this.OnClick = this.onClick
    member this.OnCreateContextMenu = this.onCreateContextMenu
    member this.OnDrag = this.onDrag
    member this.OnGenericMotion = this.onGenericMotion
    member this.OnHover = this.onHover
    member this.OnKey = this.onKey
    member this.OnLongClick = this.onLongClick
    member this.OnSystemUiVisibilityChange = this.onSystemUiVisibilityChange
    member this.OnTouch = this.onTouch
    member this.Padding = this.padding
    member this.Pivot = this.pivot
    member this.RequestFocus = this.requestFocus
    member this.ScrollBarSize = this.scrollBarSize
    member this.ScrollBarStyle = this.scrollBarStyle
    member this.Selected = this.selected
    member this.SoundEffectsEnabled = this.soundEffectsEnabled
    member this.SystemUiVisibility = this.systemUiVisibility
    member this.TextAlignment = this.textAlignment
    member this.TextDirection = this.textDirection
    member this.TransitionName = this.transitionName
    member this.Translation = this.translation
    member this.VerticalFadingEdgeEnabled = this.verticalFadingEdgeEnabled
    member this.VerticalScrollBarEnabled = this.verticalScrollBarEnabled
    member this.VerticalScrollbarPosition = this.verticalScrollbarPosition
    member this.Visibility = this.visibility

// This is a hack around the F# compiler. We want to ensure that
// the default event handlers are values so that they can be effectively
// cached and don't break record equality
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module private ViewProps =
  let private defaultLayoutParameters = new ViewGroup.LayoutParams(-2, -2)

  let private defaultOnClick =
    Func<unit, unit>(fun () -> ())

  let private defaultOnCreateContextMenu =
    Func<IContextMenu, IContextMenuContextMenuInfo, unit> (fun _ _ -> ())

  let private defaultOnDrag =
    Func<DragEvent, bool> (fun _ -> false)

  let private defaultOnGenericMotion =
    Func<MotionEvent, bool>(fun _ -> false)

  let private defaultOnHover =
    Func<MotionEvent, bool>(fun _ -> false)

  let private defaultOnKey =
    Func<Keycode, KeyEvent, bool>(fun _ _ -> false)

  let private defaultOnLongClick =
    Func<unit, bool>(fun _ -> false)

  let private defaultOnSystemUiVisibilityChange =
    Func<StatusBarVisibility, unit>(fun _ -> ())

  let private defaultOnTouch =
    Func<MotionEvent, bool>(fun _ -> false)

  let internal defaultRequestFocus: IObservable<unit> = Observable.empty<unit>

  let internal defaultProps = {
    accessibilityLiveRegion = ViewCompat.AccessibilityLiveRegionNone
    alpha = 1.0f
    backgroundColor = Color.White
    backgroundTintMode = PorterDuff.Mode.SrcIn
    clickable = false
    contentDescription = ""
    contextClickable = false
    elevation = 0.0f
    enabled = false
    filterTouchesWhenObscured = false
    focusable = false
    focusableInTouchMode = false
    hapticFeedbackEnabled = false
    horizontalFadingEdgeEnabled = false
    horizontalScrollBarEnabled = false
    id = 0
    layoutParameters = defaultLayoutParameters
    onClick = defaultOnClick
    onCreateContextMenu = defaultOnCreateContextMenu
    onDrag = defaultOnDrag
    onGenericMotion = defaultOnGenericMotion
    onHover = defaultOnHover
    onKey = defaultOnKey
    onLongClick = defaultOnLongClick
    onSystemUiVisibilityChange = defaultOnSystemUiVisibilityChange
    onTouch = defaultOnTouch
    padding = Unchecked.defaultof<Padding>
    pivot = Pivot(0.0f, 0.0f)
    requestFocus = defaultRequestFocus
    scrollBarSize = 0
    scrollBarStyle = ScrollbarStyles.InsideOverlay
    selected = false
    soundEffectsEnabled = true
    systemUiVisibility =  StatusBarVisibility.Visible
    textAlignment = TextAlignment.Inherit
    textDirection = TextDirection.Inherit
    transitionName = ""
    translation = Translation(0.0f, 0.0f, 0.0f)
    verticalFadingEdgeEnabled = false
    verticalScrollBarEnabled = false
    verticalScrollbarPosition = ScrollbarPosition.Default
    visibility = ViewStates.Visible
  }

type ViewProps with
  static member Default = ViewProps.defaultProps

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module View =
  type private OnClickListener (onClick) =
    inherit Java.Lang.Object ()

    static let cache =
      new ConditionalWeakTable<Func<unit, unit>, Android.Views.View.IOnClickListener>()

    static member Create(onClick) =
      cache.GetValue(
        onClick,
        fun onClick -> (new OnClickListener(onClick)) :> View.IOnClickListener
      )

    interface View.IOnClickListener with
      member this.OnClick view = onClick.Invoke ()

  type private OnCreateContextMenuListener (onCreateContextMenu) =
    inherit Java.Lang.Object ()

    static let cache =
      new ConditionalWeakTable<
          Func<IContextMenu, IContextMenuContextMenuInfo, unit>,
          Android.Views.View.IOnCreateContextMenuListener
        >()

    static member Create(onCreateContextMenu) =
      cache.GetValue(
        onCreateContextMenu,
        fun onCreateContextMenu -> (new OnCreateContextMenuListener(onCreateContextMenu)) :> View.IOnCreateContextMenuListener
      )

    interface View.IOnCreateContextMenuListener with
      member this.OnCreateContextMenu (menu, view, info) = onCreateContextMenu.Invoke(menu, info)

  type private OnDragListener (onDrag) =
    inherit Java.Lang.Object ()

    static let cache =
      new ConditionalWeakTable<Func<DragEvent, bool>, View.IOnDragListener>()

    static member Create(onDrag) =
      cache.GetValue(
        onDrag,
        fun onDrag -> (new OnDragListener(onDrag)) :> View.IOnDragListener
      )

    interface Android.Views.View.IOnDragListener with
      member this.OnDrag (view, motionEvent) = onDrag.Invoke(motionEvent)

  type private OnHoverListener (onHover) =
    inherit Java.Lang.Object ()

    static let cache =
      new ConditionalWeakTable<Func<MotionEvent, bool>, View.IOnHoverListener>()

    static member Create(onHover) =
      cache.GetValue(
        onHover,
        fun onHover -> (new OnHoverListener(onHover)) :> View.IOnHoverListener
      )

    interface Android.Views.View.IOnHoverListener with
      member this.OnHover (view, motionEvent) = onHover.Invoke(motionEvent)

  type private OnGenericMotionListener (onGenericMotion) =
    inherit Java.Lang.Object ()

    static let cache =
      new ConditionalWeakTable<Func<MotionEvent, bool>, View.IOnGenericMotionListener>()

    static member Create(onGenericMotion) =
      cache.GetValue(
        onGenericMotion,
        fun onGenericMotion -> (new OnGenericMotionListener(onGenericMotion)) :> View.IOnGenericMotionListener
      )

    interface Android.Views.View.IOnGenericMotionListener with
      member this.OnGenericMotion (view, motionEvent) = onGenericMotion.Invoke(motionEvent)

  type private OnKeyListener (onKey) =
    inherit Java.Lang.Object ()

    static let cache =
      new ConditionalWeakTable<Func<Keycode, KeyEvent, bool>, View.IOnKeyListener>()

    static member Create(onKey) =
      cache.GetValue(
        onKey,
        fun onKey -> (new OnKeyListener(onKey)) :> View.IOnKeyListener
      )

    interface View.IOnKeyListener with
      member this.OnKey (view, keyCode, keyEvent) = onKey.Invoke(keyCode, keyEvent)

  type private OnLongClickListener (onLongClick) =
    inherit Java.Lang.Object ()

    static let cache =
      new ConditionalWeakTable<Func<unit, bool>, Android.Views.View.IOnLongClickListener>()

    static member Create(onLongClick) =
      cache.GetValue(
        onLongClick,
        fun onLongClick -> (new OnLongClickListener(onLongClick)) :> View.IOnLongClickListener
      )

    interface View.IOnLongClickListener with
      member this.OnLongClick view = onLongClick.Invoke ()

  type private OnSystemUiVisibilityChangeListener (onSystemUiVisibilityChange) =
    inherit Java.Lang.Object ()

    static let cache =
      new ConditionalWeakTable<Func<StatusBarVisibility, unit>, View.IOnSystemUiVisibilityChangeListener>()

    static member Create(onSystemUiVisibilityChange) =
      cache.GetValue(
        onSystemUiVisibilityChange,
        fun onSystemUiVisibilityChange ->
          (new OnSystemUiVisibilityChangeListener(onSystemUiVisibilityChange)) :> View.IOnSystemUiVisibilityChangeListener
      )

    interface View.IOnSystemUiVisibilityChangeListener with
      member this.OnSystemUiVisibilityChange(sbv) = onSystemUiVisibilityChange.Invoke(sbv)

  type private OnTouchListener (onTouch) =
    inherit Java.Lang.Object ()

    static let cache =
      new ConditionalWeakTable<Func<MotionEvent, bool>, View.IOnTouchListener>()

    static member Create(onTouch) =
      cache.GetValue(
        onTouch,
        fun onTouch -> (new OnTouchListener(onTouch)) :> View.IOnTouchListener
      )

    interface Android.Views.View.IOnTouchListener with
      member this.OnTouch (view, motionEvent) = onTouch.Invoke(motionEvent)

  let setProps (onError: Exception -> unit) (view: View) (props: IViewProps): IDisposable =
    // Prevent Android from saving view state and trying to refresh it
    view.SaveEnabled <- false
    ViewCompat.SetSaveFromParentEnabled(view, false)

    view.SetOnClickListener (OnClickListener.Create props.OnClick)
    view.SetOnCreateContextMenuListener (OnCreateContextMenuListener.Create props.OnCreateContextMenu)
    view.SetOnDragListener (OnDragListener.Create props.OnDrag)
    view.SetOnGenericMotionListener (OnGenericMotionListener.Create props.OnGenericMotion)
    view.SetOnHoverListener (OnHoverListener.Create props.OnHover)
    view.SetOnKeyListener (OnKeyListener.Create props.OnKey)
    view.SetOnLongClickListener (OnLongClickListener.Create props.OnLongClick)
    view.SetOnSystemUiVisibilityChangeListener (
      OnSystemUiVisibilityChangeListener.Create props.OnSystemUiVisibilityChange
    )
    view.SetOnTouchListener(OnTouchListener.Create props.OnTouch)

    //view.SetOnContextClickListener null
    //view.SetOnApplyWindowInsetsListener null

    ViewCompat.SetAccessibilityLiveRegion(view, props.AccessibilityLiveRegion)
    //view.Activated
    ViewCompat.SetAlpha(view, props.Alpha)
    view.SetBackgroundColor props.BackgroundColor
    ViewCompat.SetBackgroundTintMode(view, props.BackgroundTintMode)
    view.Clickable <- props.Clickable
    view.ContentDescription <- props.ContentDescription
    view.ContextClickable <- props.ContextClickable
    ViewCompat.SetElevation(view,props.Elevation)
    view.Enabled <- props.Enabled
    view.FilterTouchesWhenObscured <- props.FilterTouchesWhenObscured
    view.Focusable <- props.Focusable
    view.FocusableInTouchMode <- props.FocusableInTouchMode
    view.HapticFeedbackEnabled <- props.HapticFeedbackEnabled
    view.HorizontalFadingEdgeEnabled <- props.HorizontalFadingEdgeEnabled
    view.HorizontalScrollBarEnabled <- props.HorizontalScrollBarEnabled
    view.Id <- props.Id
    view.LayoutParameters <- props.LayoutParameters
    ViewCompat.SetPaddingRelative(
      view, 
      props.Padding.start, 
      props.Padding.top, 
      props.Padding.end_, 
      props.Padding.bottom
    )
    ViewCompat.SetPivotX(view, props.Pivot.x)
    ViewCompat.SetPivotY(view, props.Pivot.y)
    view.ScrollBarSize <- props.ScrollBarSize
    view.ScrollBarStyle <- props.ScrollBarStyle
    view.Selected <- props.Selected
    view.SoundEffectsEnabled <- props.SoundEffectsEnabled
    view.SystemUiVisibility <- props.SystemUiVisibility
    view.TextAlignment <- props.TextAlignment
    view.TextDirection <- props.TextDirection
    ViewCompat.SetTransitionName(view, props.TransitionName)
    ViewCompat.SetTranslationX(view, props.Translation.x)
    ViewCompat.SetTranslationY(view, props.Translation.y)
    ViewCompat.SetTranslationZ(view, props.Translation.z)
    view.VerticalFadingEdgeEnabled <- props.VerticalFadingEdgeEnabled
    view.VerticalScrollBarEnabled <- props.VerticalScrollBarEnabled
    view.VerticalScrollbarPosition <- props.VerticalScrollbarPosition
    view.Visibility <- props.Visibility

    if props.RequestFocus <> ViewProps.defaultRequestFocus then
      props.RequestFocus
      |> Observable.observeOn Scheduler.mainLoopScheduler
      |> Observable.iter (view.RequestFocus >> ignore)
      |> Observable.subscribeWithError
          (fun _ -> ())
          onError
    else Disposable.Empty



