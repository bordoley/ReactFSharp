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

type ITextViewProps =
  inherit IViewProps

  abstract member Text: string

type TextViewProps =
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

    // TextView Props
    text: string
  }

  interface ITextViewProps with
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

    // TextView Props
    member this.Text = this.text

module TextView =
  let name = "Android.Widget.TextView"

  let defaultProps = {
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

    // TextView Props
    text = ""
  }

  let dispose (view: TextView) =
    View.dispose view

  let setProps (view: TextView) (props: ITextViewProps)  =
    View.setProps view props
    view.Text <- props.Text

  let private viewProvider context = new TextView(context)

  let createView: Context -> obj -> ReactView =
    AndroidReactView.createView name viewProvider setProps dispose

  let reactComponent = ReactStatelessComponent (fun (props: TextViewProps) -> ReactNativeElement {
    name = name
    props = props
  })