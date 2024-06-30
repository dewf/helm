module FSharpQt.Widgets.Widget

open System
open FSharpQt.BuilderNode
open FSharpQt.MiscTypes
open FSharpQt.Props
open Org.Whatever.QtTesting

open FSharpQt.Attrs
open FSharpQt.Props.Widget

type private Model<'msg>(dispatch: 'msg -> unit) as this =
    let mutable widget = Widget.Create(this)
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable currentMask = enum<Widget.SignalMask> 0
    
    let signalDispatch (s: Signal) =
        signalMap s
        |> Option.iter dispatch
        
    member this.Widget with get() = widget
    member this.SignalMap with set value = signalMap <- value
    
    member this.SignalMask with set value =
        if value <> currentMask then
            widget.SetSignalMask(value)
            currentMask <- value
    
    member this.ApplyAttrs(attrs: IAttr list) =
        for attr in attrs do
            attr.ApplyTo(this)
            
    interface WidgetAttrTarget with
        override this.Widget = widget
                
    interface Widget.SignalHandler with
        member this.CustomContextMenuRequested pos =
            signalDispatch (Point.From pos |> CustomContextMenuRequested)
        member this.WindowIconChanged icon =
            signalDispatch (IconProxy(icon) |> WindowIconChanged)
        member this.WindowTitleChanged title =
            signalDispatch (WindowTitleChanged title)
                
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
        
let private create (attrs: IAttr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (signalMask: Widget.SignalMask) =
    let model = new Model<'msg>(dispatch)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private migrate (model: Model<'msg>) (attrs: IAttr list) (signalMap: Signal -> 'msg option) (signalMask: Widget.SignalMask) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()

type Widget<'msg>() =
    inherit Props<'msg>()
    
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    member this.Attrs = this._attrs |> List.rev
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    let mutable maybeLayout: ILayoutNode<'msg> option = None
    member this.Layout with set value = maybeLayout <- Some value
    
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
            this.model <- create this.Attrs this.SignalMap dispatch this.SignalMask
            
        override this.AttachDeps () =
            maybeLayout
            |> Option.iter (fun node -> this.model.AddLayout(node.Layout))
            
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> Widget<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged
            this.model <- migrate left'.model nextAttrs this.SignalMap this.SignalMask
            this.MigrateContent (depsChanges |> Map.ofList)

        override this.Dispose() =
            (this.model :> IDisposable).Dispose()
            
        override this.Widget =
            this.model.Widget
            
        override this.ContentKey =
            this.model.Widget
            
        override this.Attachments =
            this.Attachments

