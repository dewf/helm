module FSharpQt.Widgets.TreeView

open FSharpQt.BuilderNode
open System
open Org.Whatever.QtTesting
open FSharpQt.MiscTypes

type Signal =
    | CustomContextMenuRequested of pos: Point
    | Activated of index: ModelIndexProxy
    | Clicked of index: ModelIndexProxy
    | DoubleClicked of index: ModelIndexProxy
    | Entered of index: ModelIndexProxy
    | IconSizeChanged of size: Size
    | Pressed of index: ModelIndexProxy
    | ViewportEntered
    | Collapsed of index: ModelIndexProxy
    | Expanded of index: ModelIndexProxy
    
type Attr =
    | AllColumnsShowFocus of enabled: bool
    | Animated of enabled: bool
    | AutoExpandDelay of delay: int
    | ExpandsOnDoubleClick of enabled: bool
    | HeaderHidden of hidden: bool
    | Indentation of indent: int
    | ItemsExpandable of enabled: bool
    | RootIsDecorated of show: bool
    | SortingEnabled of enabled: bool
    | UniformRowHeights of uniform: bool
    | WordWrap of enabled: bool
    
let private keyFunc = function
    | AllColumnsShowFocus _ -> 0
    | Animated _ -> 1
    | AutoExpandDelay _ -> 2
    | ExpandsOnDoubleClick _ -> 3
    | HeaderHidden _ -> 4
    | Indentation _ -> 5
    | ItemsExpandable _ -> 6
    | RootIsDecorated _ -> 7
    | SortingEnabled _ -> 8
    | UniformRowHeights _ -> 9
    | WordWrap _ -> 10
    
let private diffAttrs =
    genericDiffAttrs keyFunc

type private Model<'msg>(dispatch: 'msg -> unit) as this =
    let mutable treeView = TreeView.Create(this)
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable currentMask = enum<TreeView.SignalMask> 0
    
    let signalDispatch (s: Signal) =
        signalMap s
        |> Option.iter dispatch
        
    member this.TreeView = treeView
    member this.SignalMap with set value = signalMap <- value
    
    member this.SignalMask with set value =
        if value <> currentMask then
            treeView.SetSignalMask(value)
            currentMask <- value
            
    member this.AddQtModel (model: AbstractItemModel.Handle) =
        treeView.SetModel(model)
        
    member this.RemoveQtModel () =
        // well if it gets deleted (as a dependency), won't that delete from the view automatically?
        ()
    
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | AllColumnsShowFocus enabled ->
                treeView.SetAllColumnsShowFocus(enabled)
            | Animated enabled ->
                treeView.SetAnimated(enabled)
            | AutoExpandDelay delay ->
                treeView.SetAutoExpandDelay(delay)
            | ExpandsOnDoubleClick enabled ->
                treeView.SetExpandsOnDoubleClick(enabled)
            | HeaderHidden hidden ->
                treeView.SetHeaderHidden(hidden)
            | Indentation indent ->
                treeView.SetIndentation(indent)
            | ItemsExpandable enabled ->
                treeView.SetItemsExpandable(enabled)
            | RootIsDecorated show ->
                treeView.SetRootIsDecorated(show)
            | SortingEnabled enabled ->
                treeView.SetSortingEnabled(enabled)
            | UniformRowHeights uniform ->
                treeView.SetUniformRowHeights(uniform)
            | WordWrap enabled ->
                treeView.SetWordWrap(enabled)
                
    interface TreeView.SignalHandler with
        member this.CustomContextMenuRequested pos =
            signalDispatch (Point.From pos |> CustomContextMenuRequested)
        member this.Activated index =
            signalDispatch (ModelIndexProxy(index) |> Activated)
        member this.Clicked index =
            signalDispatch (ModelIndexProxy(index) |> Clicked)
        member this.DoubleClicked index =
            signalDispatch (ModelIndexProxy(index) |> DoubleClicked)
        member this.Entered index =
            signalDispatch (ModelIndexProxy(index) |> Entered)
        member this.IconSizeChanged size =
            signalDispatch (Size.From size |> IconSizeChanged)
        member this.Pressed index =
            signalDispatch (ModelIndexProxy(index) |> Pressed)
        member this.ViewportEntered() =
            signalDispatch ViewportEntered
        member this.Collapsed index =
            signalDispatch (ModelIndexProxy(index) |> Collapsed)
        member this.Expanded index =
            signalDispatch (ModelIndexProxy(index) |> Expanded)
            
    interface IDisposable with
        member this.Dispose() =
            treeView.Dispose()

let private create (attrs: Attr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (signalMask: TreeView.SignalMask) =
    let model = new Model<'msg>(dispatch)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private migrate (model: Model<'msg>) (attrs: Attr list) (signalMap: Signal -> 'msg option) (signalMask: TreeView.SignalMask) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()


type TreeView<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    member val Attrs: Attr list = [] with get, set
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    let mutable maybeTreeModel: IModelNode<'msg> option = None
    member this.TreeModel with set value = maybeTreeModel <- Some value
    
    let mutable signalMask = enum<TreeView.SignalMask> 0
    
    let mutable onCustomContextMenuRequested: (Point -> 'msg) option = None
    let mutable onActivated: (ModelIndexProxy -> 'msg) option = None
    let mutable onClicked: (ModelIndexProxy -> 'msg) option = None
    let mutable onDoubleClicked: (ModelIndexProxy -> 'msg) option = None
    let mutable onEntered: (ModelIndexProxy -> 'msg) option = None
    let mutable onIconSizeChanged: (Size -> 'msg) option = None
    let mutable onPressed: (ModelIndexProxy -> 'msg) option = None
    let mutable onViewportEntered: 'msg option = None
    let mutable onCollapsed: (ModelIndexProxy -> 'msg) option = None
    let mutable onExpanded: (ModelIndexProxy -> 'msg) option = None
    
    member this.OnCustomContextMenuRequested with set value =
        onCustomContextMenuRequested <- Some value
        signalMask <- signalMask ||| TreeView.SignalMask.CustomContextMenuRequested
        
    member this.OnActivated with set value =
        onActivated <- Some value
        signalMask <- signalMask ||| TreeView.SignalMask.Activated
        
    member this.OnClicked with set value =
        onClicked <- Some value
        signalMask <- signalMask ||| TreeView.SignalMask.Clicked
        
    member this.OnDoubleClicked with set value =
        onDoubleClicked <- Some value
        signalMask <- signalMask ||| TreeView.SignalMask.DoubleClicked
        
    member this.OnEntered with set value =
        onEntered <- Some value
        signalMask <- signalMask ||| TreeView.SignalMask.Entered
        
    member this.OnIconSizeChanged with set value =
        onIconSizeChanged <- Some value
        signalMask <- signalMask ||| TreeView.SignalMask.IconSizeChanged
        
    member this.OnPressed with set value =
        onPressed <- Some value
        signalMask <- signalMask ||| TreeView.SignalMask.Pressed
        
    member this.OnViewportEntered with set value =
        onViewportEntered <- Some value
        signalMask <- signalMask ||| TreeView.SignalMask.ViewportEntered
    
    member this.OnCollapsed with set value =
        onCollapsed <- Some value
        signalMask <- signalMask ||| TreeView.SignalMask.Collapsed
        
    member this.OnExpanded with set value =
        onExpanded <- Some value
        signalMask <- signalMask ||| TreeView.SignalMask.Expanded
        
    let signalMap = function
        | CustomContextMenuRequested pos ->
            onCustomContextMenuRequested
            |> Option.map (fun f -> f pos)
        | Activated index ->
            onActivated
            |> Option.map (fun f -> f index)
        | Clicked index ->
            onClicked
            |> Option.map (fun f -> f index)
        | DoubleClicked index ->
            onDoubleClicked
            |> Option.map (fun f -> f index)
        | Entered index ->
            onEntered
            |> Option.map (fun f -> f index)
        | IconSizeChanged size ->
            onIconSizeChanged
            |> Option.map (fun f -> f size)
        | Pressed index ->
            onPressed
            |> Option.map (fun f -> f index)
        | ViewportEntered ->
            onViewportEntered
        | Collapsed index ->
            onCollapsed
            |> Option.map (fun f -> f index)
        | Expanded index ->
            onExpanded
            |> Option.map (fun f -> f index)
            
    member this.MigrateDeps (changeMap: Map<DepsKey, DepsChange>) =
        match changeMap.TryFind (StrKey "model") with
        | Some change ->
            match change with
            | Unchanged ->
                ()
            | Added ->
                this.model.AddQtModel(maybeTreeModel.Value.QtModel)
            | Removed ->
                this.model.RemoveQtModel()
            | Swapped ->
                this.model.RemoveQtModel()
                this.model.AddQtModel(maybeTreeModel.Value.QtModel)
        | None ->
            // neither side had one
            ()
    
    interface IWidgetNode<'msg> with
        override this.Dependencies =
            maybeTreeModel
            |> Option.map (fun node -> StrKey "model", node :> IBuilderNode<'msg>)
            |> Option.toList

        override this.Create dispatch buildContext =
            this.model <- create this.Attrs signalMap dispatch signalMask
            
        override this.AttachDeps () =
            maybeTreeModel
            |> Option.iter (fun node ->
                this.model.AddQtModel(node.QtModel))

        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> TreeView<'msg>)
            let nextAttrs = diffAttrs left'.Attrs this.Attrs |> createdOrChanged__old
            this.model <- migrate left'.model nextAttrs signalMap signalMask
            this.MigrateDeps(depsChanges |> Map.ofList)

        override this.Dispose() =
            (this.model :> IDisposable).Dispose()

        override this.Widget =
            this.model.TreeView
            
        override this.ContentKey =
            this.model.TreeView
            
        override this.Attachments =
            this.Attachments
