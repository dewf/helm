module FSharpQt.Widgets.MainWindow

open System
open FSharpQt.BuilderNode
open Org.Whatever.QtTesting

open FSharpQt
open MiscTypes

type Signal =
    | CustomContextMenuRequested of pos: Point
    | WindowIconChanged of icon: IconProxy
    | WindowTitleChanged of title: string
    | IconSizeChanged of size: Size
    | TabifiedDockWidgetActivated of widget: DockWidgetProxy
    | ToolButtonStyleChanged of style: ToolButtonStyle
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
    
type private Model<'msg>(dispatch: 'msg -> unit) as this =
    let mutable mainWindow = MainWindow.Create(this)
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable currentMask = enum<MainWindow.SignalMask> 0
    
    let mutable syntheticLayoutWidget: Widget.Handle option = None
    let mutable visible = true
    
    let signalDispatch (s: Signal) =
        match signalMap s with
        | Some msg ->
            dispatch msg
        | None ->
            ()
            
    member this.Widget with get() = mainWindow
    member this.SignalMap with set value = signalMap <- value
    
    member this.SignalMask with set value =
        if value <> currentMask then
            mainWindow.SetSignalMask(value)
            currentMask <- value
    
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
        
    member this.RemoveMenuBar() =
        // we can't remove it (especially without a handle, which is no doubt gone because it's been destroyed)
        // but doesn't widget destruction actually remove it for us? hmmm
        ()
    
    member this.AddMenuBar(menuBar: MenuBar.Handle) =
        mainWindow.SetMenuBar(menuBar)
        
    member this.RemoveToolBar() =
        // see note in .RemoveMenuBar() above
        ()
        
    member this.AddToolBar (toolBar: ToolBar.Handle) =
        mainWindow.AddToolBar(toolBar)
        
    member this.RemoveContent() =
        // TODO: need to do some serious testing with all this
        // scrollArea too
        // if we're doing this ... hasn't the content node actually been disposed? so why all the fuss?
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
            let widget = Widget.Create(new NullWidgetHandler())
            widget.SetLayout(layout.Layout)
            mainWindow.SetCentralWidget(widget)
            syntheticLayoutWidget <- Some widget
        | _ ->
            failwith "MainWindow.Model.AddContent - unknown node type"
            
    member this.AddAction(action: Action.Handle) =
        mainWindow.AddAction(action)
        
    member this.ShowIfVisible () =
        // long story short, this is our workaround for Qt's layout system which doesn't necessarily play nicely with how we want to create our widget trees
        // this is invoked in a special step in the builder diff, after .AttachDeps has occurred
        // because a window/widget needs all its content attached first, before the layout will work properly (on .show)
        // during migrations however we can change visibility just fine, the layout has already occurred of course
        if visible then
            mainWindow.Show()
            
    interface MainWindow.SignalHandler with
        member this.CustomContextMenuRequested pos =
            signalDispatch (Point.From pos |> CustomContextMenuRequested)
        member this.WindowIconChanged icon =
            signalDispatch (IconProxy(icon) |> WindowIconChanged)
        member this.WindowTitleChanged title =
            signalDispatch (WindowTitleChanged title)
        member this.IconSizeChanged size =
            signalDispatch (Size.From size |> IconSizeChanged)
        member this.TabifiedDockWidgetActivated widget =
            signalDispatch (DockWidgetProxy(widget) |> TabifiedDockWidgetActivated)
        member this.ToolButtonStyleChanged style =
            signalDispatch (fromQtToolButtonStyle style |> ToolButtonStyleChanged)
        member this.Closed () =
            signalDispatch Closed
    
    interface IDisposable with
        member this.Dispose() =
            // synthetic layout widget out to be automatically disposed (on the C++ side) right?
            mainWindow.Dispose()

let private create2 (attrs: Attr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (signalMask: MainWindow.SignalMask) =
    let model = new Model<'msg>(dispatch)
    model.ApplyAttrs attrs true
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model
    
let private migrate (model: Model<'msg>) (attrs: Attr list) (signalMap: Signal -> 'msg option) (signalMask: MainWindow.SignalMask) =
    model.ApplyAttrs attrs false
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()

type MainWindow<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    member val Attrs: Attr list = [] with get, set
    member val Actions: (string * IActionNode<'msg>) list = [] with get, set
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set

    let mutable maybeContent: IWidgetOrLayoutNode<'msg> option = None
    member this.Content with set value = maybeContent <- Some value
    
    let mutable maybeMenuBar: IMenuBarNode<'msg> option = None
    member this.MenuBar with set value = maybeMenuBar <- Some value
    
    let mutable maybeToolBar: IToolBarNode<'msg> option = None
    member this.ToolBar with set value = maybeToolBar <- Some value
    
    let mutable signalMask = enum<MainWindow.SignalMask> 0
    
    let mutable onCustomContextMenuRequested: (Point -> 'msg) option = None
    let mutable onWindowIconChanged: (IconProxy -> 'msg) option = None
    let mutable onWindowTitleChanged: (string -> 'msg) option = None
    let mutable onIconSizeChanged: (Size -> 'msg) option = None
    let mutable onTabifiedDockWidgetActivated: (DockWidgetProxy -> 'msg) option = None
    let mutable onToolButtonStyleChanged: (ToolButtonStyle -> 'msg) option = None
    let mutable onClosed: 'msg option = None

    member this.OnCustomContextMenuRequested with set value =
        onCustomContextMenuRequested <- Some value
        signalMask <- signalMask ||| MainWindow.SignalMask.CustomContextMenuRequested
        
    member this.OnWindowIconChanged with set value =
        onWindowIconChanged <- Some value
        signalMask <- signalMask ||| MainWindow.SignalMask.WindowIconChanged
        
    member this.OnWindowTitleChanged with set value =
        onWindowTitleChanged <- Some value
        signalMask <- signalMask ||| MainWindow.SignalMask.WindowTitleChanged
        
    member this.OnIconSizeChanged with set value =
        onIconSizeChanged <- Some value
        signalMask <- signalMask ||| MainWindow.SignalMask.IconSizeChanged
        
    member this.OnTabifiedDockWidgetActivated with set value =
        onTabifiedDockWidgetActivated <- Some value
        signalMask <- signalMask ||| MainWindow.SignalMask.TabifiedDockWidgetActivated
        
    member this.OnToolButtonStylechanged with set value =
        onToolButtonStyleChanged <- Some value
        signalMask <- signalMask ||| MainWindow.SignalMask.ToolButtonStyleChanged
        
    member this.OnClosed with set value =
        onClosed <- Some value
        signalMask <- signalMask ||| MainWindow.SignalMask.Closed
        
    let signalMap = function
        | CustomContextMenuRequested pos ->
            onCustomContextMenuRequested
            |> Option.map (fun f -> f pos)
        | WindowIconChanged icon ->
            onWindowIconChanged
            |> Option.map (fun f -> f icon)
        | WindowTitleChanged title ->
            onWindowTitleChanged
            |> Option.map (fun f -> f title)
        | IconSizeChanged size ->
            onIconSizeChanged
            |> Option.map (fun f -> f size)
        | TabifiedDockWidgetActivated widget ->
            onTabifiedDockWidgetActivated
            |> Option.map (fun f -> f widget)
        | ToolButtonStyleChanged style ->
            onToolButtonStyleChanged
            |> Option.map (fun f -> f style)
        | Closed ->
            onClosed
    
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
            
        match changeMap.TryFind (StrKey "toolbar") with
        | Some change ->
            match change with
            | Unchanged ->
                ()
            | Added ->
                this.model.AddToolBar(maybeToolBar.Value.ToolBar)
            | Removed ->
                this.model.RemoveToolBar()
            | Swapped ->
                this.model.RemoveToolBar()
                this.model.AddToolBar(maybeToolBar.Value.ToolBar)
        | None ->
            // neither side had 'toolbar'
            ()
            
        this.Actions
        |> List.iteri (fun i (key, action) ->
            let key =
                StrStrKey("action", key)
            match changeMap.TryFind key with
            | Some change ->
                match change with
                | Unchanged ->
                    ()
                | Removed ->
                    // we'll never see this, since we're only iterating over the actions we have now
                    // that aside, we're trusting that Qt knows how to handle action removals gracefully? otherwise we will have to beef up how we handle migrations perhaps,
                    // and give parent nodes a chance to cleanup just before sub-nodes get disposed?
                    // would making this Removed <contentKey> be helpful? but the widget would already be destroyed ...
                    ()
                | Added ->
                    this.model.AddAction(action.Action)
                | Swapped ->
                    // as above in the removal case, we'll trust that Qt cleaned up whatever previous action was removed at this sub-key
                    // all that remains is to add this one
                    this.model.AddAction(action.Action)
            | None ->
                // can't happen, if it exists now on this node, it will appear in the changeMap as 'Added' at the very least
                failwith "can't happen" )
        
    interface IWindowNode<'msg> with
        override this.Dependencies =
            let menuBarList =
                maybeMenuBar
                |> Option.map (fun menuBar -> StrKey "menu", menuBar :> IBuilderNode<'msg>)
                |> Option.toList
            let contentList =
                maybeContent
                |> Option.map (fun content -> StrKey "content", content :> IBuilderNode<'msg>)
                |> Option.toList
            let toolBarList =
                maybeToolBar
                |> Option.map (fun toolBar -> StrKey "toolbar", toolBar :> IBuilderNode<'msg>)
                |> Option.toList
            let actions =
                this.Actions
                |> List.map (fun (key, action) -> StrStrKey ("action", key), action :> IBuilderNode<'msg>)
            menuBarList @ contentList @ toolBarList @ actions
            
        override this.Create dispatch buildContext =
            this.model <- create2 this.Attrs signalMap dispatch signalMask
            
        override this.AttachDeps () =
            maybeContent
            |> Option.iter this.model.AddContent
            maybeMenuBar
            |> Option.iter (fun node -> this.model.AddMenuBar node.MenuBar)
            maybeToolBar
            |> Option.iter (fun node -> this.model.AddToolBar node.ToolBar)
            this.Actions
            |> List.iter (fun (_, node) -> this.model.AddAction(node.Action))

        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> MainWindow<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged
            this.model <- migrate left'.model nextAttrs signalMap signalMask
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
