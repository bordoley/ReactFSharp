// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open ImmutableCollections
open System

let persistentVectorTests n = 
  let testSeq = seq { 0 .. n }

  let empty1 = PersistentVector.empty ()

  let empty2 = PersistentVector.empty ()

  printfn "empty are equal? %b" (Object.ReferenceEquals (empty1, empty2))

  let testEnumeration (vec: IPersistentVector<_>) =
    seq{ 0 .. (vec |> ImmutableVector.lastIndex) }
    |> Seq.zip (vec |> ImmutableMap.values)
    |> Seq.iter (
      fun (i, v) -> if i <> v then failwith (sprintf "vec.count = %i, index: %i, actual: %i"  (vec |> Collection.count) i v)
    )

  let vec =
    testSeq
    |> Seq.fold (
        fun acc i -> acc |> PersistentVector.add i
      ) empty1

  let vec2 =
    testSeq
    |> Seq.fold (
        fun acc (v: int) -> PersistentVector.update v v acc
      ) vec

  printfn "vecs are equal? %b" (Object.ReferenceEquals (vec, vec2))

  Seq.zip testSeq (vec |> ImmutableMap.values)
  |> Seq.iter (
      fun (i, v) -> if i <> v then printfn "notEqual %i %i" i v
    )

  testSeq |> Seq.iter (
    fun i ->
      let v = vec |> PersistentVector.get i
      if i <> v then printfn "get failed %i %i" i v
  )

  testSeq
  |> Seq.fold (
      fun acc (v: int) -> PersistentVector.update v (n - v) acc
    ) vec
  |> ImmutableMap.values
  |> Seq.iteri (
      fun i v ->
        if (n - i) <> v then printfn "expect %i but was %i" (n - i) v
    )

  let mutable empty = vec
  while (empty |> Collection.count > 0) do
    let prev = empty
    empty <- PersistentVector.pop empty
    //testEnumeration empty

let persistentMapTests n = 
  let empty = PersistentMap.empty ()

  let testSeq = seq { 0 .. n }

  let result =
    testSeq |> Seq.fold (fun acc i -> 
      acc |> PersistentMap.put (100 * i) i
    ) empty

  testSeq 
  |> Seq.iter (fun i ->
      let v = result |> PersistentMap.get (100 * i)
      if (i <> v) then failwith (sprintf "expected %i got %i" i v)
    )

  let result2 =
    testSeq |> Seq.map (fun i -> n - i) |> Seq.fold (fun acc i -> 
      acc |> PersistentMap.remove i
    ) result
  ()

open BitCount

[<EntryPoint>]
let main argv =
  let n = 1000000

  persistentVectorTests 65568
  //persistentMapTests n

  0


