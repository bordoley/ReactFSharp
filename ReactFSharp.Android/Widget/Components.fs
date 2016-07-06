namespace React.Android.Widget

open Android.Content
open ImmutableCollections
open React

module Components =
  let nativeViews: IPersistentMap<string, Context -> obj -> ReactView> =
    PersistentMap.create
      [|
        (Button.name, Button.createView)
        (LinearLayout.name, LinearLayout.createView)
        (TextView.name, TextView.createView)
        (Toolbar.name, Toolbar.createView)
      |]

  let Button = Button.reactComponent
  let LinearLayout = LinearLayout.reactComponent
  let TextView = TextView.reactComponent
  let Toolbar = Toolbar.reactComponent

