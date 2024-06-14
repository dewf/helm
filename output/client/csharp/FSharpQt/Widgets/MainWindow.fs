module FSharpQt.Widgets.MainWindow

open System
open FSharpQt.BuilderNode
open Org.Whatever.QtTesting

type Signal =
    | TitleChanged of title: string
    | Closed
    
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

type private Model<'msg>(dispatch: 'msg -> unit) =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable mainWindow = MainWindow.Create()
    let mutable syntheticLayoutWidget: Widget.Handle option = None
    let mutable visible = true
    do
        let signalDispatch (s: Signal) =
            match signalMap s with
            | Some msg ->
                dispatch msg
            | None ->
                ()
                
        mainWindow.OnWindowTitleChanged (fun title ->
            signalDispatch (TitleChanged title))
        
        mainWindow.OnClosed (fun _ ->
            signalDispatch Closed)
        
    member this.AttachDeps (maybeMenuBar: MenuBar.Handle option) (maybeContentNode: IWidgetOrLayoutNode<'msg> option) = // we use node for content because we need to invoke 'attachedToWindow')
        maybeContentNode
        |> Option.iter this.AddContent
        maybeMenuBar
        |> Option.iter this.AddMenuBar
        
    member this.RemoveMenuBar() =
        printfn "*** not currently possible to remove MenuBar from Window ***"
    
    member this.AddMenuBar(menuBar: MenuBar.Handle) =
        mainWindow.SetMenuBar(menuBar)
        
    member this.RemoveContent() =
        // TODO: need to do some serious testing with all this
        // scrollArea too
        match syntheticLayoutWidget with
        | Some widget ->
            widget.GetLayout().RemoveAll() // detach any children just in case
            widget.SetLayout(null)
            widget.Dispose()
            // deleting should automatically remove from the parent mainWindow, right?
            syntheticLayoutWidget <- None
        | None ->
            mainWindow.SetCentralWidget(null) // sufficient?
        
    member this.AddContent(node: IWidgetOrLayoutNode<'msg>) =
        match node with
        | :? IWidgetNode<'msg> as widgetNode ->
            mainWindow.SetCentralWidget(widgetNode.Widget)
        | :? ILayoutNode<'msg> as layout ->
            let widget = Widget.Create()
            widget.SetLayout(layout.Layout)
            mainWindow.SetCentralWidget(widget)
            syntheticLayoutWidget <- Some widget
        | _ ->
            failwith "MainWindow.Model.AddContent - unknown node type"
        
    member this.Widget with get() = mainWindow
    member this.SignalMap with set(value) = signalMap <- value
    
    member this.ApplyAttrs (attrs: Attr list) (isCreate: bool) =
        attrs |> List.iter (function
            | Title text ->
                mainWindow.SetWindowTitle(text)
            | Size (width, height) ->
                mainWindow.Resize(width, height)
            | Visible state ->
                visible <- state
                if not isCreate then
                    // on creation we just set the flag
                    mainWindow.SetVisible(state))
        
    member this.ShowIfVisible () =
        if visible then
            mainWindow.Show()
    
    interface IDisposable with
        member this.Dispose() =
            // synthetic layout widget out to be automatically disposed (on the C++ side) right?
            mainWindow.Dispose()

let private create2 (attrs: Attr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) =
    let model = new Model<'msg>(dispatch)
    model.ApplyAttrs attrs true
    model.SignalMap <- signalMap
    model
    
let private migrate (model: Model<'msg>) (attrs: Attr list) (signalMap: Signal -> 'msg option) =
    model.ApplyAttrs attrs false
    model.SignalMap <- signalMap
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()

type MainWindow<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    let mutable maybeMenuBar: IMenuBarNode<'msg> option = None
    let mutable maybeContent: IWidgetOrLayoutNode<'msg> option = None
    let mutable onTitleChanged: (string -> 'msg) option = None
    let mutable onClosed: 'msg option = None
    member this.Content with set value = maybeContent <- Some value
    member this.MenuBar with set value = maybeMenuBar <- Some value
    member this.OnTitleChanged with set value = onTitleChanged <- Some value
    member this.OnClosed with set value = onClosed <- Some value
    
    member private this.SignalMap
        with get() = function
            | TitleChanged title ->
                onTitleChanged
                |> Option.map (fun f -> f title)
            | Closed ->
                onClosed
                
    member val Attrs: Attr list = [] with get, set
    
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
                this.model.AddContent(maybeContent.Value)
            | Removed ->
                this.model.RemoveContent()
            | Swapped ->
                this.model.RemoveContent()
                this.model.AddContent(maybeContent.Value)
        | None ->
            // neither side had 'content'
            ()
        
    interface IWindowNode<'msg> with
        override this.Dependencies =
            let menuBarList =
                maybeMenuBar
                |> Option.map (fun menuBar -> (StrKey "menu", menuBar :> IBuilderNode<'msg>))
                |> Option.toList
            let contentList =
                maybeContent
                |> Option.map (fun content -> (StrKey "content", content :> IBuilderNode<'msg>))
                |> Option.toList
            menuBarList @ contentList
            
        override this.Create dispatch buildContext =
            this.model <- create2 this.Attrs this.SignalMap dispatch
            
        override this.AttachDeps () =
            let maybeMenuBarHandle =
                maybeMenuBar
                |> Option.map (_.MenuBar)
            this.model.AttachDeps maybeMenuBarHandle maybeContent

        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> MainWindow<'msg>)
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
            
        override this.Attachments = this.Attachments
        
        override this.ShowIfVisible () =
            this.model.ShowIfVisible()
