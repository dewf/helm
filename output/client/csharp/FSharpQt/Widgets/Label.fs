module FSharpQt.Widgets.Label

open System
open FSharpQt.BuilderNode
open Org.Whatever.QtTesting

open FSharpQt.Attrs
open FSharpQt.MiscTypes

type internal Signal =
    // inherited
    | FrameSignal of signal: Frame.Signal
    // ours
    | LinkActivated of link: string
    | LinkHovered of link: string

type private Attr =
    | Alignment of align: Alignment
    | Indent of indent: int
    | Margin of margin: int
    | OpenExternalLinks of state: bool
//    | Pixmap of pixmap: Pixmap
    | ScaledContents of state: bool
    | Text of text: string
    | TextFormat of format: TextFormat
    | TextInteractionFlags of flags: TextInteractionFlag seq
    | WordWrap of state: bool
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
            | Alignment _ -> "label:alignment"
            | Indent _ -> "label:indent"
            | Margin _ -> "label:margin"
            | OpenExternalLinks _ -> "label:openexternallinks"
            | ScaledContents _ -> "label:scaledcontents"
            | Text _ -> "label:text"
            | TextFormat _ -> "label:textformat"
            | TextInteractionFlags _ -> "label:textinteractionflags"
            | WordWrap _ -> "label:wordwrap"
        override this.ApplyTo (target: IAttrTarget, maybePrev: IAttr option) =
            match target with
            | :? LabelAttrTarget as attrTarget ->
                let label =
                    attrTarget.Label
                match this with
                | Alignment align ->
                    label.SetAlignment(align.QtValue)
                | Indent indent ->
                    label.SetIndent(indent)
                | Margin margin ->
                    label.SetMargin(margin)
                | OpenExternalLinks state ->
                    label.SetOpenExternalLinks(state)
                | ScaledContents state ->
                    label.SetScaledContents(state)
                | Text text ->
                    label.SetText(text)
                | TextFormat format ->
                    label.SetTextFormat(format.QtValue)
                | TextInteractionFlags flags ->
                    label.SetTextInteractionFlags(flags |> TextInteractionFlag.QtSetFrom)
                | WordWrap state ->
                    label.SetWordWrap(state)
            | _ ->
                printfn "warning: Label.Attr couldn't ApplyTo() unknown target type [%A]" target
                
type Props<'msg>() =
    inherit Frame.Props<'msg>()
    
    let mutable maybeOnLinkActivated: (string -> 'msg) option = None
    let mutable maybeOnLinkHovered: (string -> 'msg) option = None

    member internal this.SignalMask = enum<Label.SignalMask> (int this._signalMask)
    
    member this.OnLinkActivated with set value =
        maybeOnLinkActivated <- value
        this.AddSignal(int Label.SignalMask.LinkActivated)
        
    member this.OnLinkHovered with set value =
        maybeOnLinkHovered <- value
        this.AddSignal(int Label.SignalMask.LinkHovered)
        
    member internal this.SignalMap = function
        | FrameSignal signal ->
            (this :> Frame.Props<'msg>).SignalMap signal
        | LinkActivated link ->
            maybeOnLinkActivated
            |> Option.map (fun f -> f link)
        | LinkHovered link ->
            maybeOnLinkHovered
            |> Option.map (fun f -> f link)
        
    member this.Alignment with set value =
        this.PushAttr(Alignment value)
        
    member this.Indent with set value =
        this.PushAttr(Indent value)
        
    member this.Margin with set value =
        this.PushAttr(Margin value)
        
    member this.OpenExternalLinks with set value =
        this.PushAttr(OpenExternalLinks value)
        
    // member this.Pixmap with set value =
    //     this.PushAttr(Pixmap value)

    member this.ScaledContents with set value =
        this.PushAttr(ScaledContents value)
        
    member this.Text with set value =
        this.PushAttr(Text value)
        
    member this.TextFormat with set value =
        this.PushAttr(TextFormat value)
        
    member this.TextInteractionFlags with set value =
        this.PushAttr(TextInteractionFlags value)
        
    member this.WordWrap with set value =
        this.PushAttr(WordWrap value)

type private Model<'msg>(dispatch: 'msg -> unit) as this =
    let mutable label = Label.Create(this)
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable currentMask = enum<Label.SignalMask> 0
    
    let signalDispatch (s: Signal) =
        signalMap s
        |> Option.iter dispatch
    
    member this.Label with get() = label
    member this.SignalMap with set value = signalMap <- value
    
    member this.SignalMask with set value =
        if value <> currentMask then
            label.SetSignalMask(value)
            currentMask <- value
    
    member this.ApplyAttrs(attrs: (IAttr option * IAttr) list) =
        for maybePrev, attr in attrs do
            attr.ApplyTo(this, maybePrev)
            
    interface LabelAttrTarget with
        member this.Widget = label
        member this.Frame = label
        member this.Label = label
        
    interface Label.SignalHandler with
        // Widget:
        member this.CustomContextMenuRequested pos =
            Point.From pos
            |> Widget.Signal.CustomContextMenuRequested
            |> Frame.WidgetSignal
            |> FrameSignal
            |> signalDispatch
        member this.WindowIconChanged icon =
            IconProxy(icon)
            |> Widget.Signal.WindowIconChanged
            |> Frame.WidgetSignal
            |> FrameSignal
            |> signalDispatch
        member this.WindowTitleChanged title =
            Widget.Signal.WindowTitleChanged title
            |> Frame.WidgetSignal
            |> FrameSignal
            |> signalDispatch
        // Label:
        member this.LinkActivated link =
            signalDispatch (LinkActivated link)
        member this.LinkHovered link =
            signalDispatch (LinkHovered link)
                
    interface IDisposable with
        member this.Dispose() =
            label.Dispose()
            
let private create (attrs: IAttr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (signalMask: Label.SignalMask) =
    let model = new Model<'msg>(dispatch)
    model.ApplyAttrs (attrs |> List.map (fun attr -> None, attr))
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private migrate (model: Model<'msg>) (attrs: (IAttr option * IAttr) list) (signalMap: Signal -> 'msg option) (signalMask: Label.SignalMask) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()

type Label<'msg>() =
    inherit Props<'msg>()
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
            
    interface IWidgetNode<'msg> with
        override this.Dependencies = []
        
        override this.Create dispatch buildContext =
            this.model <- create this.Attrs this.SignalMap dispatch this.SignalMask
            
        override this.AttachDeps() =
            ()

        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> Label<'msg>)
            let nextAttrs = diffAttrs left'.Attrs this.Attrs |> createdOrChanged
            this.model <- migrate left'.model nextAttrs this.SignalMap this.SignalMask
            
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()

        override this.Widget =
            this.model.Label
            
        override this.ContentKey =
            this.model.Label
            
        override this.Attachments =
            this.Attachments
