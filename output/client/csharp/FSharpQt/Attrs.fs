module FSharpQt.Attrs

type IAttr =
    interface
        abstract member Key: string
        abstract member AttrEquals: IAttr -> bool
        abstract member ApplyTo: Org.Whatever.QtTesting.Object.Handle -> unit
    end
    
type AttrDiffResult =
    | Created of attr: IAttr
    | Changed of prev: IAttr * next: IAttr
    | Deleted of prev: IAttr
    
let diffAttrs (left: IAttr list) (right: IAttr list) =
    let leftList = left |> List.map (fun attr -> attr.Key, attr)
    let rightList = right |> List.map (fun attr -> attr.Key, attr)
    let leftMap = leftList |> Map.ofList
    let rightMap = rightList |> Map.ofList

    let allKeys =
        (leftList @ rightList)
        |> List.map fst
        |> List.distinct
        |> List.sort        // why?

    allKeys
    |> List.choose (fun key ->
        match leftMap.TryFind key, rightMap.TryFind key with
        | Some left, Some right ->
            if left.AttrEquals right then
                None
            else
                Changed (left, right) |> Some
        | Some left, None ->
            Deleted left |> Some
        | None, Some right ->
            Created right |> Some
        | _ ->
            failwith "can't happen")

let createdOrChanged (changes: AttrDiffResult list) =
    changes
    |> List.choose (function | Created attr | Changed (_, attr) -> Some attr | _ -> None)
