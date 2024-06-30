module FSharpQt.Widgets.LineEdit

open System
open FSharpQt.BuilderNode
open FSharpQt.MiscTypes
open FSharpQt.Props.LineEdit
open Org.Whatever.QtTesting

open FSharpQt.Attrs
open FSharpQt.Props

type private Model<'msg>(dispatch: 'msg -> unit) as this =
    let mutable lineEdit = LineEdit.Create(this)
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable currentMask = enum<LineEdit.SignalMask> 0
    
    let mutable lastValue = ""
    let mutable lastCursorPos = -1
    
    let signalDispatch (s: Signal) =
        match signalMap s with
        | Some msg ->
            dispatch msg
        | None ->
            ()
        
    member this.Widget with get() = lineEdit
    member this.SignalMap with set value = signalMap <- value
    member this.SignalMask with set value =
        if value <> currentMask then
            lineEdit.SetSignalMask(value)
            currentMask <- value
    
    member this.ApplyAttrs(attrs: IAttr list) =
        for attr in attrs do
            attr.ApplyTo(this)
            
    interface LineEditAttrTarget with
        member this.Widget = lineEdit
        member this.LineEdit = lineEdit
        member this.SetText newValue =
            if lastValue <> newValue then
                lastValue <- newValue
                true
            else
                false
        member this.SetCursorPos newPos =
            if lastCursorPos <> newPos then
                lastCursorPos <- newPos
                true
            else
                false
                
    interface LineEdit.SignalHandler with
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
        // LineEdit:
        member this.CursorPositionChanged (oldPos, newPos) =
            lastCursorPos <- newPos
            signalDispatch (CursorPositionChanged (oldPos, newPos))
        member this.EditingFinished () =
            signalDispatch EditingFinished
        member this.InputRejected () =
            signalDispatch InputRejected
        member this.ReturnPressed () =
            signalDispatch ReturnPressed
        member this.SelectionChanged () =
            signalDispatch SelectionChanged
        member this.TextChanged text =
            lastValue <- text
            signalDispatch (TextChanged text)
        member this.TextEdited text =
            lastValue <- text
            signalDispatch (TextEdited text)
                
    interface IDisposable with
        member this.Dispose() =
            lineEdit.Dispose()

let private create (attrs: IAttr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (signalMask: LineEdit.SignalMask) =
    let model = new Model<'msg>(dispatch)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private migrate (model: Model<'msg>) (attrs: IAttr list) (signalMap: Signal -> 'msg option) (signalMask: LineEdit.SignalMask) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()

type LineEdit<'msg>() =
    inherit LineEditProps<'msg>()
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    member this.Attrs = this._attrs |> List.rev
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    interface IWidgetNode<'msg> with
        override this.Dependencies = []
        
        override this.Create dispatch buildContextr =
            this.model <- create this.Attrs this.SignalMap dispatch this.SignalMask
            
        override this.AttachDeps () =
            ()
            
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> LineEdit<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged
            this.model <-
                migrate left'.model nextAttrs this.SignalMap this.SignalMask
                
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()
            
        override this.Widget =
            (this.model.Widget :> Widget.Handle)
            
        override this.ContentKey =
            (this :> IWidgetNode<'msg>).Widget
            
        override this.Attachments =
            this.Attachments
     
