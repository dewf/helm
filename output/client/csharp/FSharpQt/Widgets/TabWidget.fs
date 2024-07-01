module FSharpQt.Widgets.TabWidget

open System
open FSharpQt.BuilderNode
open Org.Whatever.QtTesting

type Signal =
    | CurrentChanged of index: int
    | TabBarClicked of index: int
    | TabBarDoubleClicked of index: int
    | TabCloseRequested of index: int
    
type Attr =
    | NoneYet
    
let private keyFunc = function
    | NoneYet -> 0
    
let private diffAttrs =
    genericDiffAttrs keyFunc
    
type private Model<'msg>(dispatch: 'msg -> unit) as this =
    let mutable tabWidget = TabWidget.Create(this)
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable currentMask = enum<TabWidget.SignalMask> 0
    
    let signalDispatch (s: Signal) =
        match signalMap s with
        | Some msg ->
            dispatch msg
        | None ->
            ()
        
    member this.Widget with get() = tabWidget
    member this.SignalMap with set value = signalMap <- value
    
    member this.SignalMask with set value =
        if value <> currentMask then
            tabWidget.SetSignalMask(value)
            currentMask <- value
            
    member this.ApplyAttrs(attrs: Attr list) =
        attrs |> List.iter (function
            | NoneYet ->
                ())
    
    member this.Refill(pages: (string * Widget.Handle) list) =
        tabWidget.Clear()
        for label, widget in pages do
            tabWidget.AddTab(widget, label)
            
    interface TabWidget.SignalHandler with
        member this.CurrentChanged index =
            signalDispatch (CurrentChanged index)
        member this.TabBarClicked index =
            signalDispatch (TabBarClicked index)
        member this.TabBarDoubleClicked index =
            signalDispatch (TabBarDoubleClicked index)
        member this.TabCloseRequested index =
            signalDispatch (TabCloseRequested index)
        
    interface IDisposable with
        member this.Dispose() =
            tabWidget.Dispose()

let private create (attrs: Attr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (signalMask: TabWidget.SignalMask) =
    let model = new Model<'msg>(dispatch)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private migrate (model: Model<'msg>) (attrs: Attr list) (signalMap: Signal -> 'msg option) (signalMask: TabWidget.SignalMask) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()

type TabWidget<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>

    member val Attrs: Attr list = [] with get, set
    member val Pages: (string * IWidgetNode<'msg>) list = [] with get, set
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    let mutable signalMask = enum<TabWidget.SignalMask> 0
    
    let mutable onCurrentChanged: (int -> 'msg) option = None
    let mutable onTabBarClicked: (int -> 'msg) option = None
    let mutable onTabBarDoubleClicked: (int -> 'msg) option = None
    let mutable onTabCloseRequested: (int -> 'msg) option = None
    
    member this.OnCurrentChanged with set value =
        onCurrentChanged <- Some value
        signalMask <- signalMask ||| TabWidget.SignalMask.CurrentChanged
        
    member this.OnTabBarClicked with set value =
        onTabBarClicked <- Some value
        signalMask <- signalMask ||| TabWidget.SignalMask.TabBarClicked
        
    member this.OnTabBarDoubleClicked with set value =
        onTabBarDoubleClicked <- Some value
        signalMask <- signalMask ||| TabWidget.SignalMask.TabBarDoubleClicked
        
    member this.OnTabCloseRequested with set value =
        onTabCloseRequested <- Some value
        signalMask <- signalMask ||| TabWidget.SignalMask.TabCloseRequested

    let signalMap = function
        | CurrentChanged index ->
            onCurrentChanged
            |> Option.map (fun f -> f index)
        | TabBarClicked index ->
            onTabBarClicked
            |> Option.map (fun f -> f index)
        | TabBarDoubleClicked index ->
            onTabBarDoubleClicked
            |> Option.map (fun f -> f index)
        | TabCloseRequested index ->
            onTabCloseRequested
            |> Option.map (fun f -> f index)
            
    member private this.MigrateContent(leftTabWidget: TabWidget<'msg>) =
        let leftContents =
            leftTabWidget.Pages
            |> List.map (fun (label, node) -> label, node.ContentKey)
        let thisContents =
            this.Pages
            |> List.map (fun (label, node) -> label, node.ContentKey)
        if leftContents <> thisContents then
            let pageLabelsAndHandles =
                this.Pages
                |> List.map (fun (label, node) -> label, node.Widget)
            this.model.Refill(pageLabelsAndHandles)
        else
            ()
        
    interface IWidgetNode<'msg> with
        override this.Dependencies =
            // as usual, this is order-based (since the outside 'user' is not providing keys)
            // ... we should probably switch to string keys anyway, and/or rethink how widgets can survive reorderings
            // seems silly to needlessly destroy/create things just because the order changed and the user didn't provide keys,
            // especially when .ContentKeys exist
            this.Pages
            |> List.mapi (fun i (_, node) -> IntKey i, node :> IBuilderNode<'msg>)
            
        override this.Create dispatch buildContext =
            this.model <- create this.Attrs signalMap dispatch signalMask
            
        override this.AttachDeps () =
            let pageLabelsAndHandles =
                this.Pages
                |> List.map (fun (label, widget) -> label, widget.Widget)
            this.model.Refill(pageLabelsAndHandles)
            
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> TabWidget<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged__old
            this.model <- migrate left'.model nextAttrs signalMap signalMask
            this.MigrateContent(left')
            
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()

        override this.Widget =
            this.model.Widget
            
        override this.ContentKey =
            (this :> IWidgetNode<'msg>).Widget
            
        override this.Attachments =
            this.Attachments
