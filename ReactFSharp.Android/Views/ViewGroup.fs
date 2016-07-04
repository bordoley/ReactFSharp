namespace React.Android.Views

open Android.Content.Res
open Android.Graphics
open Android.Views

open ImmutableCollections

open React
open System

type IViewGroupProps = 
  inherit IViewProps

module ViewGroup =
  let dispose (view: ViewGroup) =
    React.Android.Views.View.dispose view

  let setProps (view: View) (props: IViewGroupProps)  = 
    View.setProps view props 
   
  let updateChildren 
      (view: ViewGroup)
      (oldChildren: IImmutableMap<string, ReactView>)
      (newChildren: IImmutableMap<string, ReactView>) =
    Seq.zipAll oldChildren newChildren
    |> Seq.iteri (
      fun indx (prev, next) ->
        match (prev, next) with
        | (Some (prevKey, prevChild), Some(nextKey, nextChild)) when prevKey = nextKey -> ()
        | (Some (prevKey, prevChild), Some(nextKey, nextChild)) ->
            view.RemoveViewAt indx

            match AndroidReactView.getView nextChild with
            | Some child -> view.AddView (child , indx)
            | None -> ()

        | (Some (_, prevChild), None) ->
            match AndroidReactView.getView prevChild with
            | Some child -> view.RemoveView child
            | None -> ()

        | (None, Some(_, nextChild)) ->
            match AndroidReactView.getView nextChild with
            | Some child -> view.AddView child
            | None -> ()

        | (None, None) -> ()
      ) 