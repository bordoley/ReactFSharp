namespace ImmutableCollections

open System
open System.Collections
open System.Collections.Generic

type IImmutableCollection<[<EqualityConditionalOn>] 't> =
  inherit ICollection
  inherit IReadOnlyCollection<'t>
  inherit IEquatable<IImmutableCollection<'t>>
  inherit IStructuralEquatable

type IImmutableMap<[<EqualityConditionalOn>] 'k, [<EqualityConditionalOn>] 'v> =
  inherit IImmutableCollection<'k * 'v>

  abstract member Item: 'k -> 'v
  abstract member TryItem: 'k -> Option<'v>

type IPersistentMap<[<EqualityConditionalOn>] 'k, [<EqualityConditionalOn>] 'v> =
  inherit IImmutableMap<'k, 'v>

  abstract member Mutate: unit -> ITransientMap<'k, 'v>
  abstract member Put: 'k * 'v -> IPersistentMap<'k, 'v>
  abstract member Remove: 'k -> IPersistentMap<'k, 'v>

and ITransientMap<'k, 'v> =

  abstract member Persist: unit -> IPersistentMap<'k, 'v>
  abstract member Put: 'k * 'v -> ITransientMap<'k, 'v>
  abstract member Remove: 'k -> ITransientMap<'k, 'v>


type IImmutableMultiset<[<EqualityConditionalOn>] 'v> =
  inherit IImmutableCollection<'v * int>

  abstract member Item: 'v -> int

type IPersistentMultiset<[<EqualityConditionalOn>] 'v> =
  inherit IImmutableMultiset<'v>

  abstract member Mutate: unit -> ITransientMultiset<'v>
  abstract member SetItemCount: 'v * int -> IPersistentMultiset<'v>

and ITransientMultiset<'v> =

  abstract member Persist: unit -> IPersistentMultiset<'v>
  abstract member SetItemCount: 'v * int -> ITransientMultiset<'v>


type IImmutableSet<[<EqualityConditionalOn>] 'v> =
  inherit IImmutableCollection<'v>

  abstract member Item: 'v -> bool

type IPersistentSet<[<EqualityConditionalOn>] 'v> =
  inherit IImmutableSet<'v>

  abstract member Mutate: unit -> ITransientSet<'v>
  abstract member Put: 'v -> IPersistentSet<'v>
  abstract member Remove: 'v -> IPersistentSet<'v>

and ITransientSet<'v> =

  abstract member Persist: unit -> IPersistentSet<'v>
  abstract member Put: 'v -> ITransientSet<'v>
  abstract member Remove: 'v -> ITransientSet<'v>


type IImmutableVector<[<EqualityConditionalOn>] 'v> =
  inherit IImmutableMap<int, 'v>

  abstract member CopyTo: int * array<'v> * int * int -> unit

type IPersistentVector<[<EqualityConditionalOn>] 'v> =
  inherit IImmutableVector<'v>

  abstract member Add: 'v -> IPersistentVector<'v>
  abstract member Mutate: unit -> ITransientVector<'v>
  abstract member Pop: unit -> IPersistentVector<'v>
  abstract member Update: int * 'v -> IPersistentVector<'v>

and ITransientVector<'v> =

  abstract member Add: 'v -> ITransientVector<'v>
  abstract member Persist: unit -> IPersistentVector<'v>
  abstract member Pop: unit -> ITransientVector<'v>
  abstract member Update: int * 'v -> ITransientVector<'v>


type IImmutableMultimap<[<EqualityConditionalOn>] 'k, [<EqualityConditionalOn>] 'v> =
  inherit IImmutableCollection<'k * 'v>

  abstract member Item: 'k -> seq<'v>


type IImmutableSetMultimap<[<EqualityConditionalOn>] 'k, [<EqualityConditionalOn>] 'v> =
  inherit IImmutableMultimap<'k, 'v>

  abstract member Item: 'k -> IImmutableSet<'v>

type IPersistentSetMultimap<[<EqualityConditionalOn>] 'k, [<EqualityConditionalOn>] 'v> =
  inherit IImmutableSetMultimap<'k, 'v>

  abstract member Mutate: unit -> ITransientSetMultimap<'k, 'v>
  abstract member Put: 'k * 'v -> IPersistentSetMultimap<'k, 'v>
  abstract member Remove: 'k * seq<'v> -> IPersistentSetMultimap<'k, 'v>

and ITransientSetMultimap<'k, 'v> =
  abstract member Persist: unit -> IPersistentSetMultimap<'k, 'v>
  abstract member Put: 'k * 'v -> ITransientSetMultimap<'k, 'v>
  abstract member Remove: 'k * seq<'v> -> ITransientSetMultimap<'k, 'v>


type IImmutableListMultimap<[<EqualityConditionalOn>] 'k, [<EqualityConditionalOn>] 'v> =
  inherit IImmutableMultimap<'k, 'v>

  abstract member Item: 'k -> 'v list

type IPersistentListMultimap<[<EqualityConditionalOn>] 'k, [<EqualityConditionalOn>] 'v> =
  inherit IImmutableListMultimap<'k, 'v>

  abstract member Add: 'k * 'v -> IPersistentListMultimap<'k, 'v>
  abstract member Mutate: unit -> ITransientListMultimap<'k, 'v>
  abstract member Pop: 'k * int -> IPersistentListMultimap<'k, 'v>

and ITransientListMultimap<'k, 'v> =
  abstract member Add: 'k * 'v -> ITransientListMultimap<'k, 'v>
  abstract member Persist: unit -> IPersistentListMultimap<'k, 'v>
  abstract member Pop: 'k * int -> IPersistentListMultimap<'k, 'v>


type IImmutableTable<[<EqualityConditionalOn>] 'row, [<EqualityConditionalOn>] 'column, [<EqualityConditionalOn>] 'value> =
  inherit IImmutableCollection<'row * 'column * 'value>

  abstract member Item: 'row * 'column -> 'value
  abstract member TryItem: 'row * 'column -> Option<'value>

type IPersistentTable<[<EqualityConditionalOn>] 'row, [<EqualityConditionalOn>] 'column, [<EqualityConditionalOn>] 'value> =
  inherit IImmutableTable<'row, 'column, 'value>

  abstract member Mutate: unit -> ITransientTable<'row, 'column, 'value>
  abstract member Put: 'row * 'column * 'value -> IPersistentTable<'row, 'column, 'value>
  abstract member Remove: 'row * 'column -> IPersistentTable<'row, 'column, 'value>

and ITransientTable<'row, 'column, 'value> =
  abstract member Persist: unit -> IPersistentTable<'row, 'column, 'value>
  abstract member Put: 'row * 'column * 'value -> ITransientTable<'row, 'column, 'value>
  abstract member Remove: 'row * 'column -> ITransientTable<'row, 'column, 'value>


type IImmutableCountingTable<[<EqualityConditionalOn>] 'row, [<EqualityConditionalOn>] 'column> =
  inherit ICollection
  inherit IReadOnlyCollection<'row * 'column * int>

  abstract member Item: 'row * 'column -> int

type IPersistentCountingTable<[<EqualityConditionalOn>] 'row, [<EqualityConditionalOn>] 'column> =
  inherit IImmutableCountingTable<'row, 'column>

  abstract member Mutate: unit -> ITransientCountingTable<'row, 'column>
  abstract member SetItemCount: 'row * 'column * int -> IPersistentCountingTable<'row, 'column>

and ITransientCountingTable<'row, 'column> =
  abstract member Persist: unit -> IPersistentCountingTable<'row, 'column>
  abstract member SetItemCount: 'row * 'column * int -> ITransientCountingTable<'row, 'column>