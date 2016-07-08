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

type ToolbarProps ( // View Props
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

                    // Toolbar Props
                    ?subTitle: string,
                    ?title: string
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
                          ?visibility = visibility
                         )

  let subTitle = defaultArg subTitle ""
  let title = defaultArg title ""

  member this.SubTitle = subTitle
  member this.Title = title

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Toolbar =
  let name = "Android.Widget.Toolbar"

  let dispose (view: Toolbar) =
    ViewGroup.dispose view

  let setProps (view: Toolbar) (props: ToolbarProps)   =
    ViewGroup.setProps view props
    view.Subtitle <- props.SubTitle
    view.Title <- props.Title

  let private viewProvider context = new Toolbar(context)

  let createView: Context -> obj -> ReactView =
    ReactView.createView name viewProvider setProps dispose

  let internal reactComponent = ReactStatelessComponent (fun (props: ToolbarProps) -> ReactNativeElement {
    Name = name
    Props = props
  })