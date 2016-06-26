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

  let emptyWithComparer (comparer: IEqualityComparer<'v>) : IPersistentMultiset<'v> =
    let backingMap =
      PersistentMap.emptyWithComparer {
        key = comparer
        value = System.Collections.Generic.EqualityComparer.Default
      }
    PersistentMultisetImpl.Create (backingMap, 0)

  let empty () = emptyWithComparer System.Collections.Generic.EqualityComparer.Default

  let get v (multiset: IPersistentMultiset<'v>) =
    multiset.Item v

  let setItemCount v itemCount (multiset: IPersistentMultiset<'v>) =
    multiset.SetItemCount (v, itemCount)

  let add v (multiset: IPersistentMultiset<'v>) =
    let currentCount = multiset |> get v
    multiset |> setItemCount v (currentCount + 1)

  let remove v (multiset: IPersistentMultiset<'v>) =
    let currentCount = multiset |> get v
    if currentCount = 0 then
      multiset
    else
      multiset |> setItemCount v (currentCount - 1)

  let removeAll v (multiset: IPersistentMultiset<'v>) =
    multiset |> setItemCount v 0

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
        map
        |> Seq.map (fun (k, values) -> values |> Seq.map (fun v -> (k, v)))
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
        map
        |> Seq.map (fun (k, values) -> values |> Seq.map (fun v -> (k, v)))
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
            PersistentListMultimapImpl<'k, 'v>.Create(newMap, count + 1)
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

type [<ReferenceEquality>] CountingTableComparer<'row, 'column> = {
  row: IEqualityComparer<'row>
  column: IEqualityComparer<'column>
}

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module PersistentCountingTable =
  type private PersistentCountingTableImpl<'row, 'column> private (map: IPersistentMap<'row, IPersistentMultiset<'column>>,
                                                                   count: int,
                                                                   comparer: CountingTableComparer<'row, 'column>) =

    static member Create (map: IPersistentMap<'row, IPersistentMultiset<'column>>,
                          count: int,
                          comparer: CountingTableComparer<'row, 'column>) =
      (new PersistentCountingTableImpl<'row, 'column>(map, count, comparer)) :> IPersistentCountingTable<'row, 'column>

    interface IPersistentCountingTable<'row, 'column> with
      member this.Count = count

      member this.GetEnumerator () =
        map
        |> Seq.map (fun (row, values) -> values |> Seq.map (fun (column, count)-> (row, column, count)))
        |> Seq.concat
        |> Seq.getEnumerator
      member this.GetEnumerator () = ((this :> seq<'row * 'column * int>).GetEnumerator()) :> IEnumerator

      member this.Item (rowKey, columnKey) =
        match map |> ImmutableMap.tryGet rowKey with
        | None -> 0
        | Some column -> column |> ImmutableMultiset.get columnKey

      member this.SetItemCount (rowKey, columnKey, itemCount) =
        if itemCount < 0 then
          failwith "itemCount must be greater than or equal 0"

        match map |> ImmutableMap.tryGet rowKey with
        | None when itemCount = 0 ->
            this :> IPersistentCountingTable<'row, 'column>
        | None ->
            let newMultiset =
              PersistentMultiset.emptyWithComparer comparer.column
              |> PersistentMultiset.setItemCount columnKey itemCount
            let newMap = map |> PersistentMap.put rowKey newMultiset
            let newCount = count + itemCount
            PersistentCountingTableImpl.Create(newMap, newCount, comparer)
        | Some row ->
            match row |> PersistentMultiset.get columnKey with
            | 0 when itemCount = 0 ->
                this :> IPersistentCountingTable<'row, 'column>
            | columnCount ->
                let newRow = row |> PersistentMultiset.setItemCount columnKey itemCount
                let newCount = count + newRow.Count - row.Count
                let newMap =
                  if newRow |> ImmutableCollection.isEmpty then
                    map |> PersistentMap.remove rowKey
                  else map |> PersistentMap.put rowKey newRow
                PersistentCountingTableImpl.Create(newMap, newCount, comparer)

  let emptyWithComparer (comparer: CountingTableComparer<'row, 'column>) =
    let backingMap = PersistentMap.emptyWithComparer {
      key = comparer.row
      value = EqualityComparer.Default
    }
    PersistentCountingTableImpl.Create (backingMap, 0, comparer)

  let empty () = emptyWithComparer {
    row = System.Collections.Generic.EqualityComparer.Default
    column = System.Collections.Generic.EqualityComparer.Default
  }
