module Widgets.Menus.MenuBar

open System
open BuilderNode
open Org.Whatever.QtTesting

type Signal =
    | Triggered of actionName: string

type Attr =
    | NoneYet
let private attrKey = function
    | NoneYet -> 0
    
let private diffAttrs =
    genericDiffAttrs attrKey

type private Model<'msg>(dispatch: 'msg -> unit, menus: Menu.Handle list) =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable menuBar = MenuBar.Create()
    do
        for menu in menus do
            menuBar.AddMenu(menu)

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
    
let private create (attrs: Attr list) (menus: Menu.Handle list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) =
    let model = new Model<'msg>(dispatch, menus)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model

let private migrate (model: Model<'msg>) (attrs: Attr list) (signalMap: Signal -> 'msg option) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()
    
type Node<'msg>() =
    inherit MenuBarNode<'msg>()
    let mutable menus: MenuNode<'msg> list = []
    
    [<DefaultValue>] val mutable private model: Model<'msg>
    member val Attrs: Attr list = [] with get, set
    let mutable onTriggered: (string -> 'msg) option = None
    member this.OnTriggered
        with set value =
            onTriggered <- Some value
    member private this.SignalMap
        with get() = function
            | Triggered actionName ->
                onTriggered
                |> Option.map (fun f -> f actionName)
    member this.Menus
        with get() = menus
        and set value = menus <- value
    override this.Dependencies() =
        // see long note on same BoxLayout method
        menus
        |> List.mapi (fun i menu -> (IntKey i, menu :> BuilderNode<'msg>))
    override this.Create(dispatch: 'msg -> unit) =
        let menuHandles =
            menus
            |> List.map (_.Menu)
        this.model <- create this.Attrs menuHandles this.SignalMap dispatch
    member private this.MigrateContent (leftMenuBar: Node<'msg>) =
        let leftMenus =
            leftMenuBar.Menus
            |> List.map (_.ContentKey)
        let thisMenus =
            menus
            |> List.map (_.ContentKey)
        if leftMenus <> thisMenus then
            let menuHandles =
                menus
                |> List.map (_.Menu)
            this.model.Refill(menuHandles)
        else
            ()
    override this.MigrateFrom(left: BuilderNode<'msg>) =
        let leftNode = (left :?> Node<'msg>)
        let nextAttrs =
            diffAttrs leftNode.Attrs this.Attrs
            |> createdOrChanged
        this.model <- migrate leftNode.model nextAttrs this.SignalMap
        this.MigrateContent(leftNode)
    override this.Dispose() =
        (this.model :> IDisposable).Dispose()
    override this.MenuBar =
        this.model.MenuBar
        
let make (attrs: Attr list) (menus: MenuNode<'msg> list) =
    Node(Attrs = attrs, Menus = menus)
   
