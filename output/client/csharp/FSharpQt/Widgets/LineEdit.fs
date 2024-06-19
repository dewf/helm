module FSharpQt.Widgets.LineEdit

open System
open FSharpQt.BuilderNode
open Org.Whatever.QtTesting

type Signal =
    | CursorPositionChanged of oldPos: int * newPos: int
    | EditingFinished
    | InputRejected
    | ReturnPressed
    | SelectionChanged
    | TextChanged of text: string
    | TextEdited of text: string
    
type Attr =
    | Value of string
    | Enabled of bool
    
let private attrKey = function
    | Value _ -> 0
    | Enabled _ -> 1

let private diffAttrs =
    genericDiffAttrs attrKey
    
type private Model<'msg>(dispatch: 'msg -> unit) as this =
    let mutable lineEdit = LineEdit.Create(this)
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable currentMask = enum<LineEdit.SignalMask> 0
    
    let mutable lastValue = ""
    
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
    
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Value str ->
                // short-circuit identical current values
                // dispatching is disabled during tree diffing so it's not the end of the world (no infinite feedback loops),
                // BUT it can result in certain annoyances and needless calls into the C++ side,
                // so we should probably always avoid setting identical values since they are probably the result of a previous signal
                if str <> lastValue then
                    lastValue <- str
                    lineEdit.SetText(str)
            | Enabled value ->
                lineEdit.SetEnabled(value)
                
    interface LineEdit.SignalHandler with
        member this.CursorPositionChanged (oldPos, newPos) =
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
            signalDispatch (TextChanged text)
        member this.TextEdited text =
            signalDispatch (TextEdited text)
                
    interface IDisposable with
        member this.Dispose() =
            lineEdit.Dispose()

let private create (attrs: Attr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (signalMask: LineEdit.SignalMask) =
    let model = new Model<'msg>(dispatch)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private migrate (model: Model<'msg>) (attrs: Attr list) (signalMap: Signal -> 'msg option) (signalMask: LineEdit.SignalMask) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()

type LineEdit<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    member val Attrs: Attr list = [] with get, set
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    let mutable signalMask = enum<LineEdit.SignalMask> 0
    
    let mutable onCursorPositionChanged: (int * int -> 'msg) option = None
    let mutable onEditingFinished: 'msg option = None
    let mutable onInputRejected: 'msg option = None
    let mutable onReturnPressed: 'msg option = None
    let mutable onSelectionChanged: 'msg option = None
    let mutable onTextChanged: (string -> 'msg) option = None
    let mutable onTextEdited: (string -> 'msg) option = None
    
    member this.OnCursorPositionChanged with set value =
        onCursorPositionChanged <- Some value
        signalMask <- signalMask ||| LineEdit.SignalMask.CursorPositionChanged
        
    member this.OnEditingFinished with set value =
        onEditingFinished <- Some value
        signalMask <- signalMask ||| LineEdit.SignalMask.EditingFinished
        
    member this.OnInputRejected with set value =
        onInputRejected <- Some value
        signalMask <- signalMask ||| LineEdit.SignalMask.InputRejected
        
    member this.OnReturnPressed with set value =
        onReturnPressed <- Some value
        signalMask <- signalMask ||| LineEdit.SignalMask.ReturnPressed
        
    member this.OnSelectionChanged with set value =
        onSelectionChanged <- Some value
        signalMask <- signalMask ||| LineEdit.SignalMask.SelectionChanged
        
    member this.OnTextChanged with set value =
        onTextChanged <- Some value
        signalMask <- signalMask ||| LineEdit.SignalMask.TextChanged
        
    member this.OnTextEdited with set value =
        onTextEdited <- Some value
        signalMask <- signalMask ||| LineEdit.SignalMask.TextEdited

    let signalMap = function
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
                
    interface IWidgetNode<'msg> with
        override this.Dependencies = []
        
        override this.Create dispatch buildContextr =
            this.model <- create this.Attrs signalMap dispatch signalMask
            
        override this.AttachDeps () =
            ()
            
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> LineEdit<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged
            this.model <-
                migrate left'.model nextAttrs signalMap signalMask
                
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()
            
        override this.Widget =
            (this.model.Widget :> Widget.Handle)
            
        override this.ContentKey =
            (this :> IWidgetNode<'msg>).Widget
            
        override this.Attachments =
            this.Attachments
     
