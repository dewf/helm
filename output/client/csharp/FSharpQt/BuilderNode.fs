module FSharpQt.BuilderNode

open Org.Whatever.QtTesting
open Extensions

type AttrChange<'a> =
    | Created of 'a
    | Deleted of 'a
    | Changed of 'a * 'a

let createdOrChanged (changes: AttrChange<'a> list) =
    changes
    |> List.choose (function | Created attr | Changed (_, attr) -> Some attr | _ -> None)
    
type DepsKey =
    | IntKey of i: int
    | StrKey of str: string
    
type DepsChange =
    | Unchanged
    | Added
    | Removed
    | Swapped
    
type IBuilderNode<'msg> =
    interface
        abstract Dependencies: (DepsKey * IBuilderNode<'msg>) list
        abstract Create2: ('msg -> unit) -> IBuilderNode<'msg> option -> unit
        abstract AttachDeps: unit -> unit
        abstract MigrateFrom: IBuilderNode<'msg> -> (DepsKey * DepsChange) list -> unit // will the dispatch ever change?
        abstract Dispose: unit -> unit
        abstract ContentKey: System.Object
        
        // just for now? could this be avoided with the right sort of interface hierarchy, and having stuff like Dialog check for those on self-creation?
        // basically this proceeds upwards until we hit a genuine Widget which can call .getWindow() on itself ...
        abstract ContainingWindowWidget: IBuilderNode<'msg> -> Widget.Handle option
    end

// this will allow certain widgets (eg MainWindow) to accept either type
// also removes the burden of implementing IWidgetNode from every single ILayoutNode implementation (the former approach), which was annoying
type IWidgetOrLayoutNode<'msg> =
    interface
        inherit IBuilderNode<'msg>
    end

type IWidgetNode<'msg> =
    interface
        inherit IWidgetOrLayoutNode<'msg>
        abstract Widget: Widget.Handle
    end
    
type ILayoutNode<'msg> =
    interface
        inherit IWidgetOrLayoutNode<'msg>
        abstract member Layout: Layout.Handle
    end
    
// leaving this below for future reference, in case fancier inheritance (eg default interface methods / traits) ever comes to F#
// unfortunately we have to manually implement this pattern below in anything that implements LayoutNode interface for now
// but it's a small price to pay for cleaner, less redundant implementation of the various reactor node types (eg LayoutReactorNode, WindowReactorNode, etc)
// (that was the problem that necessitated changing the *Node inheritance hierarchy from abstract classes to interfaces)
// (but/and the only reason they were abstract classes to begin with, was to get some default implementation behavior -
//  back in the original Scala/Swing experiment that led to this framework, the *Node types were defined as Scala traits)
    
// [<AbstractClass>]
// type BaseLayoutNode<'msg>() =
//     let mutable maybeSyntheticParent: Widget.Handle option = None
//     interface LayoutNode<'msg> with
//         override this.ContentKey = (this :> LayoutNode<'msg>).Layout
//         override this.Widget =
//             // create a widget on demand to hold the layout
//             // TODO: set up .Dipose() inheritance business so that everything up and down the hierarchy disposes properly
//             match maybeSyntheticParent with
//             | Some widget ->
//                 widget
//             | None ->
//                 let widget = Widget.Create()
//                 widget.SetLayout((this :> LayoutNode<'msg>).Layout)
//                 maybeSyntheticParent <- Some widget
//                 widget
    
type IMenuBarNode<'msg> =
    interface
        inherit IBuilderNode<'msg>
            abstract MenuBar: MenuBar.Handle
    end
    
type IMenuNode<'msg> =
    interface
        inherit IBuilderNode<'msg>
            abstract member Menu: Menu.Handle
            abstract member Popup: Common.Point -> unit
    end
    
type IActionNode<'msg> =
    interface
        inherit IBuilderNode<'msg>
            abstract member Action: Action.Handle
    end
    
type ITopLevelNode<'msg> =
    interface
        inherit IBuilderNode<'msg>
        // doesn't define any properties / contentkey itself
    end
    
type IWindowNode<'msg> =
    interface
        inherit ITopLevelNode<'msg>
        abstract member WindowWidget: Widget.Handle
    end
    
type IDialogNode<'msg> =
    interface
        inherit ITopLevelNode<'msg>
        abstract member Dialog: Dialog.Handle
    end
    
type IDialogParent<'msg> =
    interface
        abstract member RelativeToWidget: Widget.Handle option              // for showing dialogs relative to a point
        abstract member AttachedDialogs: (string * IDialogNode<'msg>) list
    end
    
type IPopupMenuParent<'msg> =
    interface
        abstract member RelativeToWidget: Widget.Handle  // for translating relative coords to global
        abstract member AttachedPopups: (string * IMenuNode<'msg>) list
    end
        
let rec disposeTree(node: IBuilderNode<'msg>) =
    match node with
    | :? IDialogNode<'msg> ->
        printfn "not destroying dialog or dependencies (owned by a window ... probably)"
    | _ ->
        for (_, node) in node.Dependencies do
            disposeTree node
        node.Dispose()

let inline genericDiffAttrs (keyFunc: 'a -> int) (a1: 'a list) (a2: 'a list)  =
    let leftList = a1 |> List.map (fun a -> keyFunc a, a)
    let rightList = a2 |> List.map (fun a -> keyFunc a, a)
    let leftMap = leftList |> Map.ofList
    let rightMap = rightList |> Map.ofList

    let allKeys =
        (leftList @ rightList)
        |> List.map fst
        |> List.distinct
        |> List.sort

    allKeys
    |> List.choose (fun key ->
        let (leftVal, rightVal) = (Map.tryFind key leftMap, Map.tryFind key rightMap)
        match leftVal, rightVal with
        | Some left, Some right ->
            if left = right then None else Changed (left, right) |> Some
        | Some left, None ->
            Deleted left |> Some
        | None, Some right ->
            Created right |> Some
        | _ -> failwith "shouldn't happen")
    
let nullDiffAttrs (a1: 'a list) (a2: 'a list) =
    []

let rec diff (dispatch: 'msg -> unit) (maybeLeft: IBuilderNode<'msg> option) (maybeRight: IBuilderNode<'msg> option) (maybeParent: IBuilderNode<'msg> option) =
    let createRight (dispatch: 'msg -> unit) (right: IBuilderNode<'msg>) =
        // initial create w/ our already-created parent (if any)
        right.Create2 dispatch maybeParent
        // realize dependencies
        for _, node in right.Dependencies do
            diff dispatch None (Some node) (Some right)
        // only now attach dependencies (we used to create dependencies first, then create a parentless node)
        right.AttachDeps()

    match (maybeLeft, maybeRight) with
    | None, None ->
        failwith "both sides empty??"

    | None, Some right ->
        createRight dispatch right

    | Some left, None ->
        disposeTree left

    | Some left, Some right when left.GetType() = right.GetType() ->
        // neither side empty, but same type - diff and migrate
        // reconcile and order children via ID
        let leftMap = Map.ofList (left.Dependencies)
        let rightMap = Map.ofList (right.Dependencies)
        let uniqueIds = (Map.keys leftMap @ Map.keys rightMap) |> List.distinct |> List.sort
        let leftChildren = uniqueIds |> List.map leftMap.TryFind
        let rightChildren = uniqueIds |> List.map rightMap.TryFind
        let zipped =
            List.zip leftChildren rightChildren
        for lch, rch in zipped do
            // re: using left as parent - we haven't migrated to the right yet, so only the left has a model, and the children will need to reference its widget etc
            // also, would there be any reason to delay this until after the migration occurs? node CREATION needs to happen top-down, but does migration matter?
            diff dispatch lch rch (Some left)  
            
        let depsChanges =
            // provide a more precise breakdown of the dependency changes
            // saves redundant comparison code in widgets with dependencies of different types (eg MainWindow)
            // see MainWindow.MigrateContent to see how this is typically used
            [for id, (lch, rch) in List.zip uniqueIds zipped ->
                let changeType =
                    match lch, rch with
                    | None, None ->
                        // how would the ID even exist if it wasn't in either side to begin with?
                        failwith "shouldn't happen (BuilderNode.diff - depsChanges)"
                    | None, Some _ ->
                        Added
                    | Some _, None ->
                        Removed
                    | Some left, Some right ->
                        if left.ContentKey = right.ContentKey then
                            Unchanged
                        else
                            Swapped
                id, changeType]

        // now merge the nodes themselves, with the children having been recursively reconciled above
        // attrs are handled internally
        right.MigrateFrom left depsChanges

    | Some left, Some right ->
        // different types - dispose left, create right
        // (combination of [right empty] and [left empty] cases above)
        // in theeeeory we could reparent existing children to a different parent type, preserving state
        // but honestly that's an extremely exotic use case, why bother?
        disposeTree left
        createRight dispatch right

let build (dispatch: 'msg -> unit) (root: IBuilderNode<'msg>) =
    diff dispatch None (Some root) None
