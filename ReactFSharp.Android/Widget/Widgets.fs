namespace React.Android.Widget

open Android.Content
open ImmutableCollections
open React 

module Widgets =
  let views: IPersistentMap<string, Context -> obj -> ReactView> = 
    PersistentMap.create 
      [| 
        (TextView.name, TextView.createView)
        (Button.name, Button.createView)
        (LinearLayout.name, LinearLayout.createView)
      |]
