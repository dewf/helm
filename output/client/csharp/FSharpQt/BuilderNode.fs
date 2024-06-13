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
    
type BuilderContext<'msg> = {
    ContainingWindow: IBuilderNode<'msg> option // window, dialog, whatever
}
and IContextRewriter<'msg> =
    interface
        // certain utility nodes (eg ...WithDialogs) will rewrite the builder context
        abstract ContextFor: DepsKey -> BuilderContext<'msg> -> BuilderContext<'msg>
    end
and IBuilderNode<'msg> =
    interface
        abstract Dependencies: (DepsKey * IBuilderNode<'msg>) list
        abstract Create2: ('msg -> unit) -> BuilderContext<'msg> -> unit
        abstract AttachDeps: unit -> unit
        abstract MigrateFrom: IBuilderNode<'msg> -> (DepsKey * DepsChange) list -> unit // will the dispatch ever change?
        abstract Dispose: unit -> unit
        abstract ContentKey: System.Object
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
        for _, node in node.Dependencies do
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
        let leftVal, rightVal = (Map.tryFind key leftMap, Map.tryFind key rightMap)
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

let rec diff (dispatch: 'msg -> unit) (maybeLeft: IBuilderNode<'msg> option) (maybeRight: IBuilderNode<'msg> option) (context: BuilderContext<'msg>) =
    let createRight (dispatch: 'msg -> unit) (right: IBuilderNode<'msg>) =
        // initial create
        right.Create2 dispatch context
        
        // are we a window/dialog parent for things deeper down?
        let nextContext, maybeRewriter =
            match right with
            | :? IContextRewriter<'msg> as rewriter ->
                context, Some rewriter
            | :? IWindowNode<'msg> as windowNode ->
                { context with ContainingWindow = Some windowNode }, None
            | :? IDialogNode<'msg> as dialogNode ->
                { context with ContainingWindow = Some dialogNode }, None
            | _ ->
                context, None
                
        // realize dependencies
        match maybeRewriter with
        | Some rewriter ->
            // special handling for utility nodes, they need to be able to modify the context for their dependencies
            for key, node in right.Dependencies do
                let rewritten =
                    rewriter.ContextFor key nextContext
                diff dispatch None (Some node) rewritten
        | _ ->
            // normal processing
            for _, node in right.Dependencies do
                diff dispatch None (Some node) nextContext
                
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
        let leftMap = Map.ofList left.Dependencies
        let rightMap = Map.ofList right.Dependencies
        let uniqueIds = (Map.keys leftMap @ Map.keys rightMap) |> List.distinct |> List.sort
        let correlated =
            uniqueIds
            |> List.map (fun key ->
                let left = leftMap.TryFind key
                let right = rightMap.TryFind key
                key, left, right)
            
        // are we a window/dialog parent for things deeper down?
        let nextContext, maybeRewriter =
            // only left has a model right now (pre-migration), and created children could potentially query for the .Widget or whatever (eg Dialogs wanting to know parent windows)
            match left with
            | :? IContextRewriter<'msg> as rewriter ->
                context, Some rewriter
            | :? IWindowNode<'msg> as windowNode ->
                { context with ContainingWindow = Some windowNode }, None
            | :? IDialogNode<'msg> as dialogNode ->
                { context with ContainingWindow = Some dialogNode }, None
            | _ ->
                context, None
                
        match maybeRewriter with
        | Some rewriter ->
            // utility node builder context rewriting
            for key, lch, rch in correlated do
                let rewritten =
                    rewriter.ContextFor key nextContext
                diff dispatch lch rch rewritten
        | _ ->
            // normal processing
            for _, lch, rch in correlated do
                diff dispatch lch rch nextContext
            
        let depsChanges =
            // provide a more precise breakdown of the dependency changes
            // saves redundant comparison code in widgets with dependencies of different types (eg MainWindow)
            // see MainWindow.MigrateContent to see how this is typically used
            [for key, lch, rch in correlated ->
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
                key, changeType]

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
    let context =
        { ContainingWindow = None }
    diff dispatch None (Some root) context
