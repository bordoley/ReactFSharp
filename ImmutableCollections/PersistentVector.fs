namespace ImmutableCollections

open System
open System.Collections
open System.Collections.Generic

// http://hypirion.com/musings/understanding-persistent-vector-pt-1

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module private PersistentVectorImpl =
  type [<ReferenceEquality>] TrieNode<'v> =
    | ValuesNode of ValuesNode<'v>
    | LeafNode of LeafNode<'v>
    | LevelNode of LevelNode<'v>

  and ValuesNode<'v> = {
    owner: obj
    values: array<'v>
  }

  and LeafNode<'v> = {
    owner: obj
    valueNodes: array<ValuesNode<'v>>
  }

  and LevelNode<'v> = {
    depth: int
    owner: obj
    nodes: array<TrieNode<'v>>
  }
  
  type [<ReferenceEquality>] HashedTriePersistentVector<'v> = {
    comparer: IEqualityComparer<'v>
    count: int
    owner: obj
    root: Option<TrieNode<'v>>
    tail: array<'v>
  }

  let private bits = 5
  let private width = 1 <<< bits
  let private mask = width - 1

  let private computeIndex depth index =
    let level = depth * bits
    let ret = (index >>> level) &&& mask
    ret

  let create (comparer: IEqualityComparer<'v>) = {
    comparer = comparer
    count = 0
    owner = Unchecked.defaultof<obj>
    root = None
    tail = Array.empty
  }

  let private getTailOffset vec = vec.count - vec.tail.Length

  let private pushTail (vec: HashedTriePersistentVector<'v>) : TrieNode<'v> =
    let tail = vec.tail
    if tail.Length <> width then failwith "tail wrong size"

    match vec.root with
    | None -> ValuesNode {
        owner = vec.owner
        values = tail
      }
    | Some node ->
        let rec pushTailIntoTrie = function
          | ValuesNode valuesNode as node -> LeafNode {
                owner = vec.owner
                valueNodes = [| valuesNode; { owner = vec.owner; values = tail } |]
              }
          | LeafNode leafNode as node->
              if leafNode.valueNodes.Length < width then
                LeafNode {
                  owner = vec.owner
                  valueNodes = leafNode.valueNodes |> Array.add { owner = vec.owner; values = tail }
                }
              else   
                LevelNode {
                  depth = 2
                  owner = vec.owner
                  nodes = [| node; ValuesNode { owner = vec.owner; values = tail } |]
                }  
          | LevelNode levelNode as node ->
              let index = computeIndex levelNode.depth (vec.count - 1)

              // node is full
              if levelNode.nodes.Length = width && index < (width - 1) then 
                LevelNode {
                  depth = levelNode.depth + 1
                  owner = vec.owner
                  nodes = [| node; ValuesNode { owner = vec.owner; values = tail } |]
                }
              else
            
              match levelNode.nodes |> Array.tryItem index with
              | Some childAtIndex ->
                  let newChildAtIndex = childAtIndex |> pushTailIntoTrie
                  LevelNode {
                    depth = levelNode.depth
                    owner = vec.owner
                    nodes = levelNode.nodes |> Array.cloneAndSet index newChildAtIndex
                  }
              | None ->
                  let newLevelNode = {
                    depth = levelNode.depth
                    owner = vec.owner
                    nodes = levelNode.nodes |> Array.add (ValuesNode { owner = vec.owner; values = tail })
                  }

                  LevelNode newLevelNode

        node|> pushTailIntoTrie

  let add (v: 'v) (vec: HashedTriePersistentVector<'v>) =
    if vec.tail.Length < width then {
        vec with
          count = vec.count + 1
          tail = vec.tail |> Array.add v
      }
    else {
        vec with
          count = vec.count + 1
          root = vec |> pushTail |> Some
          tail = [| v |]
      }

  let toSeq (vec: HashedTriePersistentVector<'v>) = seq {
    let rec toSeq = function
      | ValuesNode valuesNode -> valuesNode.values |> Seq.ofArray
      | LeafNode leafNode -> 
          leafNode.valueNodes |> Seq.map (fun n -> n.values) |> Seq.concat
      | LevelNode trieNode ->
          trieNode.nodes |> Seq.map toSeq |> Seq.concat

    match vec.root with
    | Some root ->
        yield! root |> toSeq
    | None -> ()

    yield! vec.tail
  }
 
  let private leafNodeFor index vec =
    let tailOffset = getTailOffset vec

    let rec findLeafNode = function
      | ValuesNode valuesNode -> Some valuesNode.values
      | LeafNode leafNode ->
          let nodeIndex = computeIndex 1 index
          match leafNode.valueNodes |> Array.tryItem nodeIndex with
          | Some valuesNode -> Some valuesNode.values
          | _ -> None
      | LevelNode trieNode ->
          let nodeIndex = computeIndex trieNode.depth index
          match trieNode.nodes |> Array.tryItem nodeIndex with
          | Some node -> node |> findLeafNode
          | _ -> None

    if index < 0 || index >= vec.count then
      None
    elif index >= tailOffset then
      Some vec.tail
    else
      match vec.root with
      | Some node -> node |> findLeafNode
      | None -> None

  let tryGet (index: int) (vec: HashedTriePersistentVector<'v>) =
    match vec |> leafNodeFor index with
    | Some node ->
        let index = computeIndex 0 index
        node |> Array.tryItem index
    | _ -> None

  let get (index: int) (vec: HashedTriePersistentVector<'v>) =
    match vec |> tryGet index with
    | Some v -> v
    | _ -> failwith "index out of bounds"

  let update (index: int) (v: 'v) (vec: HashedTriePersistentVector<'v>): HashedTriePersistentVector<'v> =
    if index >= vec.count || index < 0 then
      failwith "index out of range"
    elif index >= (getTailOffset vec) then
      let nodeIndex = computeIndex 0 index
      let currentValue = vec.tail.[nodeIndex]

      if vec.comparer.Equals(currentValue, v) then
        vec
      else
        let newTail = vec.tail |> Array.cloneAndSet nodeIndex v
        { vec with tail = newTail }
    else
      let rec doUpdate node = 
        match node with
        | ValuesNode valuesNode ->
            let index = computeIndex 0 index
            let currentValue = valuesNode.values.[index]

            if vec.comparer.Equals(currentValue, v) then
              node
            else ValuesNode {
              owner = vec.owner
              values = valuesNode.values |> Array.cloneAndSet index v
            }
        | LeafNode leafNode ->
            let leafIndex = computeIndex 1 index
            let valuesNode = leafNode.valueNodes.[leafIndex]

            let valueIndex = computeIndex 0 index
            let currentValue = valuesNode.values.[valueIndex]

            if vec.comparer.Equals(currentValue, v) then
              node
            else 
              let newValuesNode = {
                owner = vec.owner
                values = valuesNode.values |> Array.cloneAndSet valueIndex v
              }

              LeafNode {
                owner = vec.owner
                valueNodes = leafNode.valueNodes |> Array.cloneAndSet leafIndex newValuesNode
              }
        | LevelNode levelNode -> 
            let index = computeIndex levelNode.depth index
            let childAtIndex = levelNode.nodes.[index]
            let newChildAtIndex = childAtIndex |> doUpdate

            if Object.ReferenceEquals(childAtIndex, newChildAtIndex) then
              node
            else LevelNode {
              depth = levelNode.depth
              owner = vec.owner
              nodes = levelNode.nodes |> Array.cloneAndSet index newChildAtIndex
            }

      let currentRoot = vec.root |> Option.get
      let newRoot = currentRoot |> doUpdate
      if Object.ReferenceEquals(currentRoot, newRoot) then
        vec
      else
        { vec with root = Some newRoot }

  let pop (vec: HashedTriePersistentVector<'v>) =
    if vec.count = 0 then
      failwith "Can't pop empty vector"
    elif vec.count = 1 then
        create vec.comparer
    elif vec.tail.Length > 1 then
      let newTail = vec.tail |> Array.pop

      { vec with
          count = vec.count - 1
          tail = newTail
      }
    else
      let index = vec.count - 2
      let newTail = vec |> leafNodeFor index |> Option.get    

      let rec popTail = function
        | ValuesNode valuesNode -> None      
        | LeafNode leafNode when leafNode.valueNodes.Length > 2 ->
            LeafNode { owner = vec.owner; valueNodes = leafNode.valueNodes |> Array.pop } |> Some
        | LeafNode leafNode ->
            ValuesNode { owner = vec.owner; values = leafNode.valueNodes.[0].values } |> Some
        | LevelNode levelNode ->
            let levelIndex = levelNode.nodes.Length - 1

            match popTail levelNode.nodes.[levelIndex] with
            | Some newNodeAtIndex -> 
                LevelNode { 
                  depth = levelNode.depth; 
                  owner = vec.owner; 
                  nodes = levelNode.nodes |> Array.cloneAndSet levelIndex newNodeAtIndex
                } |> Some
            | None when levelNode.nodes.Length > 2 -> 
                LevelNode { 
                  depth = levelNode.depth; 
                  owner = vec.owner; 
                  nodes = levelNode.nodes |> Array.pop 
                } |> Some
            | None -> levelNode.nodes.[0] |> Some
    
      let currentRoot = vec.root |> Option.get
      let newRoot = currentRoot |> popTail

      { vec with
          owner = vec.owner
          count = vec.count - 1
          root = newRoot
          tail = newTail
      }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module PersistentVector =
  open PersistentVectorImpl

  let rec private createInternal (backingVector: HashedTriePersistentVector<'v>) =
    ({ new PersistentVectorBase<'v> () with
        override this.Add v = 
          let newBackingVector = backingVector |> add v
          if Object.ReferenceEquals(backingVector, newBackingVector) then (this :> IPersistentVector<'v>)
          else createInternal newBackingVector
        override this.Count = backingVector.count
        override this.Item index = backingVector |> get index
        override this.GetEnumerator () = 
          backingVector 
          |> toSeq 
          |> Seq.mapi (fun i v -> (i, v))
          |> Seq.getEnumerator
        override this.Pop () =
          let newBackingVector = backingVector |> pop
          createInternal newBackingVector
        override this.TryItem index = backingVector |> tryGet index
        override this.Update(index, value) =
          let newBackingVector = backingVector |> update index value
          if Object.ReferenceEquals(backingVector, newBackingVector) then (this :> IPersistentVector<'v>)
          else createInternal newBackingVector
    }) :> IPersistentVector<'v>

  let rec sub startIndex count (backingVector: IPersistentVector<'v>) =
    if startIndex < 0 || startIndex >= backingVector.Count then 
      failwith "startindex out of range"
    elif startIndex + count > backingVector.Count then 
      failwith "count out of range"
    elif startIndex = 0 && count = backingVector.Count then 
      backingVector
    else 
      // FIXME: Add some heuristics to determine if we should instead copy to a new vector
      ({ new PersistentVectorBase<'v> () with
          member this.Add v = 
            let index = startIndex + count
            let newBackingVector =
              if index < backingVector.Count then backingVector.Update (index, v)
              else backingVector.Add v
            newBackingVector |> sub startIndex (count + 1)
      
          member this.Count = count
          member this.Item index = 
            if index >= 0 && index < count then
              backingVector |> ImmutableMap.get (index + startIndex)
            else failwith "index out of range"
          member this.GetEnumerator () = 
            backingVector |> Seq.skip startIndex |> Seq.take count |> Seq.getEnumerator
     
          member this.Pop () = 
            if count = 0 then
              failwith "Can't pop empty vector"
            else backingVector |> sub startIndex (count - 1)
      
          member this.TryItem index = 
            if index >= 0 && index < count then
              backingVector |> ImmutableMap.tryGet (index + startIndex)
            else None
      
          member this.Update(index, value) =
            if index >= 0 && index < count then
              let newBackingVector = backingVector.Update (index, value)
      
              if Object.ReferenceEquals(backingVector, newBackingVector) then 
                (this :> IPersistentVector<'v>)
              else newBackingVector |> sub startIndex count
            else failwith "index out of range"
      }) :> IPersistentVector<'v>
  
  let emptyWithComparer (comparer: IEqualityComparer<'v>) = 
    let backingVector = PersistentVectorImpl.create comparer
    createInternal backingVector
      
  let empty () =
    emptyWithComparer EqualityComparer.Default

  let add (v: 'v) (vec: IPersistentVector<'v>) = vec.Add v

  let get (index: int) (vec: IPersistentVector<'v>) = vec.Item index

  let pop (vec: IPersistentVector<'v>) = vec.Pop ()

  let tryGet (index: int) (vec: IPersistentVector<'v>) = vec.TryItem index

  let update (index: int) (v: 'v) (vec: IPersistentVector<'v>): IPersistentVector<'v> = 
    vec.Update (index, v)
