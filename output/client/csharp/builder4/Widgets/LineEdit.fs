module Widgets.LineEdit

open System
open BuilderNode
open Org.Whatever.QtTesting

type Signal =
    | Changed of string
    | ReturnPressed
    
type Attr =
    | Value of string
    | Enabled of bool
let private attrKey = function
    | Value _ -> 0
    | Enabled _ -> 1

let private diffAttrs =
    genericDiffAttrs attrKey
    
type private Model<'msg>(dispatch: 'msg -> unit) =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable edit = LineEdit.Create()
    do
        let dispatchSignal (s: Signal) =
            match signalMap s with
            | Some msg ->
                dispatch msg
            | None ->
                ()
        edit.OnTextEdited (fun str -> dispatchSignal (Changed str))
        edit.OnReturnPressed (fun _ -> dispatchSignal ReturnPressed)
    member this.Widget with get() = edit
    member this.SignalMap with set(value) = signalMap <- value
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Value str ->
                edit.SetText(str)
            | Enabled value ->
                edit.SetEnabled(value)
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

type Node<'msg>() =
    inherit WidgetNode<'msg>()

    [<DefaultValue>] val mutable private model: Model<'msg>
    member val Attrs: Attr list = [] with get, set
    let mutable onChanged: (string -> 'msg) option = None
    member this.OnChanged
        with set value = onChanged <- Some value
    let mutable onReturnPressed: 'msg option = None
    member this.OnReturnPressed
        with set value = onReturnPressed <- Some value
    member private this.SignalMap
        with get() = function
            | Changed s ->
                onChanged
                |> Option.map (fun f -> f s)
            | ReturnPressed ->
                onReturnPressed
                
    override this.Dependencies() = []

    override this.Create(dispatch: 'msg -> unit) =
        this.model <- create this.Attrs this.SignalMap dispatch

    override this.MigrateFrom(left: BuilderNode<'msg>) =
        let left' = (left :?> Node<'msg>)
        let nextAttrs =
            diffAttrs left'.Attrs this.Attrs
            |> createdOrChanged
        this.model <-
            migrate left'.model nextAttrs this.SignalMap

    override this.Dispose() =
        (this.model :> IDisposable).Dispose()
        
    override this.Widget =
        (this.model.Widget :> Widget.Handle)
