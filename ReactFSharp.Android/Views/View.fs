namespace React.Android.Views

open Android.Content.Res
open Android.Graphics
open Android.Views

open System

type IViewProps = 
  abstract member Alpha: float32
  abstract member BackgroundColor: Color
  abstract member BackgroundTintMode: Option<PorterDuff.Mode>
  abstract member Clickable: bool
  abstract member ContentDescription: string
  abstract member ContextClickable: bool
  abstract member LayoutParameters: ViewGroup.LayoutParams
  abstract member OnClick: Option<unit -> unit>
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
    pivotX = Unchecked.defaultof<Single>
    pivotY = Unchecked.defaultof<Single>
    soundEffectsEnabled = true
    textAlignment = TextAlignment.Inherit
    textDirection = TextDirection.Inherit
    transitionName = ""
    translationX = Unchecked.defaultof<Single>
    translationY = Unchecked.defaultof<Single>
    translationZ = Unchecked.defaultof<Single>
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

  let updateWithProps (oldProps: Option<IViewProps>) (newProps: IViewProps) (view: View) =

    let layoutParameters = view.LayoutParameters
    view.Alpha <- newProps.Alpha
    view.SetBackgroundColor newProps.BackgroundColor
    newProps.BackgroundTintMode |> Option.map (fun tint -> view.BackgroundTintMode <- tint) |> ignore
    view.Clickable <- newProps.Clickable
    view.ContentDescription <- newProps.ContentDescription
    view.ContextClickable <- newProps.ContextClickable
    view.LayoutParameters <- newProps.LayoutParameters
    view.PivotX <- newProps.PivotX
    view.PivotY <- newProps.PivotY
    view.SoundEffectsEnabled <- newProps.SoundEffectsEnabled
    view.TextAlignment <- newProps.TextAlignment
    view.TextDirection <- newProps.TextDirection
    view.TransitionName <- newProps.TransitionName
    view.TranslationX <- newProps.TranslationX
    view.TranslationY <- newProps.TranslationY
    view.TranslationZ <- newProps.TranslationZ
    view.Visibility <- newProps.Visibility

    match newProps.OnClick with 
    | Some onClick -> view.SetOnClickListener (new OnClickListener (onClick))
    | None -> view.SetOnClickListener null

