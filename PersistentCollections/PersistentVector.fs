namespace PersistentCollections

// http://hypirion.com/musings/understanding-persistent-vector-pt-1

type PersistentVectorTrieNodeDepth = int

type [<ReferenceEquality>] PersistentVectorTrie<'v> = 
  | PersistentVectorLeafNode of IVector<IVector<'v>>
  | PersistentVectorTrieNode of PersistentVectorTrieNode<'v>

and PersistentVectorTrieNode<'v> = {
  nodes: IVector<PersistentVectorTrie<'v>>
  depth: int
}

type [<ReferenceEquality>] PersistentVector<'v> = {
  count: int
  root: PersistentVectorTrie<'v>
  tail: IVector<'v>
}

module PersistentVector =
  let private bits = 5
  let private width = 1 <<< bits
  let private mask = width - 1

  let private minDepth = 1

  let empty = {
    count = 0
    root = PersistentVectorLeafNode Vector.empty
    tail = Vector.empty
  }

  let rec private newPath depth (tail: IVector<'v>) =
    if depth = minDepth then 
      PersistentVectorLeafNode (Vector.createUnsafe [| tail |])
    else 
      let child = newPath (depth - 1) tail
      PersistentVectorTrieNode {
        nodes = (Vector.createUnsafe [| child |])
        depth = depth
      }

  let private computeIndex depth index =
    let level = depth * bits
    let ret = (index >>> level) &&& mask 
    ret

  let getTailOffset vec = vec.count - vec.tail.Count

  let private pushTail (vec: PersistentVector<'v>) : PersistentVectorTrie<'v> =
    let tail = vec.tail
    if tail.Count <> width then failwith "tail wrong size"

    let depth = 
      match vec.root with
      | PersistentVectorLeafNode _ -> minDepth
      | PersistentVectorTrieNode trieNode -> trieNode.depth

    match vec.root with
    | PersistentVectorLeafNode leafNode ->
        if leafNode.Count < width then 
          PersistentVectorLeafNode (leafNode |> Vector.add tail)
        else 
          let newLeafNode = PersistentVectorLeafNode (Vector.createUnsafe [| tail |])
          PersistentVectorTrieNode {
            nodes = (Vector.createUnsafe [| vec.root; newLeafNode |])
            depth = depth + 1
          }
    | PersistentVectorTrieNode trie -> 
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
          PersistentVectorTrieNode {
            nodes = Vector.createUnsafe [| vec.root; (newPath depth tail) |]
            depth = depth + 1
          }
        else pushTail depth trie.nodes

  let add (v: 'v) (vec: PersistentVector<'v>) =
    if vec.tail.Count < width then {
        count = vec.count + 1
        root = vec.root
        tail = vec.tail |> Vector.add v
      }
    else {
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

    yield! toSeq vec.root
    yield! vec.tail |> Collection.values
  }

  let private leafNodeFor index vec =
    let tailOffset = getTailOffset vec

    let rec findLeafNode = function
      | PersistentVectorLeafNode leafNode -> 
          let nodeIndex = computeIndex minDepth index
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
      findLeafNode vec.root

  let tryGet (index: int) (vec: PersistentVector<'v>) =
    let v = vec |> leafNodeFor index 
    match vec |> leafNodeFor index with
    | Some vec -> 
        let index = index &&& mask
        vec |> Collection.tryGet index
    | _ -> None

  let get (index: int) (vec: PersistentVector<'v>) =
    match vec |> tryGet index with
    | Some v -> v
    | _ -> failwith "index out of bounds"

  let rec private doUpdate index v = function 
    | PersistentVectorLeafNode leafNode -> 
        let leafIndex = computeIndex minDepth index
        let node = leafNode |> Collection.get leafIndex

        let nodeIndex = index &&& mask
        let newNode = node |> Vector.cloneAndSet nodeIndex v

        PersistentVectorLeafNode (leafNode |> Vector.cloneAndSet leafIndex  newNode)
    | PersistentVectorTrieNode trieNode -> 
        let pos = computeIndex trieNode.depth index
        let node = trieNode.nodes |> Collection.get pos |> doUpdate index v
        PersistentVectorTrieNode {
          depth = trieNode.depth
          nodes = (trieNode.nodes |> Vector.cloneAndSet pos node)
        }

  let update (index: int) (v: 'v) (vec: PersistentVector<'v>): PersistentVector<'v> =
    let tailOffset = getTailOffset vec

    if index > vec.count || index < 0 then
      failwith "index out of range"
    elif index = vec.count then
      vec |> add v
    elif index >= tailOffset then
      let nodeIndex = index &&& mask
      let newTail = vec.tail |> Vector.cloneAndSet nodeIndex v
      { vec with tail = newTail }
    else { vec with root = vec.root |> doUpdate index v }
(*
  let pop (vec: PersistentVector<'v>) =
    let tailOffset = getTailOffset vec

    if vec.count = 0 then
      failwith "Can't pop empty vector"
    elif vec.count = 1 then
      empty
    elif vec.count - tailOffset > 1 then
      let newTail = vec.tail |> Vector.pop
      { vec with tail = newTail }
    else 
      let newTail = vec |> leafNodeFor (vec.count - 2) |> Option.get

      let rec popTail = function
        | PersistentVectorLeafNode leafNode -> 
            if leafNode.Count > 1 then
              let newLeafNode = leafNode |> Vector.pop
              Some (PersistentVectorLeafNode newLeafNode)
            else None
        | PersistentVectorTrieNode trieNode -> 
            let subidx = computeIndex trieNode.depth (vec.count - 2)
            let child = trieNode.nodes |> Collection.get subidx

            match popTail child with
            | Some newChild -> Some (PersistentVectorTrieNode {
                nodes = trieNode.nodes |> Vector.cloneAndSet subidx newChild
                depth = trieNode.depth
              })
            | _ when subidx > 0 ->
               let child = trieNode.nodes |> Collection.get (subidx - 1)


            |_ -> None


      let newRoot = popTail vec.root

      {
        count = vec.count - 1
        root = newRoot
        tail = newTail
      }*)