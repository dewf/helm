module FSharpQt.Widgets.Widget

open System
open FSharpQt.BuilderNode
open Org.Whatever.QtTesting

// no signals yet

type Attr =
    | Visible of state: bool
    
let private keyFunc = function
    | Visible _ -> 0
    
let private diffAttrs =
    genericDiffAttrs keyFunc

type private Model<'msg>(dispatch: 'msg -> unit) =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable widget = Widget.Create()
    
    do
        let signalDispatch (s: Signal) =
            match signalMap s with
            | Some msg ->
                dispatch msg
            | None ->
                ()
        // no signals yet
        
        // I guess show it initially, in case it's being used as a top-level?
        widget.Show()
        
    member this.Widget with get() = widget
    member this.SignalMap with set(value) = signalMap <- value
    
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Visible state ->
                widget.SetVisible(state)
                
    interface IDisposable with
        member this.Dispose() =
            widget.Dispose()
            
    member this.RemoveLayout() =
        let existing =
            widget.GetLayout()
        existing.RemoveAll()
        widget.SetLayout(null)
        
    member this.AddLayout(layout: Layout.Handle) =
        widget.SetLayout(layout)
        
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
            
 
type Widget<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    let mutable maybeLayout: ILayoutNode<'msg> option = None
    member this.Layout with set value = maybeLayout <- Some value

    member val Attrs: Attr list = [] with get, set
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    let signalMap = (fun _ -> None)
    
    member private this.MigrateContent (changeMap: Map<DepsKey, DepsChange>) =
        match changeMap.TryFind (StrKey "layout") with
        | Some change ->
            match change with
            | Unchanged ->
                ()
            | Added ->
                this.model.AddLayout(maybeLayout.Value.Layout)
            | Removed ->
                this.model.RemoveLayout()
            | Swapped ->
                this.model.RemoveLayout()
                this.model.AddLayout(maybeLayout.Value.Layout)
        | None ->
            // neither side had a layout
            ()
    
    interface IWidgetNode<'msg> with
        override this.Dependencies =
            maybeLayout
            |> Option.map (fun content -> (StrKey "layout", content :> IBuilderNode<'msg>))
            |> Option.toList
  
        override this.Create dispatch buildContext =
            this.model <- create this.Attrs signalMap dispatch
            
        override this.AttachDeps () =
            maybeLayout
            |> Option.iter (fun node -> this.model.AddLayout(node.Layout))
            
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> Widget<'msg>)
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
            (this :> IWidgetNode<'msg>).ContentKey
            
        override this.Attachments =
            this.Attachments
