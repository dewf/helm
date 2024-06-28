module FSharpQt.Widgets.Widget

open System
open FSharpQt.BuilderNode
open FSharpQt.MiscTypes
open Org.Whatever.QtTesting

open FSharpQt.Attrs

type Signal =
    | CustomContextMenuRequested of pos: Point
    | WindowIconChanged of icon: IconProxy
    | WindowTitleChanged of title: string
    
type Attr =
    | Size of width: int * height: int
    | Enabled of state: bool
    | MinimumWidth of width: int
    | MinimumHeight of height: int
    | MaximumWidth of width: int
    | MaximumHeight of height: int
    | SizePolicy of hPolicy: SizePolicy * vPolicy: SizePolicy
    | FixedWidth of width: int
    | FixedHeight of height: int
    | FixedSize of width: int * height: int
    | Visible of state: bool
    | WindowTitle of title: string
    | WindowModality of modality: WindowModality
    | ContextMenuPolicy of policy: ContextMenuPolicy
    | UpdatesEnabled of enabled: bool
    | MouseTracking of enabled: bool
    | AcceptDrops of enabled: bool
with
    interface IAttr with
        override this.AttrEquals other =
            match other with
            | :? Attr as attrOther ->
                this = attrOther
            | _ ->
                false
        override this.Key =
            match this with
            | Size _ -> "widget:size"
            | Enabled _ -> "widget:enabled"
            | MinimumWidth _ -> "widget:minwidth"
            | MinimumHeight _ -> "widget:minheight"
            | MaximumWidth _ -> "widget:maxwidth"
            | MaximumHeight _ -> "widget:maxheight"
            | SizePolicy _ -> "widget:sizepolicy"
            | FixedWidth _ -> "widget:fixedwidth"
            | FixedHeight _ -> "widget:fixedheight"
            | FixedSize _ -> "widget:fixedsize"
            | Visible _ -> "widget:visible"
            | WindowTitle _ -> "widget:windowtitle"
            | WindowModality _ -> "widget:windowmodality"
            | ContextMenuPolicy _ -> "widget:contextmenupolicy"
            | UpdatesEnabled _ -> "widget:updatesenabled"
            | MouseTracking _ -> "widget:mousetracking"
            | AcceptDrops _ -> "widget:acceptdrops"
        override this.ApplyTo (target: IAttrTarget) =
            match target with
            | :? WidgetAttrTarget as widgetTarget ->
                let widget =
                    widgetTarget.Widget
                match this with
                | Size (width, height) ->
                    widget.Resize(width, height)
                | Enabled state ->
                    widget.SetEnabled(state)
                | MinimumWidth width ->
                    widget.SetMinimumWidth(width)
                | MinimumHeight height ->
                    widget.SetMinimumHeight(height)
                | MaximumWidth width ->
                    widget.SetMaximumWidth(width)
                | MaximumHeight height ->
                    widget.SetMaximumHeight(height)
                | SizePolicy(hPolicy, vPolicy) ->
                    widget.SetSizePolicy(hPolicy.QtValue, vPolicy.QtValue)
                | FixedWidth width ->
                    widget.SetFixedWidth(width)
                | FixedHeight height ->
                    widget.SetFixedHeight(height)
                | FixedSize(width, height) ->
                    widget.SetFixedSize(width, height)
                | Visible state ->
                    widget.SetVisible(state)
                | WindowTitle title ->
                    widget.SetWindowTitle(title)
                | WindowModality modality ->
                    widget.SetWindowModality(modality.QtValue)
                | ContextMenuPolicy policy ->
                    widget.SetContextMenuPolicy(policy.QtValue)
                | UpdatesEnabled enabled ->
                    widget.SetUpdatesEnabled(enabled)
                | MouseTracking enabled ->
                    widget.SetMouseTracking(enabled)
                | AcceptDrops enabled ->
                    widget.SetAcceptDrops(enabled)
            | _ ->
                printfn "warning: Widget.Attr couldn't ApplyTo() unknown target type [%A]" target
    
type WidgetProps() =
    // internal attribute-from-properties storage that will be shared by all subclasses (eg Widget -> AbstractButton -> PushButton)
    // needs to be reversed before use to maintain the order that was originally assigned
    member val internal _attrs: IAttr list = [] with get, set
    member internal this.PushAttr(attr: IAttr) =
        this._attrs <- attr :: this._attrs
    
    member this.Size with set value =
        this.PushAttr(Size value)
        
    member this.Enabled with set value =
        this.PushAttr(Enabled value)
        
    member this.MinimumWidth with set value =
        this.PushAttr(MinimumWidth value)
        
    member this.MinimumHeight with set value =
        this.PushAttr(MinimumHeight value)
        
    member this.MaximumWidth with set value =
        this.PushAttr(MaximumWidth value)
        
    member this.MaximumHeight with set value =
        this.PushAttr(MaximumHeight value)
        
    member this.SizePolicy with set value =
        this.PushAttr(SizePolicy value)
        
    member this.FixedWidth with set value =
        this.PushAttr(FixedWidth value)
        
    member this.FixedHeight with set value =
        this.PushAttr(FixedHeight value)
        
    member this.FixedSize with set value =
        this.PushAttr(FixedSize value)
        
    member this.Visible with set value =
        this.PushAttr(Visible value)
        
    member this.WindowTitle with set value =
        this.PushAttr(WindowTitle value)
        
    member this.WindowModality with set value =
        this.PushAttr(WindowModality value)
        
    member this.ContextMenuPolicy with set value =
        this.PushAttr(ContextMenuPolicy value)
        
    member this.UpdatesEnabled with set value =
        this.PushAttr(UpdatesEnabled value)
        
    member this.MouseTracking with set value =
        this.PushAttr(MouseTracking value)
        
    member this.AcceptDrops with set value =
        this.PushAttr(AcceptDrops value)
    
    
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
    inherit WidgetProps()
    
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    member this.Attrs = this._attrs |> List.rev
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    let mutable maybeLayout: ILayoutNode<'msg> option = None
    member this.Layout with set value = maybeLayout <- Some value
    
    let mutable signalMask = enum<Widget.SignalMask> 0
    
    let mutable onCustomContextMenuRequested: (Point -> 'msg) option = None
    let mutable onWindowIconChanged: (IconProxy -> 'msg) option = None
    let mutable onWindowTitleChanged: (string -> 'msg) option = None
    
    member this.OnCustomContextMenuRequested with set value =
        onCustomContextMenuRequested <- Some value
        signalMask <- signalMask ||| Widget.SignalMask.CustomContextMenuRequested
        
    member this.OnWindowIconChanged with set value =
        onWindowIconChanged <- Some value
        signalMask <- signalMask ||| Widget.SignalMask.WindowIconChanged
        
    member this.OnWindowTitleChanged with set value =
        onWindowTitleChanged <- Some value
        signalMask <- signalMask ||| Widget.SignalMask.WindowTitleChanged
        
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
            this.model <- create this.Attrs signalMap dispatch signalMask
            
        override this.AttachDeps () =
            maybeLayout
            |> Option.iter (fun node -> this.model.AddLayout(node.Layout))
            
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> Widget<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged
            this.model <- migrate left'.model nextAttrs signalMap signalMask
            this.MigrateContent (depsChanges |> Map.ofList)

        override this.Dispose() =
            (this.model :> IDisposable).Dispose()
            
        override this.Widget =
            this.model.Widget
            
        override this.ContentKey =
            this.model.Widget
            
        override this.Attachments =
            this.Attachments

