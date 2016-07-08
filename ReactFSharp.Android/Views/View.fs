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

type Translation =
  val x: Single
  val y: Single
  val z: Single

  new (x, y, z) = { x = x; y = y; z=z }

type IViewProps =
  abstract member Alpha: float32
  abstract member BackgroundColor: Color
  abstract member BackgroundTintMode: PorterDuff.Mode
  abstract member Clickable: bool
  abstract member ContentDescription: string
  abstract member ContextClickable: bool
  abstract member LayoutParameters: ViewGroup.LayoutParams
  abstract member OnClick: unit -> unit
  abstract member OnCreateContextMenu: IContextMenu * IContextMenuContextMenuInfo -> unit
  abstract member OnDrag: DragEvent -> bool
  abstract member OnGenericMotion: MotionEvent -> bool
  abstract member OnHover: MotionEvent -> bool
  abstract member OnKey: Keycode * KeyEvent -> bool
  abstract member OnLongClick: unit -> bool
  abstract member OnSystemUiVisibilityChange: StatusBarVisibility -> unit
  abstract member OnTouch: MotionEvent -> bool
  abstract member Padding: Padding
  abstract member Pivot: Pivot
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
    alpha: float32
    backgroundColor: Color
    backgroundTintMode: PorterDuff.Mode
    clickable: bool
    contentDescription: string
    contextClickable: bool
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
    member this.Alpha = this.alpha
    member this.BackgroundColor = this.backgroundColor
    member this.BackgroundTintMode = this.backgroundTintMode
    member this.Clickable = this.clickable
    member this.ContentDescription = this.contentDescription
    member this.ContextClickable = this.contextClickable
    member this.LayoutParameters = this.layoutParameters
    member this.OnClick () = this.onClick ()
    member this.OnCreateContextMenu (menu, info) = this.onCreateContextMenu menu info
    member this.OnDrag de = this.onDrag de
    member this.OnGenericMotion me = this.onGenericMotion me 
    member this.OnHover me = this.onHover me
    member this.OnKey (keyCode, keyEvent) = this.onKey keyCode keyEvent
    member this.OnLongClick () = this.onLongClick ()
    member this.OnSystemUiVisibilityChange sbv = this.onSystemUiVisibilityChange sbv
    member this.OnTouch me = this.onTouch me
    member this.Padding = this.padding
    member this.Pivot = this.pivot
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
          IContextMenu * IContextMenuContextMenuInfo -> unit, 
          Android.Views.View.IOnCreateContextMenuListener
        >()

    static member Create(onCreateContextMenu) =
      cache.GetValue(
        onCreateContextMenu, 
        fun onClick -> (new OnCreateContextMenuListener(onCreateContextMenu)) :> View.IOnCreateContextMenuListener
      )

    interface View.IOnCreateContextMenuListener with
      member this.OnCreateContextMenu (menu, view, info) = onCreateContextMenu (menu, info)
    
  type private OnDragListener (onDrag) =
    inherit Java.Lang.Object () 

    static let cache = 
      new ConditionalWeakTable<DragEvent -> bool, View.IOnDragListener>()

    static member Create(onDrag) =
      cache.GetValue(
        onDrag,
        fun onDrag -> 
          (new OnDragListener(onDrag)) :> View.IOnDragListener
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
        fun onHover -> 
          (new OnHoverListener(onHover)) :> View.IOnHoverListener
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
        fun onGenericMotion -> 
          (new OnGenericMotionListener(onGenericMotion)) :> View.IOnGenericMotionListener
      )

    interface Android.Views.View.IOnGenericMotionListener with 
      member this.OnGenericMotion (view, motionEvent) = onGenericMotion(motionEvent)

  type private OnKeyListener (onKey) =
    inherit Java.Lang.Object ()

    static let cache = 
      new ConditionalWeakTable<Keycode * KeyEvent -> bool, View.IOnKeyListener>()

    static member Create(onKey) =
      cache.GetValue(
        onKey, 
        fun onKey -> (new OnKeyListener(onKey)) :> View.IOnKeyListener
      )

    interface View.IOnKeyListener with
      member this.OnKey (view, keyCode, keyEvent) = onKey (keyCode, keyEvent)

  type private OnLongClickListener (onLongClick) =
    inherit Java.Lang.Object ()

    static let cache = 
      new ConditionalWeakTable<unit -> bool, Android.Views.View.IOnLongClickListener>()

    static member Create(onClick) =
      cache.GetValue(
        onClick, 
        fun onClick -> (new OnLongClickListener(onClick)) :> View.IOnLongClickListener
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
        fun onTcouh -> 
          (new OnTouchListener(onTouch)) :> View.IOnTouchListener
      )

    interface Android.Views.View.IOnTouchListener with 
      member this.OnTouch (view, motionEvent) = onTouch(motionEvent)

  let defaultProps = {
    alpha = 1.0f
    backgroundColor = Color.White
    backgroundTintMode = PorterDuff.Mode.SrcIn
    clickable = true
    contentDescription = ""
    contextClickable = true
    layoutParameters = new ViewGroup.LayoutParams(-2, -2)
    onClick = fun () -> ()
    onCreateContextMenu = fun _ _ -> ()
    onDrag = fun _ -> false
    onGenericMotion = fun _ -> false
    onHover = fun _ -> false
    onKey = fun _ _ -> false
    onLongClick = fun () -> false
    onSystemUiVisibilityChange = fun _ -> ()
    onTouch = fun _ -> false
    padding = Unchecked.defaultof<Padding>
    pivot = Pivot(0.0f, 0.0f)
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

  let dispose (view: View) =
    view.SetOnClickListener null
    view.SetOnCreateContextMenuListener null
    view.SetOnDragListener null
    view.SetOnGenericMotionListener null
    view.SetOnHoverListener null
    view.SetOnKeyListener null
    view.SetOnLongClickListener null
    view.SetOnSystemUiVisibilityChangeListener null
    view.SetOnTouchListener null
   
    //view.SetOnContextClickListener null
    //view.SetOnApplyWindowInsetsListener null

    ()

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

    view.Alpha <- props.Alpha
    view.SetBackgroundColor props.BackgroundColor
    view.BackgroundTintMode <- props.BackgroundTintMode
    view.Clickable <- props.Clickable
    view.ContentDescription <- props.ContentDescription
    view.ContextClickable <- props.ContextClickable
    view.LayoutParameters <- props.LayoutParameters
    view.SetPaddingRelative(props.Padding.start, props.Padding.top, props.Padding.end_, props.Padding.bottom)
    view.PivotX <- props.Pivot.x
    view.PivotY <- props.Pivot.y
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



