module FSharpQt.Widgets.TreeView

open FSharpQt.BuilderNode
open System
open Org.Whatever.QtTesting
open FSharpQt.MiscTypes

type Signal =
    | Collapsed of index: ModelIndexProxy
    | Expanded of index: ModelIndexProxy
    
type Attr =
    | NothingYet
    
let private keyFunc = function
    | NothingYet -> 0
    
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
            | NothingYet ->
                ()

    interface TreeView.SignalHandler with
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
    
    let mutable onCollapsed: (ModelIndexProxy -> 'msg) option = None
    let mutable onExpanded: (ModelIndexProxy -> 'msg) option = None
    
    member this.OnCollapsed with set value =
        onCollapsed <- Some value
        signalMask <- signalMask ||| TreeView.SignalMask.Collapsed
        
    member this.OnExpanded with set value =
        onExpanded <- Some value
        signalMask <- signalMask ||| TreeView.SignalMask.Expanded
        
    let signalMap = function
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
            let nextAttrs = diffAttrs left'.Attrs this.Attrs |> createdOrChanged
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
