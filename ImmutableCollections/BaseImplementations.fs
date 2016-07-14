namespace ImmutableCollections

open System
open System.Collections
open System.Collections.Generic

[<AbstractClass>]
type private ImmutableCollectionBase<[<EqualityConditionalOn>] 'T> () =
  abstract Count: int
  abstract GetEnumerator: unit -> IEnumerator<'T>
  abstract CopyTo: Array * int -> unit

  default this.CopyTo (array, index) =
    let mutable i = 0;
    for ele in this do
      (array :> IList).[index + i] <- ele
      i <- i + 1

  override this.Equals(that: obj) =
    (this :> IStructuralEquatable).Equals(that, EqualityComparer.Default)

  override this.GetHashCode() =
     (this :> IStructuralEquatable).GetHashCode(EqualityComparer.Default)

  member private this.DoEquals(that: IImmutableCollection<'T>, comparer: IEqualityComparer) =
    if Object.ReferenceEquals(this, that) then true
    elif this.Count <> that.Count then false
    else
      Seq.zip this that
      |> Seq.map comparer.Equals
      |> Seq.tryFind (fun result -> result = false)
      |> Option.isNone

  interface IEnumerable<'T> with
    member this.GetEnumerator () = this.GetEnumerator()

  interface IReadOnlyCollection<'T> with
    member this.Count = this.Count

  interface IImmutableCollection<'T> with
    member this.Count = this.Count
    member this.CopyTo (array, index) =
      let mutable i = 0;
      for ele in this do
        (array :> IList).[index + i] <- ele
        i <- i + 1
    member this.Equals(that: IImmutableCollection<'T>) =
      this.DoEquals(that, EqualityComparer.Default)

    member this.IsSynchronized = true
    member this.SyncRoot with get () = new Object()

  interface IEnumerable with
    member this.GetEnumerator () = ((this :> IEnumerable<'T>).GetEnumerator()) :> IEnumerator

  interface IStructuralEquatable with
    member this.Equals(that, comparer) =
      match that with
      | :? IImmutableCollection<'T> as that ->
          this.DoEquals (that, comparer)
      | _ -> false

    member this.GetHashCode(comparer: IEqualityComparer) =
      let combineHash x y = (x <<< 1) + y + 631
      let mutable res = 0
      for t in this do
        res <- combineHash res (comparer.GetHashCode(t))
      abs res

[<AbstractClass>]
type private ImmutableMapBase<[<EqualityConditionalOn>] 'k, [<EqualityConditionalOn>] 'v> () =
  inherit ImmutableCollectionBase<'k * 'v>()

  abstract Item: 'k -> 'v
  abstract TryItem: 'k -> Option<'v>

  interface IImmutableMap<'k, 'v> with
    member this.Item key = this.Item key
    member this.TryItem key = this.TryItem key

[<AbstractClass>]
type private PersistentMapBase<[<EqualityConditionalOn>] 'k, [<EqualityConditionalOn>] 'v> () =
  inherit ImmutableMapBase<'k, 'v>()

  abstract Mutate: unit -> ITransientMap<'k, 'v>
  abstract Put: 'k * 'v -> IPersistentMap<'k, 'v>
  abstract Remove: 'k -> IPersistentMap<'k, 'v>

  interface IPersistentMap<'k, 'v> with
    member this.Mutate () = this.Mutate ()
    member this.Put (k, v) = this.Put (k, v)
    member this.Remove key = this.Remove key

[<AbstractClass>]
type private ImmutableVectorBase<[<EqualityConditionalOn>] 'v> () =
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
type private PersistentVectorBase<[<EqualityConditionalOn>] 'v> () =
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
type private ImmutableSetBase<[<EqualityConditionalOn>] 'v> () =
  inherit ImmutableCollectionBase<'v>()

  abstract Item: 'v -> bool

  interface IImmutableSet<'v> with
    member this.Item v = this.Item v

[<AbstractClass>]
type private PersistentSetBase<[<EqualityConditionalOn>] 'v> () =
  inherit ImmutableSetBase<'v> ()

  abstract member Mutate: unit -> ITransientSet<'v>
  abstract member Put: 'v -> IPersistentSet<'v>
  abstract member Remove: 'v -> IPersistentSet<'v>

  interface IPersistentSet<'v>  with
    member this.Mutate () = this.Mutate ()
    member this.Put v = this.Put v
    member this.Remove v = this.Remove v

[<AbstractClass>]
type private ImmutableMultisetBase<[<EqualityConditionalOn>] 'v> () =
  inherit ImmutableCollectionBase<'v * int>()

  abstract member Item: 'v -> int

  interface IImmutableMultiset<'v> with
    member this.Item v = this.Item v

[<AbstractClass>]
type private PersistentMultisetBase<[<EqualityConditionalOn>] 'v> () =
  inherit ImmutableMultisetBase<'v>()

  abstract member Mutate: unit -> ITransientMultiset<'v>
  abstract member SetItemCount: 'v * int -> IPersistentMultiset<'v>

  interface IPersistentMultiset<'v> with
    member this.Mutate () = this.Mutate ()
    member this.SetItemCount (v, count) = this.SetItemCount(v, count)

[<AbstractClass>]
type private ImmutableMultimapBase<[<EqualityConditionalOn>] 'k, [<EqualityConditionalOn>] 'v> () =
  inherit ImmutableCollectionBase<'k * 'v> ()

  abstract member Item: 'k -> seq<'v>

  interface IImmutableMultimap<'k, 'v> with
    member this.Item k = this.Item k

[<AbstractClass>]
type private ImmutableSetMultimapBase<[<EqualityConditionalOn>] 'k, [<EqualityConditionalOn>] 'v> () =
  inherit ImmutableMultimapBase<'k, 'v>()

  abstract member Item: 'k -> IImmutableSet<'v>

  default this.Item k  = (this.Item k) :> seq<'v>

  interface IImmutableSetMultimap<'k, 'v> with
    member this.Item k = this.Item k

[<AbstractClass>]
type private PersistentSetMultimapBase<[<EqualityConditionalOn>] 'k, [<EqualityConditionalOn>] 'v> () =
  inherit ImmutableSetMultimapBase<'k, 'v>()

  abstract member Mutate: unit -> ITransientSetMultimap<'k, 'v>
  abstract member Put: 'k * 'v -> IPersistentSetMultimap<'k, 'v>
  abstract member Remove: 'k * seq<'v> -> IPersistentSetMultimap<'k, 'v>

  interface IPersistentSetMultimap<'k, 'v> with
    member this.Mutate () = this.Mutate ()
    member this.Put (k, v) = this.Put (k, v)
    member this.Remove (k, values) = this.Remove(k, values)

[<AbstractClass>]
type private ImmutableListMultimapBase<[<EqualityConditionalOn>] 'k, [<EqualityConditionalOn>] 'v> () =
  inherit ImmutableMultimapBase<'k, 'v>()

  abstract member Item: 'k -> 'v list

  default this.Item k  = (this.Item k) :> seq<'v>

  interface IImmutableListMultimap<'k, 'v> with
    member this.Item k = this.Item k

[<AbstractClass>]
type private PersistentListMultimapBase<[<EqualityConditionalOn>] 'k, [<EqualityConditionalOn>] 'v> () =
  inherit ImmutableListMultimapBase<'k, 'v> ()

  abstract member Add: 'k * 'v -> IPersistentListMultimap<'k, 'v>
  abstract member Mutate: unit -> ITransientListMultimap<'k, 'v>
  abstract member Pop: 'k * int -> IPersistentListMultimap<'k, 'v>

  interface IPersistentListMultimap<'k, 'v> with
    member this.Add (k, v) = this.Add (k, v)
    member this.Mutate () = this.Mutate ()
    member this.Pop (k, count) = this.Pop (k, count)

[<AbstractClass>]
type private ImmutableTableBase<[<EqualityConditionalOn>] 'row, [<EqualityConditionalOn>] 'column, [<EqualityConditionalOn>] 'value> () =
  inherit ImmutableCollectionBase<'row * 'column * 'value> ()

  abstract member Item: 'row * 'column -> 'value
  abstract member TryItem: 'row * 'column -> Option<'value>

  interface IImmutableTable<'row, 'column, 'value> with
    member this.Item (r, c) = this.Item (r, c)
    member this.TryItem (r, c) = this.TryItem (r, c)

[<AbstractClass>]
type private PersistentTableBase<[<EqualityConditionalOn>] 'row, [<EqualityConditionalOn>] 'column, [<EqualityConditionalOn>] 'value> () =
  inherit ImmutableTableBase<'row, 'column, 'value> ()

  abstract member Mutate: unit -> ITransientTable<'row, 'column, 'value>
  abstract member Put: 'row * 'column * 'value -> IPersistentTable<'row, 'column, 'value>
  abstract member Remove: 'row * 'column -> IPersistentTable<'row, 'column, 'value>

  interface IPersistentTable<'row, 'column, 'value> with
    member this.Mutate () = this.Mutate ()
    member this.Put (r, c, v) = this.Put (r, c, v)
    member this.Remove (r, c) = this.Remove (r, c)

[<AbstractClass>]
type private ImmutableCountingTableBase<[<EqualityConditionalOn>] 'row, [<EqualityConditionalOn>] 'column> () =
  inherit ImmutableCollectionBase<'row * 'column * int> ()

  abstract member Item: 'row * 'column -> int

  interface IImmutableCountingTable<'row, 'column> with
    member this.Item (r, c) = this.Item (r, c)

[<AbstractClass>]
type private PersistentCountingTableBase<[<EqualityConditionalOn>] 'row, [<EqualityConditionalOn>] 'column> () =
  inherit ImmutableCountingTableBase<'row, 'column> ()

  abstract member Mutate: unit -> ITransientCountingTable<'row, 'column>
  abstract member SetItemCount: 'row * 'column * int -> IPersistentCountingTable<'row, 'column>

  interface IPersistentCountingTable<'row, 'column> with
    member this.Mutate () = this.Mutate ()
    member this.SetItemCount (r, c, count) = this.SetItemCount (r, c, count)