namespace React.Android.Widget

open Android.Content
open Android.Support.V7.Widget
open Android.Widget

open React
open React.Android.Views

open System

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Button =
  let private name = typeof<AppCompatButton>.Name

  let setProps (onError: Exception -> unit) (view: Button) (props: ITextViewProps)   =
    TextView.setProps onError view props

  let private createView context (onError: Exception -> unit) =
    let viewProvider () = new AppCompatButton(context)
    ReactView.createView name viewProvider (setProps onError)

  let viewProvider = (name, createView)

  let internal reactComponent = ReactComponent.makeLazy (fun (props: TextViewProps) -> ReactNativeElement {
    Name = name
    Props = props
  })