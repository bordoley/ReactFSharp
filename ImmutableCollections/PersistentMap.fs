namespace ImmutableCollections
(*
open System.Collections.Generic



type HashedTriePersistentMapNode<'k, 'v> =
  | ArrayMapNode of ImmutableArray<HashedTriePersistentMapNode<'k, 'v>>
  | 

type [<ReferenceEquality>] HashedTriePersistentMapRootNode<'k, 'v> =
  | HashedTriePersistentMapLevelRootNode of HashedTriePersistentMapLevelNode<'k, 'v>
  | HashedTriePersistentMapKeyValueRootNode of HashedTriePersistentMapKeyValueNode<'k, 'v>
  | HashedTriePersistentMapNoneRootNode


type [<ReferenceEquality>] private HashedTriePersistentMap<'k, 'v> = {
  count: int
  keyComparer: IEqualityComparer<'k>
  valueComparer: IEqualityComparer<'v>
  root: HashedTriePersistentMapRootNode<'k, 'v>
}

module private BitCount =
    let bitCounts = 
        let bitCounts = Array.create 65536 0
        let position1 = ref -1
        let position2 = ref -1

        for i in 1 .. 65535 do
           if !position1 = !position2 then       
                position1 := 0
                position2 := i

           bitCounts.[i] <- bitCounts.[!position1] + 1
           position1 := !position1 + 1
        bitCounts

    let inline NumberOfSetBits value =
        bitCounts.[value &&& 65535] + bitCounts.[(value >>> 16) &&& 65535]

    let inline mask(hash, shift) = (hash >>> shift) &&& 0x01f
    let inline bitpos(hash, shift) = 1 <<< mask(hash, shift)
    let inline index(bitmap,bit) = NumberOfSetBits(bitmap &&& (bit - 1))

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module private HashedTriePersistentMap = 
  open BitCount 

  let create (keyComparer: IEqualityComparer<'v>) (valueComparer: IEqualityComparer<'v>) = {
    keyComparer = keyComparer
    valueComparer = valueComparer
    count = 0
    root = HashedTriePersistentMapNoneRootNode
  }

 
  let put (k: 'k) (v: 'v) (map: HashedTriePersistentMap<'k, 'v>) : HashedTriePersistentMap<'k, 'v> =
    let hash = map.keyComparer.GetHashCode(k)

    match map.root with
    | HashedTriePersistentMapNoneRootNode -> 
        let empty = emptyKeyValueNode ()
        map
    | HashedTriePersistentMapLevelRootNode levelNode -> 
        map
    | HashedTriePersistentMapKeyValueRootNode keyValueNode -> 
        map

  let remove: (k: 'k) (map: HashedTriePersistentMap<'k, 'v>) : HashedTriePersistentMap<'k, 'v> =
    match map.root with
    | HashedTriePersistentMapNoneRootNode -> map
    | HashedTriePersistentMapLevelRootNode levelNode -> map
    | HashedTriePersistentMapKeyValueRootNode keyValueNode -> map
    *)