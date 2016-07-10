namespace React.Android.Widget

open Android.Content
open Android.Content.Res
open Android.Graphics
open Android.Support.V7.Widget
open Android.Views
open Android.Widget
open React
open React.Android
open React.Android.Views
open System

type IEditTextProps =
  inherit ITextViewProps

type EditTextProps =
  {
    // View Props
    accessibilityLiveRegion: int
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
    requestFocus: IObservable<unit>
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

  interface IEditTextProps with
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
    member this.RequestFocus = this.requestFocus
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
module private EditTextProps =
  let internal defaultProps = {
    // View Props
    accessibilityLiveRegion = TextViewProps.Default.accessibilityLiveRegion
    alpha = TextViewProps.Default.alpha
    backgroundColor = TextViewProps.Default.backgroundColor
    backgroundTintMode = TextViewProps.Default.backgroundTintMode
    clickable = TextViewProps.Default.clickable
    contentDescription = TextViewProps.Default.contentDescription
    contextClickable = TextViewProps.Default.contextClickable
    elevation = TextViewProps.Default.elevation
    enabled = TextViewProps.Default.enabled
    filterTouchesWhenObscured = TextViewProps.Default.filterTouchesWhenObscured
    focusable = TextViewProps.Default.focusable
    focusableInTouchMode = TextViewProps.Default.focusableInTouchMode
    hapticFeedbackEnabled = TextViewProps.Default.hapticFeedbackEnabled
    horizontalFadingEdgeEnabled = TextViewProps.Default.horizontalFadingEdgeEnabled
    horizontalScrollBarEnabled = TextViewProps.Default.horizontalScrollBarEnabled
    id = TextViewProps.Default.id
    layoutParameters = TextViewProps.Default.layoutParameters
    onClick = TextViewProps.Default.onClick
    onCreateContextMenu = TextViewProps.Default.onCreateContextMenu
    onDrag = TextViewProps.Default.onDrag
    onGenericMotion = TextViewProps.Default.onGenericMotion
    onHover = TextViewProps.Default.onHover
    onKey = TextViewProps.Default.onKey
    onLongClick = TextViewProps.Default.onLongClick
    onSystemUiVisibilityChange = TextViewProps.Default.onSystemUiVisibilityChange
    onTouch = TextViewProps.Default.onTouch
    padding = TextViewProps.Default.padding
    pivot = TextViewProps.Default.pivot
    requestFocus = TextViewProps.Default.requestFocus
    scrollBarSize = TextViewProps.Default.scrollBarSize
    scrollBarStyle = TextViewProps.Default.scrollBarStyle
    selected = TextViewProps.Default.selected
    soundEffectsEnabled = TextViewProps.Default.soundEffectsEnabled
    systemUiVisibility =  TextViewProps.Default.systemUiVisibility
    textAlignment = TextViewProps.Default.textAlignment
    textDirection = TextViewProps.Default.textDirection
    transitionName = TextViewProps.Default.transitionName
    translation = TextViewProps.Default.translation
    verticalFadingEdgeEnabled = TextViewProps.Default.verticalFadingEdgeEnabled
    verticalScrollBarEnabled = TextViewProps.Default.verticalScrollBarEnabled
    verticalScrollbarPosition = TextViewProps.Default.verticalScrollbarPosition
    visibility = TextViewProps.Default.visibility

    // TextView Props
    text = ""
  }

type EditTextProps with
  static member Default = EditTextProps.defaultProps

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module EditText =
  let private name = typeof<AppCompatEditText>.Name

  let setProps (onError: Exception -> unit) (view: EditText) (props: IEditTextProps) =
    // FIXME: hack
    view.SetTextColor Color.Blue
    TextView.setProps onError view props

  let private createView (context: Context) (onError: Exception -> unit) =
    let viewProvider () =  new AppCompatEditText(context)
    ReactView.createView name viewProvider (setProps onError)

  let viewProvider = (name, createView)

  let internal reactComponent = ReactComponent.makeLazy (fun (props: EditTextProps) -> ReactNativeElement {
    Name = name
    Props = props
  }) 
