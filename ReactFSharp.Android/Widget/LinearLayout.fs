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
    padding = View.defaultProps.padding
    pivotX = View.defaultProps.pivotX
    pivotY = View.defaultProps.pivotY
    soundEffectsEnabled = View.defaultProps.soundEffectsEnabled
    textAlignment = View.defaultProps.textAlignment
    textDirection = View.defaultProps.textDirection
    transitionName = View.defaultProps.transitionName
    translationX = View.defaultProps.translationX
    translationY = View.defaultProps.translationY
    translationZ = View.defaultProps.translationZ
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
    AndroidReactView.createViewGroup name viewProvider setProps ViewGroup.updateChildren dispose

  let reactComponent = ReactStatelessComponent (fun (props: LinearLayoutComponentProps) -> ReactNativeElementGroup {
    name = name
    props = props.props
    children = props.children
  })