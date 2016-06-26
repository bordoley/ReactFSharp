namespace ImmutableCollections

open System 
open System.Collections
open System.Collections.Generic
 
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module PersistentSet =
  type private PersistentSetImpl<'v> private (map: IPersistentMap<'v,'v>) =
    static member Create (map: IPersistentMap<'v,'v>) =
      (new PersistentSetImpl<'v>(map)) :> IPersistentSet<'v>

    interface IPersistentSet<'v> with
      member this.Count = (map :> IImmutableMap<'v, 'v>).Count

      member this.GetEnumerator () = 
        map |> Seq.map (fun (k, v) -> k) |> Seq.getEnumerator
      member this.GetEnumerator () = ((this :> seq<'v>).GetEnumerator()) :> IEnumerator
      member this.Item v = 
        match map.TryItem v with
        | Some _ -> true
        | _ -> false
    
      member this.Put v = 
        let newMap = map.Put (v,v)
        if (Object.ReferenceEquals(map, newMap)) then (this :> IPersistentSet<'v>)
        else PersistentSetImpl.Create(map)

      member this.Remove v = 
        let newMap = map.Remove v
        if (Object.ReferenceEquals(map, newMap)) then (this :> IPersistentSet<'v>)
        else PersistentSetImpl.Create(map)

  let emptyWithComparer (comparer: IEqualityComparer<'k>) =
    let backingMap = 
      PersistentMap.emptyWithComparer {
        key = comparer
        value = comparer
      }
    PersistentSetImpl.Create backingMap

  let empty () = emptyWithComparer EqualityComparer.Default

  let put v (set: IPersistentSet<'v>) =
    set.Put v

  let remove v (set: IPersistentSet<'v>) =
    set.Remove v

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module PersistentMultiset =
  type private PersistentMultisetImpl<'v> private (map: IPersistentMap<'v, int>, count: int) =
    static member Create (map: IPersistentMap<'v,int>, count: int) =
      (new PersistentMultisetImpl<'v>(map, count)) :> IPersistentMultiset<'v>

    interface IPersistentMultiset<'v> with
      member this.Count = count
      member this.GetEnumerator () = map |> Seq.getEnumerator
      member this.GetEnumerator () = ((this :> seq<'v * int>).GetEnumerator()) :> IEnumerator
      member this.Item v = 
        match map |> PersistentMap.tryGet v with
        | Some v -> v
        | _ -> 0
    
      member this.SetItemCount (v, itemCount) = 
        if itemCount < 0 then
          failwith "itemCount must be greater than or equal 0"

        let newMap = 
          if itemCount = 0 then 
            map |> PersistentMap.remove v
          else map |> PersistentMap.put v itemCount

        if Object.ReferenceEquals(map, newMap) then
          this :> IPersistentMultiset<'v>
        else
          let currentItemCount = (this :> IPersistentMultiset<'v>).Item v
          let newCount = count + (itemCount - currentItemCount)
          PersistentMultisetImpl.Create(map, newCount)

  let emptyWithComparer (comparer: IEqualityComparer<'v>) : IPersistentMultiset<'v>=
    let backingMap = 
      PersistentMap.emptyWithComparer {
        key = comparer
        value = System.Collections.Generic.EqualityComparer.Default
      }
    PersistentMultisetImpl.Create (backingMap, 0)

  let empty () = emptyWithComparer System.Collections.Generic.EqualityComparer.Default

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module PersistentSetMultimap =
  type private PersistentSetMultimapImpl<'k, 'v> private (map: IPersistentMap<'k, IPersistentSet<'v>>, 
                                                          count: int,
                                                          valueComparer: IEqualityComparer<'v>) =

    static member Create (map: IPersistentMap<'k, IPersistentSet<'v>>, count: int, valueComparer: IEqualityComparer<'v>) =
      (new PersistentSetMultimapImpl<'k, 'v>(map, count, valueComparer)) :> IPersistentSetMultimap<'k, 'v>

    interface IPersistentSetMultimap<'k, 'v> with
      member this.Count = count 
      member this.GetEnumerator () = 
        map |> Seq.map (
          fun (k, values) -> values |> Seq.map (fun v -> (k, v))
        ) 
        |> Seq.concat
        |> Seq.getEnumerator
      member this.GetEnumerator () = ((this :> seq<'k *'v>).GetEnumerator()) :> IEnumerator
      member this.Item k = 
        match map |> ImmutableMap.tryGet k with
        | Some v -> (v :> IImmutableSet<'v>)
        | None -> ImmutableSet.empty
      member this.Item k = ((this :> IPersistentSetMultimap<'k, 'v>).Item k) :> seq<'v>
      member this.Put (k, v) = 
        match map |> ImmutableMap.tryGet k with
        | Some set when set |> ImmutableSet.contains v ->
            (this :> IPersistentSetMultimap<'k, 'v>)
        | Some set ->
            let newSet = set |> PersistentSet.put v
            let newMap = map |> PersistentMap.put k newSet
            PersistentSetMultimapImpl.Create(newMap, count + 1, valueComparer)
        | None ->
            let newSet = (PersistentSet.emptyWithComparer valueComparer) |> PersistentSet.put v
            let newMap = map |> PersistentMap.put k newSet
            PersistentSetMultimapImpl.Create(newMap, count + 1, valueComparer)

      member this.Remove (k, valuesToRemove) =
        match map |> ImmutableMap.tryGet k with
        | None -> 
            (this :> IPersistentSetMultimap<'k, 'v>)
        | Some set ->
            let setCount = set.Count
            let newSet = valuesToRemove |> Seq.fold (fun acc v -> acc |> PersistentSet.remove v) set
            let newCount = count + newSet.Count- setCount
            let newMap = map |> PersistentMap.put k newSet

            PersistentSetMultimapImpl.Create(newMap, newCount, valueComparer)

  let emptyWithComparer (comparer: KeyValueComparer<'k, 'v>) =
    let backingMap = PersistentMap.emptyWithComparer {
      key = comparer.key
      value = System.Collections.Generic.EqualityComparer.Default
    }
    PersistentSetMultimapImpl.Create (backingMap, 0, comparer.value)

  let empty () = emptyWithComparer {
    key = System.Collections.Generic.EqualityComparer.Default
    value = System.Collections.Generic.EqualityComparer.Default
  }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module PersistentListMultimap =
  type private PersistentListMultimapImpl<'k, 'v> private (map: IPersistentMap<'k, 'v list>, count: int) =

    static member Create (map: IPersistentMap<'k, 'v list>, count: int) =
      (new PersistentListMultimapImpl<'k, 'v>(map, count)) :> IPersistentListMultimap<'k, 'v>

    interface IPersistentListMultimap<'k, 'v> with
      member this.Count = count 
      member this.GetEnumerator () =
        map |> Seq.map (
          fun (k, values) -> values |> Seq.map (fun v -> (k, v))
        ) 
        |> Seq.concat
        |> Seq.getEnumerator
      member this.GetEnumerator () = ((this :> seq<'k * 'v>).GetEnumerator()) :> IEnumerator
      member this.Item k = 
        match map |> ImmutableMap.tryGet k with
        | Some v -> v
        | None -> List.empty
      member this.Item k = ((this :> IPersistentListMultimap<'k, 'v>).Item k) :> seq<'v>
      member this.Add (k, v) = 
        match map |> ImmutableMap.tryGet k with
        | Some list -> 
            let newList = v :: list
            let newMap = map |> PersistentMap.put k newList
            PersistentListMultimapImpl.Create(newMap, count + 1)
        | None -> 
            let newList = v :: []
            let newMap = map |> PersistentMap.put k newList
            PersistentListMultimapImpl.Create(newMap, count + 1)

      member this.Pop (k, removeCount) = 
        match map |> ImmutableMap.tryGet k with
        | Some list when list.Length > removeCount -> 
            let numItemsToRemove = list.Length - removeCount
            let newList =
              seq { 0 .. (numItemsToRemove - 1) }
              |> Seq.fold (fun (head :: tail) i -> tail) list
            let newMap = map |> PersistentMap.put k newList
            PersistentListMultimapImpl.Create(newMap, count - numItemsToRemove) 
        | Some list ->
            let newMap = map |> PersistentMap.remove k
            PersistentListMultimapImpl.Create(newMap, count - list.Length) 
        | None -> this :> IPersistentListMultimap<'k, 'v>

  let emptyWithComparer (comparer: KeyValueComparer<'k, 'v>) =
    let backingMap = PersistentMap.emptyWithComparer {
      key = comparer.key
      value = System.Collections.Generic.EqualityComparer.Default
    }
    PersistentListMultimapImpl.Create (backingMap, 0)

  let empty () = emptyWithComparer {
    key = System.Collections.Generic.EqualityComparer.Default
    value = System.Collections.Generic.EqualityComparer.Default
  }

(*
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module PersistentVectorMultimap =
  type private PersistentVectorMultimapImpl<'k, 'v> private (map: IPersistentMap<'k, IPersistentVector<'v>>, 
                                                             count: int,
                                                             valueComparer: IEqualityComparer<'v>) =

    static member Create (map: IPersistentMap<'k, IPersistentVector<'v>>, count: int, valueComparer: IEqualityComparer<'v>) =
      (new PersistentVectorMultimapImpl<'k, 'v>(map, count, valueComparer)) :> IPersistentVectorMultimap<'k, 'v>

    interface IPersistentVectorMultimap<'k, 'v> with
      member this.Count = count 
      member this.GetEnumerator () =
        map |> Seq.map (
          fun (k, values) -> values |> Seq.map (fun v -> (k, v))
        ) 
        |> Seq.concat
        |> Seq.getEnumerator
      member this.GetEnumerator () = ((this :> seq<'k * 'v>).GetEnumerator()) :> IEnumerator
      member this.Item k = 
        match map |> ImmutableMap.tryGet k with
        | Some v -> (v :> IImmutableVector<'v>)
        | None -> ImmutableVector.empty
      member this.Item k = ((this :> IPersistentVectorMultimap<'k, 'v>).Item k) :> seq<'v>
      member this.Add (k, v) = 
        match map |> ImmutableMap.tryGet k with
        | Some vector -> 
            let newVector = vector |> PersistentVector.add v
            let newMap = map |> PersistentMap.put k newVector
            PersistentVectorMultimapImpl.Create(newMap, count + 1, valueComparer)
        | None -> 
            let newVector = 
              PersistentVector.emptyWithComparer valueComparer
              |> PersistentVector.add v
            let newMap = map |> PersistentMap.put k newVector
            PersistentVectorMultimapImpl.Create(newMap, count + 1, valueComparer)

      member this.Pop (k, removeCount) = 
        match map |> ImmutableMap.tryGet k with
        | Some vector when vector.Count > removeCount -> 
            let numItemsToRemove = vector.Count - removeCount
            let newVector =
              seq { 0 .. (numItemsToRemove - 1) }
              |> Seq.fold
                  (fun acc i -> acc |> PersistentVector.pop)
                  vector
            let newMap = map |> PersistentMap.put k newVector
            PersistentVectorMultimapImpl.Create(newMap, count - numItemsToRemove, valueComparer) 
        | Some vector ->
            let newMap = map |> PersistentMap.remove k
            PersistentVectorMultimapImpl.Create(newMap, count - vector.Count, valueComparer) 
        | None -> this :> IPersistentVectorMultimap<'k, 'v>

  let emptyWithComparer (comparer: KeyValueComparer<'k, 'v>) =
    let backingMap = PersistentMap.emptyWithComparer {
      key = comparer.key
      value = System.Collections.Generic.EqualityComparer.Default
    }
    PersistentVectorMultimapImpl.Create (backingMap, 0, comparer.value)

  let empty () = emptyWithComparer {
    key = System.Collections.Generic.EqualityComparer.Default
    value = System.Collections.Generic.EqualityComparer.Default
  }

*)