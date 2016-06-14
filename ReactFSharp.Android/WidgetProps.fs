namespace React.Android.Widget

open Android.Widget

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