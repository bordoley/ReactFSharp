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

    // Toolbar Props
    subTitle: string
    title: string
  }

  static member Default = {
    // View Props
    accessibilityLiveRegion = ViewProps.Default.accessibilityLiveRegion
    alpha = ViewProps.Default.alpha
    backgroundColor = ViewProps.Default.backgroundColor
    backgroundTintMode = ViewProps.Default.backgroundTintMode
    clickable = ViewProps.Default.clickable
    contentDescription = ViewProps.Default.contentDescription
    contextClickable = ViewProps.Default.contextClickable
    elevation = ViewProps.Default.elevation
    enabled = ViewProps.Default.enabled
    filterTouchesWhenObscured = ViewProps.Default.filterTouchesWhenObscured
    focusable = ViewProps.Default.focusable
    focusableInTouchMode = ViewProps.Default.focusableInTouchMode
    hapticFeedbackEnabled = ViewProps.Default.hapticFeedbackEnabled
    horizontalFadingEdgeEnabled = ViewProps.Default.horizontalFadingEdgeEnabled
    horizontalScrollBarEnabled = ViewProps.Default.horizontalScrollBarEnabled
    id = ViewProps.Default.id
    layoutParameters = ViewProps.Default.layoutParameters
    onClick = ViewProps.Default.onClick
    onCreateContextMenu = ViewProps.Default.onCreateContextMenu
    onDrag = ViewProps.Default.onDrag
    onGenericMotion = ViewProps.Default.onGenericMotion
    onHover = ViewProps.Default.onHover
    onKey = ViewProps.Default.onKey
    onLongClick = ViewProps.Default.onLongClick
    onSystemUiVisibilityChange = ViewProps.Default.onSystemUiVisibilityChange
    onTouch = ViewProps.Default.onTouch
    padding = ViewProps.Default.padding
    pivot = ViewProps.Default.pivot
    scrollBarSize = ViewProps.Default.scrollBarSize
    scrollBarStyle = ViewProps.Default.scrollBarStyle
    selected = ViewProps.Default.selected
    soundEffectsEnabled = ViewProps.Default.soundEffectsEnabled
    systemUiVisibility =  ViewProps.Default.systemUiVisibility
    textAlignment = ViewProps.Default.textAlignment
    textDirection = ViewProps.Default.textDirection
    transitionName = ViewProps.Default.transitionName
    translation = ViewProps.Default.translation
    verticalFadingEdgeEnabled = ViewProps.Default.verticalFadingEdgeEnabled
    verticalScrollBarEnabled = ViewProps.Default.verticalScrollBarEnabled
    verticalScrollbarPosition = ViewProps.Default.verticalScrollbarPosition
    visibility = ViewProps.Default.visibility

    // Toolbar Props
    subTitle = ""
    title = ""
  }

  interface IToolbarProps with
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

    // Toolbar Props
    member this.SubTitle = this.subTitle
    member this.Title = this.title

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Toolbar =
  let name = "Android.Widget.Toolbar"

  let setProps (view: Toolbar) (props: IToolbarProps) =
    Console.WriteLine("toolbar set props");
    ViewGroup.setProps view props
    view.Subtitle <- props.SubTitle
    view.Title <- props.Title

  let createView context =
    let viewProvider () = new Toolbar(context)
    ReactView.createView name viewProvider setProps

  let private toolbarComponent =
    let toolbarComponent (props: ToolbarProps) = ReactNativeElement {
      Name = name
      Props = props
    }
    toolbarComponent

  let internal reactComponent = ReactComponent.makeLazy toolbarComponent