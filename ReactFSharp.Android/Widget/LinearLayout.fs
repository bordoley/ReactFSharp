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

type LinearLayoutProps ( // View Props
                        ?alpha: float32,
                        ?backgroundColor: Color,
                        ?backgroundTintMode: PorterDuff.Mode,
                        ?clickable: bool,
                        ?contentDescription: string,
                        ?contextClickable: bool,
                        ?layoutParameters: ViewGroup.LayoutParams,
                        ?onClick: unit -> unit,
                        ?onCreateContextMenu: IContextMenu -> IContextMenuContextMenuInfo -> unit,
                        ?onDrag: DragEvent -> bool,
                        ?onGenericMotion: MotionEvent -> bool,
                        ?onHover: MotionEvent -> bool,
                        ?onKey: Keycode -> KeyEvent -> bool,
                        ?onLongClick: unit -> bool,
                        ?onSystemUiVisibilityChange: StatusBarVisibility -> unit,
                        ?onTouch: MotionEvent -> bool,
                        ?padding: Padding,
                        ?pivot: Pivot,
                        ?soundEffectsEnabled: bool,
                        ?systemUiVisibility: StatusBarVisibility,
                        ?textAlignment: TextAlignment,
                        ?textDirection: TextDirection,
                        ?transitionName: string,
                        ?translation: Translation,
                        ?verticalFadingEdgeEnabled: bool,
                        ?verticalScrollBarEnabled: bool,
                        ?verticalScrollbarPosition: ScrollbarPosition,
                        ?visibility: ViewStates,

                        // LinearLayout Props
                        ?baselineAligned: bool,
                        ?baselineAlignedChildIndex: int,
                        ?dividerPadding: int,
                        ?measureWithLargestChildEnabled: bool,
                        ?orientation: Orientation,
                        ?showDividers: ShowDividers,
                        ?weightSum: Single
                       ) =
  inherit ViewGroupProps( ?alpha = alpha,
                          ?backgroundColor = backgroundColor,
                          ?backgroundTintMode = backgroundTintMode ,
                          ?clickable = clickable,
                          ?contentDescription = contentDescription,
                          ?contextClickable = contextClickable,
                          ?layoutParameters = layoutParameters,
                          ?onClick = onClick,
                          ?onCreateContextMenu = onCreateContextMenu,
                          ?onDrag = onDrag,
                          ?onGenericMotion = onGenericMotion,
                          ?onHover = onHover,
                          ?onKey = onKey,
                          ?onLongClick = onLongClick,
                          ?onSystemUiVisibilityChange = onSystemUiVisibilityChange,
                          ?onTouch = onTouch,
                          ?padding = padding,
                          ?pivot = pivot,
                          ?soundEffectsEnabled = soundEffectsEnabled,
                          ?systemUiVisibility = systemUiVisibility,
                          ?textAlignment = textAlignment,
                          ?textDirection = textDirection,
                          ?transitionName = transitionName,
                          ?translation = translation,
                          ?verticalFadingEdgeEnabled = verticalFadingEdgeEnabled,
                          ?verticalScrollBarEnabled = verticalScrollBarEnabled,
                          ?verticalScrollbarPosition = verticalScrollbarPosition,
                          ?visibility = visibility)

  let baselineAligned = defaultArg baselineAligned true
  let baselineAlignedChildIndex = defaultArg baselineAlignedChildIndex -1
  let dividerPadding = defaultArg dividerPadding 0
  let measureWithLargestChildEnabled = defaultArg measureWithLargestChildEnabled false
  let orientation = defaultArg orientation Orientation.Horizontal
  let showDividers = defaultArg showDividers ShowDividers.None
  let weightSum = defaultArg weightSum -1.0f

  member this.BaselineAligned = baselineAligned
  member this.BaselineAlignedChildIndex = baselineAlignedChildIndex
  member this.DividerPadding = dividerPadding
  member this.MeasureWithLargestChildEnabled = measureWithLargestChildEnabled
  member this.Orientation = orientation
  member this.ShowDividers = showDividers
  member this.WeightSum = weightSum

type LinearLayoutComponentProps = {
  props: LinearLayoutProps
  children: IImmutableMap<string, ReactElement>
}

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module LinearLayout =
  let name = "Android.Widget.LinearLayout"

  let setProps (view: LinearLayout) (props: LinearLayoutProps) =
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
    Name = name
    Props = props.props
    Children = props.children
  })