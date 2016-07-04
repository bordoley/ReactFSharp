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

  let updateWithProps (oldProps: Option<IViewGroupProps>) (newProps: IViewGroupProps) (view: ViewGroup) =
    React.Android.Views.View.updateWithProps 
      (oldProps |> Option.map (fun props -> props :> IViewProps))
      newProps view
   
  let updateChildren 
      (oldChildren: IImmutableMap<string, ReactView>)
      (newChildren: IImmutableMap<string, ReactView>)
      (view: ViewGroup) =
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