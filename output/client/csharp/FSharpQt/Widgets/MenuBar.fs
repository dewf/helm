module FSharpQt.Widgets.MenuBar

open System
open FSharpQt.BuilderNode
open Org.Whatever.QtTesting

open FSharpQt.Attrs
open FSharpQt.MiscTypes

type Signal =
    | Hovered of action: ActionProxy
    | Triggered of action: ActionProxy

type Attr =
    | DefaultUp of state: bool
    | NativeMenuBar of state: bool
with
    interface IAttr with
        override this.AttrEquals other =
            match other with
            | :? Attr as otherAttr ->
                this = otherAttr
            | _ ->
                false
        override this.Key =
            match this with
            | DefaultUp _ -> "menubar:defaultup"
            | NativeMenuBar _ -> "menubar:nativemenubar"
        override this.ApplyTo (target: IAttrTarget, maybePrev: IAttr option) =
            match target with
            | :? MenuBarAttrTarget as attrTarget ->
                let menuBar =
                    attrTarget.MenuBar
                match this with
                | DefaultUp state ->
                    menuBar.SetDefaultUp(state)
                | NativeMenuBar state ->
                    menuBar.SetNativeMenuBar(state)
            | _ ->
                printfn "warning: MenuBar.Attr couldn't ApplyTo() unknown object type [%A]" target
                
type Props<'msg>() =
    inherit Widget.Props<'msg>()
    
    let mutable onHovered: (ActionProxy -> 'msg) option = None
    let mutable onTriggered: (ActionProxy -> 'msg) option = None
    
    member internal this.SignalMask = enum<MenuBar.SignalMask> (int this._signalMask)

    member this.OnHovered with set value =
        onHovered <- Some value
        this.AddSignal(int MenuBar.SignalMask.Hovered)
        
    member this.OnTriggered with set value =
        onTriggered <- Some value
        this.AddSignal(int MenuBar.SignalMask.Triggered)
            
    member internal this.SignalMap = function
        | Hovered action ->
            onHovered
            |> Option.map (fun f -> f action)
        | Triggered action ->
            onTriggered
            |> Option.map (fun f -> f action)
    
    member this.DefaultUp with set value =
        this.PushAttr(DefaultUp value)
        
    member this.NativeMenuBar with set value =
        this.PushAttr(NativeMenuBar value)
    
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
    
    member this.ApplyAttrs(attrs: (IAttr option * IAttr) list) =
        for maybePrev, attr in attrs do
            attr.ApplyTo(this, maybePrev)

    member this.Refill(menus: Menu.Handle list) =
        menuBar.Clear()
        for menu in menus do
            menuBar.AddMenu(menu)
            
    interface MenuBarAttrTarget with
        member this.Widget = menuBar
        member this.MenuBar = menuBar
            
    interface MenuBar.SignalHandler with
        override this.Hovered action =
            signalDispatch (ActionProxy(action) |> Hovered)
        override this.Triggered action =
            signalDispatch (ActionProxy(action) |> Triggered)
                
    interface IDisposable with
        member this.Dispose() =
            menuBar.Dispose()
    
let private create (attrs: IAttr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (signalMask: MenuBar.SignalMask) =
    let model = new Model<'msg>(dispatch)
    model.ApplyAttrs (attrs |> List.map (fun attr -> None, attr))
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private migrate (model: Model<'msg>) (attrs: (IAttr option * IAttr) list) (signalMap: Signal -> 'msg option) (signalMask: MenuBar.SignalMask) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()
    
type MenuBar<'msg>() =
    inherit Props<'msg>()
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    member val Menus: IMenuNode<'msg> list = [] with get, set
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
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
            this.model <- create this.Attrs this.SignalMap dispatch this.SignalMask
            
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
            this.model <- migrate leftNode.model nextAttrs this.SignalMap this.SignalMask
            this.MigrateContent(leftNode)
            
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()
            
        override this.MenuBar =
            this.model.MenuBar
            
        override this.ContentKey =
            (this :> IMenuBarNode<'msg>).MenuBar
        
        override this.Attachments =
            this.Attachments
