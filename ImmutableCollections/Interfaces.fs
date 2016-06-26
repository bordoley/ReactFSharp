namespace ImmutableCollections

type IImmutableMap<'k, 'v> =
  inherit seq<'k * 'v>

  abstract member Count: int
  abstract member Item: 'k -> 'v
  abstract member TryItem: 'k -> Option<'v>

type IPersistentMap<'k, 'v> =
  inherit IImmutableMap<'k, 'v>

  abstract member Put: 'k * 'v -> IPersistentMap<'k, 'v>
  abstract member Remove: 'k -> IPersistentMap<'k, 'v>


type IImmutableMultiset<'v> = 
  inherit seq<'v*int>

  abstract member Count: int
  abstract member Item: 'v -> int

type IPersistentMultiset<'v> = 
  inherit IImmutableMultiset<'v>

  abstract member SetCount: 'v * int -> IPersistentMultiset<'v>


type IImmutableSet<'v> =
  inherit seq<'v>

  abstract member Count: int
  abstract member Item: 'v -> bool

type IPersistentSet<'v> =
  inherit IImmutableSet<'v>

  abstract member Put: 'v -> IPersistentSet<'v>
  abstract member Remove: 'v -> IPersistentSet<'v>


type IImmutableVector<'v> =
  inherit IImmutableMap<int, 'v> 

type IPersistentVector<'v> = 
  inherit IImmutableVector<'v>

  abstract member Add: 'v -> IPersistentVector<'v>
  abstract member Pop: unit -> IPersistentVector<'v>
  abstract member Update: int * 'v -> IPersistentVector<'v>


type ISeqMultimap<'k, 'v> = 
  inherit seq<'k * seq<'v>>

  abstract member Count: int
  abstract member Item: 'k -> seq<'v>


type IImmutableSetMultimap<'k, 'v> =
  inherit seq<'k * IImmutableSet<'v>>

  abstract member Count: int
  abstract member Item: 'k -> IImmutableSet<'v>

type IPersistentSetMultimap<'k, 'v> = 
  inherit IImmutableSetMultimap<'k, 'v>

  abstract member Put: 'k * 'v -> IPersistentSetMultimap<'k, 'v>
  abstract member Remove: 'k * seq<'v> -> IPersistentSetMultimap<'k, 'v>


type IImmutableVectorMultimap<'k, 'v> =
  inherit seq<'k * IImmutableVector<'v>>

  abstract member Count: int
  abstract member Item: 'k -> IImmutableVector<'v>

type IPersistentVectorMultimap<'k, 'v> = 
  inherit IImmutableVectorMultimap<'k, 'v>

  abstract member Add: 'k * 'v -> IPersistentVectorMultimap<'k, 'v>
  abstract member Pop: 'k * int -> IPersistentVectorMultimap<'k, 'v>
