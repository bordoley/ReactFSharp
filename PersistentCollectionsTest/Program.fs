// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open PersistentCollections
open System

[<EntryPoint>]
let main argv = 
  let n = 300000
  let testSeq = seq { 0 .. n } 

  let empty1 = PersistentVector.create ()

  let empty2 = PersistentVector.create ()

  printfn "empty are equal? %b" (Object.ReferenceEquals (empty1, empty2))


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

  Seq.zip testSeq (vec |> PersistentVector.toSeq )
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
  |> PersistentVector.toSeq
  |> Seq.iteri (
      fun i v ->
        if (n - i) <> v then printfn "expect %i but was %i" (n - i) v
    )

    (*
  let mutable empty = vec
  while (empty.count >= 2) do
    let prev = empty
    empty <- PersistentVector.pop empty
   *)

  printfn "%A" argv
  0 // return an integer exit code

