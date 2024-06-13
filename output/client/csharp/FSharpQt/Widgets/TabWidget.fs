module FSharpQt.Widgets.TabWidget

open System
open FSharpQt.BuilderNode
open Org.Whatever.QtTesting

type Signal =
    | CurrentChanged of index: int
    
type Attr =
    | NoneYet
    
let private keyFunc = function
    | NoneYet -> 0
    
let private diffAttrs =
    genericDiffAttrs keyFunc
    
type private Model<'msg>(dispatch: 'msg -> unit) =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable tabWidget = TabWidget.Create()
    
    let addPages(pages: (string * Widget.Handle) list) =
        for label, widget in pages do
            tabWidget.AddTab(widget, label)
    do
        let signalDispatch (s: Signal) =
            match signalMap s with
            | Some msg ->
                dispatch msg
            | None ->
                ()
        tabWidget.OnCurrentChanged (fun index ->
            signalDispatch (CurrentChanged index))
        
    member this.Refill(pages: (string * Widget.Handle) list) =
        tabWidget.Clear()
        addPages pages
        
    member this.Widget with get() = tabWidget
    member this.SignalMap with set(value) = signalMap <- value
    
    member this.ApplyAttrs(attrs: Attr list) =
        attrs |> List.iter (function
            | NoneYet ->
                ())
        
    interface IDisposable with
        member this.Dispose() =
            tabWidget.Dispose()

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

type TabWidget<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>

    member val Attrs: Attr list = [] with get, set
    member val Pages: (string * IWidgetNode<'msg>) list = [] with get, set
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    let mutable onCurrentChanged: (int -> 'msg) option = None
    member this.OnCurrentChanged with set value = onCurrentChanged <- Some value
    
    let signalMap = function
        | CurrentChanged index ->
            onCurrentChanged
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
            
        override this.Create2 dispatch buildContext =
            this.model <- create this.Attrs signalMap dispatch
            
        override this.AttachDeps () =
            let pageLabelsAndHandles =
                this.Pages
                |> List.map (fun (label, widget) -> label, widget.Widget)
            this.model.Refill(pageLabelsAndHandles)
            
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> TabWidget<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged
            this.model <- migrate left'.model nextAttrs signalMap
            this.MigrateContent(left')
            
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()

        override this.Widget =
            this.model.Widget
            
        override this.ContentKey =
            (this :> IWidgetNode<'msg>).Widget
            
        override this.Attachments =
            this.Attachments
