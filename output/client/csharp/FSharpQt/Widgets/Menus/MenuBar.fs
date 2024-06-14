module FSharpQt.Widgets.Menus.MenuBar

open System
open FSharpQt.BuilderNode
open Org.Whatever.QtTesting

type Signal =
    | Triggered of actionName: string

type Attr =
    | NoneYet
let private attrKey = function
    | NoneYet -> 0
    
let private diffAttrs =
    genericDiffAttrs attrKey

type private Model<'msg>(dispatch: 'msg -> unit) =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable menuBar = MenuBar.Create()
    do
        let signalDispatch (s: Signal) =
            match signalMap s with
            | Some msg ->
                dispatch msg
            | None ->
                ()
        menuBar.OnTriggered (fun action ->
            signalDispatch (action.GetText() |> Triggered))
        
    member this.MenuBar with get() = menuBar
    member this.SignalMap with set value = signalMap <- value
    
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | NoneYet ->
                ()
                
    interface IDisposable with
        member this.Dispose() =
            menuBar.Dispose()
            
    member this.Refill(menus: Menu.Handle list) =
        menuBar.Clear()
        for menu in menus do
            menuBar.AddMenu(menu)
    
let private create (attrs: Attr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) =
    let model = new Model<'msg>(dispatch)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model

let private migrate (model: Model<'msg>) (attrs: Attr list) (signalMap: Signal -> 'msg option) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()
    
type MenuBar<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    member val Attrs: Attr list = [] with get, set
    member val Menus: IMenuNode<'msg> list = [] with get, set
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    let mutable onTriggered: (string -> 'msg) option = None
    member this.OnTriggered
        with set value =
            onTriggered <- Some value
            
    let signalMap = function
        | Triggered actionName ->
            onTriggered
            |> Option.map (fun f -> f actionName)
                
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
            this.model <- create this.Attrs signalMap dispatch
            
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
            this.model <- migrate leftNode.model nextAttrs signalMap
            this.MigrateContent(leftNode)
            
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()
            
        override this.MenuBar =
            this.model.MenuBar
            
        override this.ContentKey =
            (this :> IMenuBarNode<'msg>).MenuBar
        
        override this.Attachments =
            this.Attachments
