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

type private Model<'msg>(dispatch: 'msg -> unit, maybeMenuBar: MenuBar.Handle option, maybeWidget: Widget.Handle option, dialogs: Dialog.Handle list) =
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
        
        dialogs
        |> List.iter (_.SetParentDialogFlags(mainWindow)) // special version of .setParent which also sets the Qt::Dialog window flags, without which window modals will not work
        
        // always show by default
        // hopefully this won't flicker if users want them hidden initially, but we can attend to that later
        mainWindow.Show()
        
    member this.RemoveMenuBar() =
        printfn "*** not currently possible to remove MenuBar from Window ***"
    
    member this.AddMenuBar(menuBar: MenuBar.Handle) =
        mainWindow.SetMenuBar(menuBar)
        
    member this.RemoveContent() =
        mainWindow.SetCentralWidget(null) // sufficient?
        
    member this.AddContent(widget: Widget.Handle) =
        mainWindow.SetCentralWidget(widget)
        
    member this.ReattachDialogs(dialogs: Dialog.Handle list) =
        printfn "*** MainWindow.ReattachDialogs not yet implemented"

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

let private create (attrs: Attr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (maybeMenuBar: MenuBar.Handle option) (maybeWidget: Widget.Handle option) (dialogs: Dialog.Handle list) =
    let model = new Model<'msg>(dispatch, maybeMenuBar, maybeWidget, dialogs)
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
    let mutable attachedDialogs: (string * IDialogNode<'msg>) list = []
    let mutable onTitleChanged: (string -> 'msg) option = None

    [<DefaultValue>] val mutable private model: Model<'msg>
    member private this.SignalMap
        with get() = function
            | TitleChanged title ->
                onTitleChanged
                |> Option.map (fun f -> f title)
    member val Attrs: Attr list = [] with get, set
    member this.OnTitleChanged with set value = onTitleChanged <- Some value
    member this.Content with set value = maybeContent <- Some value
    member this.MenuBar with set value = maybeMenuBar <- Some value
    member this.Dialogs with set value = attachedDialogs <- value
    
    member private this.MigrateContent (changeMap: Map<DepsKey, DepsChange>) =
        match changeMap.TryFind (StrKey "menu") with
        | Some change ->
            match change with
            | Unchanged ->
                ()
            | Added ->
                this.model.AddMenuBar(maybeMenuBar.Value.MenuBar)
            | Removed ->
                this.model.RemoveMenuBar()
            | Swapped ->
                this.model.RemoveMenuBar()
                this.model.AddMenuBar(maybeMenuBar.Value.MenuBar)
        | None ->
            // neither side had a menubar
            ()
        
        match changeMap.TryFind (StrKey "content") with
        | Some change ->
            match change with
            | Unchanged ->
                ()
            | Added ->
                this.model.AddContent(maybeContent.Value.Widget)
            | Removed ->
                this.model.RemoveContent()
            | Swapped ->
                this.model.RemoveContent()
                this.model.AddContent(maybeContent.Value.Widget)
        | None ->
            // neither side had 'content'
            ()
            
        // migrating attached dialogs not yet handled
        // (we can .Create with them but not yet .MigrateFrom)
        
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
            let dialogsList =
                attachedDialogs
                |> List.map (fun (name, node) -> (StrKey $"dlg_{name}", node :> IBuilderNode<'msg>)) // prefixed just so they don't collide with internal stuff (menu/content, if for some reason you named your dialogs that)
            menuBarList @ contentList @ dialogsList

        override this.Create(dispatch: 'msg -> unit) =
            let maybeMenuBarHandle =
                maybeMenuBar
                |> Option.map (_.MenuBar)
            let maybeWidgetHandle =
                maybeContent
                |> Option.map (_.Widget)
            let dialogHandles =
                attachedDialogs
                |> List.map (fun (_, node) -> node.Dialog)
            this.model <- create this.Attrs this.SignalMap dispatch maybeMenuBarHandle maybeWidgetHandle dialogHandles

        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> Node<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged
            this.model <- migrate left'.model nextAttrs this.SignalMap
            this.MigrateContent (depsChanges |> Map.ofList)

        override this.Dispose() =
            (this.model :> IDisposable).Dispose()
        override this.WindowWidget =
            this.model.Widget
        override this.ContentKey =
            (this :> IWindowNode<'msg>).WindowWidget
            
    interface IDialogParent<'msg> with
        override this.AttachedDialogs = attachedDialogs
            
