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
    onClick: Func<unit, unit>
    onCreateContextMenu: Func<IContextMenu, IContextMenuContextMenuInfo, unit>
    onDrag: Func<DragEvent, bool>
    onGenericMotion: Func<MotionEvent, bool>
    onHover: Func<MotionEvent, bool>
    onKey: Func<Keycode, KeyEvent, bool>
    onLongClick: Func<unit, bool>
    onSystemUiVisibilityChange: Func<StatusBarVisibility, unit>
    onTouch: Func<MotionEvent, bool>
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

    // TextView Props
    text: string
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

    // TextView Props
    text = ""
  }

  interface ITextViewProps with
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

    // TextView Props
    member this.Text = this.text

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module TextView =
  let name = "Android.Widget.TextView"

  let setProps (view: TextView) (props: ITextViewProps)  =
    View.setProps view props
    view.Text <- props.Text

  let createView (context: Context) =
    let viewProvider () = new TextView(context)
    ReactView.createView name viewProvider setProps

  let internal reactComponent = ReactComponent.makeLazy (fun (props: TextViewProps) -> ReactNativeElement {
    Name = name
    Props = props
  }) 