module Extensions

    
module List =
    let replaceAtIndex (index: int) (replaceFunc: 'a -> 'a) (xs: 'a list) =
        let before, after =
            List.splitAt index xs
        match after with
        | h :: etc ->
            before @ replaceFunc h :: etc
        | _ ->
            failwith "replaceAtIndex fail"
