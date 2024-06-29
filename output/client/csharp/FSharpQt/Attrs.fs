module FSharpQt.Attrs

open Org.Whatever.QtTesting

type IAttrTarget =
    interface
    end
    
type IAttr =
    interface
        abstract member Key: string
        abstract member AttrEquals: IAttr -> bool
        abstract member ApplyTo: IAttrTarget -> unit
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
        // |> List.sort        // I think we used to do this for numeric key ordering? but we don't use those anymore

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
    |> List.choose (function
        | Created attr | Changed (_, attr) -> Some attr
        | _ -> None)

// various interfaces for accessing qobjects/widgets, + 2-way binding guard setters where applicable
// if you want to support a given type of attribute, you have to implement the target interface
// reasonable enough!

type internal WidgetAttrTarget =
    interface
        inherit IAttrTarget
        abstract member Widget: Widget.Handle
    end
    
type internal AbstractButtonAttrTarget =
    interface
        inherit WidgetAttrTarget
        abstract member AbstractButton: AbstractButton.Handle
        abstract member SetChecked: bool -> bool   // binding guard - for example, toggled signal emits 'checked' value
    end
    
type internal PushButtonAttrTarget =
    interface
        inherit AbstractButtonAttrTarget
        abstract member PushButton: PushButton.Handle
    end
    
type internal LineEditAttrTarget =
    interface
        inherit WidgetAttrTarget
        abstract member LineEdit: LineEdit.Handle
        abstract member SetText: string -> bool     // binding guards
        abstract member SetCursorPos: int -> bool
    end
    
type internal ActionAttrTarget =
    interface
        inherit IAttrTarget
        abstract member Action: Action.Handle
        abstract member SetEnabled: bool -> bool   // return value: internal value did change
        abstract member SetCheckable: bool -> bool
        abstract member SetChecked: bool -> bool
    end
  
type internal SortFilterProxyModelAttrTarget =
    interface
        inherit IAttrTarget
        abstract member ProxyModel: SortFilterProxyModel.Handle
    end
