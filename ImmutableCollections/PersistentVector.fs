﻿namespace ImmutableCollections

open System
open System.Collections.Generic
open System.Runtime.CompilerServices

// http://hypirion.com/musings/understanding-persistent-vector-pt-1

type [<ReferenceEquality>] PersistentVectorTrie<'v> =
  | PersistentVectorLeafNode of ImmutableArray<ImmutableArray<'v>>
  | PersistentVectorTrieNode of PersistentVectorTrieNode<'v>

and [<ReferenceEquality>] PersistentVectorTrieNode<'v> = {
  nodes: ImmutableArray<PersistentVectorTrie<'v>>
  depth: int
}

type [<ReferenceEquality>] PersistentVectorRootNode<'v> =
  | PersistentVectorTrieRootNode of PersistentVectorTrieNode<'v>
  | PersistentVectorLeafRootNode of ImmutableArray<ImmutableArray<'v>>
  | PersistentVectorValuesRootNode of ImmutableArray<'v>
  | PersistentVectorNoneRootNode

type [<ReferenceEquality>] PersistentVector<'v> = {
  comparer: IEqualityComparer<'v>
  count: int
  root: PersistentVectorRootNode<'v>
  tail: ImmutableArray<'v>
}

module PersistentVector =
  let private bits = 5
  let private width = 1 <<< bits
  let private mask = width - 1

  let private leafDepth = 1

  let private computeIndex depth index =
    let level = depth * bits
    let ret = (index >>> level) &&& mask
    ret

  let createWithComparer (comparer: IEqualityComparer<'v>) = {
    comparer = comparer
    count = 0
    root = PersistentVectorNoneRootNode
    tail = ImmutableArray.empty ()
  }

  let create () =
    createWithComparer EqualityComparer.Default

  let rec private newPath depth (tail: ImmutableArray<'v>) =
    if depth = leafDepth then
      PersistentVectorLeafNode (ImmutableArray.createUnsafe [| tail |])
    else
      let child = newPath (depth - 1) tail
      PersistentVectorTrieNode {
        nodes = (ImmutableArray.createUnsafe [| child |])
        depth = depth
      }

  let private getTailOffset vec = vec.count - (vec.tail |> Collection.count)

  let private getDepthFromRoot = function
    | PersistentVectorTrieRootNode trie -> trie.depth
    | PersistentVectorLeafRootNode _ -> leafDepth
    | PersistentVectorValuesRootNode _ -> 0
    | PersistentVectorNoneRootNode -> 0

  let private pushTail (vec: PersistentVector<'v>) : PersistentVectorRootNode<'v> =
    let tail = vec.tail
    if (tail |> Collection.count) <> width then failwith "tail wrong size"

    let depth = getDepthFromRoot vec.root

    match vec.root with
    | PersistentVectorNoneRootNode ->
        PersistentVectorValuesRootNode tail

    | PersistentVectorValuesRootNode values ->
        PersistentVectorLeafRootNode (ImmutableArray.createUnsafe [| values; tail |])

    | PersistentVectorLeafRootNode leafNode ->
        if (leafNode |> Collection.count) < width then
          PersistentVectorLeafRootNode (leafNode |> ImmutableArray.add tail)
        else
          let oldLeafNode = PersistentVectorLeafNode leafNode
          let newLeafNode = PersistentVectorLeafNode (ImmutableArray.createUnsafe [| tail |])

          PersistentVectorTrieRootNode {
            nodes = (ImmutableArray.createUnsafe [| oldLeafNode; newLeafNode |])
            depth = depth + 1
          }

    | PersistentVectorTrieRootNode trie ->
        let rec pushTail depth (trie: ImmutableArray<PersistentVectorTrie<'v>>) =
          let index = computeIndex depth (vec.count - 1)

          match trie |> Collection.tryGet index with
          | Some (PersistentVectorLeafNode leafNode) when (leafNode |> Collection.count) < width ->
              let newLeafNode = PersistentVectorLeafNode (leafNode |> ImmutableArray.add tail)
              PersistentVectorTrieNode {
                nodes = trie |> ImmutableArray.cloneAndSet index newLeafNode
                depth = depth
              }

          | None when (trie |> Collection.count) < width ->
              let newNode = newPath (depth - 1) tail
              PersistentVectorTrieNode {
                nodes = trie |> ImmutableArray.add newNode
                depth = depth
              }

          | Some (PersistentVectorTrieNode trieNode) ->
              let newTrieNode = pushTail (depth - 1) trieNode.nodes
              PersistentVectorTrieNode {
                nodes = trie |> ImmutableArray.cloneAndSet index newTrieNode
                depth = depth
              }

          | _ -> failwith "node is full"

        let shift = depth * bits

        if ((vec.count - 1) >>> bits) >= (1 <<< shift) then
          let oldRootNode = PersistentVectorTrieNode trie
          PersistentVectorTrieRootNode {
            nodes = ImmutableArray.createUnsafe [| oldRootNode; (newPath depth tail) |]
            depth = depth + 1
          }
        else
          match pushTail depth trie.nodes with
          | PersistentVectorLeafNode leafNode ->
              failwith "how can this happen?"
              PersistentVectorLeafRootNode leafNode
          | PersistentVectorTrieNode trieNode ->
              PersistentVectorTrieRootNode trieNode

  let add (v: 'v) (vec: PersistentVector<'v>) =
    if (vec.tail |> Collection.count) < width then {
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

  let toSeq (vec: PersistentVector<'v>) = seq {
    let rec toSeq = function
      | PersistentVectorLeafNode leafNode ->
          leafNode |> Collection.values |> Seq.map Collection.values |> Seq.concat
      | PersistentVectorTrieNode trieNode ->
          trieNode.nodes |> Collection.values |> Seq.map toSeq |> Seq.concat

    match vec.root with
    | PersistentVectorTrieRootNode trieNode ->
        yield! (PersistentVectorTrieNode trieNode) |> toSeq
    | PersistentVectorLeafRootNode leafNode ->
        yield! (PersistentVectorLeafNode leafNode) |> toSeq
    | PersistentVectorValuesRootNode values ->
        yield! values |> Collection.values
    | PersistentVectorNoneRootNode -> ()

    yield! vec.tail |> Collection.values
  }

  let private leafNodeFor index vec =
    let tailOffset = getTailOffset vec

    let rec findLeafNode = function
      | PersistentVectorLeafNode leafNode ->
          let nodeIndex = computeIndex leafDepth index
          leafNode |> Collection.tryGet nodeIndex
      | PersistentVectorTrieNode trieNode ->
          let nodeIndex = computeIndex trieNode.depth index
          match trieNode.nodes |> Collection.tryGet nodeIndex with
          | Some node -> findLeafNode node
          | _ -> None

    if index < 0 || index >= vec.count then
      None
    elif index >= tailOffset then
      Some vec.tail
    else
      match vec.root with
      | PersistentVectorTrieRootNode trieNode ->
          (PersistentVectorTrieNode trieNode) |> findLeafNode
      | PersistentVectorLeafRootNode leafNode ->
          (PersistentVectorLeafNode leafNode) |> findLeafNode
      | PersistentVectorValuesRootNode values ->
          Some values
      | PersistentVectorNoneRootNode ->
          None

  let tryGet (index: int) (vec: PersistentVector<'v>) =
    let v = vec |> leafNodeFor index
    match vec |> leafNodeFor index with
    | Some vec ->
        let index = computeIndex 0 index
        vec |> Collection.tryGet index
    | _ -> None

  let get (index: int) (vec: PersistentVector<'v>) =
    match vec |> tryGet index with
    | Some v -> v
    | _ -> failwith "index out of bounds"

  let private doUpdateValuesNode (comparer: IEqualityComparer<'v>) index v (valuesNode: ImmutableArray<'v>) =
    let nodeIndex = computeIndex 0 index
    let currentValue = valuesNode |> Collection.get nodeIndex

    if comparer.Equals(currentValue, v) then
      valuesNode
    else
      valuesNode |> ImmutableArray.cloneAndSet nodeIndex v

  let private doUpdateLeafNode (comparer: IEqualityComparer<'v>) index v (leafNode: ImmutableArray<ImmutableArray<'v>>) =
    let leafIndex = computeIndex leafDepth index
    let valuesNode = leafNode |> Collection.get leafIndex
    let newValuesNode = valuesNode |> doUpdateValuesNode comparer index v

    if Object.ReferenceEquals(valuesNode, newValuesNode) then
      leafNode
    else
      leafNode |> ImmutableArray.cloneAndSet leafIndex newValuesNode

  let rec private doUpdateTrieNode (comparer: IEqualityComparer<'v>) index v (trieNode: PersistentVectorTrieNode<'v>) =
    let pos = computeIndex trieNode.depth index

    let currentChildeNodeAtPos = trieNode.nodes |> Collection.get pos
    let newChildNodeAtPos =
      match currentChildeNodeAtPos  with
      | PersistentVectorLeafNode leafNode as currentTrieNode ->
          let newLeafNode = leafNode |> doUpdateLeafNode comparer index v

          if Object.ReferenceEquals(leafNode, newLeafNode) then
            currentTrieNode
          else
            PersistentVectorLeafNode newLeafNode

      | PersistentVectorTrieNode childTrieNode as currentTrieNode ->
          let newChildTrieNode = childTrieNode |> doUpdateTrieNode comparer index v

          if Object.ReferenceEquals(childTrieNode, newChildTrieNode) then
            currentTrieNode
          else
            PersistentVectorTrieNode newChildTrieNode

    if Object.ReferenceEquals(currentChildeNodeAtPos, newChildNodeAtPos) then
      trieNode
    else
      {
        depth = trieNode.depth
        nodes = (trieNode.nodes |> ImmutableArray.cloneAndSet pos newChildNodeAtPos)
      }

  let update (index: int) (v: 'v) (vec: PersistentVector<'v>): PersistentVector<'v> =
    if index > vec.count || index < 0 then
      failwith "index out of range"
    elif index = vec.count then
      vec |> add v
    elif index >= (getTailOffset vec) then
      let nodeIndex = computeIndex 0 index
      let currentValue = vec.tail |> Collection.get nodeIndex

      if vec.comparer.Equals(currentValue, v) then
        vec
      else
        let newTail = vec.tail |> ImmutableArray.cloneAndSet nodeIndex v
        { vec with tail = newTail }
    else
      let newRoot =
        match vec.root with
        | PersistentVectorTrieRootNode trieNode as currentRootNode ->
            let newTrieNode = trieNode |> doUpdateTrieNode vec.comparer index v

            if Object.ReferenceEquals(trieNode, newTrieNode) then
              currentRootNode
            else
              PersistentVectorTrieRootNode newTrieNode

        | PersistentVectorLeafRootNode leafNode as currentRootNode ->
            let newLeafNode = leafNode |> doUpdateLeafNode vec.comparer index v

            if Object.ReferenceEquals(leafNode, newLeafNode) then
              currentRootNode
            else
              PersistentVectorLeafRootNode newLeafNode

        | PersistentVectorValuesRootNode valuesNode as currentRootNode ->
            let newValuesNode = valuesNode |> doUpdateValuesNode vec.comparer index v

            if Object.ReferenceEquals(valuesNode, newValuesNode) then
              currentRootNode
            else
              PersistentVectorValuesRootNode newValuesNode

        | PersistentVectorNoneRootNode ->
            failwith "something went wrong"

      if Object.ReferenceEquals(vec.root, newRoot) then
        vec
      else
        { vec with root = newRoot }

  let private popTrieRootNodeTail (trieRootNode: PersistentVectorTrieNode<'v>) : PersistentVectorTrie<'v> =
    let rec doPop = function
      | PersistentVectorLeafNode leafNode when (leafNode |> Collection.count) > 1  ->
          let newLeafNode = leafNode |> ImmutableArray.pop
          Some (PersistentVectorLeafNode newLeafNode)
      | PersistentVectorTrieNode trieNode ->
          let childNode = trieNode.nodes |> Vector.last
          match doPop childNode with
          | None when (trieNode.nodes |> Collection.count) > 1 ->
              let newTrieNodeNodes = trieNode.nodes |> ImmutableArray.pop
              Some (PersistentVectorTrieNode {
                trieNode with
                  nodes = newTrieNodeNodes
              })
          | Some newChildNode ->
              let subidx = trieNode.nodes |> Vector.lastIndex
              let newTrieNodeNodes = trieNode.nodes |> ImmutableArray.cloneAndSet subidx newChildNode
              Some (PersistentVectorTrieNode {
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
        PersistentVectorTrieNode {
          trieRootNode with
            nodes = newTrieRootNodeNodes
        }
    | None when (trieRootNode.nodes |> Collection.count) > 2 ->
        let newTrieRootNodeNodes = trieRootNode.nodes |> ImmutableArray.pop
        PersistentVectorTrieNode {
          trieRootNode with
            nodes = newTrieRootNodeNodes
        }
    | None when (trieRootNode.nodes |> Collection.count) = 1 ->
        trieRootNode.nodes |> Collection.get 0
    | None -> failwith "should never happen"

  let pop (vec: PersistentVector<'v>) =
    if vec.count = 0 then
      failwith "Can't pop empty vector"
    elif vec.count = 1 then
        createWithComparer vec.comparer
    elif (vec.tail |> Collection.count) > 1 then
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
        | PersistentVectorTrieRootNode trieNode ->
            match trieNode |> popTrieRootNodeTail with
            | PersistentVectorLeafNode leafNode ->
                PersistentVectorLeafRootNode leafNode
            | PersistentVectorTrieNode trieRootNode ->
                PersistentVectorTrieRootNode trieNode

        | PersistentVectorLeafRootNode leafNode ->
            if ((leafNode |> Collection.count) > 2) then
              let newLeafNode = leafNode |> ImmutableArray.pop
              PersistentVectorLeafRootNode newLeafNode
            else
              PersistentVectorValuesRootNode (leafNode |> Collection.get 0)
        | PersistentVectorValuesRootNode valuesNode ->
            PersistentVectorNoneRootNode
        | PersistentVectorNoneRootNode ->
            failwith "Something went wrong"

      { vec with
          count = vec.count - 1
          root = newRoot
          tail = newTail
      }