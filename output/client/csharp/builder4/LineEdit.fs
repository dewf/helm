module LineEdit

open System
open BuilderNode

type Signal =
    | Changed of string
    | Activated
    
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
    let mutable entry = new Gtk.Entry()
    do
        let dispatchSignal (s: Signal) =
            match signalMap s with
            | Some msg ->
                dispatch msg
            | None ->
                ()
        entry.Changed.Add (fun _ ->
            dispatchSignal (Changed entry.Text))
        entry.Activated.Add (fun _ ->
            dispatchSignal Activated)
    member this.Widget with get() = entry
    member this.SignalMap with set(value) = signalMap <- value
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Value str ->
                entry.Text <- str
            | Enabled value ->
                entry.Sensitive <- value
                // entry.IsEditable <- value
    interface IDisposable with
        member this.Dispose() =
            entry.Dispose()

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
    member val OnChanged: (string -> 'msg) option = None with get, set
    member val OnActivated: 'msg option = None with get, set
    member private this.SignalMap
        with get() = function
            | Changed s ->
                this.OnChanged
                |> Option.map (fun f -> f s)
            | Activated -> this.OnActivated
    
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
        (this.model.Widget :> Gtk.Widget)
