// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open PersistentCollections

[<EntryPoint>]
let main argv = 
  let vec = 
    seq { 0 .. 30000 } 
    |> Seq.fold (
        fun acc i -> acc |> PersistentVector.add i
      ) PersistentVector.empty

  let v2 =  PersistentVector.update 1 3 vec
  
  let (_, vec2) = 
    seq { 0 .. 30000 } 
    |> Seq.fold (
        fun (i, acc) (v: int) -> 
          let acc = PersistentVector.update v (i - v) acc
          (i, acc)
      ) (30000, vec)

   
  Seq.zip (seq { 0 .. 30000 }) (vec2 |> PersistentVector.toSeq )
  |> Seq.iteri (
    fun i (a, b) -> 
      let getResult = vec2 |> PersistentVector.get i
      if getResult <> 30000 - i then printfn "get result failure at %i %i" getResult i
  )

  printfn "%A" argv
  0 // return an integer exit code

