namespace PersistentCollections

type ICollection<'k, 'v> =
  abstract member Count: int
  abstract member Get: 'k -> 'v
  abstract member Keys: seq<'k>
  abstract member TryGet: 'k -> Option<'v>

type IMap<'k, 'v> =
  inherit ICollection<'k, 'v>

type ISet<'k> = IMap<'k, 'k>

type IVector<'v> =
  inherit ICollection<int, 'v>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Seq =
  open System.Collections
  open System.Collections.Generic

  let fromEnumeratorFunc (f: unit -> IEnumerator<'a>) = {
    new IEnumerable<'a> with
      member this.GetEnumerator() = f ()
    interface IEnumerable with
      member this.GetEnumerator() = (f () :> IEnumerator)
  }

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
module Collection =
  let empty = {
    new ICollection<'k, 'v> with
      member this.Count = 0
      member this.Get _ = failwith "key not found"
      member this.Keys = Seq.empty
      member this.TryGet _ = None
  }

  let tryGet (key: 'k) (collection: ICollection<'k, 'v>) = 
    collection.TryGet key

  let get (key: 'k) (collection: ICollection<'k, 'v>) =
    collection.Get key 

  let toSeq (collection: ICollection<'k, 'v>) =
    collection.Keys |> Seq.map (fun k -> (k, collection |> get k))

  let values (collection: ICollection<'k, 'v>) =
    collection.Keys |> Seq.map (fun k -> collection |> get k)

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Vector =
  let createUnsafe (items: array<'v>) : IVector<'v> = {
    new IVector<'v> with
      member this.Count = items.Length
      member this.Get index = items.[index]
      member this.Keys = seq { 0 .. (items.Length - 1) }
      member this.TryGet index =
        if index >= 0 && index < items.Length then
          Some items.[index]
        else None
  }

  let create (items: seq<'v>): IVector<'v> =
    items |> Seq.toArray |> createUnsafe

  let empty = {
    new IVector<'v> with
      member this.Count = 0
      member this.Get _ = failwith "index out of range"
      member this.Keys = Seq.empty
      member this.TryGet index = None
  }

  let add (v: 'v) (vec: IVector<'v>) =
    let arr = 
      Array.init (vec.Count + 1) (
        fun i -> if i < vec.Count then vec |> Collection.get i else v
      )
    arr.[vec.Count] <- v
    createUnsafe arr

  let toArray (vec: IVector<'v>) =
    Array.init vec.Count (fun i -> vec |> Collection.get i)

  let cloneAndSet (index: int) (item: 'v) (vec: IVector<'v>) =
    let clone = vec |> toArray
    clone.[index] <- item
    createUnsafe clone

  let cloneAndSet2 (index1, item1) (index2, item2) (vec: IVector<'v>) =
    let clone = vec |> toArray
    clone.[index1] <- item1
    clone.[index2] <- item2
    createUnsafe clone

  let copy (vec: IVector<'v>) =
    vec |> toArray |> createUnsafe

  let pop (vec: IVector<'v>) =
    if vec.Count > 0 then
      let arr = Array.init (vec.Count - 1) (fun i -> vec |> Collection.get i)
      arr |> createUnsafe
    else failwith "can not pop empty vector"

  let sub (startIndex: int) (count: int) (vec: IVector<'v>) : IVector<'v> =
    if startIndex < 0 || startIndex >= vec.Count then
      failwith "startIndex out of range"
    elif startIndex + count >= vec.Count then
      failwith "count out of range"

    {
      new IVector<'v> with
        member this.Count = count
        member this.Get index = 
          if index >= 0 && index < count then
            vec.Get (index + startIndex)
          else failwith "index out of range"
        member this.Keys = seq { 0 .. (count - 1) }
        member this.TryGet index =
          if index >= 0 && index < count then
            vec.TryGet (index + startIndex)
          else None
    }
  
  let reverse (vec: IVector<'v>) : IVector<'v> = {
    new IVector<'v> with
      member this.Count = vec.Count
      member this.Get index = 
        if index >= 0 && index < vec.Count then
          vec.Get (vec.Count - index - 1)
        else failwith "index out of range" 
      member this.Keys = vec.Keys
      member this.TryGet index =
        if index >= 0 && index < vec.Count then
          vec.TryGet (vec.Count - index - 1)
        else None
  }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Map =
  open System.Collections.Generic

  let create (comparer: IEqualityComparer<'k>) (entries: seq<'k * 'v>) =
    let backingDictionary = new Dictionary<'k, 'v>(comparer)
    entries |> Seq.iter (fun (k, v) -> backingDictionary.Add(k, v))

    {
      new IMap<'k, 'v> with
        member this.Count = backingDictionary.Count
        member this.Get key = backingDictionary.Item key
        member this.Keys = (backingDictionary.Keys :> seq<'k>)
        member this.TryGet key =
          match backingDictionary.TryGetValue(key) with
          | (true, v) -> Some v
          | _ -> None
    }

  let createWithDefaultEquality (entries: seq<'k * 'v>) =
    create EqualityComparer.Default entries

  let empty = {
    new IMap<'k, 'v> with
      member this.Count = 0
      member this.Get _ = failwith "key not found"
      member this.Keys = Seq.empty
      member this.TryGet _ = None
  }

  let map (f: 'v -> 'u) (map: IMap<'k, 'v>) =
    map
    |> Collection.toSeq
    |> Seq.map (fun (k, v) -> (k, f v))

  let mapWithKey (f: 'k -> 'v -> 'u) (map: IMap<'k, 'v>) =
    map
    |> Collection.toSeq
    |> Seq.map (fun (k, v) -> (k, f k v))

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Set =
  let create (comparer: System.Collections.Generic.IEqualityComparer<'k>) (items: seq<'k>): ISet<'k> =
    items |> Seq.map (fun k -> (k, k)) |> Map.create comparer
