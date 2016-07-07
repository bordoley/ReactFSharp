namespace React.Android.Views

open Android.Views

open React
open System

type IViewGroupProps = 
  inherit IViewProps

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ViewGroup =
  let dispose (view: ViewGroup) =
    React.Android.Views.View.dispose view

  let setProps (view: View) (props: IViewGroupProps)  = 
    View.setProps view props 