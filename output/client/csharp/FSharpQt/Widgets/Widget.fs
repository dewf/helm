module FSharpQt.Widgets.Widget

open System
open FSharpQt
open FSharpQt.BuilderNode
open FSharpQt.MiscTypes
open Org.Whatever.QtTesting

open FSharpQt.Attrs

type Signal =
    | CustomContextMenuRequested of pos: Point
    | WindowIconChanged of icon: IconProxy
    | WindowTitleChanged of title: string
    
[<RequireQualifiedAccess>]
type internal AttrValue =
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

[<AbstractClass>]
type Attr internal(value: AttrValue) =
    member val private Value = value
    interface IAttr with
        override this.AttrEquals other =
            match other with
            | :? Attr as attr ->
                this.Value = attr.Value
            | _ ->
                false
        override this.Key =
            match value with
            | AttrValue.Size _ -> "widget:size"
            | AttrValue.Enabled _ -> "widget:enabled"
            | AttrValue.MinimumWidth _ -> "widget:minwidth"
            | AttrValue.MinimumHeight _ -> "widget:minheight"
            | AttrValue.MaximumWidth _ -> "widget:maxwidth"
            | AttrValue.MaximumHeight _ -> "widget:maxheight"
            | AttrValue.SizePolicy _ -> "widget:sizepolicy"
            | AttrValue.FixedWidth _ -> "widget:fixedwidth"
            | AttrValue.FixedHeight _ -> "widget:fixedheight"
            | AttrValue.FixedSize _ -> "widget:fixedsize"
            | AttrValue.Visible _ -> "widget:visible"
            | AttrValue.WindowTitle _ -> "widget:windowtitle"
            | AttrValue.WindowModality _ -> "widget:windowmodality"
            | AttrValue.ContextMenuPolicy _ -> "widget:contextmenupolicy"
            | AttrValue.UpdatesEnabled _ -> "widget:updatesenabled"
            | AttrValue.MouseTracking _ -> "widget:mousetracking"
            | AttrValue.AcceptDrops _ -> "widget:acceptdrops"
        override this.ApplyTo (target: IAttrTarget) =
            match target with
            | :? WidgetAttrTarget as widgetTarget ->
                let widget =
                    widgetTarget.Widget
                match value with
                | AttrValue.Size (width, height) ->
                    widget.Resize(width, height)
                | AttrValue.Enabled state ->
                    widget.SetEnabled(state)
                | AttrValue.MinimumWidth width ->
                    widget.SetMinimumWidth(width)
                | AttrValue.MinimumHeight height ->
                    widget.SetMinimumHeight(height)
                | AttrValue.MaximumWidth width ->
                    widget.SetMaximumWidth(width)
                | AttrValue.MaximumHeight height ->
                    widget.SetMaximumHeight(height)
                | AttrValue.SizePolicy(hPolicy, vPolicy) ->
                    widget.SetSizePolicy(hPolicy.QtValue, vPolicy.QtValue)
                | AttrValue.FixedWidth width ->
                    widget.SetFixedWidth(width)
                | AttrValue.FixedHeight height ->
                    widget.SetFixedHeight(height)
                | AttrValue.FixedSize(width, height) ->
                    widget.SetFixedSize(width, height)
                | AttrValue.Visible state ->
                    widget.SetVisible(state)
                | AttrValue.WindowTitle title ->
                    widget.SetWindowTitle(title)
                | AttrValue.WindowModality modality ->
                    widget.SetWindowModality(modality.QtValue)
                | AttrValue.ContextMenuPolicy policy ->
                    widget.SetContextMenuPolicy(policy.QtValue)
                | AttrValue.UpdatesEnabled enabled ->
                    widget.SetUpdatesEnabled(enabled)
                | AttrValue.MouseTracking enabled ->
                    widget.SetMouseTracking(enabled)
                | AttrValue.AcceptDrops enabled ->
                    widget.SetAcceptDrops(enabled)
            | _ ->
                printfn "warning: Widget.Attr couldn't ApplyTo() unknown target type [%A]" target
                
type Size(width: int, height: int) =
    inherit Attr(AttrValue.Size(width, height))
                
type Enabled(state: bool) =
    inherit Attr(AttrValue.Enabled state)

type MinimumWidth(width: int) =
    inherit Attr(AttrValue.MinimumWidth width)

type MinimumHeight(height: int) =
    inherit Attr(AttrValue.MinimumHeight height)

type MaximumWidth(width: int) =
    inherit Attr(AttrValue.MaximumWidth width)
    
type MaximumHeight(height: int) =
    inherit Attr(AttrValue.MaximumHeight height)
            
type SizePolicy(hPolicy: MiscTypes.SizePolicy, vPolicy: MiscTypes.SizePolicy) =
    inherit Attr(AttrValue.SizePolicy(hPolicy, vPolicy))
    
type FixedWidth(width: int) =
    inherit Attr(AttrValue.FixedWidth(width))
    
type FixedHeight(height: int) =
    inherit Attr(AttrValue.FixedHeight(height))

type FixedSize(width: int, height: int) =
    inherit Attr(AttrValue.FixedSize(width, height))
    
type Visible(state: bool) =
    inherit Attr(AttrValue.Visible(state))

type WindowTitle(title: string) =
    inherit Attr(AttrValue.WindowTitle(title))
    
type WindowModality(modality: MiscTypes.WindowModality) =
    inherit Attr(AttrValue.WindowModality(modality))
    
type ContextMenuPolicy(policy: MiscTypes.ContextMenuPolicy) =
    inherit Attr(AttrValue.ContextMenuPolicy(policy))
    
type UpdatesEnabled(enabled: bool) =
    inherit Attr(AttrValue.UpdatesEnabled(enabled))
    
type MouseTracking(enabled: bool) =
    inherit Attr(AttrValue.MouseTracking(enabled))
    
type AcceptDrops(enabled: bool) =
    inherit Attr(AttrValue.AcceptDrops(enabled))
    
type WidgetProps() =
    let mutable attrs: IAttr list = []
    member this.WidgetAttrs = attrs
    
    member this.Size with set (w, h) =
        attrs <- Size(w,h) :: attrs
        
    member this.Enabled with set value =
        attrs <- Enabled(value) :: attrs
        
    // | MinimumWidth of width: int
    // | MinimumHeight of height: int
    // | MaximumWidth of width: int
    // | MaximumHeight of height: int
    // | SizePolicy of hPolicy: SizePolicy * vPolicy: SizePolicy
    // | FixedWidth of width: int
    // | FixedHeight of height: int
    // | FixedSize of width: int * height: int
    // | Visible of state: bool
    // | WindowTitle of title: string
    // | WindowModality of modality: WindowModality
    // | ContextMenuPolicy of policy: ContextMenuPolicy
    // | UpdatesEnabled of enabled: bool
    // | MouseTracking of enabled: bool
    // | AcceptDrops of enabled: bool
    
    
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
    
    member this.Attrs = this.WidgetAttrs
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

