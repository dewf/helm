module Window

open System
open BuilderNode

type Signal =
    | Shown
    | Destroyed
type Attr =
    | Title of string
    | Size of int * int
    | Visible of bool
    | ExitOnClose of bool
let private keyFunc = function
    | Title _ -> 0
    | Size _ -> 1
    | Visible _ -> 2
    | ExitOnClose _ -> 3
let private diffAttrs = genericDiffAttrs keyFunc

type private Model<'msg>(dispatch: 'msg -> unit, maybeContent: Gtk.Widget option) =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable exitOnClose = false
    let mutable box = new Gtk.Box(Gtk.Orientation.Vertical, 0)
    let mutable window = new Gtk.Window(Gtk.WindowType.Toplevel)
    do
        window.Mapped.Add (fun _ ->
            signalMap(Shown)
            |> Option.iter dispatch)

        window.Destroyed.Add (fun _ ->
            signalMap(Destroyed)
            |> Option.iter dispatch)

        window.DeleteEvent.Add (fun _ ->
            if exitOnClose then
                Gtk.Application.Quit())

        box.BorderWidth <- uint32 10
        maybeContent
        |> Option.iter box.Add // pack?
        window.Add box
        window.ShowAll()

    member this.RemoveContent() =
        for ch in box.Children do
            ch.Destroy()
        window.ShowAll()

    member this.AddContent(thing: Gtk.Widget) =
        box.Add(thing)
        window.ShowAll()

    member this.Widget with get() = window
    member this.SignalMap with set(value) = signalMap <- value
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Title text -> window.Title <- text
            | Size (w, h) -> window.Resize(w, h)
            | Visible v -> window.Visible <- v
            | ExitOnClose v -> exitOnClose <- v

    interface IDisposable with
        member this.Dispose() =
            window.Dispose()

let private exitHandler (args: Gtk.DeleteEventArgs) =
    Gtk.Application.Quit()

let private create (attrs: Attr list) (maybeWidget: Gtk.Widget option) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) =
    let model = new Model<'msg>(dispatch, maybeWidget)
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
    let mutable maybeContent: WidgetNode<'msg> option = None
    member private this.MaybeContent = maybeContent // need to be able to access from migration (does this need to be a function?)

    [<DefaultValue>] val mutable private model: Model<'msg>
    member val Attrs: Attr list = [] with get, set
    member val OnShown: 'msg option = None with get, set
    member val OnDestroyed: 'msg option = None with get, set
    member private this.SignalMap
        with get() = function
            | Shown -> this.OnShown
            | Destroyed -> this.OnDestroyed
    member this.Content with set(value) = maybeContent <- Some value

    override this.Dependencies() =
        maybeContent
        |> Option.map (fun content -> (0, content :> BuilderNode<'msg>))
        |> Option.toList

    override this.Create(dispatch: 'msg -> unit) =
        let maybeWidget =
            maybeContent |> Option.map (fun c -> c.Widget)
        this.model <- create this.Attrs maybeWidget this.SignalMap dispatch

    member private this.MigrateContent(leftFrame: Node<'msg>) =
        let leftContentKey =
            leftFrame.MaybeContent
            |> Option.map (_.ContentKey)
        let contentKey =
            maybeContent
            |> Option.map (_.ContentKey)
        match leftContentKey, contentKey with
        | None, None ->
            // both no content
            ()
        | Some x, Some y when x = y ->
            // same content
            ()
        | None, Some _ ->
            // added content, from nothing
            this.model.AddContent(maybeContent.Value.Widget)
        | Some _, None ->
            // removed content
            this.model.RemoveContent()
        | Some _, Some _ -> // implicit "when x <> y" because of first case
            // changed content
            this.model.RemoveContent()
            this.model.AddContent(maybeContent.Value.Widget)

    override this.MigrateFrom(left: BuilderNode<'msg>) =
        let left' = (left :?> Node<'msg>)
        let nextAttrs =
            diffAttrs left'.Attrs this.Attrs
            |> createdOrChanged
        this.model <- migrate left'.model nextAttrs this.SignalMap
        this.MigrateContent(left')

    override this.Dispose() =
        (this.model :> IDisposable).Dispose()

    override this.Widget =
        (this.model.Widget :> Gtk.Widget)
