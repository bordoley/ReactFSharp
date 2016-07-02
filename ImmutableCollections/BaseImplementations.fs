﻿namespace ImmutableCollections

open System
open System.Collections
open System.Collections.Generic

[<AbstractClass>]
type private ImmutableCollectionBase<'T> () =
  abstract Count: int
  abstract GetEnumerator: unit -> IEnumerator<'T>
  abstract CopyTo: Array * int -> unit

  default this.CopyTo (array, index) = 
    let mutable i = 0;
    for ele in this do
      (array :> IList).[index + i] <- ele
      i <- i + 1
  
  interface IEnumerable<'T> with
    member this.GetEnumerator () = this.GetEnumerator()

  interface IReadOnlyCollection<'T> with
    member this.Count = this.Count

  interface ICollection with
    member this.Count = this.Count
    member this.CopyTo (array, index) = 
      let mutable i = 0;
      for ele in this do
        (array :> IList).[index + i] <- ele
        i <- i + 1

    member this.IsSynchronized = true
    member this.SyncRoot with get () = new Object()

  interface IEnumerable with
    member this.GetEnumerator () = ((this :> IEnumerable<'T>).GetEnumerator()) :> IEnumerator

[<AbstractClass>]
type private ImmutableMapBase<'k, 'v> () =
  inherit ImmutableCollectionBase<'k * 'v>()

  abstract Item: 'k -> 'v
  abstract TryItem: 'k -> Option<'v>

  interface IImmutableMap<'k, 'v> with
    member this.Item key = this.Item key
    member this.TryItem key = this.TryItem key

[<AbstractClass>]
type private PersistentMapBase<'k, 'v> () =
  inherit ImmutableMapBase<'k, 'v>()

  abstract Put: 'k * 'v -> IPersistentMap<'k, 'v>
  abstract Remove: 'k -> IPersistentMap<'k, 'v>

  interface IPersistentMap<'k, 'v> with
    member this.Put (k, v) = this.Put (k, v)
    member this.Remove key = this.Remove key

[<AbstractClass>]
type private ImmutableVectorBase<'v> () =
  inherit ImmutableCollectionBase<int * 'v>()

  abstract CopyTo: int * array<'v> * int * int -> unit
  abstract Item: int -> 'v
  abstract TryItem: int -> Option<'v>

  default this.CopyTo (sourceIndex: int, 
                       destinationArray: array<'v>, 
                       destinationIndex: int, 
                       length: int) =
    // FIXME: Bounds checking
    // FIXME: Maybe add some heuristics to choose between item looks up
    // vs. enumerating and skipping items before source index.
    // would be more efficient for persistent vector
    for i = 0 to length - 1  do
      destinationArray.[destinationIndex + i] <- this.[sourceIndex + i]

  interface IImmutableVector<'v> with
    member this.Item key = this.Item key
    member this.TryItem key = this.TryItem key
    member this.CopyTo (sourceIndex: int, 
                        destinationArray: array<'v>, 
                        destinationIndex: int, 
                        length: int) = this.CopyTo (sourceIndex, destinationArray, destinationIndex, length)

[<AbstractClass>]
type private PersistentVectorBase<'v> () =
  inherit ImmutableVectorBase<'v>()

  abstract Add: 'v -> IPersistentVector<'v>
  abstract Mutate: unit -> ITransientVector<'v>
  abstract Pop: unit -> IPersistentVector<'v>
  abstract Update: int * 'v -> IPersistentVector<'v>

  interface IPersistentVector<'v> with
    member this.Add v = this.Add v
    member this.Mutate () = this.Mutate ()
    member this.Pop () = this.Pop ()
    member this.Update(index, value) = this.Update(index, value)

[<AbstractClass>]
type private ImmutableSetBase<'v> () =
  inherit ImmutableCollectionBase<'v>()

  abstract Item: 'v -> bool

  interface IImmutableSet<'v> with
    member this.Item v = this.Item v

[<AbstractClass>]
type private PersistentSetBase<'v> () =
  inherit ImmutableSetBase<'v> ()

  abstract member Put: 'v -> IPersistentSet<'v>
  abstract member Remove: 'v -> IPersistentSet<'v>

  interface IPersistentSet<'v>  with
    member this.Put v = this.Put v
    member this.Remove v = this.Remove v

[<AbstractClass>]
type private ImmutableMultisetBase<'v> () = 
  inherit ImmutableCollectionBase<'v * int>()

  abstract member Item: 'v -> int

  interface IImmutableMultiset<'v> with
    member this.Item v = this.Item v

[<AbstractClass>]
type private PersistentMultisetBase<'v> () = 
  inherit ImmutableMultisetBase<'v>()

  abstract member SetItemCount: 'v * int -> IPersistentMultiset<'v>

  interface IPersistentMultiset<'v> with
    member this.SetItemCount (v, count) = this.SetItemCount(v, count)

[<AbstractClass>]
type private ImmutableMultimapBase<'k, 'v> () = 
  inherit ImmutableCollectionBase<'k * 'v> ()

  abstract member Item: 'k -> seq<'v>

  interface IImmutableMultimap<'k, 'v> with
    member this.Item k = this.Item k

[<AbstractClass>]
type private ImmutableSetMultimapBase<'k, 'v> () =
  inherit ImmutableMultimapBase<'k, 'v>()

  abstract member Item: 'k -> IImmutableSet<'v>

  interface IImmutableSetMultimap<'k, 'v> with
    member this.Item k = this.Item k

[<AbstractClass>]
type private PersistentSetMultimapBase<'k, 'v> () = 
  inherit ImmutableSetMultimapBase<'k, 'v>()

  abstract member Put: 'k * 'v -> IPersistentSetMultimap<'k, 'v>
  abstract member Remove: 'k * seq<'v> -> IPersistentSetMultimap<'k, 'v>

  interface IPersistentSetMultimap<'k, 'v> with
    member this.Put (k, v) = this.Put (k, v)
    member this.Remove (k, values) = this.Remove(k, values)

[<AbstractClass>]
type private ImmutableListMultimapBase<'k, 'v> () =
  inherit ImmutableMultimapBase<'k, 'v>()

  abstract member Item: 'k -> 'v list

  interface IImmutableListMultimap<'k, 'v> with
    member this.Item k = this.Item k

[<AbstractClass>]
type private PersistentListMultimapBase<'k, 'v> () = 
  inherit ImmutableListMultimapBase<'k, 'v> ()

  abstract member Add: 'k * 'v -> IPersistentListMultimap<'k, 'v>
  abstract member Pop: 'k * int -> IPersistentListMultimap<'k, 'v>

  interface IPersistentListMultimap<'k, 'v> with
    member this.Add (k, v) = this.Add (k, v)
    member this.Pop (k, count) = this.Pop (k, count)

[<AbstractClass>]
type private ImmutableTableBase<'row, 'column, 'value> () =
  inherit ImmutableCollectionBase<'row * 'column * 'value> ()

  abstract member Item: 'row * 'column -> 'value
  abstract member TryItem: 'row * 'column -> Option<'value>

  interface IImmutableTable<'row, 'column, 'value> with
    member this.Item (r, c) = this.Item (r, c)
    member this.TryItem (r, c) = this.TryItem (r, c)

[<AbstractClass>]
type private PersistentTableBase<'row, 'column, 'value> () =
  inherit ImmutableTableBase<'row, 'column, 'value> ()

  abstract member Put: 'row * 'column * 'value -> IPersistentTable<'row, 'column, 'value>
  abstract member Remove: 'row * 'column -> IPersistentTable<'row, 'column, 'value>

  interface IPersistentTable<'row, 'column, 'value> with
    member this.Put (r, c, v) = this.Put (r, c, v)
    member this.Remove (r, c) = this.Remove (r, c)

[<AbstractClass>] 
type private ImmutableCountingTableBase<'row, 'column> () =
  inherit ImmutableCollectionBase<'row * 'column * int> ()

  abstract member Item: 'row * 'column -> int

  interface IImmutableCountingTable<'row, 'column> with
    member this.Item (r, c) = this.Item (r, c)

[<AbstractClass>]
type private PersistentCountingTableBase<'row, 'column> () =
  inherit ImmutableCountingTableBase<'row, 'column> ()

  abstract member SetItemCount: 'row * 'column * int -> IPersistentCountingTable<'row, 'column>

  interface IPersistentCountingTable<'row, 'column> with
    member this.SetItemCount (r, c, count) = this.SetItemCount (r, c, count) 