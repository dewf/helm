module Util

let sigMap (items: ('a * 'b) list) =
    let map = Map.ofList items
    map.TryFind

