﻿module FSharpQt.Widgets.MainWindow

open System
open FSharpQt.BuilderNode
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

type private Model<'msg>(dispatch: 'msg -> unit, maybeMenuBar: MenuBar.Handle option, maybeContentNode: IWidgetNode<'msg> option) = // we use node for content because we need to invoke 'attachedToWindow'
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
        
        maybeContentNode
        |> Option.iter (fun node ->
            mainWindow.SetCentralWidget node.Widget
            node.AttachedToWindow mainWindow)
        
        maybeMenuBar
        |> Option.iter mainWindow.SetMenuBar
        
        // always show by default
        // hopefully this won't flicker if users want them hidden initially, but we can attend to that later
        mainWindow.Show()
        
    member this.RemoveMenuBar() =
        printfn "*** not currently possible to remove MenuBar from Window ***"
    
    member this.AddMenuBar(menuBar: MenuBar.Handle) =
        mainWindow.SetMenuBar(menuBar)
        
    member this.RemoveContent() =
        mainWindow.SetCentralWidget(null) // sufficient?
        
    member this.AddContent(node: IWidgetNode<'msg>) =
        mainWindow.SetCentralWidget(node.Widget)
        // the below is for dialogs ... and probably nothing else! sigh
        node.AttachedToWindow mainWindow
        
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

let private create (attrs: Attr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (maybeMenuBar: MenuBar.Handle option) (maybeContentNode: IWidgetNode<'msg> option) =
    let model = new Model<'msg>(dispatch, maybeMenuBar, maybeContentNode)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model

let private migrate (model: Model<'msg>) (attrs: Attr list) (signalMap: Signal -> 'msg option) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()

type MainWindow<'msg>() =
    let mutable maybeMenuBar: IMenuBarNode<'msg> option = None
    let mutable maybeContent: IWidgetNode<'msg> option = None
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

        override this.Create(dispatch: 'msg -> unit) =
            let maybeMenuBarHandle =
                maybeMenuBar
                |> Option.map (_.MenuBar)
            this.model <- create this.Attrs this.SignalMap dispatch maybeMenuBarHandle maybeContent

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
        override this.AttachedToWindow window =
            // obviously a window isn't going to be contained in another window,
            // but due to how the 'WithDialogs' type currently works, it manually calls this on top-level windows (with themselves as argument)
            // so we need to propagate down to content, so dialogs declared internally to a window's content tree will be attached
            match maybeContent with
            | Some content ->
                content.AttachedToWindow window
            | None ->
                ()