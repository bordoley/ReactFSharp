namespace ImmutableCollections

open System

type ImmutableArray<'v> internal (backingArray: array<'v>) =
  member this.CopyTo (sourceIndex: int, destinationArray: array<'v>, destinationIndex: int, length: int) =
    Array.Copy(backingArray, sourceIndex, destinationArray, destinationIndex, length)

  interface IVector<'v> with
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
    arr.CopyTo (0, target, 0, Math.Min(target.Length, arr |> Map.count))

  let toArray (arr: ImmutableArray<'v>) =
    let newArray = Array.zeroCreate (arr |> Map.count)
    arr |> copyTo newArray
    newArray

  let add (v: 'v) (arr: ImmutableArray<'v>) =
    let oldSize = arr |> Map.count
    let newSize = oldSize + 1;

    let backingArray = Array.zeroCreate newSize
    arr |> copyTo backingArray
    backingArray.[oldSize] <- v

    createUnsafe backingArray

  let cloneAndSet (index: int) (item: 'v) (arr: ImmutableArray<'v>) =
    let size = (arr|> Map.count)

    let clone = arr |> toArray
    clone.[index] <- item

    createUnsafe clone

  let pop (arr: ImmutableArray<'v>) =
    let count = (arr|> Map.count)
    if count > 1 then
      let popped = Array.zeroCreate (count - 1)
      arr |> copyTo popped
      popped  |> createUnsafe
    elif count = 1 then
      empty ()
    else failwith "can not pop empty array"

  let sub (startIndex: int) (count: int) (arr: ImmutableArray<'v>) : IVector<'v> =
    if startIndex < 0 || startIndex >= (arr |> Map.count) then
      failwith "startIndex out of range"
    elif startIndex + count >= (arr |> Map.count) then
      failwith "count out of range"

    {
      new IVector<'v> with
        member this.Count = count
        member this.Item index =
          if index >= 0 && index < count then
            arr |> Map.get (index + startIndex)
          else failwith "index out of range"
        member this.GetEnumerator () =
          seq { 0 .. (count - 1) } |> Seq.map (fun i -> (i, this.Item i)) |> Seq.getEnumerator
        member this.GetEnumerator () =
          this.GetEnumerator() :> System.Collections.IEnumerator
        member this.TryItem index =
          if index >= 0 && index < count then
            arr |> Map.tryGet (index + startIndex)
          else None
    }

  let reverse (arr: ImmutableArray<'v>) : IVector<'v> = {
    new IVector<'v> with
      member this.Count = arr |> Map.count
      member this.Item index =
        if index >= 0 && index < this.Count then
          arr |> Map.get (this.Count - index - 1)
        else failwith "index out of range"
      member this.GetEnumerator () =
        seq { 0 .. (this.Count - 1) }
        |> Seq.map (fun i -> (i, this.Item (this.Count - i - 1)))
        |> Seq.getEnumerator
      member this.GetEnumerator () =
        this.GetEnumerator() :> System.Collections.IEnumerator
      member this.TryItem index =
        if index >= 0 && index < this.Count then
          arr |> Map.tryGet (this.Count - index - 1)
        else None
    }