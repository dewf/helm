module ComboBox

open System
open BuilderNode

type Signal =
    | Selected of selected: (int * string) option
    
type Attr =
    | Items of items: string list
    | SelectedIndex of maybeIndex: int option

let private attrKey = function
    | Items _ -> 0
    | SelectedIndex _ -> 1
    
let private diffAttrs =
    genericDiffAttrs attrKey
    
type private Model<'msg>(dispatch: 'msg -> unit) =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable combo = new Gtk.ComboBoxText()
    do
        let signalDispatch (s: Signal) =
            match signalMap s with
            | Some msg ->
                dispatch msg
            | None ->
                ()
        combo.Changed.Add (fun _ ->
            let value =
                if combo.Active >= 0 then
                    Some (combo.Active, combo.ActiveText)
                else
                    None
            signalDispatch (Selected value))
    member this.Widget with get() = combo
    member this.SignalMap with set(value) = signalMap <- value
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Items items ->
                combo.RemoveAll()
                items
                |> List.iter combo.AppendText
            | SelectedIndex maybeIndex ->
                match maybeIndex with
                | Some value ->
                    combo.Active <- value
                | None ->
                    combo.Active <- -1
    interface IDisposable with
        member this.Dispose() =
            combo.Dispose()

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
    member val OnSelected: ((int * string) option -> 'msg) option = None with get, set
    member private this.SignalMap
        with get() = function
            | Selected maybeArgs ->
                this.OnSelected
                |> Option.map (fun f -> f maybeArgs)
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
