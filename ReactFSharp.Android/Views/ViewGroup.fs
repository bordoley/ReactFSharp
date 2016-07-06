namespace React.Android.Views

open Android.Content.Res
open Android.Graphics
open Android.Views

open ImmutableCollections

open React
open System
open System.Reactive.Disposables

module FSXObservable = FSharp.Control.Reactive.Observable

type IViewGroupProps = 
  inherit IViewProps

module ViewGroup =
  let dispose (view: ViewGroup) =
    React.Android.Views.View.dispose view

  let setProps (view: View) (props: IViewGroupProps)  = 
    View.setProps view props 