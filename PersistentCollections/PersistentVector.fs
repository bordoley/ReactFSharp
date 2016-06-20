namespace PersistentCollections

open System
open System.Collections.Generic
// http://hypirion.com/musings/understanding-persistent-vector-pt-1

type [<ReferenceEquality>] PersistentVectorTrie<'v> = 
  | PersistentVectorLeafNode of IVector<IVector<'v>>
  | PersistentVectorTrieNode of PersistentVectorTrieNode<'v>

and [<ReferenceEquality>] PersistentVectorTrieNode<'v> = {
  nodes: IVector<PersistentVectorTrie<'v>>
  depth: int
}

type [<ReferenceEquality>] PersistentVectorRootNode<'v> =
  | PersistentVectorTrieRootNode of PersistentVectorTrieNode<'v>
  | PersistentVectorLeafRootNode of IVector<IVector<'v>>
  | PersistentVectorValuesRootNode of IVector<'v>
  | PersistentVectorNoneRootNode

type [<ReferenceEquality>] PersistentVector<'v> = {
  comparer: IEqualityComparer<'v>
  count: int
  root: PersistentVectorRootNode<'v>
  tail: IVector<'v>
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

  let create comparer = {
    comparer = comparer
    count = 0
    root = PersistentVectorNoneRootNode
    tail = Vector.empty
  }

  let createWithDefaultEquality () =
    create EqualityComparer.Default

  let rec private newPath depth (tail: IVector<'v>) =
    if depth = leafDepth then 
      PersistentVectorLeafNode (Vector.createUnsafe [| tail |])
    else 
      let child = newPath (depth - 1) tail
      PersistentVectorTrieNode {
        nodes = (Vector.createUnsafe [| child |])
        depth = depth
      }

  let getTailOffset vec = vec.count - vec.tail.Count

  let getDepthFromRoot = function
    | PersistentVectorTrieRootNode trie -> trie.depth
    | PersistentVectorLeafRootNode _ -> leafDepth
    | PersistentVectorValuesRootNode _ -> 0
    | PersistentVectorNoneRootNode -> 0

  let private pushTail (vec: PersistentVector<'v>) : PersistentVectorRootNode<'v> =
    let tail = vec.tail
    if tail.Count <> width then failwith "tail wrong size"

    let depth = getDepthFromRoot vec.root

    match vec.root with
    | PersistentVectorNoneRootNode -> 
        PersistentVectorValuesRootNode tail

    | PersistentVectorValuesRootNode values -> 
        PersistentVectorLeafRootNode (Vector.createUnsafe [| values; tail |])

    | PersistentVectorLeafRootNode leafNode ->
        if leafNode.Count < width then 
          PersistentVectorLeafRootNode (leafNode |> Vector.add tail)
        else 
          let oldLeafNode = PersistentVectorLeafNode leafNode
          let newLeafNode = PersistentVectorLeafNode (Vector.createUnsafe [| tail |])
         
          PersistentVectorTrieRootNode {
            nodes = (Vector.createUnsafe [| oldLeafNode; newLeafNode |])
            depth = depth + 1
          }

    | PersistentVectorTrieRootNode trie -> 
        let rec pushTail depth (trie: IVector<PersistentVectorTrie<'v>>) = 
          let index = computeIndex depth (vec.count - 1)

          match trie |> Collection.tryGet index with
          | Some (PersistentVectorLeafNode leafNode) when leafNode.Count < width -> 
              let newLeafNode = PersistentVectorLeafNode (leafNode |> Vector.add tail)
              PersistentVectorTrieNode {
                nodes = trie |> Vector.cloneAndSet index newLeafNode
                depth = depth
              }

          | None when trie.Count < width -> 
              let newNode = newPath (depth - 1) tail
              PersistentVectorTrieNode {
                nodes = trie |> Vector.add newNode
                depth = depth
              }

          | Some (PersistentVectorTrieNode trieNode) -> 
              let newTrieNode = pushTail (depth - 1) trieNode.nodes
              PersistentVectorTrieNode {
                nodes = trie |> Vector.cloneAndSet index newTrieNode
                depth = depth
              }               

          | _ -> failwith "node is full"  
        
        let shift = depth * bits

        if ((vec.count - 1) >>> bits) >= (1 <<< shift) then
          let oldRootNode = PersistentVectorTrieNode trie
          PersistentVectorTrieRootNode {
            nodes = Vector.createUnsafe [| oldRootNode; (newPath depth tail) |]
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
    if vec.tail.Count < width then { 
        vec with
          count = vec.count + 1
          root = vec.root
          tail = vec.tail |> Vector.add v
      }
    else {
        vec with
          count = vec.count + 1
          root = pushTail vec 
          tail = Vector.createUnsafe [| v |]
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

  let private doUpdateValuesNode (comparer: IEqualityComparer<'v>) index v (valuesNode: IVector<'v>) =
    let nodeIndex = computeIndex 0 index
    let currentValue = valuesNode |> Collection.get nodeIndex

    if comparer.Equals(currentValue, v) then
      valuesNode
    else 
      valuesNode |> Vector.cloneAndSet nodeIndex v

  let private doUpdateLeafNode (comparer: IEqualityComparer<'v>) index v (leafNode: IVector<IVector<'v>>) =
    let leafIndex = computeIndex leafDepth index
    let valuesNode = leafNode |> Collection.get leafIndex
    let newValuesNode = valuesNode |> doUpdateValuesNode comparer index v

    if Object.ReferenceEquals(valuesNode, newValuesNode) then
      leafNode
    else 
      leafNode |> Vector.cloneAndSet leafIndex newValuesNode

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
        nodes = (trieNode.nodes |> Vector.cloneAndSet pos newChildNodeAtPos)
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
        let newTail = vec.tail |> Vector.cloneAndSet nodeIndex v
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

      (*
  let pop (vec: PersistentVector<'v>) =
    if vec.count = 0 then
      failwith "Can't pop empty vector"
    elif vec.count = 1 then
      empty
    elif vec.count - (getTailOffset vec) > 1 then
      let newTail = vec.tail |> Vector.pop
      { vec with 
          count = vec.count - 1
          tail = newTail
      }
    else 
      let index = vec.count - 2
      let newTail = vec |> leafNodeFor index  |> Option.get

      let rec popTrieNodeTail (trieNode: PersistentVectorTrieNode<'v>) : PersistentVectorTrieNode<'v>= 
        let subidx = trieNode.nodes.Count - 1
        let newChildNode = 
          match trieNode.nodes |> Collection.get subidx with
          | PersistentVectorLeafNode childLeafNode ->
              PersistentVectorLeafNode (childLeafNode |> Vector.pop)
              
          | PersistentVectorTrieNode childTrieNode -> 
              PersistentVectorTrieNode (popTrieNodeTail childTrieNode)
        {
          depth = trieNode.depth
          nodes = trieNode.nodes |> Vector.cloneAndSet subidx newChildNode
        } 

      let rec compress (node: PersistentVectorTrie<'v>) =
        match node with
        | PersistentVectorLeafNode leafNode ->
            if leafNode.Count > 1 && (leafNode |> Vector.last) |> Vector.isEmpty then 
              let leafNode = leafNode |> Vector.pop
              Some (Choice1Of3 leafNode)
            elif leafNode.Count > 1 then 
              Some (Choice1Of3 leafNode)
            elif leafNode.Count = 1 then 
              Some (Choice2Of3 (leafNode |> Collection.get 0))
            else
              None
        | PersistentVectorTrieNode trieNode ->
            if trieNode.nodes.Count > 1 then
              Some (Choice3Of3  trieNode)
            else 
              compress (trieNode.nodes |> Collection.get 0)

      let compressTrieNodeRoot (trieNode: PersistentVectorTrieNode<'v>) : PersistentVectorRootNode<'v> =
        if (trieNode.nodes.Count > 1) then 
          match compress (PersistentVectorTrieNode trieNode) with
          | Some (Choice1Of3 leafNode) -> PersistentVectorLeafRootNode leafNode
          | Some (Choice2Of3 valuesNode) -> PersistentVectorValuesRootNode valuesNode
          | Some (Choice3Of3  trieNode) -> PersistentVectorTrieRootNode trieNode
          | None -> PersistentVectorNoneRootNode
        else
          let childNode = trieNode.nodes |> Collection.get 0
          match compress childNode with
          | Some (Choice1Of3 leafNode) -> PersistentVectorLeafRootNode leafNode
          | Some (Choice2Of3 valuesNode) -> PersistentVectorValuesRootNode valuesNode
          | Some (Choice3Of3  trieNode) -> PersistentVectorTrieRootNode trieNode
          | None -> PersistentVectorNoneRootNode

      let newRoot = 
        match vec.root with
        | PersistentVectorTrieRootNode trieNode ->
            let newTrieNode = popTrieNodeTail trieNode
            compressTrieNodeRoot newTrieNode

        | PersistentVectorLeafRootNode leafNode -> 
            if (leafNode.Count > 2) then 
              let newLeafNode = leafNode |> Vector.pop
              PersistentVectorLeafRootNode newLeafNode
            else 
              PersistentVectorValuesRootNode (leafNode.Get 0)

        | PersistentVectorValuesRootNode valuesNode -> 
            PersistentVectorNoneRootNode        

        | PersistentVectorNoneRootNode ->
            failwith "Something went wrong" 

      {
        count = vec.count - 1
        root = newRoot
        tail = newTail
      }*)