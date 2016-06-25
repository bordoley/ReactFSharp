namespace ImmutableCollections

open System
open System.Collections
open System.Collections.Generic

type [<ReferenceEquality>] KeyValueComparer<'k, 'v> = {
  key: IEqualityComparer<'k>
  value: IEqualityComparer<'v>
}

module BitCount =
  let inline NumberOfSetBits value =
    let mutable i = value
    i <- i - ((i >>> 1) &&& 0x55555555u);
    i <- (i &&& 0x33333333u) + ((i >>> 2) &&& 0x33333333u);
    let count = (((i + (i >>> 4)) &&& 0x0F0F0F0Fu) * 0x01010101u) >>> 24;
    (int count)

  let inline mask hash depth = (hash >>> (depth * 5)) &&& 0x3F
  let inline bitpos hash depth = 1u <<< (mask hash depth)
  let inline index (bitmap: uint32) (bit: uint32) = 
    NumberOfSetBits (bitmap &&& (uint32 (bit - 1u)))

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module private PersistentMapImpl = 
  open BitCount

  let private maxArrayMapSize = 16
  let private width = 32

  type ArrayNode<'k, 'v> = {
    count: int
    nodes: ImmutableArray<Option<TrieNode<'k, 'v>>>
  }

  and TrieNode<'k,'v> = 
    | ArrayNode of ArrayNode<'k, 'v>
    | BitmapIndexedNode of BitmapIndexedNode<'k,'v>
  
  and BitmapIndexedNodeNode<'k,'v> =
    | Entry of ('k * 'v)
    | HashCollisionNode of HashCollisionNode<'k, 'v>
    | TrieNode of TrieNode<'k, 'v>

  and HashCollisionNode<'k, 'v> = {
    hash: int
    nodes: ImmutableArray<'k*'v>
  }
  
  and BitmapIndexedNode<'k,'v> = {
    bitmap: uint32
    nodes: ImmutableArray<BitmapIndexedNodeNode<'k,'v>>
  }

  type KeyValueNode<'k, 'v> = {
    hash: int
    entry: 'k*'v
  }
  
  type RootNode<'k, 'v> =
    | TrieRootNode of TrieNode<'k,'v>
    | ArrayMapRootNode of ImmutableArray<KeyValueNode<'k, 'v>>
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

  let private emptyBitmapIndexedNode () = {
    bitmap = (uint32 0)
    nodes = ImmutableArray.empty ()
  }

  let private putInHashCollisionNode 
      (comparer: KeyValueComparer<'k, 'v>) 
      (newEntry: 'k * 'v) 
      (hashCollisionNode: HashCollisionNode<'k, 'v>) =

    let keyComparer = comparer.key
    let valueComparer = comparer.value

    let (newKey, newValue) = newEntry

    let element =
      hashCollisionNode.nodes |> Seq.tryFind (fun (_, (key, value)) ->  keyComparer.Equals(key, newKey))
    
    match element with
    | Some (index, (key, value)) when valueComparer.Equals(value, newValue) -> 
        (hashCollisionNode, 0)
    
    | Some (index, (key, value)) ->
        let newHashCollisionNode =
          { hashCollisionNode with 
              nodes = hashCollisionNode.nodes |> ImmutableArray.cloneAndSet index newEntry
          }
        (newHashCollisionNode, 0)   

    | _ ->
      let newHashCollisionNode = 
        { hashCollisionNode with 
            nodes = hashCollisionNode.nodes |> ImmutableArray.add newEntry
        }
      (newHashCollisionNode, 0)   

  let rec private putInBitmapIndexedNode 
      (comparer: KeyValueComparer<'k, 'v>) 
      (depth: int)
      (newHash: int) 
      (newEntry: 'k * 'v)
      (bitmapIndexedNode: BitmapIndexedNode<'k, 'v>) : (TrieNode<'k, 'v> * int) =

    let keyComparer = comparer.key
    let valueComparer = comparer.value
    let (newKey, newValue) = newEntry
    
    let bit = bitpos newHash depth
    let index = index bitmapIndexedNode.bitmap bit |> int
    let nodeContainsEntry = (bitmapIndexedNode.bitmap &&& (uint32 bit)) <> 0u

    if nodeContainsEntry then
      let childNode = bitmapIndexedNode.nodes |> Map.get index

      let (newChildNode, increment) = 
        match childNode with
        | Entry (key, value) 
              when keyComparer.Equals(key, newKey) && valueComparer.Equals(value, newValue) ->
            (childNode, 0)

        | Entry (key, _) when keyComparer.Equals(key, newKey) -> 
            (Entry newEntry, 0)
           
        | Entry ((key, value) as entry) ->
            let hash = keyComparer.GetHashCode (key)

            if hash = newHash then
              let hashCollisionNode = 
                HashCollisionNode {
                  hash = hash
                  nodes = ImmutableArray.createUnsafe [| entry; newEntry|]
                } 

              (hashCollisionNode , 1)
            else 
              let newNode = 
                let (newNode, _) = 
                  emptyBitmapIndexedNode ()
                  |> putInBitmapIndexedNode comparer (depth + 1) hash entry 

                let (newNode, _) = 
                  newNode 
                  |> putInTrieNode comparer (depth + 1) newHash newEntry
                TrieNode newNode

              (newNode, 1)

        | HashCollisionNode hashCollisionNode ->
            let (newHashCollisionNode, increment) = hashCollisionNode |> putInHashCollisionNode comparer newEntry
            if Object.ReferenceEquals(hashCollisionNode, newHashCollisionNode) then
              (childNode, 0)
            else 
              (HashCollisionNode newHashCollisionNode, 1)

        | TrieNode trieNode ->
            let (newTrieNode, increment) = trieNode |> putInTrieNode comparer depth newHash newEntry
            if Object.ReferenceEquals(trieNode, newTrieNode) then
              (childNode, 0)
            else 
              (TrieNode newTrieNode, increment)
      
      if Object.ReferenceEquals(childNode, newChildNode) then
        (BitmapIndexedNode bitmapIndexedNode, 0)
      else 
        let newBitmapIndexedNode = BitmapIndexedNode {
            bitmap = bitmapIndexedNode.bitmap
            nodes = bitmapIndexedNode.nodes |> ImmutableArray.cloneAndSet index newChildNode
          }
        (newBitmapIndexedNode, increment)
    else
      let count = NumberOfSetBits bitmapIndexedNode.bitmap 
      if count >= width then
        let newArray = Array.create width Option.None

        let index = mask newHash (depth + 1)
        let (childBitmapIndexedNode, _) = 
          (emptyBitmapIndexedNode ()) 
          |> putInBitmapIndexedNode comparer (depth + 1) newHash newEntry

        newArray.[index] <- Some childBitmapIndexedNode

        for i = 0 to (width - 1) do 
          if i <> index then
            let nodeAtIndex =
              match bitmapIndexedNode.nodes |> Map.get i with
              | TrieNode trieNode -> Some trieNode
              | Entry _ as entry -> 
                  BitmapIndexedNode {
                    bitmap = bitpos newHash depth |> uint32
                    nodes = ImmutableArray.createUnsafe [| entry |]
                  } |> Some
              | HashCollisionNode _ as hashCollisionNode -> 
                  BitmapIndexedNode {
                    bitmap = bitpos newHash depth |> uint32
                    nodes = ImmutableArray.createUnsafe [| hashCollisionNode |]
                  } |> Some
            newArray.[i] <- nodeAtIndex

        let arrayNode = {
          count = count + 1
          nodes = ImmutableArray.createUnsafe newArray
        }

        (ArrayNode arrayNode, 1)
      else
        let newArray = Array.zeroCreate (count + 1)
        bitmapIndexedNode.nodes.CopyTo (0, newArray, 0, count)
        bitmapIndexedNode.nodes.CopyTo(index, newArray, index + 1, count - index)
        newArray.[index] <- Entry newEntry

        let newBitmapIndexedNode = BitmapIndexedNode { 
          bitmap = bitmapIndexedNode.bitmap ||| (uint32 bit)
          nodes = ImmutableArray.createUnsafe newArray
        }
        (newBitmapIndexedNode, 1)

  and private putInArrayNode 
      (comparer: KeyValueComparer<'k, 'v>)
      (depth: int) 
      (hash: int)
      (newEntry: 'k * 'v) 
      (arrayNode: ArrayNode<'k, 'v>) =
    let index = mask hash depth
    let nodeAtIndex = arrayNode.nodes |> Map.get index

    match nodeAtIndex with
    | Some trieNode -> 
        let (newTrieNode, increment) = trieNode |> putInTrieNode comparer depth hash newEntry

        if Object.ReferenceEquals(trieNode, newTrieNode) then (ArrayNode arrayNode, 0) else
        let newArrayNode = ArrayNode {
          count = arrayNode.count + increment
          nodes = arrayNode.nodes |> ImmutableArray.cloneAndSet index (Some newTrieNode)
        }    
        (newArrayNode, increment)    

    | None -> 
        let newNodeAtIndex = BitmapIndexedNode { 
          bitmap = 0u
          nodes = ImmutableArray.createUnsafe [| Entry newEntry |]
        }

        let newArrayNode = ArrayNode {
          count = arrayNode.count + 1
          nodes = arrayNode.nodes |> ImmutableArray.cloneAndSet index (Some newNodeAtIndex)
        }

        (newArrayNode, 1)

  and private putInTrieNode 
      (comparer: KeyValueComparer<'k, 'v>) 
      (depth: int)
      (hash: int)
      (newEntry: 'k * 'v)
      (trieNode: TrieNode<'k,'v>) =

    let keyComparer = comparer.key
    let valueComparer = comparer.value
    let (newKey, newValue) = newEntry

    let (newTrieNode, increment) as newTrieNodeAndCount =
      match trieNode with
      | ArrayNode arrayNode -> 
          arrayNode |> putInArrayNode comparer depth hash newEntry 
      | BitmapIndexedNode bitmapIndexedNode -> 
          bitmapIndexedNode |> putInBitmapIndexedNode comparer depth hash newEntry 
    
    match (trieNode, newTrieNode) with
    | (ArrayNode arrayNode, ArrayNode newArrayNode) 
          when Object.ReferenceEquals(arrayNode, newArrayNode) ->
        (trieNode, 0)
        
    | (BitmapIndexedNode bitmapIndexedNode, BitmapIndexedNode newBitmapIndexedNode) 
          when Object.ReferenceEquals(bitmapIndexedNode, newBitmapIndexedNode) ->
        (trieNode, 0)

    | _ -> newTrieNodeAndCount

  let private putInArrayMap 
      (comparer: KeyValueComparer<'k, 'v>) 
      (hash: int)
      (newEntry: 'k * 'v)
      (arrayMap: ImmutableArray<KeyValueNode<'k, 'v>>) =
    let keyComparer = comparer.key
    let valueComparer = comparer.value
    let (newKey, newValue) = newEntry

    let element =
      arrayMap |> Seq.tryFind (
        fun (_, { hash = hash; entry = (key, _)}) -> 
          hash = hash && keyComparer.Equals(key, newKey)
      )
 
    let count = arrayMap |> Map.count
 
    match element with
    | Some (index, { hash = hash; entry = (key, value)}) 
          when valueComparer.Equals (value, newValue) -> 
        (ArrayMapRootNode arrayMap, 0)    

    | Some (index, _) ->
        let newArray = 
          arrayMap |> ImmutableArray.cloneAndSet index { hash = hash; entry = newEntry }
        (ArrayMapRootNode newArray, 0)
 
    | _ when count < maxArrayMapSize ->  
        let newArray = 
          arrayMap |> ImmutableArray.add { hash = hash; entry = newEntry }
 
        (ArrayMapRootNode newArray, 1)
 
    | _ ->
        let reducer trieNode (keyValueNode: KeyValueNode<'k, 'v>) =
          let (newTrieNode, _) = trieNode |> putInTrieNode comparer 0 keyValueNode.hash keyValueNode.entry
          newTrieNode

        let (trieNode, _) = 
          arrayMap |> Map.values 
          |> Seq.fold reducer (BitmapIndexedNode (emptyBitmapIndexedNode ()))
          |> putInTrieNode comparer 0 hash newEntry

        (TrieRootNode trieNode, 1)

  let put (newEntry: 'k * 'v) (map: HashedTriePersistentMap<'k, 'v>) : HashedTriePersistentMap<'k, 'v> =
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
        let newRootNodes = 
          ImmutableArray.createUnsafe [| keyValueNode; { hash = newHash; entry = newEntry } |]

        { map with count = 2; root = ArrayMapRootNode newRootNodes }

    | ArrayMapRootNode arrayMap ->
        let (newRootNode, increment) = arrayMap |> putInArrayMap map.comparer newHash newEntry

        match (map.root, newRootNode) with
        | (ArrayMapRootNode arrayMap, ArrayMapRootNode newArrayMap)
              when Object.ReferenceEquals(arrayMap, newArrayMap) ->
            map

        | _ -> 
          { map with count = map.count + increment; root = newRootNode }

    | TrieRootNode trieNode -> 
        let (newTrieNode, increment) = trieNode |> putInTrieNode map.comparer 0 newHash newEntry

        if Object.ReferenceEquals(trieNode, newTrieNode) then
          map
        else 
          { map with 
              count = map.count + increment
              root = TrieRootNode newTrieNode 
          }
  
  let remove (key: 'k) (map: HashedTriePersistentMap<'k, 'v>) : HashedTriePersistentMap<'k, 'v> = map

  let tryGet (key: 'k) (map: HashedTriePersistentMap<'k, 'v>) = None

  let toSeq (map: HashedTriePersistentMap<'k, 'v>) : seq<'k * 'v> = Seq.empty


module PersistentMap =
  open PersistentMapImpl

  type private HashedTrieBackedPersistentMap<'k,'v> private (backingMap) =
    static member Create (backingMap: HashedTriePersistentMap<'k,'v>) =
      (new HashedTrieBackedPersistentMap<'k,'v>(backingMap) :> IPersistentMap<'k,'v>)

    interface IPersistentMap<'k, 'v> with
      member this.Count = backingMap.count
      member this.GetEnumerator () = backingMap |> toSeq |> Seq.getEnumerator
      member this.GetEnumerator () = 
        (this :> IEnumerable<'k * 'v>).GetEnumerator() :> IEnumerator
      member this.Item k =
        match backingMap |> tryGet k with
        | Some v -> v
        | None -> failwith "key not found"
      member this.Put (k, v) = 
        let newBackingMap = backingMap |> put (k, v)
        if Object.ReferenceEquals(backingMap, newBackingMap) then 
          this :> IPersistentMap<'k,'v>
        else HashedTrieBackedPersistentMap.Create newBackingMap
      member this.Remove k = 
        let newBackingMap = backingMap |> remove k

        if Object.ReferenceEquals(backingMap, newBackingMap) then 
          this :> IPersistentMap<'k,'v>
        else HashedTrieBackedPersistentMap.Create newBackingMap

      member this.TryGet k = backingMap |> tryGet k

  let createWithComparer (comparer: KeyValueComparer<'k, 'v>) = 
    let backingMap = PersistentMapImpl.createWithComparer comparer
    HashedTrieBackedPersistentMap.Create backingMap
      
  let create () = createWithComparer {
    key = System.Collections.Generic.EqualityComparer.Default
    value = System.Collections.Generic.EqualityComparer.Default
  }

  let count (map: IPersistentMap<'k, 'v>) =
    map.Count
  
  let get k (map: IPersistentMap<'k, 'v>) =
    map.Item k

  let put entry (map: IPersistentMap<'k, 'v>) =
    map.Put entry

  let remove k (map: IPersistentMap<'k, 'v>) =
    map.Remove k

  let tryGet k (map: IPersistentMap<'k, 'v>) =
    map.TryGet k
