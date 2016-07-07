namespace React.Android.Views

open Android.Content.Res
open Android.Graphics
open Android.Views

open System

[<Struct>]
type Padding = 
  val start: int
  val top: int 
  val end_: int
  val bottom: int

  new (start, top, end_, bottom) = { start = start; top = top; end_ = end_; bottom = bottom }

type IViewProps =
  abstract member Alpha: float32
  abstract member BackgroundColor: Color
  abstract member BackgroundTintMode: Option<PorterDuff.Mode>
  abstract member Clickable: bool
  abstract member ContentDescription: string
  abstract member ContextClickable: bool
  abstract member LayoutParameters: ViewGroup.LayoutParams
  abstract member OnClick: Option<unit -> unit>
  abstract member Padding: Padding
  abstract member PivotX: Single
  abstract member PivotY: Single
  abstract member SoundEffectsEnabled: bool
  abstract member TextAlignment: TextAlignment
  abstract member TextDirection: TextDirection
  abstract member TransitionName: string
  abstract member TranslationX: Single
  abstract member TranslationY: Single
  abstract member TranslationZ: Single
  abstract member Visibility: ViewStates

type ViewProps =
  {
    // View Props
    alpha: float32
    backgroundColor: Color
    backgroundTintMode: Option<PorterDuff.Mode>
    clickable: bool
    contentDescription: string
    contextClickable: bool
    layoutParameters: ViewGroup.LayoutParams
    onClick: Option<unit -> unit>
    padding: Padding
    pivotX: Single
    pivotY: Single
    soundEffectsEnabled: bool
    textAlignment: TextAlignment
    textDirection: TextDirection
    transitionName: string
    translationX: Single
    translationY: Single
    translationZ: Single
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
    member this.OnClick = this.onClick
    member this.Padding = this.padding
    member this.PivotX = this.pivotX
    member this.PivotY = this.pivotY
    member this.SoundEffectsEnabled = this.soundEffectsEnabled
    member this.TextAlignment = this.textAlignment
    member this.TextDirection = this.textDirection
    member this.TransitionName = this.transitionName
    member this.TranslationX = this.translationX
    member this.TranslationY = this.translationY
    member this.TranslationZ = this.translationZ
    member this.Visibility = this.visibility

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module View =
  type private OnClickListener (onClick) =
    inherit Java.Lang.Object ()

    interface View.IOnClickListener with
        member this.OnClick view = onClick ()

  let defaultProps = {
    alpha = 1.0f
    backgroundColor = Color.White
    backgroundTintMode = None
    clickable = true
    contentDescription = ""
    contextClickable = true
    layoutParameters = new ViewGroup.LayoutParams(-2, -2)
    onClick = None
    padding = Unchecked.defaultof<Padding>
    pivotX = 0.0f
    pivotY = 0.0f
    soundEffectsEnabled = true
    textAlignment = TextAlignment.Inherit
    textDirection = TextDirection.Inherit
    transitionName = ""
    translationX = 0.0f
    translationY = 0.0f
    translationZ = 0.0f
    visibility = ViewStates.Visible
  }

  let dispose (view: View) =
    view.SetOnKeyListener null
    view.SetOnDragListener null
    view.SetOnTouchListener null
    view.SetOnClickListener null
    view.SetOnHoverListener null
    view.SetOnLongClickListener null
    view.SetOnContextClickListener null
    view.SetOnGenericMotionListener null
    view.SetOnCreateContextMenuListener null
    view.SetOnApplyWindowInsetsListener null
    view.SetOnSystemUiVisibilityChangeListener null
    ()

  let setProps (view: View) (props: IViewProps) =
    view.Alpha <- props.Alpha
    view.SetBackgroundColor props.BackgroundColor
    props.BackgroundTintMode |> Option.map (fun tint -> view.BackgroundTintMode <- tint) |> ignore
    view.Clickable <- props.Clickable
    view.ContentDescription <- props.ContentDescription
    view.ContextClickable <- props.ContextClickable
    view.LayoutParameters <- props.LayoutParameters
    view.SetPaddingRelative(props.Padding.start, props.Padding.top, props.Padding.end_, props.Padding.bottom)
    view.PivotX <- props.PivotX
    view.PivotY <- props.PivotY
    view.SoundEffectsEnabled <- props.SoundEffectsEnabled
    view.TextAlignment <- props.TextAlignment
    view.TextDirection <- props.TextDirection
    view.TransitionName <- props.TransitionName
    view.TranslationX <- props.TranslationX
    view.TranslationY <- props.TranslationY
    view.TranslationZ <- props.TranslationZ
    view.Visibility <- props.Visibility

    match props.OnClick with
    | Some onClick -> view.SetOnClickListener (new OnClickListener (onClick))
    | None -> view.SetOnClickListener null

