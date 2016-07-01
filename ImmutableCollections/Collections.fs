namespace ImmutableCollections

open System
open System.Collections
open System.Collections.Generic

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Array =
  let copyTo target (arr: array<'v>) =
    Array.Copy(arr, 0, target, 0, Math.Min(target.Length, arr.Length))

  let copy (arr: array<'v>) =
    let newArray = Array.zeroCreate arr.Length
    arr |> copyTo newArray
    newArray

  let add (v: 'v) (arr: array<'v>) =
    let oldSize = arr.Length
    let newSize = oldSize + 1;

    let newArray = Array.zeroCreate newSize
    arr |> copyTo newArray
    newArray.[oldSize] <- v

    newArray

  let cloneAndSet (index: int) (item: 'v) (arr: array<'v>) =
    let size = arr.Length

    let clone = arr |> copy
    clone.[index] <- item

    clone

  let lastIndex (arr: array<'v>) = (arr.Length - 1)

  let pop (arr: array<'v>) =
    let count = arr.Length
    if count > 1 then
      let popped = Array.zeroCreate (count - 1)
      arr |> copyTo popped
      popped
    elif count = 1 then
      Array.empty
    else failwith "can not pop empty array"

  let remove index (arr: array<'v>) =
    let newArray = Array.zeroCreate (arr.Length - 1)
    Array.Copy(arr, 0, newArray, 0, index)
    Array.Copy(arr, (index + 1), newArray, index, (arr.Length - index - 1))
    newArray

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Seq =
  let getEnumerator (seq: seq<_>) =
    seq.GetEnumerator()

  let zipAll (a: seq<'a>) (b: seq<'b>) =
    let enumA = a.GetEnumerator()
    let enumB = b.GetEnumerator()

    let rec zipAll (enumA: IEnumerator<'a>) (enumB: IEnumerator<'b>) = seq {
      let a = enumA.MoveNext()
      let b = enumB.MoveNext()

      match (a, b) with
      | (true, true) ->
          yield (Some enumA.Current, Some enumB.Current)
      | (true, false) ->
          yield (Some enumA.Current, None)
      | (false, true) ->
          yield (None, Some enumB.Current)
      | _ -> ()

      if a || b then yield! zipAll enumA enumB
    }

    zipAll enumA enumB

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Collection =
  let count (collection: ICollection) =
    collection.Count

  let isEmpty (collection: ICollection) =
    collection.Count = 0

  let isNotEmpty (collection: ICollection) =
    collection.Count <> 0

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ImmutableMap =
  let tryGet (key: 'k) (map: IImmutableMap<'k, 'v>) =
    map.TryItem key

  let get (key: 'k) (map: IImmutableMap<'k, 'v>) =
    map.Item key

  let keys (map: IImmutableMap<'k, 'v>) =
    map |> Seq.map (fun (key, _) -> key)

  let values (map: IImmutableMap<'k, 'v>) =
    map |> Seq.map (fun (k, v) -> v)

  let createWithComparer (comparer: IEqualityComparer<'k>) (entries: seq<'k * 'v>) =
    let backingDictionary = new Dictionary<'k, 'v>(comparer)
    entries |> Seq.iter (fun (k, v) -> backingDictionary.Add(k, v))

    ({ new ImmutableMapBase<'k, 'v>() with
        override this.Count = backingDictionary.Count
        override this.GetEnumerator () =
          backingDictionary |> Seq.map (fun kvp -> (kvp.Key, kvp.Value)) |> Seq.getEnumerator

        override this.Item key = backingDictionary.Item key
        override this.TryItem key =
            match backingDictionary.TryGetValue(key) with
            | (true, v) -> Some v
            | _ -> None
    }) :> IImmutableMap<'k, 'v>

  let create (entries: seq<'k * 'v>) =
    createWithComparer EqualityComparer.Default entries

  let empty () = 
    ({
      new ImmutableMapBase<'k, 'v>() with
        member this.Count = 0
        member this.GetEnumerator () = (Seq.empty :> IEnumerable<'k*'v>) |> Seq.getEnumerator
        member this.Item _ = failwith "key not found"
        member this.TryItem _ = None
    }) :> IImmutableMap<'k, 'v>

  let map f (map: IImmutableMap<'k, 'v>) =
    map |> Seq.map (fun (k, v) -> (k, f k v))

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ImmutableVector =
  let empty () = 
    ({ new ImmutableVectorBase<'v>() with
        override this.Count = 0
        override this.GetEnumerator () = (Seq.empty :> IEnumerable<int*'v>) |> Seq.getEnumerator
        override this.Item index = failwith "index out of range"
        override this.TryItem index = None
        override this.CopyTo (array, index) = ()
        override this.CopyTo (sourceIndex: int, 
                              destinationArray: array<'v>, 
                              destinationIndex: int, 
                              length: int) = ()
    }) :> IImmutableVector<'v>

  let keys (arr: IImmutableVector<'v>) = seq { 0 .. (arr.Count - 1) }

  let sub (startIndex: int) (count: int) (arr: IImmutableVector<'v>) : IImmutableVector<'v> =
    if startIndex < 0 || startIndex >= (arr |> Collection.count) then
      failwith "startIndex out of range"
    elif startIndex + count >= (arr |> Collection.count) then
      failwith "count out of range"

    ({ new ImmutableVectorBase<'v>() with
        override this.Count = count
        override this.Item index =
          if index >= 0 && index < count then
            arr |> ImmutableMap.get (index + startIndex)
          else failwith "index out of range"
        override this.GetEnumerator () =
          seq { 0 .. (count - 1) } |> Seq.map (fun i -> (i, this.Item i)) |> Seq.getEnumerator
        override this.TryItem index =
          if index >= 0 && index < count then
            arr |> ImmutableMap.tryGet (index + startIndex)
          else None
    }) :> IImmutableVector<'v>

  let reverse (arr: IImmutableVector<'v>) : IImmutableVector<'v> = 
    ({ new ImmutableVectorBase<'v>() with
        override this.Count = arr |> Collection.count
        override this.Item index =
          if index >= 0 && index < this.Count then
            arr |> ImmutableMap.get (this.Count - index - 1)
          else failwith "index out of range"
        override this.GetEnumerator () =
          seq { 0 .. (this.Count - 1) }
          |> Seq.map (fun i -> (i, this.Item (this.Count - i - 1)))
          |> Seq.getEnumerator
        member this.TryItem index =
          if index >= 0 && index < this.Count then
            arr |> ImmutableMap.tryGet (this.Count - index - 1)
          else None
      }) :> IImmutableVector<'v>

  let lastIndex (vec: IImmutableVector<'v>) =
    (vec.Count - 1)

  let last (vec: IImmutableVector<'v>) =
    vec |> ImmutableMap.get (lastIndex vec)

  let createUnsafe (backingArray: array<'v>) : IImmutableVector<'v> = 
    ({ new ImmutableVectorBase<'v>() with
        override this.Count = backingArray.Length
        override this.Item index = backingArray.[index]
        override this.GetEnumerator () = 
          backingArray |> Seq.mapi (fun i v -> (i, v)) |> Seq.getEnumerator
        override this.TryItem index =
          if index >= 0 && index < backingArray.Length then
            Some backingArray.[index]
          else None
        override this.CopyTo (array, index) =
          Array.Copy(backingArray, array, index)
        override this.CopyTo (sourceIndex: int, destinationArray: array<'v>, destinationIndex: int, length: int) =
          Array.Copy(backingArray, sourceIndex, destinationArray, destinationIndex, length)

    }) :> IImmutableVector<'v>

  let create (items: seq<'v>) : IImmutableVector<'v> =
    items |> Seq.toArray |> createUnsafe

  let copyTo target (arr: IImmutableVector<'v>) =
    arr.CopyTo (0, target, 0, Math.Min(target.Length, arr |> Collection.count))

  let toArray (arr: IImmutableVector<'v>) =
    let newArray = Array.zeroCreate (arr |> Collection.count)
    arr |> copyTo newArray
    newArray

  let add (v: 'v) (arr: IImmutableVector<'v>) =
    let oldSize = arr |> Collection.count
    let newSize = oldSize + 1;

    let backingArray = Array.zeroCreate newSize
    arr |> copyTo backingArray
    backingArray.[oldSize] <- v

    createUnsafe backingArray

  let cloneAndSet (index: int) (item: 'v) (arr: IImmutableVector<'v>) =
    let size = (arr|> Collection.count)

    let clone = arr |> toArray
    clone.[index] <- item

    createUnsafe clone

  let pop (arr: IImmutableVector<'v>) =
    let count = (arr|> Collection.count)
    if count > 1 then
      let popped = Array.zeroCreate (count - 1)
      arr |> copyTo popped
      popped  |> createUnsafe
    elif count = 1 then
      empty ()
    else failwith "can not pop empty array"


[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ImmutableSet =
  let createWithComparer (comparer: IEqualityComparer<'v>) (items: seq<'v>): IImmutableSet<'v> =
    let backingDictionary = new System.Collections.Generic.Dictionary<'v, 'v>(comparer)
    items |> Seq.iter (fun v -> backingDictionary.Add(v, v))

    ({ new ImmutableSetBase<'v> () with
        override this.Count = backingDictionary.Count
        override this.GetEnumerator () = backingDictionary.Keys |> Seq.getEnumerator
        override this.Item v = backingDictionary.ContainsKey v
    }) :> IImmutableSet<'v>

  let create (items: seq<'k>) =
    createWithComparer EqualityComparer.Default items

  let empty () = 
    ({ new ImmutableSetBase<'v> () with 
        member this.Count = 0
        member this.GetEnumerator () = (Seq.empty :> seq<'v>) |> Seq.getEnumerator
        member this.Item v = false
    }) :> IImmutableSet<'v>

  let contains v (set: IImmutableSet<'v>) =
    set.Item v

module ImmutableMultiset =
  let get (item: 'v) (multiset: IImmutableMultiset<'v>) =
    multiset.Item item