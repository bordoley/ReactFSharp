namespace React.Android.Widget

open Android.Content
open Android.Widget

open React
open React.Android.Views

module Button =
  let name = "Android.Widget.Button"

  let dispose (view: Button) = 
    TextView.dispose view

  let createView (context: Context) (initialProps: obj) = 
    let view = new Button(context)

    let currentProps = ref None

    let updateProps (props: obj) =
      let props = (props :?> ITextViewProps)
      let oldProps = !currentProps
      currentProps := Some props

      view |> TextView.updateWithProps oldProps props

    let dispose () = dispose view

    let reactView: AndroidReactView = {
      dispose = dispose
      name = name
      updateProps = updateProps
      view = view
    } 

    updateProps initialProps

    ReactView reactView

  let reactComponent = ReactStatelessComponent (fun (props: TextViewProps) -> ReactNativeElement {
    name = name
    props = props
  })