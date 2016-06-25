namespace ImmutableCollections

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Seq =
  open System.Collections.Generic

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
module Map =
  open System
  open System.Collections
  open System.Collections.Generic

  let tryGet (key: 'k) (collection: IMap<'k, 'v>) =
    collection.TryGet key

  let count (collection: IMap<'k, 'v>) =
    collection.Count

  let get (key: 'k) (collection: IMap<'k, 'v>) =
    collection.Item key

  let isEmpty (collection: IMap<'k, 'v>) =
    collection.Count = 0

  let isNotEmpty (collection: IMap<'k, 'v>) =
    collection.Count <> 0

  let keys (collection: IMap<'k, 'v>) =
    collection |> Seq.map (fun (key, _) -> key)

  let values (collection: IMap<'k, 'v>) =
    collection |> Seq.map (fun (k, v) -> v)

  let createWithComparer (comparer: IEqualityComparer<'k>) (entries: seq<'k * 'v>) =
    let backingDictionary = new Dictionary<'k, 'v>(comparer)
    entries |> Seq.iter (fun (k, v) -> backingDictionary.Add(k, v))

    {
      new IMap<'k, 'v> with
        member this.Count = backingDictionary.Count
        member this.Item key = backingDictionary.Item key
        member this.GetEnumerator () =
          backingDictionary |> Seq.map (fun kvp -> (kvp.Key, kvp.Value)) |> Seq.getEnumerator
        member this.GetEnumerator () = (this.GetEnumerator()) :> IEnumerator
        member this.TryGet key =
          match backingDictionary.TryGetValue(key) with
          | (true, v) -> Some v
          | _ -> None
    }

  let create (entries: seq<'k * 'v>) =
    createWithComparer EqualityComparer.Default entries

  let empty = {
    new IMap<'k, 'v> with
      member this.Count = 0
      member this.Item _ = failwith "key not found"
      member this.GetEnumerator () = (Seq.empty :> IEnumerable<'k*'v>) |> Seq.getEnumerator
      member this.GetEnumerator () = (this.GetEnumerator()) :> IEnumerator
      member this.TryGet _ = None
  }

  let map f (collection: IMap<'k, 'v>) =
    collection |> Seq.map (fun (k, v) -> (k, f k v))

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Vector =
  open System
  open System.Collections
  open System.Collections.Generic

  let toArray (vec: IVector<'v>) =
    Array.init vec.Count (fun i -> vec |> Map.get i)

  let keys (arr: IVector<'v>) = seq { 0 .. (arr.Count - 1) }

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
          member this.TryGet index =
            if index >= 0 && index < count then
              vec.TryGet (index + startIndex)
            else None
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
      member this.TryGet index =
        if index >= 0 && index < vec.Count then
          vec.TryGet (vec.Count - index - 1)
        else None
    }

  let lastIndex (vec: IVector<'v>) =
    (vec.Count - 1)

  let last (vec: IVector<'v>) =
    vec |> Map.get (lastIndex vec)

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Set =
  let createWithComparer (comparer: System.Collections.Generic.IEqualityComparer<'v>) (items: seq<'v>): ISet<'v> =
    let backingDictionary = new System.Collections.Generic.Dictionary<'v, 'v>(comparer)
    items |> Seq.iter (fun v -> backingDictionary.Add(v, v))

    { new ISet<'v> with
        member this.Count = backingDictionary.Count
        member this.Item v = backingDictionary.ContainsKey v
        member this.GetEnumerator () = backingDictionary.Keys |> Seq.getEnumerator
        member this.GetEnumerator () = (this.GetEnumerator()) :> System.Collections.IEnumerator
    }

  let create (items: seq<'k>) =
    createWithComparer System.Collections.Generic.EqualityComparer.Default items