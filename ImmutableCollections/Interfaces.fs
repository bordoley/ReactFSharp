namespace ImmutableCollections

type IMap<'k, 'v> =
  inherit seq<'k * 'v>

  abstract member Count: int
  abstract member Item: 'k -> 'v
  abstract member TryGet: 'k -> Option<'v>

type IPersistentMap<'k, 'v> =
  inherit IMap<'k, 'v>

  abstract member Put: 'k * 'v -> IPersistentMap<'k, 'v>
  abstract member Remove: 'k -> IPersistentMap<'k, 'v>


type IMultiset<'v> = 
  inherit seq<'v*int>

  abstract member Count: int
  abstract member Item: 'v -> int

type IPersistentMultiset<'v> = 
  inherit IMultiset<'v>

  abstract member Put: 'v -> IPersistentMultiset<'v>
  abstract member Remove: 'v * int -> IPersistentMultiset<'v>


type ISet<'v> =
  inherit seq<'v>

  abstract member Count: int
  abstract member Item: 'v -> bool

type IPersistentSet<'v> =
  inherit ISet<'v>

  abstract member Put: 'v -> IPersistentSet<'v>
  abstract member Remove: 'v -> IPersistentSet<'v>


type IVector<'v> =
  inherit IMap<int, 'v> 

type IPersistentVector<'v> = 
  inherit IVector<'v>

  abstract member Add: 'v -> IPersistentVector<'v>
  abstract member Pop: unit -> IPersistentVector<'v>
  abstract member Update: int * 'v -> IPersistentVector<'v>


type ISeqMultimap<'k, 'v> = 
  inherit seq<'k * seq<'v>>

  abstract member Count: int
  abstract member Item: 'k -> seq<'v>


type ISetMultimap<'k, 'v> =
  inherit seq<'k * ISet<'v>>

  abstract member Count: int
  abstract member Item: 'k -> ISet<'v>

type IPersistentSetMultimap<'k, 'v> = 
  inherit ISetMultimap<'k, 'v>

  abstract member Put: 'k * 'v -> IPersistentSetMultimap<'k, 'v>
  abstract member Remove: 'k * seq<'v> -> IPersistentSetMultimap<'k, 'v>


type IVectorMultimap<'k, 'v> =
  inherit seq<'k * IVector<'v>>

  abstract member Count: int
  abstract member Item: 'k -> IVector<'v>

type IPersistentVectorMultimap<'k, 'v> = 
  inherit IVectorMultimap<'k, 'v>

  abstract member Add: 'k * 'v -> IPersistentVectorMultimap<'k, 'v>
  abstract member Pop: 'k * int -> IPersistentVectorMultimap<'k, 'v>