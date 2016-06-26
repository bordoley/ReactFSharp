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

  let zipAll (b: seq<'b>) (a: seq<'a>) =
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
module ImmutableCollection =
  let count (collection: IImmutableCollection<_>) =
    collection.Count

  let isEmpty (collection: IImmutableCollection<_>) =
    collection.Count = 0

  let isNotEmpty (collection: IImmutableCollection<_>) =
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

    {
      new IImmutableMap<'k, 'v> with
        member this.Count = backingDictionary.Count
        member this.Item key = backingDictionary.Item key
        member this.GetEnumerator () =
          backingDictionary |> Seq.map (fun kvp -> (kvp.Key, kvp.Value)) |> Seq.getEnumerator
        member this.GetEnumerator () = (this.GetEnumerator()) :> IEnumerator
        member this.TryItem key =
          match backingDictionary.TryGetValue(key) with
          | (true, v) -> Some v
          | _ -> None
    }

  let create (entries: seq<'k * 'v>) =
    createWithComparer EqualityComparer.Default entries

  let empty = {
    new IImmutableMap<'k, 'v> with
      member this.Count = 0
      member this.GetEnumerator () = (Seq.empty :> IEnumerable<'k*'v>) |> Seq.getEnumerator
      member this.GetEnumerator () = (this.GetEnumerator()) :> IEnumerator
      member this.Item _ = failwith "key not found"
      member this.TryItem _ = None
  }

  let map f (map: IImmutableMap<'k, 'v>) =
    map |> Seq.map (fun (k, v) -> (k, f k v))

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ImmutableVector =
  let empty = 
    { new IImmutableVector<'v> with
        member this.Count = 0
        member this.GetEnumerator () = (Seq.empty :> IEnumerable<int*'v>) |> Seq.getEnumerator
        member this.GetEnumerator () = (this.GetEnumerator()) :> IEnumerator
        member this.Item index = failwith "index out of range"
        member this.TryItem index = None
    }

  let toArray (vec: IImmutableVector<'v>) =
    Array.init vec.Count (fun i -> vec |> ImmutableMap.get i)

  let keys (arr: IImmutableVector<'v>) = seq { 0 .. (arr.Count - 1) }

  let sub (startIndex: int) (count: int) (arr: IImmutableVector<'v>) : IImmutableVector<'v> =
    if startIndex < 0 || startIndex >= (arr |> ImmutableCollection.count) then
      failwith "startIndex out of range"
    elif startIndex + count >= (arr |> ImmutableCollection.count) then
      failwith "count out of range"

    {
      new IImmutableVector<'v> with
        member this.Count = count
        member this.Item index =
          if index >= 0 && index < count then
            arr |> ImmutableMap.get (index + startIndex)
          else failwith "index out of range"
        member this.GetEnumerator () =
          seq { 0 .. (count - 1) } |> Seq.map (fun i -> (i, this.Item i)) |> Seq.getEnumerator
        member this.GetEnumerator () =
          this.GetEnumerator() :> System.Collections.IEnumerator
        member this.TryItem index =
          if index >= 0 && index < count then
            arr |> ImmutableMap.tryGet (index + startIndex)
          else None
    }

  let reverse (arr: IImmutableVector<'v>) : IImmutableVector<'v> = {
    new IImmutableVector<'v> with
      member this.Count = arr |> ImmutableCollection.count
      member this.Item index =
        if index >= 0 && index < this.Count then
          arr |> ImmutableMap.get (this.Count - index - 1)
        else failwith "index out of range"
      member this.GetEnumerator () =
        seq { 0 .. (this.Count - 1) }
        |> Seq.map (fun i -> (i, this.Item (this.Count - i - 1)))
        |> Seq.getEnumerator
      member this.GetEnumerator () =
        this.GetEnumerator() :> System.Collections.IEnumerator
      member this.TryItem index =
        if index >= 0 && index < this.Count then
          arr |> ImmutableMap.tryGet (this.Count - index - 1)
        else None
    }

  let lastIndex (vec: IImmutableVector<'v>) =
    (vec.Count - 1)

  let last (vec: IImmutableVector<'v>) =
    vec |> ImmutableMap.get (lastIndex vec)

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ImmutableSet =
  let createWithComparer (comparer: IEqualityComparer<'v>) (items: seq<'v>): IImmutableSet<'v> =
    let backingDictionary = new System.Collections.Generic.Dictionary<'v, 'v>(comparer)
    items |> Seq.iter (fun v -> backingDictionary.Add(v, v))

    { new IImmutableSet<'v> with
        member this.Count = backingDictionary.Count
        member this.GetEnumerator () = backingDictionary.Keys |> Seq.getEnumerator
        member this.GetEnumerator () = (this.GetEnumerator()) :> IEnumerator
        member this.Item v = backingDictionary.ContainsKey v
    }

  let create (items: seq<'k>) =
    createWithComparer EqualityComparer.Default items

  let empty = 
    { new IImmutableSet<'v> with 
        member this.Count = 0
        member this.GetEnumerator () = (Seq.empty :> seq<'v>) |> Seq.getEnumerator
        member this.GetEnumerator () = (this.GetEnumerator()) :> IEnumerator
        member this.Item v = false
    }

  let contains v (set: IImmutableSet<'v>) =
    set.Item v

module ImmutableMultiset =
  let get (item: 'v) (multiset: IImmutableMultiset<'v>) =
    multiset.Item item