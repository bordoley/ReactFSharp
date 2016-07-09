namespace React.Android.Views

open Android.Content.Res
open Android.Graphics
open Android.Views

open System
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
  abstract member AccessibilityLiveRegion: AccessibilityLiveRegion
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
  abstract member OnClick: (unit -> unit) with get 
  abstract member OnCreateContextMenu: (IContextMenu -> IContextMenuContextMenuInfo -> unit) with get 
  abstract member OnDrag: (DragEvent -> bool) with get 
  abstract member OnGenericMotion: (MotionEvent -> bool) with get 
  abstract member OnHover: (MotionEvent -> bool) with get 
  abstract member OnKey: (Keycode -> KeyEvent -> bool) with get 
  abstract member OnLongClick: (unit -> bool) with get 
  abstract member OnSystemUiVisibilityChange: (StatusBarVisibility -> unit) with get 
  abstract member OnTouch: (MotionEvent -> bool) with get 
  abstract member Padding: Padding
  abstract member Pivot: Pivot
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

// This is a hack around the F# compiler. We want to ensure that
// the default event handlers are values so that they can be effectively
// cached
module private  ViewDefaultEventHandlers =
  let onClick =
    let f () = ()
    f

  let onCreateContextMenu: IContextMenu -> IContextMenuContextMenuInfo -> unit =
    let f _ _ = ()
    f

  let onDrag: DragEvent -> bool =
    let f _  = false
    f

  let onGenericMotion: MotionEvent -> bool =
    let f _  = false
    f

  let onHover: MotionEvent -> bool =
    let f _  = false
    f

  let onKey: Keycode -> KeyEvent -> bool =
    let f _ _ = false
    f

  let onLongClick: unit -> bool =
    let f _  = false
    f

  let onSystemUiVisibilityChange: StatusBarVisibility -> unit =
    let f _ = ()
    f

  let onTouch: MotionEvent -> bool =
    let f _  = false
    f

type ViewProps =
  {
    // View Props
    accessibilityLiveRegion: AccessibilityLiveRegion
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
    onClick: unit -> unit
    onCreateContextMenu: IContextMenu -> IContextMenuContextMenuInfo -> unit
    onDrag: DragEvent -> bool
    onGenericMotion: MotionEvent -> bool
    onHover: MotionEvent -> bool
    onKey: Keycode -> KeyEvent -> bool
    onLongClick: unit -> bool
    onSystemUiVisibilityChange: StatusBarVisibility -> unit
    onTouch: MotionEvent -> bool
    padding: Padding
    pivot: Pivot
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

  static member Default = {
    accessibilityLiveRegion = AccessibilityLiveRegion.None
    alpha = 1.0f
    backgroundColor = Color.White
    backgroundTintMode = PorterDuff.Mode.SrcIn
    clickable = true
    contentDescription = ""
    contextClickable = true
    elevation = 0.0f
    enabled = true
    filterTouchesWhenObscured = false
    focusable = false
    focusableInTouchMode = false
    hapticFeedbackEnabled = false
    horizontalFadingEdgeEnabled = false
    horizontalScrollBarEnabled = false
    id = 0
    layoutParameters = new ViewGroup.LayoutParams(-2, -2)
    onClick = ViewDefaultEventHandlers.onClick
    onCreateContextMenu = ViewDefaultEventHandlers.onCreateContextMenu
    onDrag = ViewDefaultEventHandlers.onDrag
    onGenericMotion = ViewDefaultEventHandlers.onGenericMotion
    onHover = ViewDefaultEventHandlers.onHover
    onKey = ViewDefaultEventHandlers.onKey
    onLongClick = ViewDefaultEventHandlers.onLongClick
    onSystemUiVisibilityChange = ViewDefaultEventHandlers.onSystemUiVisibilityChange
    onTouch = ViewDefaultEventHandlers.onTouch
    padding = Unchecked.defaultof<Padding>
    pivot = Pivot(0.0f, 0.0f)
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

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module View =
  type private OnClickListener (onClick) =
    inherit Java.Lang.Object ()

    static let cache = 
      new ConditionalWeakTable<unit -> unit, Android.Views.View.IOnClickListener>()

    static member Create(onClick) =
      cache.GetValue(
        onClick, 
        fun onClick -> (new OnClickListener(onClick)) :> View.IOnClickListener
      )


    interface View.IOnClickListener with
      member this.OnClick view = onClick ()

  type private OnCreateContextMenuListener (onCreateContextMenu) =
    inherit Java.Lang.Object ()

    static let cache = 
      new ConditionalWeakTable<
          IContextMenu -> IContextMenuContextMenuInfo -> unit, 
          Android.Views.View.IOnCreateContextMenuListener
        >()

    static member Create(onCreateContextMenu) =
      cache.GetValue(
        onCreateContextMenu, 
        fun onCreateContextMenu -> (new OnCreateContextMenuListener(onCreateContextMenu)) :> View.IOnCreateContextMenuListener
      )

    interface View.IOnCreateContextMenuListener with
      member this.OnCreateContextMenu (menu, view, info) = onCreateContextMenu menu info
    
  type private OnDragListener (onDrag) =
    inherit Java.Lang.Object () 

    static let cache = 
      new ConditionalWeakTable<DragEvent -> bool, View.IOnDragListener>()

    static member Create(onDrag) =
      cache.GetValue(
        onDrag,
        fun onDrag -> (new OnDragListener(onDrag)) :> View.IOnDragListener
      )

    interface Android.Views.View.IOnDragListener with 
      member this.OnDrag (view, motionEvent) = onDrag(motionEvent)

  type private OnHoverListener (onHover) =
    inherit Java.Lang.Object () 

    static let cache = 
      new ConditionalWeakTable<MotionEvent -> bool, View.IOnHoverListener>()

    static member Create(onHover) =
      cache.GetValue(
        onHover,
        fun onHover -> (new OnHoverListener(onHover)) :> View.IOnHoverListener
      )

    interface Android.Views.View.IOnHoverListener with 
      member this.OnHover (view, motionEvent) = onHover(motionEvent)

  type private OnGenericMotionListener (onGenericMotion) =
    inherit Java.Lang.Object () 

    static let cache = 
      new ConditionalWeakTable<MotionEvent -> bool, View.IOnGenericMotionListener>()

    static member Create(onGenericMotion) =
      cache.GetValue(
        onGenericMotion,
        fun onGenericMotion -> (new OnGenericMotionListener(onGenericMotion)) :> View.IOnGenericMotionListener
      )

    interface Android.Views.View.IOnGenericMotionListener with 
      member this.OnGenericMotion (view, motionEvent) = onGenericMotion(motionEvent)

  type private OnKeyListener (onKey) =
    inherit Java.Lang.Object ()

    static let cache = 
      new ConditionalWeakTable<Keycode -> KeyEvent -> bool, View.IOnKeyListener>()

    static member Create(onKey) =
      cache.GetValue(
        onKey, 
        fun onKey -> (new OnKeyListener(onKey)) :> View.IOnKeyListener
      )

    interface View.IOnKeyListener with
      member this.OnKey (view, keyCode, keyEvent) = onKey keyCode keyEvent

  type private OnLongClickListener (onLongClick) =
    inherit Java.Lang.Object ()

    static let cache = 
      new ConditionalWeakTable<unit -> bool, Android.Views.View.IOnLongClickListener>()

    static member Create(onLongClick) =
      cache.GetValue(
        onLongClick, 
        fun onLongClick -> (new OnLongClickListener(onLongClick)) :> View.IOnLongClickListener
      )

    interface View.IOnLongClickListener with
      member this.OnLongClick view = onLongClick ()

  type private OnSystemUiVisibilityChangeListener (onSystemUiVisibilityChange) =
    inherit Java.Lang.Object ()

    static let cache = 
      new ConditionalWeakTable<StatusBarVisibility -> unit, View.IOnSystemUiVisibilityChangeListener>()

    static member Create(onSystemUiVisibilityChange) =
      cache.GetValue(
        onSystemUiVisibilityChange, 
        fun onSystemUiVisibilityChange -> 
          (new OnSystemUiVisibilityChangeListener(onSystemUiVisibilityChange)) :> View.IOnSystemUiVisibilityChangeListener
      )

    interface View.IOnSystemUiVisibilityChangeListener with
      member this.OnSystemUiVisibilityChange(sbv) = onSystemUiVisibilityChange sbv

  type private OnTouchListener (onTouch) =
    inherit Java.Lang.Object () 

    static let cache = 
      new ConditionalWeakTable<MotionEvent -> bool, View.IOnTouchListener>()

    static member Create(onTouch) =
      cache.GetValue(
        onTouch,
        fun onTouch -> (new OnTouchListener(onTouch)) :> View.IOnTouchListener
      )

    interface Android.Views.View.IOnTouchListener with 
      member this.OnTouch (view, motionEvent) = onTouch(motionEvent)

  let setProps (view: View) (props: IViewProps) =
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

    // FIXME: Might make sense to take a dependency on the compat package
    // and hide a lot of the underyling platform differences.
    //view.SetOnContextClickListener null
    //view.SetOnApplyWindowInsetsListener null

    view.AccessibilityLiveRegion <- props.AccessibilityLiveRegion
    //view.Activated
    view.Alpha <- props.Alpha
    view.SetBackgroundColor props.BackgroundColor
    view.BackgroundTintMode <- props.BackgroundTintMode
    view.Clickable <- props.Clickable
    view.ContentDescription <- props.ContentDescription
    view.ContextClickable <- props.ContextClickable
    view.Elevation <- props.Elevation
    view.Enabled <- props.Enabled
    view.FilterTouchesWhenObscured <- props.FilterTouchesWhenObscured
    view.Focusable <- props.Focusable
    view.FocusableInTouchMode <- props.FocusableInTouchMode
    view.HapticFeedbackEnabled <- props.HapticFeedbackEnabled
    view.HorizontalFadingEdgeEnabled <- props.HorizontalFadingEdgeEnabled
    view.HorizontalScrollBarEnabled <- props.HorizontalScrollBarEnabled
    view.Id <- props.Id
    view.LayoutParameters <- props.LayoutParameters
    view.SetPaddingRelative(props.Padding.start, props.Padding.top, props.Padding.end_, props.Padding.bottom)
    view.PivotX <- props.Pivot.x
    view.PivotY <- props.Pivot.y
    view.ScrollBarSize <- props.ScrollBarSize
    view.ScrollBarStyle <- props.ScrollBarStyle 
    view.Selected <- props.Selected
    view.SoundEffectsEnabled <- props.SoundEffectsEnabled
    view.SystemUiVisibility <- props.SystemUiVisibility
    view.TextAlignment <- props.TextAlignment
    view.TextDirection <- props.TextDirection
    view.TransitionName <- props.TransitionName
    view.TranslationX <- props.Translation.x
    view.TranslationY <- props.Translation.y
    view.TranslationZ <- props.Translation.z
    view.VerticalFadingEdgeEnabled <- props.VerticalFadingEdgeEnabled
    view.VerticalScrollBarEnabled <- props.VerticalScrollBarEnabled
    view.VerticalScrollbarPosition <- props.VerticalScrollbarPosition
    view.Visibility <- props.Visibility


