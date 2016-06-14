namespace React

open System

type [<ReferenceEquality>] ReactChildren<'a> = {
  keyMap: Map<string, int>
  nodes: array<(string * 'a)>
}

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ReactChildren =
  let empty = {
    keyMap = Map.empty
    nodes = [||]
  }

  let find (key: string) (children: ReactChildren<'a>) =
    let index = children.keyMap |> Map.find key
    let (key, value) = children.nodes.[index]
    value

  let tryFind (key: string) (children: ReactChildren<'a>) =
    let index = children.keyMap |> Map.tryFind key

    match index with 
    | None -> None
    | Some index -> 
        let (key, value) = children.nodes.[index]
        Some value

  let keys (children: ReactChildren<'a>) = 
    let keys = 
      children.keyMap 
      |> Map.fold (
        fun keyArray key index -> 
          Array.set keyArray index key
          keyArray
      ) (Array.create children.nodes.Length "")

    keys :> seq<string>

  let private createUnsafe (nodes: array<string * 'a>) =
    let (_, keyMap) = 
      nodes
      |> Array.fold (
        fun (index, keyMap) (key, _) -> (index + 1, keyMap |> Map.add key index)
      ) (0, Map.empty)
    {
      keyMap = keyMap
      nodes = nodes
    }

  let create (nodes: array<string * 'a>) =
    let nodes = Array.copy nodes 
    createUnsafe nodes

  let map (f: string -> 'T -> 'U) (children: ReactChildren<'T>) = 
    let nodes = children.nodes |> Array.map (fun (k, v) -> (k, f k v))
    createUnsafe nodes

  let filter (f: string -> 'T -> bool) (children: ReactChildren<'T>) = 
    let nodes = children.nodes |> Array.filter (fun (k, v) -> f k v)
    createUnsafe nodes

  let iteri2optional (f: Option<string*'a> -> Option<string*'b> -> int -> unit) (b: ReactChildren<'b>) (a: ReactChildren<'a>) =
     let length = Math.Max(a.nodes.Length, b.nodes.Length)

     for i = 0 to length do
       let valA = if i < a.nodes.Length then Some a.nodes.[i] else None
       let valB = if i < b.nodes.Length then Some b.nodes.[i] else None

       f valA valB i
