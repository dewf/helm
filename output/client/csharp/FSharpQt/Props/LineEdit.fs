module FSharpQt.Props.LineEdit

open FSharpQt
open FSharpQt.Attrs
open Org.Whatever.QtTesting

type Signal =
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
    
type Attr =
    | Alignment of align: MiscTypes.Alignment
    | ClearButtonEnabled of enabled: bool
    | CursorMoveStyle of style: MiscTypes.CursorMoveStyle
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
        override this.ApplyTo (target: IAttrTarget) =
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
                
type LineEditProps<'msg>() =
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
    
