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
    count: int
    root: Option<TrieNode<'v>>
    tail: ValuesNode<'v>
  }

  let bits = 5
  let width = 1 <<< bits
  let private mask = width - 1

  let computeIndex depth index =
    let level = depth * bits
    let ret = (index >>> level) &&& mask
    ret

  let create (owner: obj) = {
    count = 0
    root = None
    tail = 
      {
        owner = owner
        values = Array.empty
      }
  }

  let getTailOffset vec =
    if vec.count < width then 0
    else ((vec.count - 1) >>> bits) <<< bits

  let pushTail 
      owner 
      (levelNodeMutator: int -> TrieNode<'v> -> LevelNode<'v> -> LevelNode<'v>)
      (vec: HashedTriePersistentVector<'v>) : TrieNode<'v> =
    let tail = vec.tail
    if tail.values.Length <> width then failwith "tail wrong size"

    match vec.root with
    | None -> ValuesNode tail
    | Some node ->
        let rec pushTailIntoTrie tail = function
          | ValuesNode valuesNode as node -> LeafNode {
                owner = owner
                valueNodes = [| valuesNode; tail |]
              }
          | LeafNode leafNode as node->
              if leafNode.valueNodes.Length < width then
                LeafNode {
                  owner = owner
                  valueNodes = leafNode.valueNodes |> Array.add tail
                }
              else
                LevelNode {
                  depth = 2
                  owner = owner
                  nodes = [| node; ValuesNode tail |]
                }
          | LevelNode levelNode as node ->
              let index = computeIndex levelNode.depth (vec.count - 1)

              // node is full
              if levelNode.nodes.Length = width && index < (width - 1) then
                LevelNode {
                  depth = levelNode.depth + 1
                  owner = owner
                  nodes = [| node; ValuesNode tail |]
                }
              else

              match levelNode.nodes |> Array.tryItem index with
              | Some childAtIndex ->
                  let newChildAtIndex = childAtIndex |> pushTailIntoTrie tail
                  LevelNode (levelNode |> levelNodeMutator index newChildAtIndex)
              | None ->
                  let newLevelNode = {
                    depth = levelNode.depth
                    owner = owner
                    nodes = levelNode.nodes |> Array.add (ValuesNode tail)
                  }

                  LevelNode newLevelNode

        node|> pushTailIntoTrie tail

  let private addWithMutator owner levelNodeMutator (v: 'v) (vec: HashedTriePersistentVector<'v>) =
    if vec.tail.values.Length < width then {
        vec with
          count = vec.count + 1
          tail = 
            { 
              owner = owner
              values = vec.tail.values |> Array.add v
            }
      }
    else {
        vec with
          count = vec.count + 1
          root = vec |> pushTail owner levelNodeMutator |> Some
          tail = 
            {
              owner = owner
              values = [| v |]
            }
      }
  
  let private immutableLevelNodeMutator index newChildAtIndex levelNode =
    {
      depth = levelNode.depth
      owner = Unchecked.defaultof<obj>
      nodes = levelNode.nodes |> Array.cloneAndSet index newChildAtIndex
    }

  let private immutableValuesNodeMutator index v valuesNode =
    {
      owner = Unchecked.defaultof<obj>
      values = valuesNode.values |> Array.cloneAndSet index v
    }

  let private immutableLeafNodeMutator index v leafNode = {
    owner = Unchecked.defaultof<obj>
    valueNodes = leafNode.valueNodes |> Array.cloneAndSet index v
  }

  let add (v: 'v) (vec: HashedTriePersistentVector<'v>) =
    vec |> addWithMutator Unchecked.defaultof<obj> immutableLevelNodeMutator v

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

    yield! vec.tail.values
  }

  let rec private findValuesNode index = function
      | ValuesNode valuesNode -> Some valuesNode
      | LeafNode leafNode ->
          let nodeIndex = computeIndex 1 index
          match leafNode.valueNodes |> Array.tryItem nodeIndex with
          | Some valuesNode -> Some valuesNode
          | _ -> None
      | LevelNode trieNode ->
          let nodeIndex = computeIndex trieNode.depth index
          match trieNode.nodes |> Array.tryItem nodeIndex with
          | Some node -> node |> findValuesNode index
          | _ -> None

  let valuesFor index vec =
    let tailOffset = getTailOffset vec

    if index < 0 || index >= vec.count then
      None
    elif index >= tailOffset then
      Some vec.tail
    else
      match vec.root with
      | Some node ->
          node |> findValuesNode index
      | None -> None

  let tryGet (index: int) (vec: HashedTriePersistentVector<'v>) =
    match vec |> valuesFor index with
    | Some node ->
        let index = computeIndex 0 index
        node.values |> Array.tryItem index
    | _ -> None

  let get (index: int) (vec: HashedTriePersistentVector<'v>) =
    match vec |> tryGet index with
    | Some v -> v
    | _ -> failwith "index out of bounds"

  let updateWithMutator
      valuesNodeMutator
      leafNodeMutator
      levelNodeMutator
      (index: int) 
      (v: 'v) 
      (vec: HashedTriePersistentVector<'v>): HashedTriePersistentVector<'v> =
    if index >= vec.count || index < 0 then
      failwith "index out of range"
    elif index >= (getTailOffset vec) then
      let nodeIndex = computeIndex 0 index
      let currentValue = vec.tail.values.[nodeIndex]

      if currentValue = v then
        vec
      else
        let newTail = vec.tail |> valuesNodeMutator nodeIndex v
        { vec with tail = newTail }
    else
      let rec doUpdate node =
        match node with
        | ValuesNode valuesNode ->
            let index = computeIndex 0 index
            let currentValue = valuesNode.values.[index]

            if currentValue = v then
              node
            else ValuesNode (valuesNode|> valuesNodeMutator index v)
        | LeafNode leafNode ->
            let leafIndex = computeIndex 1 index
            let valuesNode = leafNode.valueNodes.[leafIndex]

            let valueIndex = computeIndex 0 index
            let currentValue = valuesNode.values.[valueIndex]

            if currentValue = v then
              node
            else
              let newValuesNode = valuesNode |> valuesNodeMutator valueIndex v

              LeafNode (leafNode |> leafNodeMutator leafIndex newValuesNode)
        | LevelNode levelNode ->
            let index = computeIndex levelNode.depth index
            let childAtIndex = levelNode.nodes.[index]
            let newChildAtIndex = childAtIndex |> doUpdate

            if Object.ReferenceEquals(childAtIndex, newChildAtIndex) then
              node
            else LevelNode (levelNode |> levelNodeMutator index newChildAtIndex)

      let currentRoot = vec.root |> Option.get
      let newRoot = currentRoot |> doUpdate
      if Object.ReferenceEquals(currentRoot, newRoot) then
        vec
      else
        { vec with root = Some newRoot }
  
  let update (index: int) (v: 'v) (vec: HashedTriePersistentVector<'v>): HashedTriePersistentVector<'v> =
    updateWithMutator
      immutableValuesNodeMutator
      immutableLeafNodeMutator
      immutableLevelNodeMutator
      index
      v
      vec

  let rec popTail (owner: obj) levelNodeMutator = function
    | ValuesNode valuesNode -> None
    | LeafNode leafNode when leafNode.valueNodes.Length > 2 ->
        LeafNode {
          owner = owner
          valueNodes = leafNode.valueNodes |> Array.pop
        } |> Some
    | LeafNode leafNode ->
        ValuesNode leafNode.valueNodes.[0] |> Some
    | LevelNode levelNode ->
        let levelIndex = levelNode.nodes.Length - 1

        match levelNode.nodes.[levelIndex]|> popTail owner levelNodeMutator with
        | Some newNodeAtIndex ->
            Some <| LevelNode (levelNode |> levelNodeMutator levelIndex newNodeAtIndex)
        | None when levelNode.nodes.Length > 2 ->
            LevelNode {
              depth = levelNode.depth
              owner = owner
              nodes = levelNode.nodes |> Array.pop
            } |> Some
        | None -> levelNode.nodes.[0] |> Some  

  let pop (vec: HashedTriePersistentVector<'v>) =
    if vec.count = 0 then
      failwith "Can't pop empty vector"
    elif vec.count = 1 then
        create Unchecked.defaultof<obj>
    elif vec.tail.values.Length > 1 then
      { vec with
          count = vec.count - 1
          tail =
            { 
              owner = Unchecked.defaultof<obj>
              values = vec.tail.values |> Array.pop
            }
      }
    else 
      let currentRoot = vec.root |> Option.get

      let newRoot = 
        currentRoot |> popTail Unchecked.defaultof<obj> immutableLevelNodeMutator

      let index = vec.count - 2
  
      { vec with
          count = vec.count - 1
          root = newRoot
          tail = vec |> valuesFor index |> Option.get
      }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module TransientVector =
  let add v (vec: ITransientVector<'v>) =
    vec.Add v

  let persist (vec: ITransientVector<'v>) =
    vec.Persist()

  let pop (vec: ITransientVector<'v>) =
    vec.Pop()

  let update index value (vec: ITransientVector<'v>) =
    vec.Update(index, value)

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module PersistentVector =
  open PersistentVectorImpl

  let mutate (vec: IPersistentVector<'v>) =
    vec.Mutate ()

  let rec private createTransient (vec: HashedTriePersistentVector<'v>) =
    // A unique object use to identify this transient vector as the owner of a node.
    let owner = new Object()

    let mutable editable = true

    let mutable vec = 
      {
        count = vec.count
        root = vec.root
        tail = 
          {
            owner = owner
            values = 
              let newTailValues = Array.zeroCreate width
              vec.tail.values |> Array.copyTo newTailValues
              newTailValues
          }
      }

    let ensureEditable () =
      if not editable then 
        failwith "TransientVector has already been persisted"
    
    let ensureLeafNodeEditable (leafNode: LeafNode<'v>) =
      if Object.ReferenceEquals(leafNode.owner, owner) then leafNode
      else {
        owner = owner
        valueNodes = leafNode.valueNodes |> Array.copy
      }

    let ensureLevelNodeEditable (levelNode: LevelNode<'v>) =
      if Object.ReferenceEquals(levelNode.owner, owner) then levelNode
      else {
        depth = levelNode.depth
        owner = owner
        nodes = levelNode.nodes |> Array.copy
      }

    let ensureValuesNodeEditable (valuesNode: ValuesNode<'v>) =
      if Object.ReferenceEquals(valuesNode.owner, owner) then valuesNode
      else {
        owner = owner
        values = valuesNode.values |> Array.copy
      }

    let transientLeafNodeMutator index values leafNode = 
      let leafNode = ensureLeafNodeEditable leafNode
      leafNode.valueNodes.[index] <- values
      leafNode

    let transientLevelNodeMutator index value levelNode =
      let levelNode = ensureLevelNodeEditable levelNode
      levelNode.nodes.[index] <- value
      levelNode

    let transientValuesNodeMutator index value valuesNode =
      let valuesNode = ensureValuesNodeEditable valuesNode
      valuesNode.values.[index] <- value
      valuesNode

    { new ITransientVector<'v>  with
        member this.Add v =
          ensureEditable () 

          let tailOffset = getTailOffset vec

          if (vec.count - tailOffset) < width then 
            let tail = ensureValuesNodeEditable vec.tail
            tail.values.[vec.count - tailOffset] <- v

            vec <- {
              vec with
                count = vec.count + 1
                tail = tail
            }
          else
            vec <- {
              vec with
                count = vec.count + 1
                root = vec |> pushTail owner transientLevelNodeMutator |> Some
                tail = 
                  {
                    owner = owner
                    values = 
                      let newTailValues = Array.zeroCreate width
                      vec.tail.values.[0] <- v
                      newTailValues
                  }
              }
          this

        member this.Persist () =
          ensureEditable () 
          editable <- false

          let backingVector = 
            { vec with
                tail = 
                  { 
                    owner = owner
                    values = 
                      let trimmedTail = Array.zeroCreate (vec.count - getTailOffset vec)
                      Array.Copy (vec.tail.values, 0, trimmedTail, 0, trimmedTail.Length)
                      trimmedTail 
                }
          }

          createInternal backingVector

        member this.Pop () =
          ensureEditable () 

          let tailOffset = getTailOffset vec

          if vec.count = 0 then
            failwith "Can't pop empty vector"
          elif vec.count = 1 then
            vec <- create owner
          elif vec.count - tailOffset > 1 then
            let count = vec.count
            let tail = ensureValuesNodeEditable vec.tail
            tail.values.[count - 1] <- Unchecked.defaultof<'v>

            vec <- 
              { vec with
                  count = vec.count - 1
                  tail = tail
              }
          else 
            let currentRoot = vec.root |> Option.get
      
            let newRoot = 
              currentRoot |> popTail owner transientLevelNodeMutator
      
            let index = vec.count - 2
        
            vec <- 
              { vec with
                  count = vec.count - 1
                  root = newRoot
                  tail = vec |> valuesFor index |> Option.get
              }

          this

        member this.Update (index, value) =
          ensureEditable () 
          vec <- 
            updateWithMutator
              transientValuesNodeMutator
              transientLeafNodeMutator
              transientLevelNodeMutator
              index
              value
              vec
          this
    }

  and private createInternal (backingVector: HashedTriePersistentVector<'v>) =
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
        override this.Mutate () = createTransient backingVector
        override this.Pop () =
          let newBackingVector = backingVector |> pop
          createInternal newBackingVector
        override this.TryItem index = backingVector |> tryGet index
        override this.Update(index, value) =
          let newBackingVector = backingVector |> update index value
          if Object.ReferenceEquals(backingVector, newBackingVector) then (this :> IPersistentVector<'v>)
          else createInternal newBackingVector
    }) :> IPersistentVector<'v>

  and empty () =
    let backingVector = PersistentVectorImpl.create Unchecked.defaultof<obj>
    createInternal backingVector

  let append (values: seq<'v>) (vec: IPersistentVector<'v>) =
    values
    |> Seq.fold (fun (acc: ITransientVector<'v>) i -> acc.Add i) (vec |> mutate)
    |> TransientVector.persist

  let create (values: seq<'v>) =
    empty () |> append values

  let add (v: 'v) (vec: IPersistentVector<'v>) = vec.Add v
   
  let get (index: int) (vec: IPersistentVector<'v>) = vec.Item index

  let pop (vec: IPersistentVector<'v>) = vec.Pop ()

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
          override this.Add v =
            let index = startIndex + count
            let newBackingVector =
              if index < backingVector.Count then backingVector.Update (index, v)
              else backingVector.Add v
            newBackingVector |> sub startIndex (count + 1)

          override this.Count = count
          override this.Item index =
            if index >= 0 && index < count then
              backingVector |> ImmutableMap.get (index + startIndex)
            else failwith "index out of range"
          override this.GetEnumerator () =
            backingVector |> Seq.skip startIndex |> Seq.take count |> Seq.getEnumerator

          override this.Mutate () = failwith "implement me"

          override this.Pop () =
            if count = 0 then
              failwith "Can't pop empty vector"
            else backingVector |> sub startIndex (count - 1)

          override this.TryItem index =
            if index >= 0 && index < count then
              backingVector |> ImmutableMap.tryGet (index + startIndex)
            else None

          override this.Update(index, value) =
            if index >= 0 && index < count then
              let newBackingVector = backingVector.Update (index, value)

              if Object.ReferenceEquals(backingVector, newBackingVector) then
                (this :> IPersistentVector<'v>)
              else newBackingVector |> sub startIndex count
            else failwith "index out of range"
      }) :> IPersistentVector<'v>

  let tryGet (index: int) (vec: IPersistentVector<'v>) = vec.TryItem index

  let update (index: int) (v: 'v) (vec: IPersistentVector<'v>): IPersistentVector<'v> =
    vec.Update (index, v)
