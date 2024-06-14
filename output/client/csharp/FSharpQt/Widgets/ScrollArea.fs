module FSharpQt.Widgets.ScrollArea

open System
open FSharpQt.BuilderNode
open Org.Whatever.QtTesting

type Signal = unit

type Policy =
    | AsNeeded
    | AlwaysOff
    | AlwaysOn
with
    member this.QtValue =
        match this with
        | AsNeeded -> AbstractScrollArea.ScrollBarPolicy.AsNeeded
        | AlwaysOff -> AbstractScrollArea.ScrollBarPolicy.AlwaysOff
        | AlwaysOn -> AbstractScrollArea.ScrollBarPolicy.AlwaysOn
    
type Attr =
    | HPolicy of policy: Policy
    | VPolicy of policy: Policy

let private keyFunc = function
    | HPolicy _ -> 0
    | VPolicy _ -> 1
    
let private diffAttrs =
    genericDiffAttrs keyFunc

type private Model<'msg>(dispatch: 'msg -> unit) = 
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable scrollArea = ScrollArea.Create()
    let mutable syntheticLayoutWidget: Widget.Handle option = None
        
    member this.RemoveContent() =
        // TODO: need to do some serious testing with all this
        // MainWindow too
        match syntheticLayoutWidget with
        | Some widget ->
            widget.GetLayout().RemoveAll() // detach any children just in case
            widget.SetLayout(null)
            widget.Dispose()
            // deleting should automatically remove from the parent mainWindow, right?
            syntheticLayoutWidget <- None
        | None ->
            scrollArea.SetWidget(null) // sufficient?
        
    member this.AddContent(node: IWidgetOrLayoutNode<'msg>) =
        match node with
        | :? IWidgetNode<'msg> as widgetNode ->
            scrollArea.SetWidget(widgetNode.Widget)
        | :? ILayoutNode<'msg> as layout ->
            let widget = Widget.Create()
            widget.SetLayout(layout.Layout)
            scrollArea.SetWidget(widget)
            syntheticLayoutWidget <- Some widget
        | _ ->
            failwith "ScrollArea.Model.AddContent - unknown node type"
        
    member this.Widget with get() = scrollArea
    member this.SignalMap with set(value) = signalMap <- value
    
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | HPolicy policy ->
                scrollArea.SetHorizontalScrollBarPolicy(policy.QtValue)
            | VPolicy policy ->
                scrollArea.SetVerticalScrollBarPolicy(policy.QtValue)
    
    interface IDisposable with
        member this.Dispose() =
            scrollArea.Dispose()


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


type ScrollArea<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    let mutable maybeContent: IWidgetOrLayoutNode<'msg> option = None
    member this.Content with set value = maybeContent <- Some value
    
    let signalMap = (fun _ -> None)
    
    member val Attrs: Attr list = [] with get, set
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    member private this.MigrateContent (changeMap: Map<DepsKey, DepsChange>) =
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
        
    interface IWidgetNode<'msg> with
        override this.Dependencies =
            let contentList =
                maybeContent
                |> Option.map (fun content -> (StrKey "content", content :> IBuilderNode<'msg>))
                |> Option.toList
            contentList

        override this.Create dispatch buildContext =
            this.model <- create this.Attrs signalMap dispatch
            
        override this.AttachDeps () =
            maybeContent
            |> Option.iter this.model.AddContent

        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> ScrollArea<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged
            this.model <- migrate left'.model nextAttrs signalMap
            this.MigrateContent (depsChanges |> Map.ofList)

        override this.Dispose() =
            (this.model :> IDisposable).Dispose()
            
        override this.Widget =
            this.model.Widget
            
        override this.ContentKey =
            (this :> IWidgetNode<'msg>).Widget
            
        override this.Attachments =
            this.Attachments
