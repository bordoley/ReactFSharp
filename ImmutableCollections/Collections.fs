﻿namespace ImmutableCollections

open System
open System.Collections
open System.Collections.Generic

type ICollection<'k, 'v> =
  inherit IEnumerable<'k*'v>

  abstract member Count: int
  abstract member Item: 'k -> 'v
  abstract member Keys: seq<'k>
  abstract member TryGet: 'k -> Option<'v>
  abstract member Values: seq<'v>

type ISet<'v> =
  inherit IEnumerable<'v>

  abstract member Count: int
  abstract member Item: 'v -> bool

type IVector<'v> =
  inherit ICollection<int, 'v>

type IPersistentMap<'k, 'v> =
  inherit ICollection<'k, 'v>

  abstract member Put: 'k * 'v -> IPersistentMap<'k, 'v>
  abstract member Remove: 'k -> IPersistentMap<'k, 'v>

type IPersistentSet<'v> =
  inherit ISet<'v>

  abstract member Add: 'v -> IPersistentSet<'v>
  abstract member Remove: 'v -> IPersistentSet<'v>

type IPersistentVector<'v> = 
  inherit IVector<'v>

  abstract member Add: 'v -> IPersistentVector<'v>
  abstract member Pop: unit -> IPersistentVector<'v>
  abstract member Update: int * 'v -> IPersistentVector<'v>

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

type ImmutableArray<'v> internal (backingArray: array<'v>) =
  member this.CopyTo (sourceIndex: int, destinationArray: array<'v>, destinationIndex: int, length: int) =
    Array.Copy(backingArray, sourceIndex, destinationArray, destinationIndex, length)

  interface IVector<'v> with
    member this.Count = backingArray.Length
    member this.Item index = backingArray.[index]
    member this.GetEnumerator () = 
      backingArray |> Seq.mapi (fun i v -> (i, v)) |> Seq.getEnumerator
    member this.GetEnumerator () = backingArray.GetEnumerator()
    member this.Keys = seq { 0 .. (backingArray.Length - 1) }
    member this.TryGet index =
      if index >= 0 && index < backingArray.Length then
        Some backingArray.[index]
      else None
    member this.Values = backingArray |> Array.toSeq


[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Collection =
  let tryGet (key: 'k) (collection: ICollection<'k, 'v>) =
    collection.TryGet key

  let count (collection: ICollection<'k, 'v>) =
    collection.Count

  let get (key: 'k) (collection: ICollection<'k, 'v>) =
    collection.Item key

  let isEmpty (collection: ICollection<'k, 'v>) =
    collection.Count = 0

  let isNotEmpty (collection: ICollection<'k, 'v>) =
    collection.Count <> 0

  let keys (collection: ICollection<'k, 'v>) =
    collection.Keys

  let values (collection: ICollection<'k, 'v>) =
    collection.Values

  let createWithComparer (comparer: IEqualityComparer<'k>) (entries: seq<'k * 'v>) =
    let backingDictionary = new Dictionary<'k, 'v>(comparer)
    entries |> Seq.iter (fun (k, v) -> backingDictionary.Add(k, v))

    {
      new ICollection<'k, 'v> with
        member this.Count = backingDictionary.Count
        member this.Item key = backingDictionary.Item key
        member this.GetEnumerator () =
          backingDictionary |> Seq.map (fun kvp -> (kvp.Key, kvp.Value)) |> Seq.getEnumerator
        member this.GetEnumerator () = (this.GetEnumerator()) :> IEnumerator
        member this.Keys = (backingDictionary.Keys :> seq<'k>)
        member this.TryGet key =
          match backingDictionary.TryGetValue(key) with
          | (true, v) -> Some v
          | _ -> None
        member this.Values = (backingDictionary.Values :> seq<'v>)
    }

  let create (entries: seq<'k * 'v>) =
    createWithComparer EqualityComparer.Default entries

  let empty = {
    new ICollection<'k, 'v> with
      member this.Count = 0
      member this.Item _ = failwith "key not found"
      member this.GetEnumerator () = (Seq.empty :> IEnumerable<'k*'v>) |> Seq.getEnumerator
      member this.GetEnumerator () = (this.GetEnumerator()) :> IEnumerator
      member this.Keys = Seq.empty
      member this.TryGet _ = None
      member this.Values = Seq.empty
  }

  let map f (collection: ICollection<'k, 'v>) =
    collection |> Seq.map (fun (k, v) -> (k, f k v))

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Vector =
  let toArray (vec: IVector<'v>) =
    Array.init vec.Count (fun i -> vec |> Collection.get i)

  let sub (startIndex: int) (count: int) (vec: IVector<'v>) : IVector<'v> =
    if startIndex < 0 || startIndex >= vec.Count then
      failwith "startIndex out of range"
    elif startIndex + count > vec.Count then
      failwith "count out of range"
    elif startIndex = 0 && count = vec.Count then 
      vec
    else
      {
        new IVector<'v> with
          member this.Count = count
          member this.Item index =
            if index >= 0 && index < count then
              vec.Item (index + startIndex)
            else failwith "index out of range"
          member this.GetEnumerator () =
            vec |> Seq.skip startIndex |> Seq.take count |> Seq.getEnumerator
          member this.GetEnumerator () =
            this.GetEnumerator() :> IEnumerator
          member this.Keys = seq { 0 .. (count - 1) }
          member this.TryGet index =
            if index >= 0 && index < count then
              vec.TryGet (index + startIndex)
            else None
          member this.Values = vec.Values |> Seq.skip startIndex |> Seq.take count
      }

  let reverse (vec: IVector<'v>) : IVector<'v> = {
    new IVector<'v> with
      member this.Count = vec.Count
      member this.Item index =
        if index >= 0 && index < vec.Count then
          vec.Item (vec.Count - index - 1)
        else failwith "index out of range"
      member this.GetEnumerator () =
        vec |> Seq.rev |> Seq.getEnumerator
      member this.GetEnumerator () =
        this.GetEnumerator() :> IEnumerator
      member this.Keys = vec.Keys
      member this.TryGet index =
        if index >= 0 && index < vec.Count then
          vec.TryGet (vec.Count - index - 1)
        else None
      member this.Values = vec.Values |> Seq.rev
    }

  let lastIndex (vec: IVector<'v>) =
    (vec.Count - 1)

  let last (vec: IVector<'v>) =
    vec |> Collection.get (lastIndex vec)

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ImmutableArray =
  let createUnsafe (backingArray: array<'v>) : ImmutableArray<'v> =
    new ImmutableArray<'v>(backingArray)

  let create (items: seq<'v>) : ImmutableArray<'v> =
    items |> Seq.toArray |> createUnsafe

  let empty () = new ImmutableArray<'v>([||])

  let copyTo target (arr: ImmutableArray<'v>) =
    arr.CopyTo (0, target, 0, Math.Min(target.Length, arr |> Collection.count))

  let toArray (arr: ImmutableArray<'v>) =
    let newArray = Array.zeroCreate (arr |> Collection.count)
    arr |> copyTo newArray
    newArray

  let add (v: 'v) (arr: ImmutableArray<'v>) =
    let oldSize = arr |> Collection.count
    let newSize = oldSize + 1;

    let backingArray = Array.zeroCreate newSize
    arr |> copyTo backingArray
    backingArray.[oldSize] <- v

    createUnsafe backingArray

  let cloneAndSet (index: int) (item: 'v) (arr: ImmutableArray<'v>) =
    let size = (arr|> Collection.count)

    let clone = arr |> toArray
    clone.[index] <- item

    createUnsafe clone

  let cloneAndSet2 (index1, item1) (index2, item2) (arr: ImmutableArray<'v>) =
    let size = (arr|> Collection.count)

    let clone = arr |> toArray
    clone.[index1] <- item1
    clone.[index2] <- item2

    createUnsafe clone

  let pop (arr: ImmutableArray<'v>) =
    let count = (arr|> Collection.count)
    if count > 1 then
      let popped = Array.zeroCreate (count - 1)
      arr |> copyTo popped
      popped  |> createUnsafe
    elif count = 1 then
      empty ()
    else failwith "can not pop empty array"

  let sub (startIndex: int) (count: int) (arr: ImmutableArray<'v>) : IVector<'v> =
    if startIndex < 0 || startIndex >= (arr |> Collection.count) then
      failwith "startIndex out of range"
    elif startIndex + count >= (arr |> Collection.count) then
      failwith "count out of range"

    {
      new IVector<'v> with
        member this.Count = count
        member this.Item index =
          if index >= 0 && index < count then
            arr |> Collection.get (index + startIndex)
          else failwith "index out of range"
        member this.GetEnumerator () =
          seq { 0 .. (count - 1) } |> Seq.map (fun i -> (i, this.Item i)) |> Seq.getEnumerator
        member this.GetEnumerator () =
          this.GetEnumerator() :> IEnumerator
        member this.Keys = seq { 0 .. (count - 1) }
        member this.TryGet index =
          if index >= 0 && index < count then
            arr |> Collection.tryGet (index + startIndex)
          else None
        member this.Values =
          seq { 0 .. (count - 1) } |> Seq.map (fun i -> this.Item i)
    }

  let reverse (arr: ImmutableArray<'v>) : IVector<'v> = {
    new IVector<'v> with
      member this.Count = arr |> Collection.count
      member this.Item index =
        if index >= 0 && index < this.Count then
          arr |> Collection.get (this.Count - index - 1)
        else failwith "index out of range"
      member this.GetEnumerator () =
        seq { 0 .. (this.Count - 1) }
        |> Seq.map (fun i -> (i, this.Item (this.Count - i - 1)))
        |> Seq.getEnumerator
      member this.GetEnumerator () =
        this.GetEnumerator() :> IEnumerator
      member this.Keys = arr |> Collection.keys
      member this.TryGet index =
        if index >= 0 && index < this.Count then
          arr |> Collection.tryGet (this.Count - index - 1)
        else None
      member this.Values =
        seq { 0 .. (this.Count - 1) }
        |> Seq.map (fun i -> this.Item (this.Count - i - 1))
    }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Set =
  let createWithComparer (comparer: IEqualityComparer<'v>) (items: seq<'v>): ISet<'v> =
    let backingDictionary = new Dictionary<'v, 'v>(comparer)
    items |> Seq.iter (fun v -> backingDictionary.Add(v, v))

    { new ISet<'v> with
        member this.Count = backingDictionary.Count
        member this.Item v = backingDictionary.ContainsKey v
        member this.GetEnumerator () = backingDictionary.Keys |> Seq.getEnumerator
        member this.GetEnumerator () = (this.GetEnumerator()) :> IEnumerator
    }

  let create (items: seq<'k>) =
    createWithComparer EqualityComparer.Default items