namespace ImmutableCollections

open System
open System.Collections
open System.Collections.Generic

// http://hypirion.com/musings/understanding-persistent-vector-pt-1

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module private PersistentVectorImpl =
  type[<ReferenceEquality>] TrieNode<'v> =
    | LeafNode of ImmutableArray<ImmutableArray<'v>>
    | LevelNode of LevelNode<'v>
  
  and [<ReferenceEquality>] LevelNode<'v> = {
    nodes: ImmutableArray<TrieNode<'v>>
    depth: int
  }
  
  type [<ReferenceEquality>] RootNode<'v> =
    | LevelRootNode of LevelNode<'v>
    | LeafRootNode of ImmutableArray<ImmutableArray<'v>>
    | ValuesRootNode of ImmutableArray<'v>
    | NoneRootNode
  
  type [<ReferenceEquality>] HashedTriePersistentVector<'v> = {
    comparer: IEqualityComparer<'v>
    count: int
    root: RootNode<'v>
    tail: ImmutableArray<'v>
  }

  let private bits = 5
  let private width = 1 <<< bits
  let private mask = width - 1

  let private leafDepth = 1

  let private computeIndex depth index =
    let level = depth * bits
    let ret = (index >>> level) &&& mask
    ret

  let create (comparer: IEqualityComparer<'v>) = {
    comparer = comparer
    count = 0
    root = NoneRootNode
    tail = ImmutableArray.empty ()
  }

  let rec private newPath depth (tail: ImmutableArray<'v>) =
    if depth = leafDepth then
      LeafNode (ImmutableArray.createUnsafe [| tail |])
    else
      let child = newPath (depth - 1) tail
      LevelNode {
        nodes = (ImmutableArray.createUnsafe [| child |])
        depth = depth
      }

  let private getTailOffset vec = vec.count - (vec.tail |> Map.count)

  let private getDepthFromRoot = function
    | LevelRootNode trie -> trie.depth
    | LeafRootNode _ -> leafDepth
    | ValuesRootNode _ -> 0
    | NoneRootNode -> 0

  let private pushTail (vec: HashedTriePersistentVector<'v>) : RootNode<'v> =
    let tail = vec.tail
    if (tail |> Map.count) <> width then failwith "tail wrong size"

    let depth = getDepthFromRoot vec.root

    match vec.root with
    | NoneRootNode ->
        ValuesRootNode tail

    | ValuesRootNode values ->
        LeafRootNode (ImmutableArray.createUnsafe [| values; tail |])

    | LeafRootNode leafNode ->
        if (leafNode |> Map.count) < width then
          LeafRootNode (leafNode |> ImmutableArray.add tail)
        else
          let oldLeafNode = LeafNode leafNode
          let newLeafNode = LeafNode (ImmutableArray.createUnsafe [| tail |])

          LevelRootNode {
            nodes = (ImmutableArray.createUnsafe [| oldLeafNode; newLeafNode |])
            depth = depth + 1
          }

    | LevelRootNode trie ->
        let rec pushTail depth (trie: ImmutableArray<TrieNode<'v>>) =
          let index = computeIndex depth (vec.count - 1)

          match trie |> Map.tryGet index with
          | Some (LeafNode leafNode) when (leafNode |> Map.count) < width ->
              let newLeafNode = LeafNode (leafNode |> ImmutableArray.add tail)
              LevelNode {
                nodes = trie |> ImmutableArray.cloneAndSet index newLeafNode
                depth = depth
              }

          | None when (trie |> Map.count) < width ->
              let newNode = newPath (depth - 1) tail
              LevelNode {
                nodes = trie |> ImmutableArray.add newNode
                depth = depth
              }

          | Some (LevelNode trieNode) ->
              let newTrieNode = pushTail (depth - 1) trieNode.nodes
              LevelNode {
                nodes = trie |> ImmutableArray.cloneAndSet index newTrieNode
                depth = depth
              }

          | _ -> failwith "node is full"

        let shift = depth * bits

        if ((vec.count - 1) >>> bits) >= (1 <<< shift) then
          let oldRootNode = LevelNode trie
          LevelRootNode {
            nodes = ImmutableArray.createUnsafe [| oldRootNode; (newPath depth tail) |]
            depth = depth + 1
          }
        else
          match pushTail depth trie.nodes with
          | LeafNode leafNode ->
              failwith "how can this happen?"
              LeafRootNode leafNode
          | LevelNode trieNode ->
              LevelRootNode trieNode

  let add (v: 'v) (vec: HashedTriePersistentVector<'v>) =
    if (vec.tail |> Map.count) < width then {
        vec with
          count = vec.count + 1
          root = vec.root
          tail = vec.tail |> ImmutableArray.add v
      }
    else {
        vec with
          count = vec.count + 1
          root = pushTail vec
          tail = ImmutableArray.createUnsafe [| v |]
      }

  let toSeq (vec: HashedTriePersistentVector<'v>) = seq {
    let rec toSeq = function
      | LeafNode leafNode ->
          leafNode |> Map.values |> Seq.map Map.values |> Seq.concat
      | LevelNode trieNode ->
          trieNode.nodes |> Map.values |> Seq.map toSeq |> Seq.concat

    match vec.root with
    | LevelRootNode trieNode ->
        yield! (LevelNode trieNode) |> toSeq
    | LeafRootNode leafNode ->
        yield! (LeafNode leafNode) |> toSeq
    | ValuesRootNode values ->
        yield! values |> Map.values
    | NoneRootNode -> ()

    yield! vec.tail |> Map.values
  }

  let private leafNodeFor index vec =
    let tailOffset = getTailOffset vec

    let rec findLeafNode = function
      | LeafNode leafNode ->
          let nodeIndex = computeIndex leafDepth index
          leafNode |> Map.tryGet nodeIndex
      | LevelNode trieNode ->
          let nodeIndex = computeIndex trieNode.depth index
          match trieNode.nodes |> Map.tryGet nodeIndex with
          | Some node -> findLeafNode node
          | _ -> None

    if index < 0 || index >= vec.count then
      None
    elif index >= tailOffset then
      Some vec.tail
    else
      match vec.root with
      | LevelRootNode trieNode ->
          (LevelNode trieNode) |> findLeafNode
      | LeafRootNode leafNode ->
          (LeafNode leafNode) |> findLeafNode
      | ValuesRootNode values ->
          Some values
      | NoneRootNode ->
          None

  let tryGet (index: int) (vec: HashedTriePersistentVector<'v>) =
    let v = vec |> leafNodeFor index
    match vec |> leafNodeFor index with
    | Some vec ->
        let index = computeIndex 0 index
        vec |> Map.tryGet index
    | _ -> None

  let get (index: int) (vec: HashedTriePersistentVector<'v>) =
    match vec |> tryGet index with
    | Some v -> v
    | _ -> failwith "index out of bounds"

  let private doUpdateValuesNode (comparer: IEqualityComparer<'v>) index v (valuesNode: ImmutableArray<'v>) =
    let nodeIndex = computeIndex 0 index
    let currentValue = valuesNode |> Map.get nodeIndex

    if comparer.Equals(currentValue, v) then
      valuesNode
    else
      valuesNode |> ImmutableArray.cloneAndSet nodeIndex v

  let private doUpdateLeafNode (comparer: IEqualityComparer<'v>) index v (leafNode: ImmutableArray<ImmutableArray<'v>>) =
    let leafIndex = computeIndex leafDepth index
    let valuesNode = leafNode |> Map.get leafIndex
    let newValuesNode = valuesNode |> doUpdateValuesNode comparer index v

    if Object.ReferenceEquals(valuesNode, newValuesNode) then
      leafNode
    else
      leafNode |> ImmutableArray.cloneAndSet leafIndex newValuesNode

  let rec private doUpdateTrieNode (comparer: IEqualityComparer<'v>) index v (trieNode: LevelNode<'v>) =
    let pos = computeIndex trieNode.depth index

    let currentChildeNodeAtPos = trieNode.nodes |> Map.get pos
    let newChildNodeAtPos =
      match currentChildeNodeAtPos  with
      | LeafNode leafNode as currentTrieNode ->
          let newLeafNode = leafNode |> doUpdateLeafNode comparer index v

          if Object.ReferenceEquals(leafNode, newLeafNode) then
            currentTrieNode
          else
            LeafNode newLeafNode

      | LevelNode childTrieNode as currentTrieNode ->
          let newChildTrieNode = childTrieNode |> doUpdateTrieNode comparer index v

          if Object.ReferenceEquals(childTrieNode, newChildTrieNode) then
            currentTrieNode
          else
            LevelNode newChildTrieNode

    if Object.ReferenceEquals(currentChildeNodeAtPos, newChildNodeAtPos) then
      trieNode
    else
      {
        depth = trieNode.depth
        nodes = (trieNode.nodes |> ImmutableArray.cloneAndSet pos newChildNodeAtPos)
      }

  let update (index: int) (v: 'v) (vec: HashedTriePersistentVector<'v>): HashedTriePersistentVector<'v> =
    if index >= vec.count || index < 0 then
      failwith "index out of range"
    elif index >= (getTailOffset vec) then
      let nodeIndex = computeIndex 0 index
      let currentValue = vec.tail |> Map.get nodeIndex

      if vec.comparer.Equals(currentValue, v) then
        vec
      else
        let newTail = vec.tail |> ImmutableArray.cloneAndSet nodeIndex v
        { vec with tail = newTail }
    else
      let newRoot =
        match vec.root with
        | LevelRootNode trieNode as currentRootNode ->
            let newTrieNode = trieNode |> doUpdateTrieNode vec.comparer index v

            if Object.ReferenceEquals(trieNode, newTrieNode) then
              currentRootNode
            else
              LevelRootNode newTrieNode

        | LeafRootNode leafNode as currentRootNode ->
            let newLeafNode = leafNode |> doUpdateLeafNode vec.comparer index v

            if Object.ReferenceEquals(leafNode, newLeafNode) then
              currentRootNode
            else
              LeafRootNode newLeafNode

        | ValuesRootNode valuesNode as currentRootNode ->
            let newValuesNode = valuesNode |> doUpdateValuesNode vec.comparer index v

            if Object.ReferenceEquals(valuesNode, newValuesNode) then
              currentRootNode
            else
              ValuesRootNode newValuesNode

        | NoneRootNode ->
            failwith "something went wrong"

      if Object.ReferenceEquals(vec.root, newRoot) then
        vec
      else
        { vec with root = newRoot }

  let private popTrieRootNodeTail (trieRootNode: LevelNode<'v>) : TrieNode<'v> =
    let rec doPop = function
      | LeafNode leafNode when (leafNode |> Map.count) > 1  ->
          let newLeafNode = leafNode |> ImmutableArray.pop
          Some (LeafNode newLeafNode)
      | LevelNode trieNode ->
          let childNode = trieNode.nodes |> Vector.last
          match doPop childNode with
          | None when (trieNode.nodes |> Map.count) > 1 ->
              let newTrieNodeNodes = trieNode.nodes |> ImmutableArray.pop
              Some (LevelNode {
                trieNode with
                  nodes = newTrieNodeNodes
              })
          | Some newChildNode ->
              let subidx = trieNode.nodes |> Vector.lastIndex
              let newTrieNodeNodes = trieNode.nodes |> ImmutableArray.cloneAndSet subidx newChildNode
              Some (LevelNode {
                trieNode with
                  nodes = newTrieNodeNodes
              })
          | None -> None
      | _ -> None

    let childNode = trieRootNode.nodes |> Vector.last
    let newChildNode = doPop childNode
    match newChildNode with
    | Some childNode ->
        let subidx = trieRootNode.nodes |> Vector.lastIndex
        let newTrieRootNodeNodes = trieRootNode.nodes |> ImmutableArray.cloneAndSet subidx childNode
        LevelNode {
          trieRootNode with
            nodes = newTrieRootNodeNodes
        }
    | None when (trieRootNode.nodes |> Map.count) > 2 ->
        let newTrieRootNodeNodes = trieRootNode.nodes |> ImmutableArray.pop
        LevelNode {
          trieRootNode with
            nodes = newTrieRootNodeNodes
        }
    | None when (trieRootNode.nodes |> Map.count) = 1 ->
        trieRootNode.nodes |> Map.get 0
    | None -> failwith "should never happen"

  let pop (vec: HashedTriePersistentVector<'v>) =
    if vec.count = 0 then
      failwith "Can't pop empty vector"
    elif vec.count = 1 then
        create vec.comparer
    elif (vec.tail |> Map.count) > 1 then
      let newTail = vec.tail |> ImmutableArray.pop

      { vec with
          count = vec.count - 1
          tail = newTail
      }
    else
      let index = vec.count - 2
      let newTail = vec |> leafNodeFor index |> Option.get

      let newRoot =
        match vec.root with
        | LevelRootNode trieNode ->
            match trieNode |> popTrieRootNodeTail with
            | LeafNode leafNode ->
                LeafRootNode leafNode
            | LevelNode trieRootNode ->
                LevelRootNode trieNode

        | LeafRootNode leafNode ->
            if ((leafNode |> Map.count) > 2) then
              let newLeafNode = leafNode |> ImmutableArray.pop
              LeafRootNode newLeafNode
            else
              ValuesRootNode (leafNode |> Map.get 0)
        | ValuesRootNode valuesNode ->
            NoneRootNode
        | NoneRootNode ->
            failwith "Something went wrong"

      { vec with
          count = vec.count - 1
          root = newRoot
          tail = newTail
      }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module PersistentVector =
  open PersistentVectorImpl

  type private HashedTrieBackedPersistentVector<'v> private (backingVector) =
    static member Create (backingVector: HashedTriePersistentVector<'v>) =
      (new HashedTrieBackedPersistentVector<'v>(backingVector) :> IPersistentVector<'v>)
  
    interface IPersistentVector<'v> with
      member this.Add v = 
        let newBackingVector = backingVector |> add v
        if Object.ReferenceEquals(backingVector, newBackingVector) then (this :> IPersistentVector<'v>)
        else HashedTrieBackedPersistentVector.Create newBackingVector
      member this.Count = backingVector.count
      member this.Item index = backingVector |> get index
      member this.GetEnumerator () = 
        backingVector 
        |> toSeq 
        |> Seq.mapi (fun i v -> (i, v))
        |> Seq.getEnumerator
      member this.GetEnumerator () = (this :> IEnumerable<int*'v>).GetEnumerator() :> IEnumerator
      member this.Pop () =
        let newBackingVector = backingVector |> pop
        HashedTrieBackedPersistentVector.Create newBackingVector
      member this.TryGet index = backingVector |> tryGet index
      member this.Update(index, value) =
        let newBackingVector = backingVector |> update index value
        if Object.ReferenceEquals(backingVector, newBackingVector) then (this :> IPersistentVector<'v>)
        else HashedTrieBackedPersistentVector.Create newBackingVector
  
  type private SubPersistentVector<'v>(backingVector, startIndex, count) = 
    static member Create (backingVector: IPersistentVector<'v>, startIndex, count) =
      if startIndex < 0 || startIndex >= backingVector.Count then 
        failwith "startindex out of range"
      elif startIndex + count > backingVector.Count then 
        failwith "count out of range"
      elif startIndex = 0 && count = backingVector.Count then 
        backingVector
      else (new SubPersistentVector<'v>(backingVector, startIndex, count) :> IPersistentVector<'v>)
  
    interface IPersistentVector<'v> with
      member this.Add v = 
        let index = startIndex + count
        let newBackingVector =
          if index < backingVector.Count then backingVector.Update (index, v)
          else backingVector.Add v
        SubPersistentVector.Create(newBackingVector, startIndex, count + 1)
  
      member this.Count = count
      member this.Item index = 
        if index >= 0 && index < count then
          backingVector |> Map.get (index + startIndex)
        else failwith "index out of range"
      member this.GetEnumerator () = 
        backingVector |> Seq.skip startIndex |> Seq.take count |> Seq.getEnumerator
  
      member this.GetEnumerator () = 
        (this :> IEnumerable<int*'v>).GetEnumerator() :> IEnumerator
  
      member this.Pop () = 
        if count = 0 then
          failwith "Can't pop empty vector"
        else SubPersistentVector.Create(backingVector, startIndex, count - 1)
  
      member this.TryGet index = 
        if index >= 0 && index < count then
          backingVector |> Map.tryGet (index + startIndex)
        else None
  
      member this.Update(index, value) =
        if index >= 0 && index < count then
          let newBackingVector = backingVector.Update (index, value)
  
          if Object.ReferenceEquals(backingVector, newBackingVector) then 
            (this :> IPersistentVector<'v>)
          else SubPersistentVector.Create(newBackingVector, startIndex, count)
        else failwith "index out of range"
  
  let createWithComparer (comparer: IEqualityComparer<'v>) = 
    let backingVector = PersistentVectorImpl.create comparer
    HashedTrieBackedPersistentVector.Create backingVector
      
  let create () =
    createWithComparer EqualityComparer.Default

  let add (v: 'v) (vec: IPersistentVector<'v>) = vec.Add v

  let get (index: int) (vec: IPersistentVector<'v>) = vec.Item index

  let pop (vec: IPersistentVector<'v>) = vec.Pop ()

  let tryGet (index: int) (vec: IPersistentVector<'v>) = vec.TryGet index

  let update (index: int) (v: 'v) (vec: IPersistentVector<'v>): IPersistentVector<'v> = 
    vec.Update (index, v)

  let sub (startIndex: int) (count: int) (vec: IPersistentVector<'v>) : IPersistentVector<'v> =
    SubPersistentVector.Create (vec, startIndex, count)