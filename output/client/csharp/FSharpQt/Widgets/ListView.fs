module FSharpQt.Widgets.ListView

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
    | IndexesMoved of indexes: ModelIndexProxy array
    
type Movement =
    | Static
    | Free
    | Snap
with
    member this.QtValue =
        match this with
        | Static -> ListView.Movement.Static
        | Free -> ListView.Movement.Free
        | Snap -> ListView.Movement.Snap
    
type Flow =
    | LeftToRight
    | TopToBottom
with
    member this.QtValue =
        match this with
        | LeftToRight -> ListView.Flow.LeftToRight
        | TopToBottom -> ListView.Flow.TopToBottom
    
type ResizeMode =
    | Fixed
    | Adjust
with
    member this.QtValue =
        match this with
        | Fixed -> ListView.ResizeMode.Fixed
        | Adjust -> ListView.ResizeMode.Adjust
    
type LayoutMode =
    | SinglePass
    | Batched
with
    member this.QtValue =
        match this with
        | SinglePass -> ListView.LayoutMode.SinglePass
        | Batched -> ListView.LayoutMode.Batched
    
type ViewMode =
    | ListMode
    | IconMode
with
    member this.QtValue =
        match this with
        | ListMode -> ListView.ViewMode.ListMode
        | IconMode -> ListView.ViewMode.IconMode

type Attr =
    | Movement of movement: Movement
    | Flow of flow: Flow
    | ResizeMode of mode: ResizeMode
    | LayoutMode of mode: LayoutMode
    | ViewMode of mode: ViewMode
    
let private keyFunc = function
    | Movement _ -> 0
    | Flow _ -> 1
    | ResizeMode _ -> 2
    | LayoutMode _ -> 3
    | ViewMode _ -> 4

let private diffAttrs =
    genericDiffAttrs keyFunc

type private Model<'msg>(dispatch: 'msg -> unit) as this =
    let mutable listView = ListView.Create(this)
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable currentMask = enum<ListView.SignalMask> 0
    
    let signalDispatch (s: Signal) =
        signalMap s
        |> Option.iter dispatch
        
    member this.ListView = listView
    member this.SignalMap with set value = signalMap <- value
    
    member this.SignalMask with set value =
        if value <> currentMask then
            listView.SetSignalMask(value)
            currentMask <- value
            
    member this.AddQtModel (model: AbstractItemModel.Handle) =
        listView.SetModel(model)
        
    member this.RemoveQtModel () =
        // well if it gets deleted (as a dependency), won't that delete from the listView automatically?
        ()
    
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Movement movement ->
                listView.SetMovement movement.QtValue
            | Flow flow ->
                listView.SetFlow flow.QtValue
            | ResizeMode mode ->
                listView.SetResizeMode mode.QtValue
            | LayoutMode mode ->
                listView.SetLayoutMode mode.QtValue
            | ViewMode mode ->
                listView.SetViewMode mode.QtValue
                
    interface ListView.SignalHandler with
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
        member this.ViewportEntered () =
            signalDispatch ViewportEntered
        member this.IndexesMoved indexes =
            signalDispatch (indexes |> Array.map ModelIndexProxy |> IndexesMoved)
            
    interface IDisposable with
        member this.Dispose() =
            listView.Dispose()

let private create (attrs: Attr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (signalMask: ListView.SignalMask) =
    let model = new Model<'msg>(dispatch)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private migrate (model: Model<'msg>) (attrs: Attr list) (signalMap: Signal -> 'msg option) (signalMask: ListView.SignalMask) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()

type ListView<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    member val Attrs: Attr list = [] with get, set
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    let mutable maybeListModel: IModelNode<'msg> option = None
    member this.ListModel with set value = maybeListModel <- Some value
    
    let mutable signalMask = enum<ListView.SignalMask> 0
    
    let mutable onCustomContextMenuRequested: (Point -> 'msg) option = None
    let mutable onActivated: (ModelIndexProxy -> 'msg) option = None
    let mutable onClicked: (ModelIndexProxy -> 'msg) option = None
    let mutable onDoubleClicked: (ModelIndexProxy -> 'msg) option = None
    let mutable onEntered: (ModelIndexProxy -> 'msg) option = None
    let mutable onIconSizeChanged: (Size -> 'msg) option = None
    let mutable onPressed: (ModelIndexProxy -> 'msg) option = None
    let mutable onViewportEntered: 'msg option = None
    let mutable onIndexesMoved: (ModelIndexProxy array -> 'msg) option = None
    
    member this.OnCustomContextMenuRequested with set value =
        onCustomContextMenuRequested <- Some value
        signalMask <- signalMask ||| ListView.SignalMask.CustomContextMenuRequested
        
    member this.OnActivated with set value =
        onActivated <- Some value
        signalMask <- signalMask ||| ListView.SignalMask.Activated
        
    member this.OnClicked with set value =
        onClicked <- Some value
        signalMask <- signalMask ||| ListView.SignalMask.Clicked
        
    member this.OnDoubleClicked with set value =
        onDoubleClicked <- Some value
        signalMask <- signalMask ||| ListView.SignalMask.DoubleClicked
        
    member this.OnEntered with set value =
        onEntered <- Some value
        signalMask <- signalMask ||| ListView.SignalMask.Entered
        
    member this.OnIconSizeChanged with set value =
        onIconSizeChanged <- Some value
        signalMask <- signalMask ||| ListView.SignalMask.IconSizeChanged
        
    member this.OnPressed with set value =
        onPressed <- Some value
        signalMask <- signalMask ||| ListView.SignalMask.Pressed
        
    member this.OnViewportEntered with set value =
        onViewportEntered <- Some value
        signalMask <- signalMask ||| ListView.SignalMask.ViewportEntered
        
    member this.OnIndexesMoved with set value =
        onIndexesMoved <- Some value
        signalMask <- signalMask ||| ListView.SignalMask.IndexesMoved
        
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
        | IndexesMoved indexes ->
            onIndexesMoved
            |> Option.map (fun f -> f indexes)
            
    member this.MigrateDeps (changeMap: Map<DepsKey, DepsChange>) =
        match changeMap.TryFind (StrKey "model") with
        | Some change ->
            match change with
            | Unchanged ->
                ()
            | Added ->
                this.model.AddQtModel(maybeListModel.Value.QtModel)
            | Removed ->
                this.model.RemoveQtModel()
            | Swapped ->
                this.model.RemoveQtModel()
                this.model.AddQtModel(maybeListModel.Value.QtModel)
        | None ->
            // neither side had one
            ()
    
    interface IWidgetNode<'msg> with
        override this.Dependencies =
            maybeListModel
            |> Option.map (fun node -> StrKey "model", node :> IBuilderNode<'msg>)
            |> Option.toList

        override this.Create dispatch buildContext =
            this.model <- create this.Attrs signalMap dispatch signalMask
            
        override this.AttachDeps () =
            maybeListModel
            |> Option.iter (fun node ->
                this.model.AddQtModel(node.QtModel))

        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> ListView<'msg>)
            let nextAttrs = diffAttrs left'.Attrs this.Attrs |> createdOrChanged
            this.model <- migrate left'.model nextAttrs signalMap signalMask
            this.MigrateDeps(depsChanges |> Map.ofList)

        override this.Dispose() =
            (this.model :> IDisposable).Dispose()

        override this.Widget =
            this.model.ListView
            
        override this.ContentKey =
            this.model.ListView
            
        override this.Attachments =
            this.Attachments
