module Widgets.Menus.Action

open System
open BuilderNode
open Org.Whatever.QtTesting

type Signal =
    | Triggered of checked_: bool
    
type Attr =
    | Text of text: string
    | Enabled of state: bool
    
let private keyFunc = function
    | Text _ -> 0
    | Enabled _ -> 1
    
let private diffAttrs =
    genericDiffAttrs keyFunc

type private Model<'msg>(dispatch: 'msg -> unit) =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable action = Action.Create()
    do
        let signalDispatch (s: Signal) =
            match signalMap s with
            | Some msg ->
                dispatch msg
            | None ->
                ()
        action.OnTriggered (fun checked_ ->
            signalDispatch (Triggered checked_))
        
    member this.Action with get() = action
    member this.SignalMap with set value = signalMap <- value
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Text text ->
                action.SetText(text)
            | Enabled state ->
                action.SetEnabled(state)
    interface IDisposable with
        member this.Dispose() =
            action.Dispose()

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
    inherit ActionNode<'msg>()

    [<DefaultValue>] val mutable private model: Model<'msg>
    member val Attrs: Attr list = [] with get, set
    let mutable onTriggered: (bool -> 'msg) option = None
    member this.OnTriggered
        with set value =
            onTriggered <- Some value
    member private this.SignalMap
        with get() = function
            | Triggered checked_ ->
                onTriggered
                |> Option.map (fun f -> f checked_)
                
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
    override this.Action =
        this.model.Action
    
                
    
    