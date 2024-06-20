module FSharpQt.Widgets.ToolBar

open FSharpQt
open FSharpQt.BuilderNode
open System
open FSharpQt.MiscTypes
open Microsoft.FSharp.Core
open Org.Whatever.QtTesting
open Extensions

type Area =
    | Left
    | Right
    | Top
    | Bottom
    | NoArea
    
let private toQtAreas (areas: Set<Area>) =
    (enum<Enums.ToolBarAreas> 0, areas)
    ||> Set.fold (fun acc item ->
        match item with
        | Left -> acc ||| Enums.ToolBarAreas.LeftToolBarArea
        | Right -> acc ||| Enums.ToolBarAreas.RightToolBarArea
        | Top -> acc ||| Enums.ToolBarAreas.TopToolBarArea
        | Bottom -> acc ||| Enums.ToolBarAreas.BottomToolBarArea
        | NoArea ->
            // already starting value of 'acc' = 0
            acc)
    
let private fromQtAreas (areas: Enums.ToolBarAreas) =
    let pairs = [
        Enums.ToolBarAreas.LeftToolBarArea, Left
        Enums.ToolBarAreas.RightToolBarArea, Right
        Enums.ToolBarAreas.TopToolBarArea, Top
        Enums.ToolBarAreas.BottomToolBarArea, Bottom
    ]
    (Set.empty<Area>, pairs)
    ||> List.fold (fun acc (flag, value) ->
        if areas.HasFlag(flag) then
            acc.Add(value)
        else
            acc)
    
type Signal =
    | ActionTriggered of action: ActionProxy
    | AllowedAreasChanged of allowed: Set<Area>
    | IconSizeChanged of size: Size
    | MovableChanged of value: bool
    | OrientationChanged of orient: Orientation
    | ToolButtonStyleChanged of style: ToolButtonStyle
    | TopLevelChanged of topLevel: bool
    | VisibilityChanged of visible: bool
    
type Attr =
    | AllowedAreas of areas: Set<Area>
    | Floatable of floatable: bool
    | IconSize of size: Size
    | Movable of value: bool
    | Orientation of value: Orientation
    | ToolButtonStyle of style: ToolButtonStyle
    
let private keyFunc = function
    | AllowedAreas _ -> 0
    | Floatable _ -> 1
    | IconSize _ -> 2
    | Movable _ -> 3
    | Orientation _ -> 4
    | ToolButtonStyle _ -> 5

let private diffAttrs =
    genericDiffAttrs keyFunc
    
[<RequireQualifiedAccess>]
type private ItemKey<'msg> =
    | ActionItem of key: ContentKey
    | Separator
    | Nothing

type internal InternalItem<'msg> =
    | ActionItem of node: IActionNode<'msg>
    | Separator
    | Nothing
    
type ToolBarItem<'msg> internal(item: InternalItem<'msg>) =
    new(node: IActionNode<'msg>) =
        ToolBarItem(InternalItem.ActionItem node)
    new(?separator: bool) =
        let item =
            match defaultArg separator true with
            | true -> InternalItem.Separator
            | false -> InternalItem.Nothing
        ToolBarItem(item)
    member internal this.MaybeNode =
        match item with
        | ActionItem node -> Some node
        | Separator -> None
        | Nothing -> None
    member internal this.InternalKey =
        match item with
        | ActionItem node -> node.ContentKey
        | Separator -> ItemKey.Separator
        | Nothing -> ItemKey.Nothing
    member internal this.AddTo (toolBar: ToolBar.Handle) =
        match item with
        | ActionItem node ->
            toolBar.AddAction(node.Action)
        | Separator ->
            toolBar.AddSeparator()
            |> ignore // we don't do anything with the returned action - hopefully Qt owns it and we're not leaking?
        | Nothing ->
            ()

type Separator<'msg>() =
    inherit ToolBarItem<'msg>(InternalItem.Separator)
    

type private Model<'msg>(dispatch: 'msg -> unit) as this =
    let mutable toolBar = ToolBar.Create(this)
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable currentMask = enum<ToolBar.SignalMask> 0
    
    let signalDispatch (s: Signal) =
        signalMap s
        |> Option.iter dispatch
        
    member this.ToolBar = toolBar
    member this.SignalMap with set value = signalMap <- value
    
    member this.SignalMask with set value =
        if value <> currentMask then
            toolBar.SetSignalMask(value)
            currentMask <- value
    
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | AllowedAreas areas ->
                toolBar.SetAllowedAreas(toQtAreas areas)
            | Floatable floatable ->
                toolBar.SetFloatable(floatable)
            | IconSize size ->
                toolBar.SetIconSize(size.QtValue)
            | Movable value ->
                toolBar.SetMovable(value)
            | Orientation value ->
                toolBar.SetOrientation(value.QtValue)
            | ToolButtonStyle style ->
                toolBar.SetToolButtonStyle(style.QtValue)
                
    interface ToolBar.SignalHandler with
        member this.ActionTriggered action =
            signalDispatch (ActionProxy(action) |> ActionTriggered)
        member this.AllowedAreasChanged allowed =
            signalDispatch (fromQtAreas allowed |> AllowedAreasChanged)
        member this.IconSizeChanged size =
            signalDispatch (Size.From size |> IconSizeChanged)
        member this.MovableChanged movable =
            signalDispatch (MovableChanged movable)
        member this.OrientationChanged orient =
            signalDispatch (Orientation.From orient |> OrientationChanged)
        member this.ToolButtonStyleChanged style =
            signalDispatch (ToolButtonStyle.From style |> ToolButtonStyleChanged)
        member this.TopLevelChanged topLevel =
            signalDispatch (TopLevelChanged topLevel)
        member this.VisibilityChanged visible =
            signalDispatch (VisibilityChanged visible)
            
    member this.AttachDeps (items: ToolBarItem<'msg> list) =
        for item in items do
            item.AddTo toolBar
            
    member this.Refill (items: ToolBarItem<'msg> list) =
        toolBar.Clear()
        for item in items do
            item.AddTo toolBar
            
    interface IDisposable with
        member this.Dispose() =
            toolBar.Dispose()

let private create (attrs: Attr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (signalMask: ToolBar.SignalMask) =
    let model = new Model<'msg>(dispatch)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private migrate (model: Model<'msg>) (attrs: Attr list) (signalMap: Signal -> 'msg option) (signalMask: ToolBar.SignalMask) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()

type ToolBar<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    member val Attrs: Attr list = [] with get, set
    member val Items: ToolBarItem<'msg> list = [] with get, set
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    let mutable signalMask = enum<ToolBar.SignalMask> 0
    
    let mutable onActionTriggered: (ActionProxy -> 'msg) option = None
    let mutable onAllowedAreasChanged: (Set<Area> -> 'msg) option = None
    let mutable onIconSizeChanged: (Size -> 'msg) option = None
    let mutable onMovableChanged: (bool -> 'msg) option = None
    let mutable onOrientationChanged: (Orientation -> 'msg) option = None
    let mutable onToolButtonStyleChanged: (ToolButtonStyle -> 'msg) option = None
    let mutable onTopLevelChanged: (bool -> 'msg) option = None
    let mutable onVisibilityChanged: (bool -> 'msg) option = None
    
    let signalMap = function
        | ActionTriggered action ->
            onActionTriggered
            |> Option.map (fun f -> f action)
        | AllowedAreasChanged allowed ->
            onAllowedAreasChanged
            |> Option.map (fun f -> f allowed)
        | IconSizeChanged size ->
            onIconSizeChanged
            |> Option.map (fun f -> f size)
        | MovableChanged value ->
            onMovableChanged
            |> Option.map (fun f -> f value)
        | OrientationChanged orient ->
            onOrientationChanged
            |> Option.map (fun f -> f orient)
        | ToolButtonStyleChanged style ->
            onToolButtonStyleChanged
            |> Option.map (fun f -> f style)
        | TopLevelChanged topLevel ->
            onTopLevelChanged
            |> Option.map (fun f -> f topLevel)
        | VisibilityChanged visible ->
            onVisibilityChanged
            |> Option.map (fun f -> f visible)
            
    member private this.MigrateContent(leftToolBar: ToolBar<'msg>) =
        let leftContents =
            leftToolBar.Items
            |> List.map (_.InternalKey)
        let thisContents =
            this.Items
            |> List.map (_.InternalKey)
        if leftContents <> thisContents then
            this.model.Refill this.Items
        else
            ()
                
    interface IToolBarNode<'msg> with
        override this.Dependencies =
            this.Items
            |> List.zipWithIndex
            |> List.choose (fun (i, item) ->
                item.MaybeNode
                |> Option.map (fun node -> IntKey i, node :> IBuilderNode<'msg>))

        override this.Create dispatch buildContext =
            this.model <- create this.Attrs signalMap dispatch signalMask
            
        override this.AttachDeps () =
            this.model.AttachDeps this.Items

        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> ToolBar<'msg>)
            let nextAttrs = diffAttrs left'.Attrs this.Attrs |> createdOrChanged
            this.model <- migrate left'.model nextAttrs signalMap signalMask
            // instead of complicated .MigrateContent,
            // why don't we just see if depsChanges is all Unchanged?
            // I guess .MigrateContent was originally from the VBox in the early days ...
            this.MigrateContent(left')

        override this.Dispose() =
            (this.model :> IDisposable).Dispose()

        override this.ToolBar =
            this.model.ToolBar
            
        override this.ContentKey =
            this.model.ToolBar
            
        override this.Attachments =
            this.Attachments
