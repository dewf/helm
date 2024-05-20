module BuilderNode

open Org.Whatever.QtTesting


module List =
    let zipWithIndex (xs: 'a list) =
        xs |> List.mapi (fun i x -> (i, x))

module Map =
    let keys (map: Map<'a,'b>) =
        map |> Map.toList |> List.map fst

type AttrChange<'a> =
    | Created of 'a
    | Deleted of 'a
    | Changed of 'a * 'a

let createdOrChanged (changes: AttrChange<'a> list) =
    changes
    |> List.choose (function | Created attr | Changed (_, attr) -> Some attr | _ -> None)

[<AbstractClass>]
type BuilderNode<'msg>() =
    abstract member Dependencies: unit -> (int * BuilderNode<'msg>) list
    abstract member Create: ('msg -> unit) -> unit
    abstract member MigrateFrom: BuilderNode<'msg> -> unit // will the dispatch ever change?
    abstract member Dispose: unit -> unit
    abstract member ContentKey: System.Object
    

[<AbstractClass>]
type WidgetNode<'msg>() =
    inherit BuilderNode<'msg>()
    abstract member Widget: Widget.Handle
    override this.ContentKey = this.Widget
    
[<AbstractClass>]
type LayoutNode<'msg>() =
    inherit WidgetNode<'msg>()
    let mutable maybeSyntheticParent: Widget.Handle option = None
    abstract member Layout: Layout.Handle
    override this.ContentKey = this.Layout
    override this.Widget =
        // create a widget on demand to hold the layout
        // TODO: need to set up a dispose system so that all the inheritance tree disposes properly
        match maybeSyntheticParent with
        | Some widget ->
            widget
        | None ->
            let widget = Widget.Create()
            widget.SetLayout(this.Layout)
            maybeSyntheticParent <- Some widget
            widget
    
[<AbstractClass>]
type MenuBarNode<'msg>() =
    inherit BuilderNode<'msg>()
    abstract member MenuBar: MenuBar.Handle
    override this.ContentKey = this.MenuBar
    
[<AbstractClass>]
type MenuNode<'msg>() =
    inherit BuilderNode<'msg>()
    abstract member Menu: Menu.Handle
    override this.ContentKey = this.Menu

[<AbstractClass>]
type ActionNode<'msg>() =
    inherit BuilderNode<'msg>()
    abstract member Action: Action.Handle
    override this.ContentKey = this.Action
    
type Empty<'msg>() =
    inherit BuilderNode<'msg>()
    override this.Dependencies() = []
    override this.Create(dispatch: 'msg -> unit) = ()
    override this.MigrateFrom(left: BuilderNode<'msg>) = ()
    override this.Dispose() = ()
    override this.ContentKey = "!!empty!!"

let rec disposeTree(node: BuilderNode<'msg>) =
    for (_, node) in node.Dependencies() do
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

let rec diff (dispatch: 'msg -> unit) (maybeLeft: BuilderNode<'msg> option) (maybeRight: BuilderNode<'msg> option) =
    let createRight (dispatch: 'msg -> unit) (right: BuilderNode<'msg>) =
        // realize dependencies
        for (_, node) in right.Dependencies() do
            diff dispatch None (Some node)
        // now create
        right.Create dispatch

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
        let leftMap = Map.ofList (left.Dependencies())
        let rightMap = Map.ofList (right.Dependencies())
        let uniqueIds = (Map.keys leftMap @ Map.keys rightMap) |> List.distinct |> List.sort
        let leftChildren = uniqueIds |> List.map leftMap.TryFind
        let rightChildren = uniqueIds |> List.map rightMap.TryFind
        for (lch, rch) in List.zip leftChildren rightChildren do
            diff dispatch lch rch

        // now merge the nodes themselves, with the children having been recursively reconciled above
        // attrs are handled internally
        right.MigrateFrom left

    | Some left, Some right ->
        // different types - dispose left, create right
        // (combination of [right empty] and [left empty] cases above)
        // in theeeeory we could reparent existing children to a different parent type, preserving state
        // but honestly that's an extremely exotic use case, why bother?
        disposeTree left
        createRight dispatch right

let build (dispatch: 'msg -> unit) (root: BuilderNode<'msg>) =
    diff dispatch None (Some root)
