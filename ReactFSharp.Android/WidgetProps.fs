namespace React.Android.Widget

open Android.Content.Res
open Android.Graphics
open Android.Views
open Android.Widget
open System

type IViewProps = 
  abstract member Alpha: float
  abstract member BackgroundColor: int
  abstract member BackgroundTintMode: Option<PorterDuff.Mode>
  abstract member Clickable: bool
  abstract member ContentDescription: string
  abstract member ContextClickable: bool
  abstract member LayoutParameters: ViewGroup.LayoutParams
  abstract member OnClick: Option<unit -> unit>
  abstract member SoundEffectsEnabled: bool
  abstract member TextAlignment: TextAlignment
  abstract member TextDirection: TextDirection 
  abstract member Theme: string
  abstract member TransformPivotX: Single
  abstract member TransformPivotY: Single
  abstract member TransitionName: string
  abstract member TranslationX: Single
  abstract member TranslationY: Single 
  abstract member TranslationZ: Single 
  abstract member Visibility: ViewStates
 
type ITextViewProps =
  inherit IViewProps

  abstract member Text: String

//type IViewGroupProps =


//type ILinearLayoutProps =



type ViewGroupLayoutParams = 
  struct
    val width: int
    val height: int
    new(width: int, height: int) = { width = width; height = height }
  end
 
type LinearLayoutLayoutParams = 
  struct
    val width: int
    val height: int
    val weight: float32
    new(width: int, height: int, weight: float32) = { width = width; height = height; weight = weight }
  end

type ButtonProps = {
  layoutParams: ViewGroupLayoutParams
  text: string
  onClick: Option<unit -> unit>
}

type LinearLayoutProps = {
  layoutParams: LinearLayoutLayoutParams
  orientation: Orientation
} 