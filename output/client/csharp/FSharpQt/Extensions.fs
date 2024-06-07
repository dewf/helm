module FSharpQt.Extensions

module List =
    let zipWithIndex (xs: 'a list) =
        xs |> List.mapi (fun i x -> (i, x))
        
    let replaceAtIndex (index: int) (replaceFunc: 'a -> 'a) (xs: 'a list) =
        let before, after =
            List.splitAt index xs
        match after with
        | h :: etc ->
            before @ replaceFunc h :: etc
        | _ ->
            failwith "replaceAtIndex fail"

module Map =
    let keys (map: Map<'a,'b>) =
        map |> Map.toList |> List.map fst

module Array =
    let zipWithIndex (items: 'a array) =
        items |> Array.mapi (fun i x -> (i, x))
        
    let replaceAtIndex (index: int) (replaceFunc: 'a -> 'a) (arr: 'a array) =
        arr
        |> Array.mapi (fun i item ->
            if i = index then
                replaceFunc item
            else
                item)

module Option =
    let someIf (cond: bool) (value: 'a) =
        if cond then
            Some value
        else
            None
