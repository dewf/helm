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

type private Model<'msg>(dispatch: 'msg -> unit, maybeMenuBar: MenuBar.Handle option, maybeWidget: Widget.Handle option) =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
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
        
        maybeWidget
        |> Option.iter mainWindow.SetCentralWidget
        
        maybeMenuBar
        |> Option.iter mainWindow.SetMenuBar
        
        // always show by default
        // hopefully this won't flicker if users want them hidden initially, but we can attend to that later
        mainWindow.Show()
        
    member this.RemoveMenuBar() =
        printfn "*** not currently possible to remove MenuBar from Window ***"
    
    member this.RemoveContent() =
        mainWindow.SetCentralWidget(null) // sufficient?
        
    member this.AddMenuBar(menuBar: MenuBar.Handle) =
        mainWindow.SetMenuBar(menuBar)
        
    member this.AddContent(widget: Widget.Handle) =
        mainWindow.SetCentralWidget(widget)

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

let private create (attrs: Attr list) (maybeMenuBar: MenuBar.Handle option) (maybeWidget: Widget.Handle option) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) =
    let model = new Model<'msg>(dispatch, maybeMenuBar, maybeWidget)
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
    let mutable maybeMenuBar: IMenuBarNode<'msg> option = None
    let mutable maybeContent: IWidgetNode<'msg> option = None
    member private this.MaybeMenuBar = maybeMenuBar
    member private this.MaybeContent = maybeContent // need to be able to access from migration (does this need to be a function?)

    [<DefaultValue>] val mutable private model: Model<'msg>
    member val Attrs: Attr list = [] with get, set
    let mutable onTitleChanged: (string -> 'msg) option = None
    member this.OnTitleChanged with set value = onTitleChanged <- Some value
    member private this.SignalMap
        with get() = function
            | TitleChanged title ->
                onTitleChanged
                |> Option.map (fun f -> f title)
    member this.Content with set value = maybeContent <- Some value
    member this.MenuBar with set value = maybeMenuBar <- Some value
    
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
            leftFrame.MaybeContent
            |> Option.map (_.ContentKey)
        let contentKey =
            maybeContent
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
            this.model.AddContent(maybeContent.Value.Widget)
        | Some _, None ->
            // removed content
            this.model.RemoveContent()
        | Some _, Some _ -> // implicit "when x <> y" because of first case
            // changed content
            this.model.RemoveContent()
            this.model.AddContent(maybeContent.Value.Widget)
    
    interface IWindowNode<'msg> with
        override this.Dependencies() =
            let menuBarList =
                maybeMenuBar
                |> Option.map (fun menuBar -> (StrKey "menu", menuBar :> IBuilderNode<'msg>))
                |> Option.toList
            let contentList =
                maybeContent
                |> Option.map (fun content -> (StrKey "content", content :> IBuilderNode<'msg>))
                |> Option.toList
            menuBarList @ contentList

        override this.Create(dispatch: 'msg -> unit) =
            let maybeMenuBarHandle =
                maybeMenuBar
                |> Option.map (_.MenuBar)
            let maybeWidgetHandle =
                maybeContent
                |> Option.map (_.Widget)
            this.model <- create this.Attrs maybeMenuBarHandle maybeWidgetHandle this.SignalMap dispatch

        override this.MigrateFrom(left: IBuilderNode<'msg>) =
            let left' = (left :?> Node<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged
            this.model <- migrate left'.model nextAttrs this.SignalMap
            this.MigrateContent(left')

        override this.Dispose() =
            (this.model :> IDisposable).Dispose()
        override this.WindowWidget =
            this.model.Widget
        override this.ContentKey =
            (this :> IWindowNode<'msg>).WindowWidget
