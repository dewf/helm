module Widgets.Button

open BuilderNode
open System
open Org.Whatever.QtTesting

type Signal =
    | Clicked
    
type Attr =
    | Label of string
    | Enabled of bool

let private keyFunc = function
    | Label _ -> 0
    | Enabled _ -> 1

let private diffAttrs = genericDiffAttrs keyFunc

type private Model<'msg>(dispatch: 'msg -> unit) =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable button = PushButton.Create("")
    do
        let dispatcher (s: Signal) =
            match signalMap s with
            | Some msg ->
                dispatch msg
            | None ->
                ()
        button.OnClicked (fun _ -> dispatcher Clicked)
        button.SetMaximumHeight(Widget.WIDGET_SIZE_MAX - 1)
    member this.Widget with get() = button
    member this.SignalMap with set(value) = signalMap <- value
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Label text ->
                button.SetText(text)
            | Enabled state ->
                button.SetEnabled(state)
    interface IDisposable with
        member this.Dispose() =
            button.Dispose()

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
    let mutable onClicked: 'msg option = None
    member this.OnClicked
        with set value = onClicked <- Some value
        
    member private this.SignalMap
        with get() = function
            | Clicked -> onClicked
            
    override this.Dependencies() = []

    override this.Create(dispatch: 'msg -> unit) =
        this.model <- create this.Attrs this.SignalMap dispatch

    override this.MigrateFrom(left: BuilderNode<'msg>) =
        let left' = (left :?> Node<'msg>)
        let nextAttrs = diffAttrs left'.Attrs this.Attrs |> createdOrChanged
        this.model <- migrate left'.model nextAttrs this.SignalMap

    override this.Dispose() =
        (this.model :> IDisposable).Dispose()

    override this.Widget =
        (this.model.Widget :> Widget.Handle)

// various easy constructors here
let make (label: string) (onClick: 'msg) =
    Node(Attrs = [Label label], OnClicked = onClick)
