namespace React.Android.Views

open Android.Views

open React
open System

type IViewGroupProps = 
  inherit IViewProps

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ViewGroup =
  let private setViewAtIndex<'view when 'view :> ViewGroup>
      (emptyViewProvider: unit -> View)
      (index: int) 
      (view: Option<obj>) 
      (viewGroup: ViewGroup)=
    if index < viewGroup.ChildCount then
      viewGroup.RemoveViewAt index
    match view with
    | Some view  -> viewGroup.AddView(view :?> View, index)
    | None _ -> viewGroup.AddView(emptyViewProvider (), index)

  let private removeViewAtIndex<'view when 'view :> ViewGroup>
      (index: int) 
      (viewGroup: 'view) =
    viewGroup.RemoveViewAt index

  let create<'view, 'props when 'view :> ViewGroup>
      (name: string)
      (viewProvider: unit -> 'view)
      (emptyViewProvider: unit -> View)
      (setProps: 'view -> 'props -> unit)
      (initialProps: obj): ReactView =
   ReactView.createViewGroup 
    name 
    viewProvider
    setProps 
    (setViewAtIndex emptyViewProvider)
    removeViewAtIndex
    initialProps

  let setProps (view: View) (props: IViewGroupProps)  = 
    View.setProps view props 

   