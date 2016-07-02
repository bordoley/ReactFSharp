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
    root: Option<TrieNode<'v>>
    tail: array<'v>
  }

  let bits = 5
  let width = 1 <<< bits
  let private mask = width - 1

  let computeIndex depth index =
    let level = depth * bits
    let ret = (index >>> level) &&& mask
    ret

  let create (comparer: IEqualityComparer<'v>) = {
    comparer = comparer
    count = 0
    root = None
    tail = Array.empty
  }

  let private getTailOffset vec = vec.count - vec.tail.Length

  let private pushTail (vec: HashedTriePersistentVector<'v>) : TrieNode<'v> =
    let tail = vec.tail
    if tail.Length <> width then failwith "tail wrong size"

    match vec.root with
    | None -> ValuesNode {
        owner = Unchecked.defaultof<obj>
        values = tail
      }
    | Some node ->
        let rec pushTailIntoTrie tail = function
          | ValuesNode valuesNode as node -> LeafNode {
                owner = Unchecked.defaultof<obj>
                valueNodes = [| valuesNode; tail |]
              }
          | LeafNode leafNode as node->
              if leafNode.valueNodes.Length < width then
                LeafNode {
                  owner = Unchecked.defaultof<obj>
                  valueNodes = leafNode.valueNodes |> Array.add tail
                }
              else
                LevelNode {
                  depth = 2
                  owner = Unchecked.defaultof<obj>
                  nodes = [| node; ValuesNode tail |]
                }
          | LevelNode levelNode as node ->
              let index = computeIndex levelNode.depth (vec.count - 1)

              // node is full
              if levelNode.nodes.Length = width && index < (width - 1) then
                LevelNode {
                  depth = levelNode.depth + 1
                  owner = Unchecked.defaultof<obj>
                  nodes = [| node; ValuesNode tail |]
                }
              else

              match levelNode.nodes |> Array.tryItem index with
              | Some childAtIndex ->
                  let newChildAtIndex = childAtIndex |> pushTailIntoTrie tail
                  LevelNode {
                    depth = levelNode.depth
                    owner = Unchecked.defaultof<obj>
                    nodes = levelNode.nodes |> Array.cloneAndSet index newChildAtIndex
                  }
              | None ->
                  let newLevelNode = {
                    depth = levelNode.depth
                    owner = Unchecked.defaultof<obj>
                    nodes = levelNode.nodes |> Array.add (ValuesNode tail)
                  }

                  LevelNode newLevelNode

        node|> pushTailIntoTrie { owner = Unchecked.defaultof<obj>; values = tail }

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

  let rec findValuesNode index = function
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

  let private arrayFor index vec =
    let tailOffset = getTailOffset vec

    if index < 0 || index >= vec.count then
      None
    elif index >= tailOffset then
      Some vec.tail
    else
      match vec.root with
      | Some node ->
          node |> findValuesNode index |> Option.map (fun v -> v.values)
      | None -> None

  let tryGet (index: int) (vec: HashedTriePersistentVector<'v>) =
    match vec |> arrayFor index with
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
              owner = Unchecked.defaultof<obj>
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
                owner = Unchecked.defaultof<obj>
                values = valuesNode.values |> Array.cloneAndSet valueIndex v
              }

              LeafNode {
                owner = Unchecked.defaultof<obj>
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
              owner = Unchecked.defaultof<obj>
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
      let newTail = vec |> arrayFor index |> Option.get

      let rec popTail = function
        | ValuesNode valuesNode -> None
        | LeafNode leafNode when leafNode.valueNodes.Length > 2 ->
            LeafNode {
              owner = Unchecked.defaultof<obj>
              valueNodes = leafNode.valueNodes |> Array.pop
            } |> Some
        | LeafNode leafNode ->
            ValuesNode leafNode.valueNodes.[0] |> Some
        | LevelNode levelNode ->
            let levelIndex = levelNode.nodes.Length - 1

            match popTail levelNode.nodes.[levelIndex] with
            | Some newNodeAtIndex ->
                LevelNode {
                  depth = levelNode.depth
                  owner = Unchecked.defaultof<obj>
                  nodes = levelNode.nodes |> Array.cloneAndSet levelIndex newNodeAtIndex
                } |> Some
            | None when levelNode.nodes.Length > 2 ->
                LevelNode {
                  depth = levelNode.depth
                  owner = Unchecked.defaultof<obj>
                  nodes = levelNode.nodes |> Array.pop
                } |> Some
            | None -> levelNode.nodes.[0] |> Some

      let currentRoot = vec.root |> Option.get
      let newRoot = currentRoot |> popTail

      { vec with
          count = vec.count - 1
          root = newRoot
          tail = newTail
      }

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

  let rec private createTransient (backingVector: HashedTriePersistentVector<'v>) =
    // A unique object use to identify this transient vector as the owner of a node.
    let owner = new Object()

    let mutable editable = true

    let mutable count = backingVector.count

    let mutable root =
      match backingVector.root with
      | Some(ValuesNode valuesNode) -> Some <| ValuesNode {
            owner = owner
            values = valuesNode.values |> Array.copy
          }
      | Some(LeafNode leafNode) -> Some <| LeafNode {
            owner = owner
            valueNodes = leafNode.valueNodes |> Array.copy
          }
      | Some(LevelNode levelNode) -> Some <| LevelNode {
            depth = levelNode.depth
            owner = owner
            nodes = levelNode.nodes |> Array.copy
          }
      | None -> None

    let mutable tail =
      let newTail = Array.zeroCreate width
      backingVector.tail |> Array.copyTo newTail
      newTail

    let getTailOffset () =
      if count < width then 0
      else ((count - 1) >>> bits) <<< bits

    let ensureEditable node =
      match node with
      | ValuesNode valuesNode when valuesNode.owner = owner -> node
      | ValuesNode valuesNode -> ValuesNode {
            owner = owner
            values = valuesNode.values |> Array.copy
          }
      | LeafNode leafNode when leafNode.owner = owner -> node
      | LeafNode leafNode -> LeafNode {
            owner = owner
            valueNodes = leafNode.valueNodes |> Array.copy
          }
      | LevelNode levelNode when levelNode.owner = owner -> node
      | LevelNode levelNode -> LevelNode {
            depth = levelNode.depth
            owner = owner
            nodes = levelNode.nodes |> Array.copy
          }

    let rec doUpdate index value node =
      let node = ensureEditable node

      match node with
      | ValuesNode valuesNode ->
          let index = computeIndex 0 index
          valuesNode.values.[index] <- value
      | LeafNode leafNode ->
          let leafIndex = computeIndex 1 index
          let valuesNode = leafNode.valueNodes.[leafIndex]
          let valuesIndex = computeIndex 0 index
          valuesNode.values.[valuesIndex] <- value
      | LevelNode levelNode ->
          let nodeIndex = computeIndex levelNode.depth index
          let node = levelNode.nodes.[nodeIndex]
          levelNode.nodes.[nodeIndex] <- node |> doUpdate index value

      node

    let rec pushTail (tail: ValuesNode<'v>) node =
      match node with
      | ValuesNode valuesNode -> LeafNode {
            owner = owner
            valueNodes = [| valuesNode; tail |]
          }
      | LeafNode leafNode ->
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
      | LevelNode levelNode ->
          let index = computeIndex levelNode.depth (count - 1)

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
              let newChildAtIndex = childAtIndex |> pushTail tail
              levelNode.nodes.[index] <- newChildAtIndex

              node
          | None ->
              let newLevelNode = {
                levelNode with
                  nodes = levelNode.nodes |> Array.add (ValuesNode tail)
              }

              LevelNode newLevelNode

    let rec popTail node =
      let node = ensureEditable node

      match node with
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

          match popTail levelNode.nodes.[levelIndex] with
          | Some newNodeAtIndex ->
              levelNode.nodes.[levelIndex] <- newNodeAtIndex
              Some node
          | None when levelNode.nodes.Length > 2 ->
              LevelNode {
                depth = levelNode.depth
                owner = owner
                nodes = levelNode.nodes |> Array.pop
              } |> Some
          | None -> levelNode.nodes.[0] |> Some

    { new ITransientVector<'v>  with
        member this.Add v =
          if not editable then failwith "TransientVector has already been persisted"

          let tailOffset = getTailOffset ()

          if (count - tailOffset) < width then
            tail.[count - tailOffset] <- v
          else
            let tailNode = { owner = owner; values = tail }

            tail <- Array.zeroCreate width
            tail.[0] <- v

            let newRoot =
              match root with
              | None -> ValuesNode tailNode
              | Some node -> node |> pushTail tailNode

            root <- Some newRoot

          count <- count + 1

          this

        member this.Persist () =
          editable <- false

          let backingVector = {
            comparer = backingVector.comparer
            count = count
            root = root
            tail =
              let trimmedTail = Array.zeroCreate (count - getTailOffset ())
              Array.Copy (tail, 0, trimmedTail, 0, trimmedTail.Length)
              trimmedTail
          }

          createInternal backingVector

        member this.Pop () =
          if not editable then failwith "TransientVector has already been persisted"

          let tailOffset = getTailOffset ()

          if count = 0 then
            failwith "Can't pop empty vector"

          elif count > tailOffset then
            tail.[count - tailOffset - 1] <- Unchecked.defaultof<'v>

          else
            let rootNode = root |> Option.get
            let newTailNode = rootNode |> findValuesNode (count - 2) |> Option.get
            tail <- newTailNode.values

          this

        member this.Update (index, value) =
          if not editable then failwith "TransientVector has already been persisted"

          let tailOffset = getTailOffset ()

          if index >= count || index < 0 then
            failwith "index out of range"
          elif index >= tailOffset then
            tail.[index - tailOffset] <- value
          else
            let rootNode = root |> Option.get
            let newRootNode = rootNode |> doUpdate index value
            root <- Some newRootNode

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

  and emptyWithComparer (comparer: IEqualityComparer<'v>) =
    let backingVector = PersistentVectorImpl.create comparer
    createInternal backingVector

  and empty () =
    emptyWithComparer EqualityComparer.Default

  let createWithComparer (comparer: IEqualityComparer<'v>) (values: seq<'v>) =
    let empty = emptyWithComparer comparer |> mutate
    values
    |> Seq.fold (fun (acc: ITransientVector<'v>) i -> acc.Add i) empty
    |> TransientVector.persist

  let create (values: seq<'v>) =
    createWithComparer EqualityComparer.Default values

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

          member this.Mutate () = failwith "implement me"

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

  let tryGet (index: int) (vec: IPersistentVector<'v>) = vec.TryItem index

  let update (index: int) (v: 'v) (vec: IPersistentVector<'v>): IPersistentVector<'v> =
    vec.Update (index, v)
