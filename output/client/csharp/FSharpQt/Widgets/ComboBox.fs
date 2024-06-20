module FSharpQt.Widgets.ComboBox

open System
open FSharpQt.BuilderNode
open Org.Whatever.QtTesting

type Signal =
    | Activated of index: int option
    | CurrentIndexChanged of index: int option
    | CurrentTextChanged of text: string
    | EditTextChanged of text: string
    | Highlighted of index: int option
    | TextActivated of text: string
    | TextHighlighted of text: string
    
type Attr =
    | Items of items: string list
    | CurrentIndex of maybeIndex: int option
    | MinimumWidth of width: int

let private attrKey = function
    | Items _ -> 0
    | CurrentIndex _ -> 1
    | MinimumWidth _ -> 2
    
let private diffAttrs =
    genericDiffAttrs attrKey
    
let someIfPositive (i: int) =
    if i >= 0 then Some i else None
    
type private Model<'msg>(dispatch: 'msg -> unit) as this =
    let mutable combo = ComboBox.Create(this)
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable currentMask = enum<ComboBox.SignalMask> 0
    let mutable selectedIndex: int option = None
    
    let signalDispatch (s: Signal) =
        match signalMap s with
        | Some msg ->
            dispatch msg
        | None ->
            ()
        
    member this.Widget with get() = combo
    member this.SignalMap with set value = signalMap <- value
    
    member this.SignalMask with set value =
        if value <> currentMask then
            combo.SetSignalMask(value)
            currentMask <- value
    
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Items items ->
                combo.Clear()
                combo.SetItems(items |> Array.ofList)
            | CurrentIndex maybeIndex ->
                // short-circuit identical values, see LineEdit comments for explanation why
                if maybeIndex <> selectedIndex then
                    selectedIndex <- maybeIndex
                    match maybeIndex with
                    | Some value ->
                        combo.SetCurrentIndex(value)
                    | None ->
                        combo.SetCurrentIndex(-1)
            | MinimumWidth width ->
                combo.SetMinimumWidth(width)
                        
    interface ComboBox.SignalHandler with
        override this.Activated index =
            signalDispatch (someIfPositive index |> Activated)
        override this.CurrentIndexChanged index =
            signalDispatch (someIfPositive index |> CurrentIndexChanged)
        override this.CurrentTextChanged text =
            signalDispatch (CurrentTextChanged text)
        override this.EditTextChanged text =
            signalDispatch (EditTextChanged text)
        override this.Highlighted index =
            signalDispatch (someIfPositive index |> Highlighted)
        override this.TextActivated text =
            signalDispatch (TextActivated text)
        override this.TextHighlighted text =
            signalDispatch (TextHighlighted text)
                        
    interface IDisposable with
        member this.Dispose() =
            combo.Dispose()

let private create (attrs: Attr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (signalMask: ComboBox.SignalMask) =
    let model = new Model<'msg>(dispatch)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private migrate (model: Model<'msg>) (attrs: Attr list) (signalMap: Signal -> 'msg option) (signalMask: ComboBox.SignalMask) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()

type ComboBox<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    member val Attrs: Attr list = [] with get, set
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    let mutable signalMask = enum<ComboBox.SignalMask> 0
    
    let mutable onActivated: (int option -> 'msg) option = None
    let mutable onCurrentIndexChanged: (int option -> 'msg) option = None
    let mutable onCurrentTextChanged: (string -> 'msg) option = None
    let mutable onEditTextChanged: (string -> 'msg) option = None
    let mutable onHighlighted: (int option -> 'msg) option = None
    let mutable onTextActivated: (string -> 'msg) option = None
    let mutable onTextHighlighted: (string -> 'msg) option = None
    
    member this.OnActivated with set value =
        onActivated <- Some value
        signalMask <- signalMask ||| ComboBox.SignalMask.Activated
    member this.OnCurrentIndexChanged with set value =
        onCurrentIndexChanged <- Some value
        signalMask <- signalMask ||| ComboBox.SignalMask.CurrentIndexChanged
    member this.OnCurrentTextChanged with set value =
        onCurrentTextChanged <- Some value
        signalMask <- signalMask ||| ComboBox.SignalMask.CurrentTextChanged
    member this.OnEditTextChanged with set value =
        onEditTextChanged <- Some value
        signalMask <- signalMask ||| ComboBox.SignalMask.EditTextChanged
    member this.OnHighlighted with set value =
        onHighlighted <- Some value
        signalMask <- signalMask ||| ComboBox.SignalMask.Highlighted
    member this.OnTextActivated with set value =
        onTextActivated <- Some value
        signalMask <- signalMask ||| ComboBox.SignalMask.TextActivated
    member this.OnTextHighlighted with set value =
        onTextHighlighted <- Some value
        signalMask <- signalMask ||| ComboBox.SignalMask.TextHighlighted
        
    let signalMap = function
        | Activated index ->
            onActivated
            |> Option.map (fun f -> f index)
        | CurrentIndexChanged index ->
            onCurrentIndexChanged
            |> Option.map (fun f -> f index)
        | CurrentTextChanged text ->
            onCurrentTextChanged
            |> Option.map (fun f -> f text)
        | EditTextChanged text ->
            onEditTextChanged
            |> Option.map (fun f -> f text)
        | Highlighted index ->
            onHighlighted
            |> Option.map (fun f -> f index)
        | TextActivated text ->
            onTextActivated
            |> Option.map (fun f -> f text)
        | TextHighlighted text ->
            onTextHighlighted
            |> Option.map (fun f -> f text)
                
    interface IWidgetNode<'msg> with
        override this.Dependencies = []
        
        override this.Create dispatch buildContext =
            this.model <- create this.Attrs signalMap dispatch signalMask
            
        override this.AttachDeps () =
            ()
            
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> ComboBox<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged
            this.model <- migrate left'.model nextAttrs signalMap signalMask
                
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()
            
        override this.Widget =
            (this.model.Widget :> Widget.Handle)
            
        override this.ContentKey =
            (this :> IWidgetNode<'msg>).Widget
            
        override this.Attachments =
            this.Attachments
