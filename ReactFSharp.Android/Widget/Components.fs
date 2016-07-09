namespace React.Android.Widget

open Android.Content
open ImmutableCollections
open React
open React.Android.Widget

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Components =
  let Button = Button.reactComponent
  let LinearLayout = LinearLayout.reactComponent
  let TextView = TextView.reactComponent
  let Toolbar = Toolbar.reactComponent
