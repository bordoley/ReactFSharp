namespace React.Android.Widget

open Android.Content
open Android.Content.Res
open Android.Graphics
open Android.Views
open Android.Widget
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

    // Toolbar Props
    subTitle: string
    title: string
  }

  interface IToolbarProps with
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

    // Toolbar Props
    member this.SubTitle = this.subTitle
    member this.Title = this.title

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Toolbar =
  let name = "Android.Widget.Toolbar"

  let defaultProps = {
    // View Props
    alpha = View.defaultProps.alpha
    backgroundColor = View.defaultProps.backgroundColor
    backgroundTintMode = View.defaultProps.backgroundTintMode
    clickable = View.defaultProps.clickable
    contentDescription = View.defaultProps.contentDescription
    contextClickable = View.defaultProps.contextClickable
    layoutParameters = View.defaultProps.layoutParameters
    onClick = View.defaultProps.onClick
    onCreateContextMenu = View.defaultProps.onCreateContextMenu
    onDrag = View.defaultProps.onDrag
    onGenericMotion = View.defaultProps.onGenericMotion
    onHover = View.defaultProps.onHover
    onKey = View.defaultProps.onKey
    onLongClick = View.defaultProps.onLongClick
    onSystemUiVisibilityChange = View.defaultProps.onSystemUiVisibilityChange
    onTouch = View.defaultProps.onTouch
    padding = View.defaultProps.padding
    pivot = View.defaultProps.pivot
    soundEffectsEnabled = View.defaultProps.soundEffectsEnabled
    systemUiVisibility = View.defaultProps.systemUiVisibility
    textAlignment = View.defaultProps.textAlignment
    textDirection = View.defaultProps.textDirection
    transitionName = View.defaultProps.transitionName
    translation = View.defaultProps.translation
    verticalFadingEdgeEnabled = View.defaultProps.verticalFadingEdgeEnabled
    verticalScrollBarEnabled = View.defaultProps.verticalScrollBarEnabled
    verticalScrollbarPosition = View.defaultProps.verticalScrollbarPosition
    visibility = View.defaultProps.visibility

    // Toolbar Props
    subTitle = ""
    title = ""
  }

  let dispose (view: Toolbar) =
    ViewGroup.dispose view

  let setProps (view: Toolbar) (props: IToolbarProps)   =
    ViewGroup.setProps view props
    view.Subtitle <- props.SubTitle
    view.Title <- props.Title

  let private viewProvider context = new Toolbar(context)

  let createView: Context -> obj -> ReactView =
    ReactView.createView name viewProvider setProps dispose

  let internal reactComponent = ReactStatelessComponent (fun (props: ToolbarProps) -> ReactNativeElement {
    name = name
    props = props
  })