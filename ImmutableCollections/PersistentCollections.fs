namespace ImmutableCollections

open System
open System.Collections
open System.Collections.Generic

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module PersistentSet =
  let rec private createTransient (map: ITransientMap<'v, 'v>) =
    { new ITransientSet<'v> with 
        member this.Persist () =
          map.Persist() |> createInternal
        member this.Put v =
          map.Put (v, v) |> ignore
          this
        member this.Remove v =
          map.Remove v |> ignore
          this
    }

  and private createInternal (map: IPersistentMap<'v, 'v>) =
    ({ new PersistentSetBase<'v> () with
        override this.Count = (map :> IImmutableMap<'v, 'v>).Count 
        override this.GetEnumerator () =
          map |> Seq.map (fun (k, v) -> k) |> Seq.getEnumerator
        override this.Item v =
          match map.TryItem v with
          | Some _ -> true
          | _ -> false
        override this.Mutate () = 
          map |> PersistentMap.mutate |> createTransient
        override this.Put v =
          let newMap = map.Put (v,v)
          if (Object.ReferenceEquals(map, newMap)) then (this :> IPersistentSet<'v>)
          else createInternal map
        override this.Remove v =
          let newMap = map.Remove v
          if (Object.ReferenceEquals(map, newMap)) then (this :> IPersistentSet<'v>)
          else createInternal map   
    }) :> IPersistentSet<'v>

  let emptyWithComparer (comparer: IEqualityComparer<'k>) =
    let backingMap =
      PersistentMap.emptyWithComparer {
        key = comparer
        value = comparer
      }
    createInternal backingMap

  let empty () = emptyWithComparer EqualityComparer.Default

  let put v (set: IPersistentSet<'v>) =
    set.Put v

  let remove v (set: IPersistentSet<'v>) =
    set.Remove v

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module PersistentMultiset =
  let rec private createTransient (map: ITransientMap<'v, int>) =
    { new ITransientMultiset<'v> with 
        member this.Persist () =
          map.Persist() |> createInternal
        member this.SetItemCount (v, itemCount) = 
          if itemCount < 0 then
            failwith "itemCount must be greater than or equal 0"
          if itemCount = 0 then
            map.Remove v |> ignore
          else 
            map.Put (v, itemCount) |> ignore
          this
    }
 
   and private createInternal (map: IPersistentMap<'v, int>) =
    ({ new PersistentMultisetBase<'v>() with
        override this.Count = map.Count
        override this.GetEnumerator () = map |> Seq.getEnumerator
        override this.Item v =
          match map |> PersistentMap.tryGet v with
          | Some v -> v
          | _ -> 0
        override this.Mutate () = 
          map |> PersistentMap.mutate |> createTransient
        override this.SetItemCount (v, itemCount) =
          if itemCount < 0 then
            failwith "itemCount must be greater than or equal 0"
  
          let newMap =
            if itemCount = 0 then
              map |> PersistentMap.remove v
            else map |> PersistentMap.put v itemCount
  
          if Object.ReferenceEquals(map, newMap) then
            this :> IPersistentMultiset<'v>
          else
            createInternal map 
    }) :> IPersistentMultiset<'v>

  let emptyWithComparer (comparer: IEqualityComparer<'v>) : IPersistentMultiset<'v> =
    let backingMap =
      PersistentMap.emptyWithComparer {
        key = comparer
        value = EqualityComparer.Default
      }
    createInternal backingMap

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
  let rec private createInternal 
      (map: IPersistentMap<'k, IPersistentSet<'v>>) 
      (count: int) 
      (valueComparer: IEqualityComparer<'v>) =
    ({ new PersistentSetMultimapBase<'k, 'v>() with
        override this.Count = count
        override this.GetEnumerator () =
          map
          |> Seq.map (fun (k, values) -> values |> Seq.map (fun v -> (k, v)))
          |> Seq.concat
          |> Seq.getEnumerator
        override this.Item k =
          match map |> ImmutableMap.tryGet k with
          | Some v -> (v :> IImmutableSet<'v>)
          | None -> ImmutableSet.empty ()
        override this.Mutate () = failwith "Not Implemented"
        override this.Put (k, v) =
          match map |> ImmutableMap.tryGet k with
          | Some set when set |> ImmutableSet.contains v ->
              (this :> IPersistentSetMultimap<'k, 'v>)
          | Some set ->
              let newSet = set |> PersistentSet.put v
              let newMap = map |> PersistentMap.put k newSet
              createInternal newMap (count + 1) valueComparer
          | None ->
              let newSet = (PersistentSet.emptyWithComparer valueComparer) |> PersistentSet.put v
              let newMap = map |> PersistentMap.put k newSet
              createInternal newMap (count + 1) valueComparer
  
        override this.Remove (k, valuesToRemove) =
          match map |> ImmutableMap.tryGet k with
          | None ->
              (this :> IPersistentSetMultimap<'k, 'v>)
          | Some set ->
              let setCount = set.Count
              let newSet = valuesToRemove |> Seq.fold (fun acc v -> acc |> PersistentSet.remove v) set
              let newCount = count + newSet.Count- setCount
              let newMap = map |> PersistentMap.put k newSet
  
              createInternal newMap newCount valueComparer
    }) :> IPersistentSetMultimap<'k, 'v>

  let emptyWithComparer (comparer: KeyValueComparer<'k, 'v>) =
    let backingMap = PersistentMap.emptyWithComparer {
      key = comparer.key
      value = System.Collections.Generic.EqualityComparer.Default
    }
    createInternal backingMap 0 comparer.value

  let empty () = emptyWithComparer {
    key = System.Collections.Generic.EqualityComparer.Default
    value = System.Collections.Generic.EqualityComparer.Default
  }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module PersistentListMultimap =
  let rec private createInternal (map: IPersistentMap<'k, 'v list>) (count: int) =
    ({ new PersistentListMultimapBase<'k, 'v> () with
        override this.Count = count
        override this.GetEnumerator () =
          map
          |> Seq.map (fun (k, values) -> values |> Seq.map (fun v -> (k, v)))
          |> Seq.concat
          |> Seq.getEnumerator
        override this.Item k =
          match map |> ImmutableMap.tryGet k with
          | Some v -> v
          | None -> List.empty
        override this.Add (k, v) =
          match map |> ImmutableMap.tryGet k with
          | Some list ->
              let newList = v :: list
              let newMap = map |> PersistentMap.put k newList
              createInternal newMap (count + 1)
          | None ->
              let newList = v :: []
              let newMap = map |> PersistentMap.put k newList
              createInternal newMap (count + 1)
        override this.Mutate () = failwith "Not Implemented"
        override this.Pop (k, removeCount) =
          match map |> ImmutableMap.tryGet k with
          | Some list when list.Length > removeCount ->
              let numItemsToRemove = list.Length - removeCount
              let newList =
                seq { 0 .. (numItemsToRemove - 1) }
                |> Seq.fold (fun (head :: tail) i -> tail) list
              let newMap = map |> PersistentMap.put k newList
              createInternal newMap (count - numItemsToRemove)
          | Some list ->
              let newMap = map |> PersistentMap.remove k
              createInternal newMap (count - list.Length)
          | None -> this :> IPersistentListMultimap<'k, 'v>
    }) :> IPersistentListMultimap<'k, 'v>

  let emptyWithComparer (comparer: KeyValueComparer<'k, 'v>) =
    let backingMap = PersistentMap.emptyWithComparer {
      key = comparer.key
      value = System.Collections.Generic.EqualityComparer.Default
    }
    createInternal backingMap 0

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
  let rec private createInternal 
      (map: IPersistentMap<'row, IPersistentMultiset<'column>>)
      (count: int)
      (comparer: CountingTableComparer<'row, 'column>) =
    ({ new PersistentCountingTableBase<'row, 'column> () with
        override this.Count = count
  
        override this.GetEnumerator () =
          map
          |> Seq.map (fun (row, values) -> values |> Seq.map (fun (column, count)-> (row, column, count)))
          |> Seq.concat
          |> Seq.getEnumerator
  
        override this.Item (rowKey, columnKey) =
          match map |> ImmutableMap.tryGet rowKey with
          | None -> 0
          | Some column -> column |> ImmutableMultiset.get columnKey
  
        override this.Mutate () = failwith "Not Implemented"

        override this.SetItemCount (rowKey, columnKey, itemCount) =
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
              createInternal newMap newCount comparer
          | Some row ->
              match row |> PersistentMultiset.get columnKey with
              | 0 when itemCount = 0 ->
                  this :> IPersistentCountingTable<'row, 'column>
              | columnCount ->
                  let newRow = row |> PersistentMultiset.setItemCount columnKey itemCount
                  let newCount = count + newRow.Count - row.Count
                  let newMap =
                    if newRow |> Collection.isEmpty then
                      map |> PersistentMap.remove rowKey
                    else map |> PersistentMap.put rowKey newRow
                  createInternal newMap newCount comparer
    }) :> IPersistentCountingTable<'row, 'column>

  let emptyWithComparer (comparer: CountingTableComparer<'row, 'column>) =
    let backingMap = PersistentMap.emptyWithComparer {
      key = comparer.row
      value = EqualityComparer.Default
    }
    createInternal backingMap 0 comparer

  let empty () = emptyWithComparer {
    row = System.Collections.Generic.EqualityComparer.Default
    column = System.Collections.Generic.EqualityComparer.Default
  }

type [<ReferenceEquality>] TableComparer<'row, 'column, 'value> = {
  row: IEqualityComparer<'row>
  column: IEqualityComparer<'column>
  value: IEqualityComparer<'value>
}

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module PersistentTable =
  let rec createInternal 
      (map: IPersistentMap<'row, IPersistentMap<'column, 'value>>)
      (count: int)
      (columnComparer: KeyValueComparer<'column, 'value>) =
    ({ new PersistentTableBase<'row, 'column, 'value> () with
        override this.Count = count
  
        override this.GetEnumerator () =
          map
          |> Seq.map (fun (row, values) -> values |> Seq.map (fun (column, value)-> (row, column, value)))
          |> Seq.concat
          |> Seq.getEnumerator

        override this.Item (rowKey, columnKey) = 
          let row = map |> ImmutableMap.get rowKey 
          row |> ImmutableMap.get columnKey
  
        override this.Mutate () = failwith "Not Implemented"

        override this.TryItem (rowKey, columnKey) = 
          match map |> ImmutableMap.tryGet rowKey with
          | None -> None
          | Some row -> row |> ImmutableMap.tryGet columnKey
        
        override this.Put (rowKey, columnKey, newValue) =
          match map |> ImmutableMap.tryGet rowKey with
          | None -> 
              let newColumn = 
                PersistentMap.emptyWithComparer columnComparer
                |> PersistentMap.put columnKey newValue
              let newMap = map |> PersistentMap.put rowKey newColumn
              let newCount = count + 1
  
              createInternal newMap newCount columnComparer
  
          | Some column -> 
              let newColumn = column |> PersistentMap.put columnKey newValue
  
              if Object.ReferenceEquals(column, newColumn) then
                this :> IPersistentTable<'row, 'column, 'value>
              else
                let newMap = map |> PersistentMap.put rowKey newColumn
                let newCount = count + 1
                createInternal newMap newCount columnComparer                
  
         override this.Remove (rowKey, columnKey) =
           match map |> ImmutableMap.tryGet rowKey with
           | None -> 
               this :> IPersistentTable<'row, 'column, 'value>
           | Some column -> 
               let newColumn = column |> PersistentMap.remove columnKey
  
               if Object.ReferenceEquals(column, newColumn) then
                 this :> IPersistentTable<'row, 'column, 'value>
  
               else if newColumn |> Collection.isEmpty then
                 let newMap = map |> PersistentMap.remove rowKey
                 let newCount = count - 1
                 createInternal newMap newCount columnComparer
  
               else 
                 let newMap = map |> PersistentMap.put rowKey newColumn
                 let newCount = count - 1
                 createInternal newMap newCount columnComparer
    }) :> IPersistentTable<'row, 'column, 'value>

  let emptyWithComparer (comparer: TableComparer<'row, 'column, 'value>) =
    let rowComparer = {
      key = comparer.row
      value = EqualityComparer.Default
    }

    let columnComparer = {
      key = comparer.column
      value = comparer.value
    }

    let backingMap = PersistentMap.emptyWithComparer {
      key = comparer.row
      value = EqualityComparer.Default
    }
    createInternal backingMap 0 columnComparer
 
  let empty () = emptyWithComparer {
    row = EqualityComparer.Default
    column = EqualityComparer.Default
    value = EqualityComparer.Default
  }
