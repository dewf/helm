module FSharpQt.Props.Widget

open FSharpQt.Attrs
open FSharpQt.MiscTypes
open Org.Whatever.QtTesting
open PropsRoot

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
    
type Props<'msg>() =
    inherit PropsRoot()
    
    // TODO: switch to using .AddSignal() instead?
    // and then this.SignalMask fetches that PropsRoot() value?
    // let mutable signalMask = enum<Widget.SignalMask> 0
    
    let mutable onCustomContextMenuRequested: (Point -> 'msg) option = None
    let mutable onWindowIconChanged: (IconProxy -> 'msg) option = None
    let mutable onWindowTitleChanged: (string -> 'msg) option = None

    member internal this.SignalMask = enum<Widget.SignalMask> (int this._signalMask)
    
    member this.OnCustomContextMenuRequested with set value =
        onCustomContextMenuRequested <- Some value
        this.AddSignal(int Widget.SignalMask.CustomContextMenuRequested)
        // signalMask <- signalMask ||| Widget.SignalMask.CustomContextMenuRequested
        
    member this.OnWindowIconChanged with set value =
        onWindowIconChanged <- Some value
        this.AddSignal(int Widget.SignalMask.WindowIconChanged)
        // signalMask <- signalMask ||| Widget.SignalMask.WindowIconChanged
        
    member this.OnWindowTitleChanged with set value =
        onWindowTitleChanged <- Some value
        this.AddSignal(int Widget.SignalMask.WindowTitleChanged)
        // signalMask <- signalMask ||| Widget.SignalMask.WindowTitleChanged
        
    member internal this.SignalMap = function
        | CustomContextMenuRequested pos ->
            onCustomContextMenuRequested
            |> Option.map (fun f -> f pos)
        | WindowIconChanged icon ->
            onWindowIconChanged
            |> Option.map (fun f -> f icon)
        | WindowTitleChanged title ->
            onWindowTitleChanged
            |> Option.map (fun f -> f title)
    
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
        
