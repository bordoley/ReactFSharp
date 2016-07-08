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

type TextViewProps ( // View Props
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

                    // TextView Props
                    ?text: string
                   ) =
  inherit ViewProps(?alpha = alpha,
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

  let text = defaultArg text ""

  member this.Text = text

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module TextView =
  let name = "Android.Widget.TextView"

  let dispose (view: TextView) =
    View.dispose view

  let setProps (view: TextView) (props: TextViewProps)  =
    View.setProps view props
    view.Text <- props.Text

  let private viewProvider context = new TextView(context)

  let createView: Context -> obj -> ReactView =
    ReactView.createView name viewProvider setProps dispose

  let internal reactComponent = ReactStatelessComponent (fun (props: TextViewProps) -> ReactNativeElement {
    Name = name
    Props = props
  })