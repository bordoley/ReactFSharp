namespace React.Android.Views

open Android.Content.Res
open Android.Graphics
open Android.Views

open React
open System

type ViewGroupProps (// View Props
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
                      ?visibility: ViewStates
                   ) =
  inherit ViewProps(?alpha = alpha,
                    ?backgroundColor = backgroundColor,
                    ?backgroundTintMode = backgroundTintMode,
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
                    ?visibility = visibility
                   )

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ViewGroup =
  let dispose (view: ViewGroup) =
    React.Android.Views.View.dispose view

  let setProps (view: View) (props: ViewGroupProps)  = 
    View.setProps view props 