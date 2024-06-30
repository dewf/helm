module FSharpQt.Widgets.Frame

open FSharpQt.BuilderNode
open System
open Org.Whatever.QtTesting

open FSharpQt.MiscTypes
open FSharpQt.Attrs

type Signal =
    | WidgetSignal of signal: Widget.Signal

type Shape =
    | NoFrame
    | Box
    | Panel
    | StyledPanel
    | HLine
    | VLine
    | WinPanel
with
    member this.QtValue =
        match this with
        | NoFrame -> Frame.Shape.NoFrame
        | Box -> Frame.Shape.Box
        | Panel -> Frame.Shape.Panel
        | StyledPanel -> Frame.Shape.StyledPanel
        | HLine -> Frame.Shape.HLine
        | VLine -> Frame.Shape.VLine
        | WinPanel -> Frame.Shape.WinPanel
    
type Shadow =
    | Plain
    | Raised
    | Sunken
with
    member this.QtValue =
        match this with
        | Plain -> Frame.Shadow.Plain
        | Raised -> Frame.Shadow.Raised
        | Sunken -> Frame.Shadow.Sunken
        
type private Attr =
    | FrameRect of rect: Rect
    | FrameShadow of shadow: Shadow
    | FrameShape of shape: Shape
    | LineWidth of width: int
    | MidLineWidth of width: int
    | FrameStyle of shape: Shape * shadow: Shadow
with
    interface IAttr with
        override this.AttrEquals other =
            match other with
            | :? Attr as otherAttr ->
                this = otherAttr
            | _ ->
                false
        override this.Key =
            match this with
            | FrameRect _ -> "frame:rect"
            | FrameShadow _ -> "frame:shadow"
            | FrameShape _ -> "frame:shape"
            | LineWidth _ -> "frame:linewidth"
            | MidLineWidth _ -> "frame:midlinewidth"
            | FrameStyle _ -> "frame:style"
        override this.ApplyTo (target: IAttrTarget) =
            match target with
            | :? FrameAttrTarget as attrTarget ->
                let frame =
                    attrTarget.Frame
                match this with
                | FrameRect rect ->
                    frame.SetFrameRect(rect.QtValue)
                | FrameShadow shadow ->
                    frame.SetFrameShadow(shadow.QtValue)
                | FrameShape shape ->
                    frame.SetFrameShape(shape.QtValue)
                | LineWidth width ->
                    frame.SetLineWidth(width)
                | MidLineWidth width ->
                    frame.SetMidLineWidth(width)
                | FrameStyle(shape, shadow) ->
                    frame.SetFrameStyle(shape.QtValue, shadow.QtValue)
            | _ ->
                printfn "warning: Frame.Attr couldn't ApplyTo() unknown target type [%A]" target

type Props<'msg>() =
    inherit Widget.Props<'msg>()
    
    member internal this.SignalMask = enum<Frame.SignalMask> (int this._signalMask)
    
    member this.SignalMap = function
        | WidgetSignal signal ->
            (this :> Widget.Props<'msg>).SignalMap signal
    
    member this.FrameRect with set value =
        this.PushAttr(FrameRect value)
        
    member this.FrameShadow with set value =
        this.PushAttr(FrameShadow value)
        
    member this.FrameShape with set value =
        this.PushAttr(FrameShape value)
        
    member this.LineWidth with set value =
        this.PushAttr(LineWidth value)
        
    member this.MidLineWidth with set value =
        this.PushAttr(MidLineWidth value)
        
    member this.FrameStyle with set value =
        this.PushAttr(FrameStyle value)
    
type private Model<'msg>(dispatch: 'msg -> unit) as this =
    let mutable frame = Frame.Create(this)
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable currentMask = enum<Frame.SignalMask> 0
    
    let signalDispatch (s: Signal) =
        signalMap s
        |> Option.iter dispatch
    
    member this.Widget with get() = frame
    member this.SignalMap with set value = signalMap <- value
    
    member this.SignalMask with set value =
        if value <> currentMask then
            frame.SetSignalMask(value)
            currentMask <- value
    
    member this.ApplyAttrs (attrs: IAttr list) =
        for attr in attrs do
            attr.ApplyTo(this)
            
    interface FrameAttrTarget with
        member this.Widget = frame
        member this.Frame = frame
        
    interface Frame.SignalHandler with
        // Widget:
        member this.CustomContextMenuRequested pos =
            Point.From pos
            |> Widget.Signal.CustomContextMenuRequested
            |> WidgetSignal
            |> signalDispatch
        member this.WindowIconChanged icon =
            IconProxy(icon)
            |> Widget.Signal.WindowIconChanged
            |> WidgetSignal
            |> signalDispatch
        member this.WindowTitleChanged title =
            Widget.Signal.WindowTitleChanged title
            |> WidgetSignal
            |> signalDispatch
                
    interface IDisposable with
        member this.Dispose() =
            frame.Dispose()

let private create (attrs: IAttr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (signalMask: Frame.SignalMask) =
    let model = new Model<'msg>(dispatch)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private migrate (model: Model<'msg>) (attrs: IAttr list) (signalMap: Signal -> 'msg option) (signalMask: Frame.SignalMask) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()

type Frame<'msg>() =
    inherit Props<'msg>()
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    interface IWidgetNode<'msg> with
        override this.Dependencies = []
        
        override this.Create dispatch buildContext =
            this.model <- create this.Attrs this.SignalMap dispatch this.SignalMask
            
        override this.AttachDeps () =
            ()

        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> Frame<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged
            this.model <- migrate left'.model nextAttrs this.SignalMap this.SignalMask
            
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()

        override this.Widget =
            (this.model.Widget :> Widget.Handle)
            
        override this.ContentKey =
            (this :> IWidgetNode<'msg>).Widget
            
        override this.Attachments =
            this.Attachments
