namespace ImmutableCollections

open System.Collections
open System.Collections.Generic

type IImmutableMap<'k, 'v> =
  inherit ICollection
  inherit IReadOnlyCollection<'k * 'v>

  abstract member Item: 'k -> 'v
  abstract member TryItem: 'k -> Option<'v>

type IPersistentMap<'k, 'v> =
  inherit IImmutableMap<'k, 'v>

  abstract member Mutate: unit -> ITransientMap<'k, 'v>
  abstract member Put: 'k * 'v -> IPersistentMap<'k, 'v>
  abstract member Remove: 'k -> IPersistentMap<'k, 'v>

and ITransientMap<'k, 'v> =

  abstract member Persist: unit -> IPersistentMap<'k, 'v>
  abstract member Put: 'k * 'v -> ITransientMap<'k, 'v>
  abstract member Remove: 'k -> ITransientMap<'k, 'v>


type IImmutableMultiset<'v> = 
  inherit ICollection
  inherit IReadOnlyCollection<'v * int>

  abstract member Item: 'v -> int

type IPersistentMultiset<'v> = 
  inherit IImmutableMultiset<'v>

  abstract member Mutate: unit -> ITransientMultiset<'v>
  abstract member SetItemCount: 'v * int -> IPersistentMultiset<'v>

and ITransientMultiset<'v> = 

  abstract member Persist: unit -> IPersistentMultiset<'v>
  abstract member SetItemCount: 'v * int -> ITransientMultiset<'v>

   
type IImmutableSet<'v> =
  inherit ICollection
  inherit IReadOnlyCollection<'v>

  abstract member Item: 'v -> bool

type IPersistentSet<'v> =
  inherit IImmutableSet<'v>

  abstract member Mutate: unit -> ITransientSet<'v>
  abstract member Put: 'v -> IPersistentSet<'v>
  abstract member Remove: 'v -> IPersistentSet<'v>

and ITransientSet<'v> =

  abstract member Persist: unit -> IPersistentSet<'v>
  abstract member Put: 'v -> ITransientSet<'v>
  abstract member Remove: 'v -> ITransientSet<'v>


type IImmutableVector<'v> =
  inherit IImmutableMap<int, 'v> 

  abstract member CopyTo: int * array<'v> * int * int -> unit

type IPersistentVector<'v> = 
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


type IImmutableMultimap<'k, 'v> = 
  inherit ICollection
  inherit IReadOnlyCollection<'k * 'v>

  abstract member Item: 'k -> seq<'v>


type IImmutableSetMultimap<'k, 'v> =
  inherit IImmutableMultimap<'k, 'v>

  abstract member Item: 'k -> IImmutableSet<'v>

type IPersistentSetMultimap<'k, 'v> = 
  inherit IImmutableSetMultimap<'k, 'v>

  abstract member Mutate: unit -> ITransientSetMultimap<'k, 'v>
  abstract member Put: 'k * 'v -> IPersistentSetMultimap<'k, 'v>
  abstract member Remove: 'k * seq<'v> -> IPersistentSetMultimap<'k, 'v>

and ITransientSetMultimap<'k, 'v> = 
  abstract member Persist: unit -> IPersistentSetMultimap<'k, 'v>
  abstract member Put: 'k * 'v -> ITransientSetMultimap<'k, 'v>
  abstract member Remove: 'k * seq<'v> -> ITransientSetMultimap<'k, 'v>


type IImmutableListMultimap<'k, 'v> =
  inherit IImmutableMultimap<'k, 'v>

  abstract member Item: 'k -> 'v list

type IPersistentListMultimap<'k, 'v> = 
  inherit IImmutableListMultimap<'k, 'v>

  abstract member Add: 'k * 'v -> IPersistentListMultimap<'k, 'v>
  abstract member Mutate: unit -> ITransientListMultimap<'k, 'v>
  abstract member Pop: 'k * int -> IPersistentListMultimap<'k, 'v>

and ITransientListMultimap<'k, 'v> = 
  abstract member Add: 'k * 'v -> ITransientListMultimap<'k, 'v>
  abstract member Persist: unit -> IPersistentListMultimap<'k, 'v>
  abstract member Pop: 'k * int -> IPersistentListMultimap<'k, 'v>


type IImmutableTable<'row, 'column, 'value> =
  inherit ICollection
  inherit IReadOnlyCollection<'row * 'column * 'value>

  abstract member Item: 'row * 'column -> 'value
  abstract member TryItem: 'row * 'column -> Option<'value>

type IPersistentTable<'row, 'column, 'value> =
  inherit IImmutableTable<'row, 'column, 'value>

  abstract member Mutate: unit -> ITransientTable<'row, 'column, 'value>
  abstract member Put: 'row * 'column * 'value -> IPersistentTable<'row, 'column, 'value>
  abstract member Remove: 'row * 'column -> IPersistentTable<'row, 'column, 'value>

and ITransientTable<'row, 'column, 'value> =
  abstract member Persist: unit -> IPersistentTable<'row, 'column, 'value>
  abstract member Put: 'row * 'column * 'value -> ITransientTable<'row, 'column, 'value>
  abstract member Remove: 'row * 'column -> ITransientTable<'row, 'column, 'value>


type IImmutableCountingTable<'row, 'column> =
  inherit ICollection
  inherit IReadOnlyCollection<'row * 'column * int>

  abstract member Item: 'row * 'column -> int

type IPersistentCountingTable<'row, 'column> =
  inherit IImmutableCountingTable<'row, 'column>

  abstract member Mutate: unit -> ITransientCountingTable<'row, 'column>
  abstract member SetItemCount: 'row * 'column * int -> IPersistentCountingTable<'row, 'column>

and ITransientCountingTable<'row, 'column> =
  abstract member Persist: unit -> IPersistentCountingTable<'row, 'column>
  abstract member SetItemCount: 'row * 'column * int -> ITransientCountingTable<'row, 'column>