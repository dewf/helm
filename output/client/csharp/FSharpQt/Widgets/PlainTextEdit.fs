module FSharpQt.Widgets.PlainTextEdit

open System
open FSharpQt.MiscTypes
open Org.Whatever.QtTesting

open FSharpQt
open BuilderNode

type Signal =
    | BlockCountChanged of newCount: int
    | CopyAvailable of state: bool
    | CursorPositionChanged
    | ModificationChanged of changed: bool
    | RedoAvailable of state: bool
    | SelectionChanged
    | TextChanged
    | UndoAvailable of state: bool
    | UpdateRequest of rect: Rect * dy: int

type Attr =
    | PlainText of text: string
    
let private keyFunc = function
    | PlainText _ -> 0

let private diffAttrs =
    genericDiffAttrs keyFunc

type private Model<'msg>(dispatch: 'msg -> unit) as this =
    let mutable plainTextEdit = PlainTextEdit.Create(this)
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable currentMask = enum<PlainTextEdit.SignalMask> 0
    
    let mutable currentText = ""
    
    let signalDispatch (s: Signal) =
        match signalMap s with
        | Some msg ->
            dispatch msg
        | None ->
            ()
        
    member this.Widget with get() = plainTextEdit
    member this.SignalMap with set value = signalMap <- value
    
    member this.SignalMask with set value =
        if value <> currentMask then
            plainTextEdit.SetSignalMask(value)
            currentMask <- value
    
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | PlainText text ->
                if text <> currentText then
                    currentText <- text
                    plainTextEdit.SetPlainText(text)
                
    interface PlainTextEdit.SignalHandler with
        member this.BlockCountChanged newCount =
            signalDispatch (BlockCountChanged newCount)
        member this.CopyAvailable state =
            signalDispatch (CopyAvailable state)
        member this.CursorPositionChanged () =
            signalDispatch CursorPositionChanged
        member this.ModificationChanged changed =
            signalDispatch (ModificationChanged changed)
        member this.RedoAvailable state =
            signalDispatch (RedoAvailable state)
        member this.SelectionChanged () =
            signalDispatch SelectionChanged
        member this.TextChanged () =
            currentText <- plainTextEdit.ToPlainText() // hmmm
            signalDispatch TextChanged
        member this.UndoAvailable state =
            signalDispatch (UndoAvailable state)
        member this.UpdateRequest (qRect, dy) =
            signalDispatch (UpdateRequest (Rect.From qRect, dy))

    interface IDisposable with
        member this.Dispose() =
            plainTextEdit.Dispose()

let private create (attrs: Attr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (signalMask: PlainTextEdit.SignalMask) =
    let model = new Model<'msg>(dispatch)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private migrate (model: Model<'msg>) (attrs: Attr list) (signalMap: Signal -> 'msg option) (signalMask: PlainTextEdit.SignalMask) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()

type PlainTextEdit<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    member val Attrs: Attr list = [] with get, set
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    let mutable maybeMethodProxy: PlainTextEditProxy option = None
    member this.MethodProxy with set value = maybeMethodProxy <- Some value
    
    let mutable signalMask = enum<PlainTextEdit.SignalMask> 0
    
    let mutable onBlockCountChanged: (int -> 'msg) option = None
    let mutable onCopyAvailable: (bool -> 'msg) option = None
    let mutable onCursorPositionChanged: 'msg option = None
    let mutable onModificationChanged: (bool -> 'msg) option = None
    let mutable onRedoAvailable: (bool -> 'msg) option = None
    let mutable onSelectionChanged: 'msg option = None
    let mutable onTextChanged: 'msg option = None
    let mutable onUndoAvailable: (bool -> 'msg) option = None
    let mutable onUpdateRequest: (Rect * int -> 'msg) option = None
    
    member this.OnBlockCountChanged with set value =
        onBlockCountChanged <- Some value
        
    member this.OnCopyAvailable with set value =
        onCopyAvailable <- Some value
        
    member this.OnCursorPositionChanged with set value =
        onCursorPositionChanged <- Some value
        
    member this.OnModificationChanged with set value =
        onModificationChanged <- Some value
        
    member this.OnRedoAvailable with set value =
        onRedoAvailable <- Some value
        
    member this.OnSelectionChanged with set value =
        onSelectionChanged <- Some value
        
    member this.OnTextChanged with set value =
        onTextChanged <- Some value
        
    member this.OnUndoAvailable with set value =
        onUndoAvailable <- Some value
        
    member this.OnUpdateRequest with set value =
        onUpdateRequest <- Some value

    let signalMap (s: Signal) =
        match s with
        | BlockCountChanged newCount ->
            onBlockCountChanged
            |> Option.map (fun f -> f newCount)
        | CopyAvailable state ->
            onCopyAvailable
            |> Option.map (fun f -> f state)
        | CursorPositionChanged ->
            onCursorPositionChanged
        | ModificationChanged changed ->
            onModificationChanged
            |> Option.map (fun f -> f changed)
        | RedoAvailable state ->
            onRedoAvailable
            |> Option.map (fun f -> f state)
        | SelectionChanged ->
            onSelectionChanged
        | TextChanged ->
            onTextChanged
        | UndoAvailable state ->
            onUndoAvailable
            |> Option.map (fun f -> f state)
        | UpdateRequest(rect, dy) ->
            onUpdateRequest
            |> Option.map (fun f -> f (rect, dy))

    interface IWidgetNode<'msg> with
        override this.Dependencies = []

        override this.Create dispatch buildContext =
            this.model <- create this.Attrs signalMap dispatch signalMask
            maybeMethodProxy
            |> Option.iter (fun mp -> mp.Handle <- this.model.Widget)
            
        override this.AttachDeps () =
            ()

        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> PlainTextEdit<'msg>)
            let nextAttrs = diffAttrs left'.Attrs this.Attrs |> createdOrChanged
            this.model <- migrate left'.model nextAttrs signalMap signalMask
            maybeMethodProxy
            |> Option.iter (fun mp -> mp.Handle <- this.model.Widget)

        override this.Dispose() =
            (this.model :> IDisposable).Dispose()

        override this.Widget =
            (this.model.Widget :> Widget.Handle)
            
        override this.ContentKey =
            this.model.Widget
            
        override this.Attachments =
            this.Attachments
