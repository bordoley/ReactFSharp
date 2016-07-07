namespace React.Android.Widget

open Android.Content
open Android.Content.Res
open Android.Graphics
open Android.Views
open Android.Widget
open ImmutableCollections
open React
open React.Android.Views
open System

type ILinearLayoutProps =
  inherit IViewGroupProps

  abstract member BaselineAligned: bool
  abstract member BaselineAlignedChildIndex: int
  abstract member DividerPadding: int
  abstract member MeasureWithLargestChildEnabled: bool
  abstract member Orientation: Orientation
  abstract member ShowDividers: ShowDividers
  abstract member WeightSum: Single

type LinearLayoutProps = 
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

    // LinearLayout Props
    baselineAligned: bool
    baselineAlignedChildIndex: int
    dividerPadding: int
    measureWithLargestChildEnabled: bool
    orientation: Orientation
    showDividers: ShowDividers
    weightSum: Single
  }

  interface ILinearLayoutProps with
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

    // LinearLayout Props
    member this.BaselineAligned = this.baselineAligned
    member this.BaselineAlignedChildIndex = this.baselineAlignedChildIndex
    member this.DividerPadding = this.dividerPadding
    member this.MeasureWithLargestChildEnabled = this.measureWithLargestChildEnabled
    member this.Orientation = this.orientation
    member this.ShowDividers = this.showDividers
    member this.WeightSum = this.weightSum

type LinearLayoutComponentProps = {
  props: ILinearLayoutProps
  children: IImmutableMap<string, ReactElement>
}

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module LinearLayout =
  let name = "Android.Widget.LinearLayout"

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

    // LinearLayout Props
    baselineAligned = true
    baselineAlignedChildIndex = -1
    dividerPadding = 0
    measureWithLargestChildEnabled = false
    orientation = Orientation.Horizontal
    showDividers = ShowDividers.None
    weightSum = -1.0f
  }

  let setProps (view: LinearLayout) (props: ILinearLayoutProps) =
    ViewGroup.setProps view props 

    view.BaselineAligned <- props.BaselineAligned

    if props.BaselineAlignedChildIndex >= 0 then
      view.BaselineAlignedChildIndex <- props.BaselineAlignedChildIndex

    view.DividerPadding <- props.DividerPadding
    view.MeasureWithLargestChildEnabled <- props.MeasureWithLargestChildEnabled
    view.Orientation <- props.Orientation
    view.ShowDividers <- props.ShowDividers
    view.WeightSum <- props.WeightSum

  let dispose (view: LinearLayout) = 
    ViewGroup.dispose view

  let private viewProvider context = new LinearLayout(context)

  let createView: Context -> obj -> ReactView =
    ReactView.createViewGroup name viewProvider setProps dispose

  let internal reactComponent = ReactStatelessComponent (fun (props: LinearLayoutComponentProps) -> ReactNativeElementGroup {
    name = name
    props = props.props
    children = props.children
  })