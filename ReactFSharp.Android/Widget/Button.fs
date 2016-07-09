namespace React.Android.Widget

open Android.Content
open Android.Widget

open React
open React.Android.Views

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Button =
  let name = "Android.Widget.Button"

  let dispose (view: Button) =
    TextView.dispose view

  let setProps (view: Button) (props: ITextViewProps)   =
    TextView.setProps view props

  let private viewProvider context = new Button(context)

  let createView: Context -> obj -> ReactView =
    ReactView.createView name viewProvider setProps dispose

  let internal reactComponent = ReactComponent.makeLazy (fun (props: TextViewProps) -> ReactNativeElement {
    Name = name
    Props = props
  })