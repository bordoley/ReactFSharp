// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open PersistentCollections

[<EntryPoint>]
let main argv = 
  let n = 3000000
  let testSeq = seq { 0 .. n } 

  let vec = 
    testSeq
    |> Seq.fold (
        fun acc i -> acc |> PersistentVector.add i
      ) PersistentVector.empty

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
  let rec drain (v: PersistentVector<_>) =
    if v.count > 1 then
       drain (v |> PersistentVector.pop)
     else v

  let empty = drain vec*)

  printfn "%A" argv
  0 // return an integer exit code

