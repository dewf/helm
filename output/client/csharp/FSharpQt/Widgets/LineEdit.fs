module FSharpQt.Widgets.LineEdit

open System
open FSharpQt.BuilderNode
open Org.Whatever.QtTesting

open FSharpQt.MiscTypes
open FSharpQt.Attrs

type internal Signal =
    // inherited
    | WidgetSignal of signal: Widget.Signal
    // ours
    | CursorPositionChanged of oldPos: int * newPos: int
    | EditingFinished
    | InputRejected
    | ReturnPressed
    | SelectionChanged
    | TextChanged of text: string  // also emitted when the text is changed programmatically
    | TextEdited of text: string   // user input only
    
type EchoMode =
    | Normal
    | NoEcho
    | Password
    | PasswordEchoOnEdit
with
    member internal this.QtValue =
        match this with
        | Normal -> LineEdit.EchoMode.Normal
        | NoEcho -> LineEdit.EchoMode.NoEcho
        | Password -> LineEdit.EchoMode.Password
        | PasswordEchoOnEdit -> LineEdit.EchoMode.PasswordEchoOnEdit
    
type private Attr =
    | Alignment of align: Alignment
    | ClearButtonEnabled of enabled: bool
    | CursorMoveStyle of style: CursorMoveStyle
    | CursorPosition of pos: int
    | DragEnabled of enabled: bool
    | EchoMode of mode: EchoMode
    | Frame of enabled: bool
    | InputMask of mask: string
    | MaxLength of length: int
    | Modified of state: bool
    | PlaceholderText of text: string
    | ReadOnly of value: bool
    | Text of text: string
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
            | Alignment _ -> "lineedit:alignment"
            | ClearButtonEnabled _ -> "lineedit:clearbuttonenabled"
            | CursorMoveStyle _ -> "lineedit:cursormovestyle"
            | CursorPosition _ -> "lineedit:cursorposition"
            | DragEnabled _ -> "lineedit:dragenabled"
            | EchoMode _ -> "lineedit:echomode"
            | Frame _ -> "lineedit:frame"
            | InputMask _ -> "lineedit:inputmask"
            | MaxLength _ -> "lineedit:maxlength"
            | Modified _ -> "lineedit:modified"
            | PlaceholderText _ -> "lineedit:placeholdertext"
            | ReadOnly _ -> "lineedit:readonly"
            | Text _ -> "lineedit:text"
        override this.ApplyTo (target: IAttrTarget, maybePrev: IAttr option) =
            match target with
            | :? LineEditAttrTarget as editTarget ->
                let lineEdit =
                    editTarget.LineEdit
                match this with
                | Alignment align ->
                    lineEdit.SetAlignment(align.QtValue)
                | ClearButtonEnabled enabled ->
                    lineEdit.SetClearButtonEnabled(enabled)
                | CursorMoveStyle style ->
                    lineEdit.SetCursorMoveStyle(style.QtValue)
                | CursorPosition pos ->
                    if editTarget.SetCursorPos(pos) then
                        lineEdit.SetCursorPosition(pos)
                | DragEnabled enabled ->
                    lineEdit.SetDragEnabled(enabled)
                | EchoMode mode ->
                    lineEdit.SetEchoMode(mode.QtValue)
                | Frame enabled ->
                    lineEdit.SetFrame(enabled)
                | InputMask mask ->
                    lineEdit.SetInputMask(mask)
                | MaxLength length ->
                    lineEdit.SetMaxLength(length)
                | Modified state ->
                    lineEdit.SetModified(state)
                | PlaceholderText text ->
                    lineEdit.SetPlaceholderText(text)
                | ReadOnly value ->
                    lineEdit.SetReadOnly(value)
                | Text text ->
                    if editTarget.SetText(text) then
                        lineEdit.SetText(text)
            | _ ->
                printfn "warning: LineEdit.Attr couldn't ApplyTo() unknown target type [%A]" target
                
type Props<'msg>() =
    inherit Widget.Props<'msg>()
    
    let mutable onCursorPositionChanged: (int * int -> 'msg) option = None
    let mutable onEditingFinished: 'msg option = None
    let mutable onInputRejected: 'msg option = None
    let mutable onReturnPressed: 'msg option = None
    let mutable onSelectionChanged: 'msg option = None
    let mutable onTextChanged: (string -> 'msg) option = None
    let mutable onTextEdited: (string -> 'msg) option = None

    member internal this.SignalMask = enum<LineEdit.SignalMask> (int this._signalMask)
    
    member this.OnCursorPositionChanged with set value =
        onCursorPositionChanged <- Some value
        this.AddSignal(int LineEdit.SignalMask.CursorPositionChanged)
        
    member this.OnEditingFinished with set value =
        onEditingFinished <- Some value
        this.AddSignal(int LineEdit.SignalMask.EditingFinished)
        
    member this.OnInputRejected with set value =
        onInputRejected <- Some value
        this.AddSignal(int LineEdit.SignalMask.InputRejected)
        
    member this.OnReturnPressed with set value =
        onReturnPressed <- Some value
        this.AddSignal(int LineEdit.SignalMask.ReturnPressed)
        
    member this.OnSelectionChanged with set value =
        onSelectionChanged <- Some value
        this.AddSignal(int LineEdit.SignalMask.SelectionChanged)
        
    member this.OnTextChanged with set value =
        onTextChanged <- Some value
        this.AddSignal(int LineEdit.SignalMask.TextChanged)
        
    member this.OnTextEdited with set value =
        onTextEdited <- Some value
        this.AddSignal(int LineEdit.SignalMask.TextEdited)

    member internal this.SignalMap = function
        | WidgetSignal signal ->
            (this :> Widget.Props<'msg>).SignalMap signal
        | CursorPositionChanged(oldPos, newPos) ->
            onCursorPositionChanged
            |> Option.map (fun f -> f (oldPos, newPos))
        | EditingFinished ->
            onEditingFinished
        | InputRejected ->
            onInputRejected
        | ReturnPressed ->
            onReturnPressed
        | SelectionChanged ->
            onSelectionChanged
        | TextChanged text ->
            onTextChanged
            |> Option.map (fun f -> f text)
        | TextEdited text ->
            onTextEdited
            |> Option.map (fun f -> f text)
    
    member this.Alignment with set value =
        this.PushAttr(Alignment value)
        
    member this.ClearButtonEnabled with set value =
        this.PushAttr(ClearButtonEnabled value)
        
    member this.CursorMoveStyle with set value =
        this.PushAttr(CursorMoveStyle value)
        
    member this.CursorPosition with set value =
        this.PushAttr(CursorPosition value)
        
    member this.DragEnabled with set value =
        this.PushAttr(DragEnabled value)
        
    member this.EchoMode with set value =
        this.PushAttr(EchoMode value)
        
    member this.Frame with set value =
        this.PushAttr(Frame value)
        
    member this.InputMask with set value =
        this.PushAttr(InputMask value)
        
    member this.MaxLength with set value =
        this.PushAttr(MaxLength value)
        
    member this.Modified with set value =
        this.PushAttr(Modified value)
        
    member this.PlaceholderText with set value =
        this.PushAttr(PlaceholderText value)
        
    member this.Readonly with set value =
        this.PushAttr(ReadOnly value)
        
    member this.Text with set value =
        this.PushAttr(Text value)

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
    
    member this.ApplyAttrs(attrs: (IAttr option * IAttr) list) =
        for maybePrev, attr in attrs do
            attr.ApplyTo(this, maybePrev)
            
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
    model.ApplyAttrs (attrs |> List.map (fun attr -> None, attr))
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private migrate (model: Model<'msg>) (attrs: (IAttr option * IAttr) list) (signalMap: Signal -> 'msg option) (signalMask: LineEdit.SignalMask) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()

type LineEdit<'msg>() =
    inherit Props<'msg>()
    [<DefaultValue>] val mutable private model: Model<'msg>
    
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
     
