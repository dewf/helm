module Widgets.LineEdit

open System
open BuilderNode
open Org.Whatever.QtTesting

type Signal =
    | Changed of string
    | ReturnPressed
    | LostFocus
    | Submitted of string   // return (and optionally lost focus)
    
type Attr =
    | Value of string
    | Enabled of bool
    | SubmitOnLostFocus of bool
    
let private attrKey = function
    | Value _ -> 0
    | Enabled _ -> 1
    | SubmitOnLostFocus _ -> 2

let private diffAttrs =
    genericDiffAttrs attrKey
    
type private Model<'msg>(dispatch: 'msg -> unit) =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable edit = LineEdit.Create()
    let mutable lastValue = ""
    let mutable submitOnLostFocus = false
    do
        let dispatchSignal (s: Signal) =
            match signalMap s with
            | Some msg ->
                dispatch msg
            | None ->
                ()
        edit.OnTextEdited (fun str ->
            lastValue <- str
            dispatchSignal (Changed str))
        edit.OnReturnPressed (fun _ ->
            dispatchSignal ReturnPressed
            dispatchSignal (Submitted lastValue))
        edit.OnLostFocus (fun _ ->
            dispatchSignal LostFocus
            if submitOnLostFocus then
                dispatchSignal (Submitted lastValue))
    member this.Widget with get() = edit
    member this.SignalMap with set value = signalMap <- value
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Value str ->
                // short-circuit identical current values
                // dispatching is disabled during tree diffing so it's not the end of the world (no infinite feedback loops),
                // BUT it can result in certain annoyances and needless calls into the C++ side,
                // so we should probably always avoid setting identical values since they are probably the result of a previous signal
                if str <> lastValue then
                    edit.SetText(str)
                    lastValue <- str
            | Enabled value ->
                edit.SetEnabled(value)
            | SubmitOnLostFocus value ->
                submitOnLostFocus <- value
    interface IDisposable with
        member this.Dispose() =
            edit.Dispose()

let private create (attrs: Attr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) =
    let model = new Model<'msg>(dispatch)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model

let private migrate (model: Model<'msg>) (attrs: Attr list) (signalMap: Signal -> 'msg option) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()

type LineEdit<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>
    let mutable onChanged: (string -> 'msg) option = None
    let mutable onReturnPressed: 'msg option = None
    let mutable onLostFocus: 'msg option = None
    let mutable onSubmitted: (string -> 'msg) option = None
    member this.OnChanged with set value = onChanged <- Some value
    member this.OnReturnPressed with set value = onReturnPressed <- Some value
    member this.OnLostFocus with set value = onLostFocus <- Some value
    member this.OnSubmitted with set value = onSubmitted <- Some value
    member val Attrs: Attr list = [] with get, set
    member private this.SignalMap
        with get() = function
            | Changed s ->
                onChanged
                |> Option.map (fun f -> f s)
            | ReturnPressed ->
                onReturnPressed
            | LostFocus ->
                onLostFocus
            | Submitted value ->
                onSubmitted
                |> Option.map (fun f -> f value)
                
    interface IWidgetNode<'msg> with
        override this.Dependencies = []
        override this.Create(dispatch: 'msg -> unit) =
            this.model <- create this.Attrs this.SignalMap dispatch
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> LineEdit<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged
            this.model <-
                migrate left'.model nextAttrs this.SignalMap
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()
        override this.Widget =
            (this.model.Widget :> Widget.Handle)
        override this.ContentKey =
            (this :> IWidgetNode<'msg>).Widget
        override this.AttachedToWindow window =
            ()
            
            
