namespace ImmutableCollections

open System
open System.Collections
open System.Collections.Generic

type [<ReferenceEquality>] KeyValueComparer<'k, 'v> = {
  key: IEqualityComparer<'k>
  value: IEqualityComparer<'v>
}

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module private PersistentMapImpl =
  let inline countBits value =
    let mutable i = value
    i <- i - ((i >>> 1) &&& 0x55555555u);
    i <- (i &&& 0x33333333u) + ((i >>> 2) &&& 0x33333333u);
    let count = (((i + (i >>> 4)) &&& 0x0F0F0F0Fu) * 0x01010101u) >>> 24;
    (int count)

  let inline mask hash depth = (hash >>> (depth * 5)) &&& 0x3F
  let inline bitpos hash depth = 1u <<< (mask hash depth)
  let inline index (bitmap: uint32) (bit: uint32) =
    countBits (bitmap &&& (uint32 (bit - 1u)))

  let private maxArrayMapSize = 16
  let private width = 32

  type HashCollisionNode<'k, 'v> = {
    hash: int
    entries: array<'k*'v>
    owner: obj
  }

  type BitmapIndexedNodeNode<'k,'v> =
    | Entry of ('k * 'v)
    | HashCollisionNode of HashCollisionNode<'k, 'v>
    | BitmapIndexedNode of BitmapIndexedNode<'k, 'v>

  and BitmapIndexedNode<'k,'v> = {
    bitmap: uint32
    nodes: array<BitmapIndexedNodeNode<'k,'v>>
    owner: obj
  }

  type KeyValueNode<'k, 'v> = {
    hash: int
    entry: 'k*'v
  }

  type ArrayMapRootNode<'k, 'v> = {
    entries: array<KeyValueNode<'k, 'v>>
    owner: obj
  }

  type RootNode<'k, 'v> =
    | BitmapIndexedMapRootNode of BitmapIndexedNode<'k,'v>
    | ArrayMapRootNode of ArrayMapRootNode<'k, 'v>
    | KeyValueRootNode of KeyValueNode<'k, 'v>
    | NoneRootNode

  type [<ReferenceEquality>] HashedTriePersistentMap<'k, 'v> = {
    comparer: KeyValueComparer<'k, 'v>
    count: int
    root: RootNode<'k, 'v>
  }

  let createWithComparer (comparer: KeyValueComparer<'k, 'v>) = {
    comparer = comparer
    count = 0
    root = NoneRootNode
  }

  let private emptyBitmapIndexedNode owner = {
    bitmap = (uint32 0)
    nodes = Array.empty
    owner = owner
  }

  let private putInHashCollisionNodeWithMutator
      (owner: obj)
      (mutator: int -> 'k * 'v -> HashCollisionNode<'k, 'v> -> HashCollisionNode<'k, 'v>)
      (comparer: KeyValueComparer<'k, 'v>)
      (newEntry: 'k * 'v)
      (hashCollisionNode: HashCollisionNode<'k, 'v>) =

    let keyComparer = comparer.key
    let valueComparer = comparer.value

    let (newKey, newValue) = newEntry

    let element =
      hashCollisionNode.entries
      |> Seq.mapi (fun i v -> (i, v))
      |> Seq.tryFind (fun (_, (key, value)) ->  keyComparer.Equals(key, newKey))

    match element with
    | Some (index, (key, value)) when valueComparer.Equals(value, newValue) ->
        (hashCollisionNode, 0)

    | Some (index, (key, value)) -> 
        let newHashCollisionNode = hashCollisionNode |> mutator index newEntry 
        (newHashCollisionNode, 0)

    | _ ->
      let newHashCollisionNode =
        { hashCollisionNode with
            owner = owner
            entries = hashCollisionNode.entries |> Array.add newEntry
        }
      (newHashCollisionNode, 0)

  let rec private putInBitmapIndexedNodeWithMutators
      (owner: obj)
      (hashCollisionNodeMutator: int -> 'k * 'v -> HashCollisionNode<'k, 'v> -> HashCollisionNode<'k, 'v>)
      (bitmapIndexedNodeMutator: int -> BitmapIndexedNodeNode<'k, 'v> -> BitmapIndexedNode<'k, 'v> -> BitmapIndexedNode<'k, 'v>)
      (comparer: KeyValueComparer<'k, 'v>)
      (depth: int)
      (newHash: int)
      (newEntry: 'k * 'v)
      (bitmapIndexedNode: BitmapIndexedNode<'k, 'v>) : (BitmapIndexedNode<'k, 'v> * int) =

    let putInBitmapIndexedNode = 
      putInBitmapIndexedNodeWithMutators owner hashCollisionNodeMutator bitmapIndexedNodeMutator comparer

    let keyComparer = comparer.key
    let valueComparer = comparer.value
    let (newKey, newValue) = newEntry

    let bit = bitpos newHash depth
    let index = index bitmapIndexedNode.bitmap bit |> int

    if index < bitmapIndexedNode.nodes.Length then
      let childNode = bitmapIndexedNode.nodes.[index]

      let (newChildNode, increment) =
        match childNode with
        | Entry (key, value) when keyComparer.Equals(key, newKey) && valueComparer.Equals(value, newValue) ->
            (childNode, 0)

        | Entry (key, _) when keyComparer.Equals(key, newKey) ->
            (Entry newEntry, 0)

        | Entry ((key, value) as entry) ->
            let hash = keyComparer.GetHashCode (key)

            if hash = newHash then
              let hashCollisionNode =
                HashCollisionNode {
                  hash = hash
                  entries = [| entry; newEntry|]
                  owner = owner
                }

              (hashCollisionNode , 1)
            else
              let (newNode, _) =
                emptyBitmapIndexedNode owner
                |> putInBitmapIndexedNode (depth + 1) hash entry


              let (newNode, _) =
                newNode
                |> putInBitmapIndexedNode (depth + 1) newHash newEntry

              (BitmapIndexedNode newNode, 1)

        | HashCollisionNode hashCollisionNode ->
            let (newHashCollisionNode, increment) = 
              hashCollisionNode 
              |> putInHashCollisionNodeWithMutator owner hashCollisionNodeMutator comparer newEntry

            if Object.ReferenceEquals(hashCollisionNode, newHashCollisionNode) then
              (childNode, 0)
            else
              (HashCollisionNode newHashCollisionNode, 1)

        | BitmapIndexedNode bitmapIndexedNode ->
            let (newBitmapIndexedNode, increment) = bitmapIndexedNode |> putInBitmapIndexedNode (depth + 1) newHash newEntry
            if Object.ReferenceEquals(bitmapIndexedNode, newBitmapIndexedNode) then
              (childNode, 0)
            else
              (BitmapIndexedNode newBitmapIndexedNode, increment)

      if Object.ReferenceEquals(childNode, newChildNode) then
        (bitmapIndexedNode, 0)
      else
        let newBitmapIndexedNode = bitmapIndexedNode |> bitmapIndexedNodeMutator index newChildNode

        (newBitmapIndexedNode, increment)
    else
      let count = bitmapIndexedNode.nodes.Length
      let newArray = Array.zeroCreate (count + 1)
      System.Array.Copy(bitmapIndexedNode.nodes, 0, newArray, 0, count)
      System.Array.Copy(bitmapIndexedNode.nodes, 0, newArray, 0, index)
      System.Array.Copy(bitmapIndexedNode.nodes,index, newArray, index + 1, count - index)
      newArray.[index] <- Entry newEntry

      let newBitmapIndexedNode = {
        bitmap = bitmapIndexedNode.bitmap ||| (uint32 bit)
        nodes = newArray
        owner = owner
      }
      (newBitmapIndexedNode, 1)

  let private putInArrayMapWithMutator
      (owner: obj)
      putInBitmapIndexedNode
      arrayMapNodeMutator
      (comparer: KeyValueComparer<'k, 'v>)
      (hash: int)
      (newEntry: 'k * 'v)
      (arrayMap: ArrayMapRootNode<'k, 'v>) =

    let putInBitmapIndexedNode = putInBitmapIndexedNode comparer

    let keyComparer = comparer.key
    let valueComparer = comparer.value
    let (newKey, newValue) = newEntry

    let element =
      arrayMap.entries
      |> Seq.mapi (fun i v -> (i, v))
      |> Seq.tryFind (
        fun (_, { hash = hash; entry = (key, _)}) ->
          hash = hash && keyComparer.Equals(key, newKey)
      )

    let count = arrayMap.entries.Length

    match element with
    | Some (index, { hash = hash; entry = (key, value)})
          when valueComparer.Equals (value, newValue) ->
        (ArrayMapRootNode arrayMap, 0)

    | Some (index, _) ->
        let newArrayMapRootNode = arrayMap |> arrayMapNodeMutator index { hash = hash; entry = newEntry }
        (ArrayMapRootNode newArrayMapRootNode, 0)

    | _ when count < maxArrayMapSize ->
        let newArray =
          arrayMap.entries |> Array.add { hash = hash; entry = newEntry }

        (ArrayMapRootNode {
          entries = newArray
          owner = owner
        }, 1)

    | _ ->
        let reducer bitmapIndexedNode (keyValueNode: KeyValueNode<'k, 'v>) =
          let (newBitmapIndexedNode, _) = bitmapIndexedNode |> putInBitmapIndexedNode 0 keyValueNode.hash keyValueNode.entry
          newBitmapIndexedNode

        let (newBitmapIndexedNode, _) =
          arrayMap.entries
          |> Seq.fold reducer (emptyBitmapIndexedNode owner)
          |> putInBitmapIndexedNode 0 hash newEntry

        (BitmapIndexedMapRootNode newBitmapIndexedNode, 1)

  let putWithMutators 
      owner
      hashCollisionNodeMutator 
      bitmapIndexedNodeMutator 
      arrayMapNodeMutator
      (newEntry: 'k * 'v) 
      (map: HashedTriePersistentMap<'k, 'v>) =

    let putInBitmapIndexedNode 
        (comparer: KeyValueComparer<'k, 'v>)
        (depth: int)
        (newHash: int)
        (newEntry: 'k * 'v)
        (bitmapIndexedNode: BitmapIndexedNode<'k, 'v>) =
      putInBitmapIndexedNodeWithMutators
        owner
        hashCollisionNodeMutator 
        bitmapIndexedNodeMutator 
        comparer
        depth
        newHash
        newEntry
        bitmapIndexedNode
  
    let putInArrayMap
        (comparer: KeyValueComparer<'k, 'v>)
        (hash: int)
        (newEntry: 'k * 'v)
        (arrayMap: ArrayMapRootNode<'k, 'v>) =
      putInArrayMapWithMutator 
        owner
        putInBitmapIndexedNode 
        arrayMapNodeMutator
        comparer
        hash
        newEntry
        arrayMap
    
    let keyComparer = map.comparer.key
    let valueComparer = map.comparer.value

    let (newKey, newValue) = newEntry
    let newHash = map.comparer.key.GetHashCode(newKey)

    match map.root with
    | NoneRootNode ->
        { map with count = 1; root = KeyValueRootNode { hash = newHash; entry = newEntry } }

    | KeyValueRootNode { hash = hash; entry = (key, value) }
          when newHash = hash && keyComparer.Equals(key, newKey) && valueComparer.Equals(value, newValue) ->
        map

    | KeyValueRootNode { hash = hash; entry = (key, _) }
          when hash = newHash && keyComparer.Equals(key, newKey) ->
        { map with root = KeyValueRootNode { hash = hash; entry = newEntry } }

    | KeyValueRootNode keyValueNode ->
        let newRootNodes = [| keyValueNode; { hash = newHash; entry = newEntry } |]

        { map with 
            count = 2
            root = ArrayMapRootNode {
              entries = newRootNodes
              owner = owner
            } 
        }

    | ArrayMapRootNode arrayMap ->
        let (newRootNode, increment) = arrayMap |> putInArrayMap map.comparer newHash newEntry

        match (map.root, newRootNode) with
        | (ArrayMapRootNode arrayMap, ArrayMapRootNode newArrayMap)
              when Object.ReferenceEquals(arrayMap, newArrayMap) ->
            map

        | _ ->
          { map with count = map.count + increment; root = newRootNode }

    | BitmapIndexedMapRootNode bitmapIndexedNode ->
        let (newBitmapIndexedNode, increment) = bitmapIndexedNode |> putInBitmapIndexedNode map.comparer 0 newHash newEntry
        if Object.ReferenceEquals(bitmapIndexedNode, newBitmapIndexedNode) then
          map
        else
          { map with
              count = map.count + increment
              root = BitmapIndexedMapRootNode newBitmapIndexedNode
          }

  let private immutableHashCollisionNodeMutator 
      index 
      (entry: 'k * 'v)
      (hashCollisionNode: HashCollisionNode<'k, 'v>) =
    { hashCollisionNode with
        entries = hashCollisionNode.entries |> Array.cloneAndSet index entry
    }

  let private immutableBitmapIndexedNodeMutator 
      index 
      (node: BitmapIndexedNodeNode<'k, 'v>) 
      (bitmapIndexedNode: BitmapIndexedNode<'k, 'v>) =
    {
      bitmap = bitmapIndexedNode.bitmap
      nodes = bitmapIndexedNode.nodes |> Array.cloneAndSet index node
      owner = Unchecked.defaultof<obj>
    }

  let private immutableArrayMapNodeMutator
      index 
      (node: KeyValueNode<'k, 'v>)
      (arrayMap: ArrayMapRootNode<'k, 'v>) = 
    {
      entries = arrayMap.entries |> Array.cloneAndSet index node
      owner = Unchecked.defaultof<obj>
    }
     
  let put (entry: 'k * 'v) (map: HashedTriePersistentMap<'k, 'v>) = 
    putWithMutators 
      Unchecked.defaultof<obj>
      immutableHashCollisionNodeMutator 
      immutableBitmapIndexedNodeMutator 
      immutableArrayMapNodeMutator
      entry
      map

  let rec private tryFindInBitmapIndexedNode
      (comparer: KeyValueComparer<'k, 'v>)
      (depth: int)
      (keyHash: int)
      (key: 'k)
      (bitmapIndexedNode: BitmapIndexedNode<'k, 'v>)  =

    let bit = bitpos keyHash depth
    let index = index bitmapIndexedNode.bitmap bit |> int

    match bitmapIndexedNode.nodes |> Array.tryItem index with
    | Some (Entry entry) ->
        Some entry

    | Some (HashCollisionNode { hash = hash; entries = entries }) when hash = keyHash ->
        entries |>  Array.tryFind (fun (k, _) -> comparer.key.Equals(key, k))

    | Some (BitmapIndexedNode bitmapIndexedNode) ->
        bitmapIndexedNode|> tryFindInBitmapIndexedNode comparer (depth + 1) keyHash key
    | _ -> None

  let tryGet (key: 'k) (map: HashedTriePersistentMap<'k, 'v>) =
    let keyHash = map.comparer.key.GetHashCode(key)

    match map.root with
    | BitmapIndexedMapRootNode root ->
        let entry = root |> tryFindInBitmapIndexedNode map.comparer 0 keyHash key
        match entry with
        | Some (key, value) -> Some value
        | _ -> None
    | ArrayMapRootNode root ->
        let node =
          root.entries
          |> Array.tryFind (
            fun { hash = hash; entry = (entryKey, _) } ->
              hash = keyHash && map.comparer.key.Equals(key, entryKey)
          )
        match node with
        | Some { entry = (_, value) } -> Some value
        | _ -> None

    | KeyValueRootNode { hash = hash; entry = (entryKey, value) }
        when hash = keyHash && map.comparer.key.Equals(key, entryKey) -> Some value
    | _ -> None

  let rec private bitmapIndexedNodeToSeq (bitmapIndexedNode: BitmapIndexedNode<'k, 'v>) = seq {
    for node in bitmapIndexedNode.nodes do
      match node with
      | Entry entry -> yield entry
      | HashCollisionNode hashCollisionNode ->
          yield! hashCollisionNode.entries
      | BitmapIndexedNode bitmapIndexedNode ->
          yield! bitmapIndexedNode |> bitmapIndexedNodeToSeq
  }

  let toSeq (map: HashedTriePersistentMap<'k, 'v>) : seq<'k * 'v> =
    match map.root with
    | BitmapIndexedMapRootNode root -> root |> bitmapIndexedNodeToSeq
    | ArrayMapRootNode root -> root.entries |> Seq.map (fun { entry = entry } -> entry)
    | KeyValueRootNode { entry = entry } -> seq { yield entry }
    | _ -> Seq.empty

  let rec private removeFromBitmapIndexedNodeWithMutators
      owner
      (bitmapIndexedNodeMutator: int -> BitmapIndexedNodeNode<'k, 'v> -> BitmapIndexedNode<'k, 'v> -> BitmapIndexedNode<'k, 'v>)
      (comparer: KeyValueComparer<'k, 'v>)
      (depth: int)
      (keyHash: int)
      (key: 'k)
      (bitmapIndexedNode: BitmapIndexedNode<'k, 'v>) : BitmapIndexedNode<'k, 'v> =

    let removeFromBitmapIndexedNode = 
      removeFromBitmapIndexedNodeWithMutators owner bitmapIndexedNodeMutator comparer

    let bit = bitpos keyHash depth
    let index = index bitmapIndexedNode.bitmap bit |> int

    match bitmapIndexedNode.nodes |> Array.tryItem index with
    | Some (Entry (entryKey, _)) when comparer.key.Equals(key, entryKey) ->
        { 
          bitmap = bitmapIndexedNode.bitmap ^^^ bit   
          nodes = bitmapIndexedNode.nodes |> Array.remove index
          owner = owner
        }

    | Some ((HashCollisionNode { hash = hash; entries = entries }) as hashCollisionNode) when hash = keyHash ->
        let entryIndex = entries |> Array.tryFindIndex (fun (k, _) -> comparer.key.Equals(key, k))
        let newNodeAtIndex =
          match entryIndex with
          | Some entryIndex when entries.Length > 2 ->
              let newEntries = entries |> Array.remove entryIndex
              HashCollisionNode { 
                hash = hash
                entries = newEntries 
                owner = owner
              }
          | Some entryIndex ->
              Entry (entries.[1 - entryIndex])
          | _ -> hashCollisionNode
        
        if Object.ReferenceEquals(newNodeAtIndex, hashCollisionNode) then 
          bitmapIndexedNode
        else    
          bitmapIndexedNode |> bitmapIndexedNodeMutator index newNodeAtIndex

    | Some (BitmapIndexedNode bitmapIndexedNodeAtIndex) ->
        let newBitmapIndexedNodeAtIndex = 
          bitmapIndexedNodeAtIndex |> removeFromBitmapIndexedNode (depth + 1) keyHash key

        if Object.ReferenceEquals(bitmapIndexedNodeAtIndex, newBitmapIndexedNodeAtIndex) then
          bitmapIndexedNode
        else if newBitmapIndexedNodeAtIndex.nodes.Length = 0 then
          {
            bitmap = bitmapIndexedNode.bitmap ^^^ bit   
            nodes = bitmapIndexedNode.nodes |> Array.remove index
            owner = owner
          }
        else if newBitmapIndexedNodeAtIndex.nodes.Length = 1 then
          bitmapIndexedNode |> bitmapIndexedNodeMutator index newBitmapIndexedNodeAtIndex.nodes.[0]
        else
          bitmapIndexedNode |> bitmapIndexedNodeMutator index (BitmapIndexedNode newBitmapIndexedNodeAtIndex)
    | _ -> 
        bitmapIndexedNode

  let removeWithMutators
      owner 
      bitmapIndexedNodeMutator 
      arrayMapNodeMutator
      (key: 'k) 
      (map: HashedTriePersistentMap<'k, 'v>) : HashedTriePersistentMap<'k, 'v> =

    let removeFromBitmapIndexedNode = 
      removeFromBitmapIndexedNodeWithMutators owner bitmapIndexedNodeMutator map.comparer

    let keyHash = map.comparer.key.GetHashCode(key)

    match map.root with
    | BitmapIndexedMapRootNode root -> 
        let newRoot = root |> removeFromBitmapIndexedNode 0 keyHash key

        if Object.ReferenceEquals(root, newRoot) then
          map
        else if (map.count - 1) = maxArrayMapSize then
          let newRoot = 
            newRoot |> bitmapIndexedNodeToSeq 
            |> Seq.map (fun ((key, _) as entry) -> { hash = map.comparer.key.GetHashCode(key); entry = entry})
            |> Seq.toArray 

          { map with 
              count = map.count - 1
              root = ArrayMapRootNode { 
                entries = newRoot
                owner = owner
              } 
          }
        else { map with count = map.count - 1; root = (BitmapIndexedMapRootNode newRoot) }
 
    | ArrayMapRootNode root ->
        let index =
          root.entries
          |> Array.tryFindIndex (
            fun { hash = hash; entry = (entryKey, _) } ->
              hash = keyHash && map.comparer.key.Equals(key, entryKey)
          )
        match index with
        | Some index when root.entries.Length > 2 -> 
            { map with 
                root = ArrayMapRootNode {
                  entries = root.entries |> Array.remove index
                  owner = owner
                }
            }
        | Some index ->
            let remainingNode = root.entries.[1 - index]
            { map with count = map.count - 1; root = KeyValueRootNode remainingNode }
        | _ -> map

    | KeyValueRootNode { hash = hash; entry = (entryKey, value) }
        when hash = keyHash && map.comparer.key.Equals(key, entryKey) ->
          createWithComparer map.comparer
    | _ -> map

  let remove (key: 'k) (map: HashedTriePersistentMap<'k, 'v>) : HashedTriePersistentMap<'k, 'v> =
    removeWithMutators
      Unchecked.defaultof<obj>
      immutableBitmapIndexedNodeMutator 
      immutableArrayMapNodeMutator
      (key: 'k) 
      (map: HashedTriePersistentMap<'k, 'v>) : HashedTriePersistentMap<'k, 'v>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module TransientMap =
  let persist (map: ITransientMap<'k, 'v>) = 
    map.Persist ()
  
  let put k v (map: ITransientMap<'k, 'v>) =
    map.Put (k, v)

  let remove k (map: ITransientMap<'k, 'v>) =
    map.Remove k

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module PersistentMap =
  open PersistentMapImpl

  let mutate (map: IPersistentMap<'k, 'v>) =
    map.Mutate ()

  let rec private createTransient (map: HashedTriePersistentMap<'k,'v>) =
    let owner = new Object()

    let mutable editable = true
    let mutable map = map

    let ensureEditable () =
      if not editable then 
        failwith "TransientMap has already been persisted"

    let ensureArrayMapNodeEditable (arrayMap: ArrayMapRootNode<'k, 'v>) =
      if Object.ReferenceEquals(arrayMap.owner, owner) then arrayMap
      else {
        entries = arrayMap.entries |> Array.copy
        owner = owner
      }

    let ensureHashCollisionNodeEditable (hashCollisionNode: HashCollisionNode<'k, 'v>) =
      if Object.ReferenceEquals(hashCollisionNode.owner, owner) then hashCollisionNode
      else {
        owner = owner
        hash = hashCollisionNode.hash
        entries = hashCollisionNode.entries |> Array.copy
      }

    let ensureBitmapIndexedNodeEditable (bitmapIndexedNode: BitmapIndexedNode<'k,'v>) = 
      if Object.ReferenceEquals(bitmapIndexedNode.owner, owner) then bitmapIndexedNode
      else {
        bitmap = bitmapIndexedNode.bitmap
        nodes = bitmapIndexedNode.nodes |> Array.copy
        owner = owner
      }
    let transientHashCollisionNodeMutator 
        index 
        (entry: 'k * 'v)
        (hashCollisionNode: HashCollisionNode<'k, 'v>) =
      let hashCollisionNode = ensureHashCollisionNodeEditable hashCollisionNode
      hashCollisionNode.entries.[index] <- entry
      hashCollisionNode
  
    let transientBitmapIndexedNodeMutator 
        index 
        (node: BitmapIndexedNodeNode<'k, 'v>) 
        (bitmapIndexedNode: BitmapIndexedNode<'k, 'v>) =
      let bitmapIndexedNode = ensureBitmapIndexedNodeEditable bitmapIndexedNode
      bitmapIndexedNode.nodes.[index] <- node
      bitmapIndexedNode
  
    let transientArrayMapNodeMutator
        index 
        (node: KeyValueNode<'k, 'v>)
        (arrayMap: ArrayMapRootNode<'k, 'v>) = 
      let arrayMap = ensureArrayMapNodeEditable arrayMap
      arrayMap.entries.[index] <- node
      arrayMap

    let put (entry: 'k * 'v) (map: HashedTriePersistentMap<'k, 'v>) : HashedTriePersistentMap<'k, 'v> = 
      putWithMutators 
        owner
        transientHashCollisionNodeMutator 
        transientBitmapIndexedNodeMutator 
        transientArrayMapNodeMutator
        entry
        map

    let remove (key: 'k) (map: HashedTriePersistentMap<'k, 'v>) : HashedTriePersistentMap<'k, 'v> =
      removeWithMutators
        owner
        transientBitmapIndexedNodeMutator 
        transientArrayMapNodeMutator
        (key: 'k) 
        (map: HashedTriePersistentMap<'k, 'v>) : HashedTriePersistentMap<'k, 'v>

    { new ITransientMap<'k, 'v> with
        member this.Persist () =
          // FIXME: Could make this more thread safe maybe
          ensureEditable ()
          editable <- false
          createInternal map

        member this.Put (k, v) =
          ensureEditable ()
          map <- map |> put (k, v)
          this

        member this.Remove k =
          ensureEditable ()
          map <- map |> remove k
          this
    }

  and private createInternal (backingMap: HashedTriePersistentMap<'k,'v>) =
    ({ new PersistentMapBase<'k, 'v> () with
        override this.Count = backingMap.count
        override this.GetEnumerator () = backingMap |> toSeq |> Seq.getEnumerator
  
        override this.Item k =
          match backingMap |> tryGet k with
          | Some v -> v
          | None -> failwith "key not found"
        override this.Mutate () = createTransient backingMap
        override this.Put (k, v) =
          let newBackingMap = backingMap |> put (k, v)
          if Object.ReferenceEquals(backingMap, newBackingMap) then
            this :> IPersistentMap<'k,'v>
          else createInternal newBackingMap
        override this.Remove k =
          let newBackingMap = backingMap |> remove k
  
          if Object.ReferenceEquals(backingMap, newBackingMap) then
            this :> IPersistentMap<'k,'v>
          else createInternal newBackingMap
  
        override this.TryItem k = backingMap |> tryGet k
    }) :> IPersistentMap<'k, 'v>

  let emptyWithComparer (comparer: KeyValueComparer<'k, 'v>) =
    let backingMap = PersistentMapImpl.createWithComparer comparer
    createInternal backingMap

  let empty () = emptyWithComparer {
    key = EqualityComparer.Default
    value = EqualityComparer.Default
  }

  let count (map: IPersistentMap<'k, 'v>) =
    map.Count

  let get k (map: IPersistentMap<'k, 'v>) =
    map.Item k

  let putAll (values: seq<'k * 'v>) (map: IPersistentMap<'k, 'v>) =
    values
    |> Seq.fold (fun (acc: ITransientMap<'k, 'v>) i -> acc.Put i) (map |> mutate)
    |> TransientMap.persist

  let createWithComparer (comparer: KeyValueComparer<'k, 'v>) (values: seq<'k * 'v>) =
    emptyWithComparer comparer |> putAll values

  let create (values: seq<'k * 'v>) =
    createWithComparer 
      {
        key = EqualityComparer.Default
        value = EqualityComparer.Default
      }
      values

  let put k v (map: IPersistentMap<'k, 'v>) =
    map.Put (k, v)

  let remove k (map: IPersistentMap<'k, 'v>) =
    map.Remove k

  let tryGet k (map: IPersistentMap<'k, 'v>) =
    map.TryItem k
