namespace React.Android.Widget

open Android.Content
open Android.Widget

open React
open React.Android.Views

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Button =
  let name = "Android.Widget.Button"

  let setProps (view: Button) (props: ITextViewProps)   =
    TextView.setProps view props

  let createView context =
    let viewProvider () = new Button(context)
    ReactView.createView name viewProvider setProps

  let internal reactComponent = ReactComponent.makeLazy (fun (props: TextViewProps) -> ReactNativeElement {
    Name = name
    Props = props
  })