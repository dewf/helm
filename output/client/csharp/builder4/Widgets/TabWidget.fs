module Widgets.TabWidget

open System
open BuilderNode
open Org.Whatever.QtTesting

type Signal =
    | CurrentChanged of index: int
    
type Attr =
    | NoneYet
let private keyFunc = function
    | NoneYet -> 0
let private diffAttrs =
    genericDiffAttrs keyFunc
    
type private Model<'msg>(dispatch: 'msg -> unit, initPages: (string * LayoutEntity) list) =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable tabWidget = TabWidget.Create()
    let mutable layoutToWidgetMap: Map<Layout.Handle, Widget.Handle> = Map.empty
    
    let widgetForLayout(layout: Layout.Handle) =
        match layoutToWidgetMap.TryFind(layout) with
        | Some widget ->
            // re-use existing
            widget
        | None ->
            // create new
            let widget =
                Widget.Create()
            widget.SetLayout(layout)
            // keep record of it ...
            layoutToWidgetMap <- layoutToWidgetMap.Add(layout, widget)
            // return it
            widget
            
    let addPages(pages: (string * LayoutEntity) list) =
        for label, page in pages do
        match page with
        | WidgetItem widget ->
            tabWidget.AddTab(widget, label)
        | LayoutItem layout ->
            let widget =
                widgetForLayout(layout)
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
        
        addPages initPages
        
    let deleteOrphanedLayoutContainers (incomingPages: (string * LayoutEntity) list) =
        // look through layoutToWidgetMap and locate any layout keys that AREN'T in the incoming set
        // their old synthetic container widgets are now orphaned and need to be deleted
        let incomingLayouts =
            incomingPages
            |> List.choose (fun (_, entity) ->
                match entity with
                | LayoutItem layout -> Some layout
                | _ -> None)
            |> Set.ofList
        let nextPairs =
            layoutToWidgetMap
            |> Map.toList
            |> List.choose (fun (layout, widget) ->
                if incomingLayouts.Contains(layout) then
                    // widget is safe for now
                    Some (layout, widget)
                else
                    // disposed of orphaned widget and continue
                    widget.Dispose()
                    // filter out pair
                    None)
        layoutToWidgetMap <-
            nextPairs |> Map.ofList
            
    member this.Refill(pages: (string * LayoutEntity) list) =
        tabWidget.Clear()
        addPages pages
        deleteOrphanedLayoutContainers pages
        
    member this.Widget with get() = tabWidget
    member this.SignalMap with set(value) = signalMap <- value
    
    member this.ApplyAttrs(attrs: Attr list) =
        attrs |> List.iter (function
            | NoneYet ->
                ())
        
    // don't forget to dispose widgets from layoutToWidgetMap
    interface IDisposable with
        member this.Dispose() =
            tabWidget.Dispose()

let private create (attrs: Attr list) (pages: (string * LayoutEntity) list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) =
    let model = new Model<'msg>(dispatch, pages)
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
    let mutable pages: (string * LayoutItemNode<'msg>) list = []
    // member private this.Pages = pages // need to be able to access from migration (does this need to be a function?)

    [<DefaultValue>] val mutable private model: Model<'msg>
    member val Attrs: Attr list = [] with get, set
    
    let mutable onCurrentChanged: (int -> 'msg) option = None
    member this.OnCurrentChanged with set value = onCurrentChanged <- Some value
    member private this.SignalMap
        with get() = function
            | CurrentChanged index ->
                onCurrentChanged
                |> Option.map (fun f -> f index)
                
    member this.Pages
        with get() = pages
        and set value = pages <- value
        
    override this.Dependencies() =
        // as usual, this is order-based (since the outside 'user' is not providing keys)
        // ... we should probably switch to string keys anyway, and/or rethink how widgets can survive reorderings
        // seems silly to needlessly destroy/create things just because the order changed and the user didn't provide keys,
        // especially when .ContentKeys exist
        pages
        |> List.mapi (fun i (_, node) -> i, node :> BuilderNode<'msg>)
        
    override this.Create(dispatch: 'msg -> unit) =
        let pageLabelsAndHandles =
            pages
            |> List.map (fun (label, node) ->
                label, node.LayoutEntity)
        this.model <- create this.Attrs pageLabelsAndHandles this.SignalMap dispatch
        
    member private this.MigrateContent(leftTabWidget: Node<'msg>) =
        let leftContents =
            leftTabWidget.Pages
            |> List.map (fun (label, node) -> label, node.ContentKey)
        let thisContents =
            pages
            |> List.map (fun (label, node) -> label, node.ContentKey)
        if leftContents <> thisContents then
            let pageLabelsAndHandles =
                pages
                |> List.map (fun (label, node) -> label, node.LayoutEntity)
            this.model.Refill(pageLabelsAndHandles)
        else
            ()
            
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
