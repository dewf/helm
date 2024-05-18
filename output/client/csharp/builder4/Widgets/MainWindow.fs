module Widgets.MainWindow

open System
open BuilderNode
open Org.Whatever.QtTesting

type Signal =
    | TitleChanged of title: string
    
type Attr =
    | Title of title: string
    | Size of width: int * height: int
    | Visible of state: bool
let private keyFunc = function
    | Title _ -> 0
    | Size _ -> 1
    | Visible _ -> 2
let private diffAttrs =
    genericDiffAttrs keyFunc

// let addRemoveOptional<'msg, 'node when 'node :> BuilderNode<'msg>> (maybeLeftThing: 'node option) (maybeThisThing: 'node option) (accessor: 'node -> 'handle) (adder: 'handle -> unit) (remover: unit -> unit) =
//     let leftKey =
//         maybeLeftThing
//         |> Option.map (_.ContentKey)
//     let thisKey =
//         maybeThisThing
//         |> Option.map (_.ContentKey)
//     match leftKey, thisKey with
//     | None, None ->
//         // both no content
//         ()
//     | Some x, Some y when x = y ->
//         // same content
//         ()
//     | None, Some _ ->
//         // added from nothing
//         maybeThisThing
//         |> Option.iter (accessor >> adder)
//     | Some _, None ->
//         // removed
//         remover()
//     | Some _, Some _ -> // implicit "when x <> y" because of first case
//         // changed
//         remover()
//         maybeThisThing
//         |> Option.iter (accessor >> adder)

type private Model<'msg>(dispatch: 'msg -> unit, maybeMenuBar: MenuBar.Handle option, maybeEntity: LayoutEntity option) =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable central = Widget.Create() // in case a layout is provided, we need a central widget to stuff it in
    let mutable mainWindow = MainWindow.Create()
    do
        let signalDispatch (s: Signal) =
            match signalMap s with
            | Some msg ->
                dispatch msg
            | None ->
                ()
        mainWindow.OnWindowTitleChanged (fun title ->
            signalDispatch (TitleChanged title))
        
        maybeEntity
        |> Option.iter (function
            | WidgetItem w ->
                mainWindow.SetCentralWidget(w)
            | LayoutItem l ->
                central.SetLayout(l)
                mainWindow.SetCentralWidget(central))
        
        maybeMenuBar
        |> Option.iter mainWindow.SetMenuBar
        
        mainWindow.Show()
        
    member this.RemoveMenuBar() =
        printfn "*** not currently possible to remove MenuBar from Window ***"
    
    member this.RemoveContent(leftNode: LayoutItemNode<'msg>) = // we have to provide what it was as a parameter, since removal is type-sensitive
        match leftNode.LayoutEntity with
        | WidgetItem _ ->
            mainWindow.SetCentralWidget(null) // sufficient??
        | LayoutItem _ ->
            let existing =
                central.GetLayout()
            existing.RemoveAll()    // remove the widgets from the layout (thus window) ... but will this cause problems later if for some reason the layout was re-used in another widget? seems an extreme/exotic use case - I would imagine it's getting destroyed
            central.SetLayout(null)
        
    member this.AddMenuBar(menuBar: MenuBar.Handle) =
        mainWindow.SetMenuBar(menuBar)
        
    member this.AddContent(entity: LayoutEntity) =
        match entity with
        | WidgetItem w ->
            mainWindow.SetCentralWidget(w)
        | LayoutItem l ->
            central.SetLayout(l)
            mainWindow.SetCentralWidget(central)

    member this.Widget with get() = mainWindow
    member this.SignalMap with set(value) = signalMap <- value
    member this.ApplyAttrs(attrs: Attr list) =
        attrs |> List.iter (function
            | Title text ->
                mainWindow.SetWindowTitle(text)
            | Size (width, height) ->
                mainWindow.Resize(width, height)
            | Visible state ->
                mainWindow.SetVisible(state))
    interface IDisposable with
        member this.Dispose() =
            mainWindow.Dispose()

let private create (attrs: Attr list) (maybeMenuBar: MenuBar.Handle option) (maybeEntity: LayoutEntity option) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) =
    let model = new Model<'msg>(dispatch, maybeMenuBar, maybeEntity)
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
    inherit WidgetNode<'msg>()
    let mutable maybeMenuBar: MenuBarNode<'msg> option = None
    let mutable maybeEntity: LayoutItemNode<'msg> option = None
    member private this.MaybeMenuBar = maybeMenuBar
    member private this.MaybeEntity = maybeEntity // need to be able to access from migration (does this need to be a function?)

    [<DefaultValue>] val mutable private model: Model<'msg>
    member val Attrs: Attr list = [] with get, set
    let mutable onTitleChanged: (string -> 'msg) option = None
    member this.OnTitleChanged with set value = onTitleChanged <- Some value
    member private this.SignalMap
        with get() = function
            | TitleChanged title ->
                onTitleChanged
                |> Option.map (fun f -> f title)
    member this.Content with set value = maybeEntity <- Some value
    member this.MenuBar with set value = maybeMenuBar <- Some value

    override this.Dependencies() =
        let menuBarList =
            maybeMenuBar
            |> Option.map (fun menuBar -> (0, menuBar :> BuilderNode<'msg>))
            |> Option.toList
        let contentList =
            maybeEntity
            |> Option.map (fun content -> (1, content :> BuilderNode<'msg>))
            |> Option.toList
        menuBarList @ contentList

    override this.Create(dispatch: 'msg -> unit) =
        let maybeMenuBarHandle =
            maybeMenuBar
            |> Option.map (_.MenuBar)
        let maybeEntityHandle =
            maybeEntity
            |> Option.map (_.LayoutEntity)
        this.model <- create this.Attrs maybeMenuBarHandle maybeEntityHandle this.SignalMap dispatch

    member private this.MigrateContent(leftFrame: Node<'msg>) =
        // MENUBAR =====================
        let leftMenuBarKey =
            leftFrame.MaybeMenuBar
            |> Option.map (_.ContentKey)
        let menuBarKey =
            maybeMenuBar
            |> Option.map (_.ContentKey)
        match leftMenuBarKey, menuBarKey with
        | None, None ->
            // both no content
            ()
        | Some x, Some y when x = y ->
            // same content
            ()
        | None, Some _ ->
            // added menubar, from nothing
            this.model.AddMenuBar(maybeMenuBar.Value.MenuBar)
        | Some _, None ->
            // removed menubar
            this.model.RemoveMenuBar()
        | Some _, Some _ -> // implicit "when x <> y" because of first case
            // changed
            this.model.RemoveMenuBar()
            this.model.AddMenuBar(maybeMenuBar.Value.MenuBar)
            
        // CONTENT (LAYOUT/WIDGET) ========================
        let leftContentKey =
            leftFrame.MaybeEntity
            |> Option.map (_.ContentKey)
        let contentKey =
            maybeEntity
            |> Option.map (_.ContentKey)
        match leftContentKey, contentKey with
        | None, None ->
            // both no content
            ()
        | Some x, Some y when x = y ->
            // same content
            ()
        | None, Some _ ->
            // added content, from nothing
            this.model.AddContent(maybeEntity.Value.LayoutEntity)
        | Some _, None ->
            // removed content
            leftFrame.MaybeEntity
            |> Option.iter this.model.RemoveContent
        | Some _, Some _ -> // implicit "when x <> y" because of first case
            // changed content
            leftFrame.MaybeEntity
            |> Option.iter this.model.RemoveContent
            this.model.AddContent(maybeEntity.Value.LayoutEntity)

    override this.MigrateFrom(left: BuilderNode<'msg>) =
        let left' = (left :?> Node<'msg>)
        let nextAttrs =
            diffAttrs left'.Attrs this.Attrs
            |> createdOrChanged
        this.model <- migrate left'.model nextAttrs this.SignalMap
        this.MigrateContent(left')

    override this.Dispose() =
        (this.model :> IDisposable).Dispose()
    override this.Widget =
        this.model.Widget
