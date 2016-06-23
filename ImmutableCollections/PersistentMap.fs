namespace ImmutableCollections

open System
open System.Collections.Generic

module private BitCount =
  let bitCounts = 
    let bitCounts = Array.create 65536 0
    let position1 = ref -1
    let position2 = ref -1

    for i in 1 .. 65535 do
     if !position1 = !position2 then       
      position1 := 0
      position2 := i

     bitCounts.[i] <- bitCounts.[!position1] + 1
     position1 := !position1 + 1
    bitCounts

  let inline NumberOfSetBits value =
    bitCounts.[value &&& 65535] + bitCounts.[(value >>> 16) &&& 65535]

  let inline mask hash depth = (hash >>> (depth * 5)) &&& 0x01f
  let inline bitpos hash depth = 1 <<< mask hash (depth * 5)
  let inline index bitmap bit = NumberOfSetBits(bitmap &&& (bit - 1))

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module PersistentMapImpl = 
  open BitCount

  let private maxArrayMapSize = 16
  let private bitmapIndexedNodeSize = 32

  type KeyValueNode<'k, 'v> = {
    keyHash: int
    pair: 'k*'v
  }

  type Node<'k,'v> = 
    | HashCollisionNode of HashCollisionNode<'k,'v>
    | BitmapIndexedNode of BitmapIndexedNode<'k,'v>

  and HashCollisionNode<'k,'v> = {
    keyHash: int
    pairs: ImmutableArray<'k*'v>
  }

  and BitmapIndexedNode<'k,'v> = {
    depth: int
    bitmap: int
    nodes: ImmutableArray<Choice<'k * 'v, Node<'k,'v>>>
  }

  type ArrayMap<'k, 'v> = ImmutableArray<KeyValueNode<'k, 'v>>
  
  type RootNode<'k, 'v> =
    | BitmapIndexedRootNode of BitmapIndexedNode<'k,'v>
    | ArrayMapRootNode of ArrayMap<'k, 'v>
    | KeyValueRootNode of KeyValueNode<'k, 'v>
    | NoneRootNode

  type [<ReferenceEquality>] MapComparer<'k, 'v> = {
    key: IEqualityComparer<'k>
    value: IEqualityComparer<'v>
  }
  
  type [<ReferenceEquality>] HashedTriePersistentMap<'k, 'v> = {
    comparer: MapComparer<'k, 'v>
    count: int
    root: RootNode<'k, 'v>
  }

  let private emptyBitmapIndexedNode () = BitmapIndexedNode {
    depth = 0
    bitmap = 0
    nodes = ImmutableArray.empty ()
  }

  let create (keyComparer: IEqualityComparer<'v>) (valueComparer: IEqualityComparer<'v>) = {
    comparer = { key = keyComparer; value = valueComparer }
    count = 0
    root = NoneRootNode
  }

  let rec createNode (comparer: MapComparer<'k, 'v>) depth (key1, val1) key2 key2hash val2 =
    let putInNode k keyHash v = putInNode comparer k keyHash v 
    let key1hash = comparer.key.GetHashCode(key1)

    if key1hash = key2hash then HashCollisionNode {
        keyHash = key1hash
        pairs = ImmutableArray.createUnsafe([| (key1, val1); (key2, val2) |])
      }
    else 
      let node = emptyBitmapIndexedNode ()
      node |> putInNode key1 key1hash val1 |> putInNode key2 key2hash val2

  and putInNode (comparer: MapComparer<'k, 'v>) (k: 'k) (keyHash: int) (v: 'v) (node: Node<'k, 'v>) = 
    let putInNode k keyHash v = putInNode comparer k keyHash v 
    let createNode = createNode comparer

    match node with
    | BitmapIndexedNode bitmapIndexedNode -> 
        let bit = bitpos keyHash bitmapIndexedNode.depth
        let idx = index bitmapIndexedNode.depth bit
        let bitmapAndBit  = bitmapIndexedNode.bitmap &&& bit

        if bitmapAndBit <> 0 then
          let childNode = bitmapIndexedNode.nodes |> Collection.get idx
          let newChildNode = 
            match childNode with
            | Choice1Of2 (key, value) when comparer.key.Equals(key, k) ->
                if comparer.value.Equals (value, v) then childNode
                else Choice1Of2 (k, v)
            | Choice1Of2 ((key, value) as kvp1)->
                Choice2Of2 (createNode (bitmapIndexedNode.depth + 1) kvp1 k keyHash v)
            | Choice2Of2 childNode ->
                Choice2Of2 (childNode |> putInNode k keyHash v) 

          if Object.ReferenceEquals(childNode, newChildNode) then 
            node
          else BitmapIndexedNode { 
            bitmapIndexedNode with
              nodes = bitmapIndexedNode.nodes |> ImmutableArray.cloneAndSet idx childNode
          }
        else
          let n = NumberOfSetBits bitmapIndexedNode.bitmap 
          if n >= bitmapIndexedNodeSize then
            node
          else
            let newArray = Array.zeroCreate (n + 1)
            bitmapIndexedNode.nodes.CopyTo (0, newArray, 0, n)
            bitmapIndexedNode.nodes.CopyTo(idx, newArray, idx+1, n - idx)
            newArray.[idx] <- Choice1Of2 (k, v)
                 
            BitmapIndexedNode { 
              depth = 0
              bitmap = bitmapIndexedNode.bitmap ||| bit
              nodes = ImmutableArray.createUnsafe newArray
            }  
    | HashCollisionNode hashCollisionNode -> node


  let put (k: 'k) (v: 'v) (map: HashedTriePersistentMap<'k, 'v>) : HashedTriePersistentMap<'k, 'v> =
    let keyHash = map.comparer.key.GetHashCode(k)

    match map.root with
    | NoneRootNode ->
        let newRoot = KeyValueRootNode { keyHash = keyHash; pair = (k, v) }
        { map with count = 1; root = newRoot }
    
    | KeyValueRootNode { keyHash = rootKeyHash; pair = (rootKey, rootValue) }
          when keyHash = rootKeyHash
            && map.comparer.key.Equals(k, rootKey)
            && map.comparer.value.Equals(v, rootValue) ->
        map

    | KeyValueRootNode { keyHash = rootKeyHash; pair = (rootKey, _) }
          when keyHash = rootKeyHash
            && map.comparer.key.Equals(k, rootKey) ->
        let newRoot = KeyValueRootNode { keyHash = keyHash; pair = (k, v) }
        { map with root = newRoot}

    | KeyValueRootNode keyValueNode ->
        let newRootNodes = ImmutableArray.createUnsafe [| keyValueNode; { keyHash = keyHash; pair = (k, v) } |]
        let newRoot = ArrayMapRootNode newRootNodes

        { map with count = 2; root = newRoot }

    | ArrayMapRootNode arrayMap ->
        let index =
          arrayMap |> Collection.values |> Seq.tryFindIndex (
            fun { keyHash = itemKeyHash; pair = (itemKey, _)} -> 
              keyHash = itemKeyHash && map.comparer.key.Equals(k, itemKey)
          )
     
        let count = arrayMap |> Collection.count
     
        match index with
        | Some index ->
           let keyValueNode = arrayMap |> Collection.get index
           let (_, itemValue) = keyValueNode.pair

           if map.comparer.value.Equals(v, itemValue) then map else 
           
           let newArray = arrayMap |> ImmutableArray.cloneAndSet index { 
             keyValueNode with pair = (k, v) 
           }
           let newRoot = ArrayMapRootNode newArray
     
           { map with count = 2; root = newRoot }
     
        | _ when count < maxArrayMapSize ->  
            let newArray = arrayMap |> ImmutableArray.add { 
              keyHash  = keyHash
              pair = (k, v) 
            }
            let newRoot = ArrayMapRootNode newArray
     
            { map with count = count + 1; root = newRoot }
     
        | _ ->
            let reducer acc node  = 
              let (key, value) = node.pair
              putInNode map.comparer key node.keyHash v acc
               
            let mapi = 
              arrayMap |> Collection.values |> Seq.fold reducer (BitmapIndexedNode {
                depth = 0
                bitmap = 0
                nodes = ImmutableArray.empty ()
              }) 
            map
    | _ -> map


(*
module PersistentMap =
  open PersistentMapImpl

  type private HashedTrieBackedPersistentMap<'k,'v> private (backingCollection) =
    static member Create (backingCollection: HashedTriePersistentMap<'k,'v>) =
      (new HashedTrieBackedPersistentMap<'k,'v>(backingCollection) :> IPersistentMap<'k,'v>)

    inherit IPersistentMap<'k, 'v>  with
      member this.Count = backingCollection.get
      member this.GetEnumerator () = 
      member this.GetEnumerator () = 
      member this.Item =
      member this.Keys =
      member this.Put (k,v) =
      member this.Remove k =
      member this.TryGet =
      member this.Values =

  let createWithComparer (comparer: IEqualityComparer<'v>) = 
    let backingCollections = HashedTriePe rsistentMap.create capomparer
    HashedTrieBackedPersistentMap.Create backingCollection
      
  let create () =
    createWithComparer EqualityComparer.Default

*)