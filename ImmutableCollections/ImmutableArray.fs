namespace ImmutableCollections

open System

type ImmutableArray<'v> internal (backingArray: array<'v>) =
  member this.CopyTo (sourceIndex: int, destinationArray: array<'v>, destinationIndex: int, length: int) =
    Array.Copy(backingArray, sourceIndex, destinationArray, destinationIndex, length)

  interface IImmutableVector<'v> with
    member this.Count = backingArray.Length
    member this.Item index = backingArray.[index]
    member this.GetEnumerator () = 
      backingArray |> Seq.mapi (fun i v -> (i, v)) |> Seq.getEnumerator
    member this.GetEnumerator () = backingArray.GetEnumerator()
    member this.TryItem index =
      if index >= 0 && index < backingArray.Length then
        Some backingArray.[index]
      else None

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ImmutableArray =
  let createUnsafe (backingArray: array<'v>) : ImmutableArray<'v> =
    new ImmutableArray<'v>(backingArray)

  let create (items: seq<'v>) : ImmutableArray<'v> =
    items |> Seq.toArray |> createUnsafe

  let empty () = new ImmutableArray<'v>([||])

  let copyTo target (arr: ImmutableArray<'v>) =
    arr.CopyTo (0, target, 0, Math.Min(target.Length, arr |> ImmutableCollection.count))

  let toArray (arr: ImmutableArray<'v>) =
    let newArray = Array.zeroCreate (arr |> ImmutableCollection.count)
    arr |> copyTo newArray
    newArray

  let add (v: 'v) (arr: ImmutableArray<'v>) =
    let oldSize = arr |> ImmutableCollection.count
    let newSize = oldSize + 1;

    let backingArray = Array.zeroCreate newSize
    arr |> copyTo backingArray
    backingArray.[oldSize] <- v

    createUnsafe backingArray

  let cloneAndSet (index: int) (item: 'v) (arr: ImmutableArray<'v>) =
    let size = (arr|> ImmutableCollection.count)

    let clone = arr |> toArray
    clone.[index] <- item

    createUnsafe clone

  let pop (arr: ImmutableArray<'v>) =
    let count = (arr|> ImmutableCollection.count)
    if count > 1 then
      let popped = Array.zeroCreate (count - 1)
      arr |> copyTo popped
      popped  |> createUnsafe
    elif count = 1 then
      empty ()
    else failwith "can not pop empty array"
