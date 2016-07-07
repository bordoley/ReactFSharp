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