namespace React.Android.Widget

open Android.Content
open React

open WidgetImpl

module Widgets =
  let Button = ReactStatelessComponent (fun (props: ButtonProps) -> ReactNativeElement {
    name = AndroidWidgetButton
    props = props
  })

  type LinearLayoutComponentProps = {
    props: LinearLayoutProps
    children: ReactElementChildren
  }
  let LinearLayout = ReactStatelessComponent (fun (props: LinearLayoutComponentProps) -> ReactNativeElementGroup {
    name = AndroidWidgetLinearLayout
    props = props.props
    children = props.children
  })

  let widgets: Map<string, Context -> obj -> ReactView> = 
    Map.empty
    |> Map.add WidgetImpl.AndroidWidgetButton WidgetImpl.Button
    |> Map.add WidgetImpl.AndroidWidgetLinearLayout WidgetImpl.LinearLayout