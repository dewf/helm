module FSharpQt.Widgets.Menus.MenuBar

open System
open FSharpQt.BuilderNode
open FSharpQt.MiscTypes
open Org.Whatever.QtTesting

type Signal =
    | Hovered of action: ActionProxy
    | Triggered of action: ActionProxy

type Attr =
    | NoneYet
    
let private attrKey = function
    | NoneYet -> 0
    
let private diffAttrs =
    genericDiffAttrs attrKey

type private Model<'msg>(dispatch: 'msg -> unit) as this =
    let mutable menuBar = MenuBar.Create(this)
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable currentMask = enum<MenuBar.SignalMask> 0
    
    let signalDispatch (s: Signal) =
        match signalMap s with
        | Some msg ->
            dispatch msg
        | None ->
            ()
            
    member this.MenuBar with get() = menuBar
    member this.SignalMap with set value = signalMap <- value
    
    member this.SignalMask with set value =
        if value <> currentMask then
            menuBar.SetSignalMask(value)
            currentMask <- value
    
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | NoneYet ->
                ()

    member this.Refill(menus: Menu.Handle list) =
        menuBar.Clear()
        for menu in menus do
            menuBar.AddMenu(menu)
            
    interface MenuBar.SignalHandler with
        override this.Hovered action =
            signalDispatch (ActionProxy(action) |> Hovered)
        override this.Triggered action =
            signalDispatch (ActionProxy(action) |> Triggered)
                
    interface IDisposable with
        member this.Dispose() =
            menuBar.Dispose()
    
let private create (attrs: Attr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (signalMask: MenuBar.SignalMask) =
    let model = new Model<'msg>(dispatch)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private migrate (model: Model<'msg>) (attrs: Attr list) (signalMap: Signal -> 'msg option) (signalMask: MenuBar.SignalMask) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()
    
type MenuBar<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    member val Attrs: Attr list = [] with get, set
    member val Menus: IMenuNode<'msg> list = [] with get, set
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    let mutable signalMask = enum<MenuBar.SignalMask> 0
    
    let mutable onHovered: (ActionProxy -> 'msg) option = None
    let mutable onTriggered: (ActionProxy -> 'msg) option = None

    member this.OnHovered with set value =
        onHovered <- Some value
        signalMask <- signalMask ||| MenuBar.SignalMask.Hovered
        
    member this.OnTriggered with set value =
        onTriggered <- Some value
        signalMask <- signalMask ||| MenuBar.SignalMask.Triggered
            
    let signalMap = function
        | Hovered action ->
            onHovered
            |> Option.map (fun f -> f action)
        | Triggered action ->
            onTriggered
            |> Option.map (fun f -> f action)
                
    member private this.MigrateContent (leftMenuBar: MenuBar<'msg>) =
        let leftKeys =
            leftMenuBar.Menus
            |> List.map (_.ContentKey)
        let thisKeys =
            this.Menus
            |> List.map (_.ContentKey)
        if leftKeys <> thisKeys then
            let menuHandles =
                this.Menus
                |> List.map (_.Menu)
            this.model.Refill(menuHandles)
        else
            ()
            
    interface IMenuBarNode<'msg> with
        override this.Dependencies =
            // see long note on same BoxLayout method
            this.Menus
            |> List.mapi (fun i menu -> (IntKey i, menu :> IBuilderNode<'msg>))
            
        override this.Create dispatch buildContext =
            this.model <- create this.Attrs signalMap dispatch signalMask
            
        override this.AttachDeps() =
            let menuHandles =
                this.Menus
                |> List.map (_.Menu)
            this.model.Refill(menuHandles)
            
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let leftNode = (left :?> MenuBar<'msg>)
            let nextAttrs =
                diffAttrs leftNode.Attrs this.Attrs
                |> createdOrChanged
            this.model <- migrate leftNode.model nextAttrs signalMap signalMask
            this.MigrateContent(leftNode)
            
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()
            
        override this.MenuBar =
            this.model.MenuBar
            
        override this.ContentKey =
            (this :> IMenuBarNode<'msg>).MenuBar
        
        override this.Attachments =
            this.Attachments
