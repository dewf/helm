module Window

open System
open BuilderNode
open Org.Whatever.QtTesting

type Signal =
    | TitleChanged of title: string
    
type Attr =
    | Title of title: string
    | Size of width: int * height: int
    | Visible of state: bool
    | ExitOnClose of value: bool
let private keyFunc = function
    | Title _ -> 0
    | Size _ -> 1
    | Visible _ -> 2
    | ExitOnClose _ -> 3
let private diffAttrs = genericDiffAttrs keyFunc

type private Model<'msg>(dispatch: 'msg -> unit, maybeContent: Layout.Handle option) =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable exitOnClose = false
    let mutable window = Widget.Create()
    do
        let signalDispatch (s: Signal) =
            match signalMap s with
            | Some msg ->
                dispatch msg
            | None ->
                ()
        window.OnWindowTitleChanged (fun title ->
            signalDispatch (TitleChanged title))

        maybeContent
        |> Option.iter window.SetLayout

        window.Show()

    member this.RemoveContent() =
        let existing =
            window.GetLayout()
        existing.RemoveAll()
        window.SetLayout(null)
        // window.ShowAll()

    member this.AddContent(thing: Layout.Handle) =
        window.SetLayout(thing)
        // window.Show()

    member this.Widget with get() = window
    member this.SignalMap with set(value) = signalMap <- value
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Title text ->
                window.SetWindowTitle(text)
            | Size (width, height) ->
                window.Resize(width, height)
            | Visible state ->
                window.SetVisible(state)
            | ExitOnClose value ->
                exitOnClose <- value

    interface IDisposable with
        member this.Dispose() =
            window.Dispose()

// let private exitHandler (args: Gtk.DeleteEventArgs) =
//     Gtk.Application.Quit()

let private create (attrs: Attr list) (maybeContent: Layout.Handle option) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) =
    let model = new Model<'msg>(dispatch, maybeContent)
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
    let mutable maybeContent: LayoutNode<'msg> option = None
    member private this.MaybeContent = maybeContent // need to be able to access from migration (does this need to be a function?)

    [<DefaultValue>] val mutable private model: Model<'msg>
    member val Attrs: Attr list = [] with get, set
    // member val OnShown: 'msg option = None with get, set
    // member val OnDestroyed: 'msg option = None with get, set
    member val OnTitleChanged: (string -> 'msg) option = None with get, set
    member private this.SignalMap
        with get() = function
            | TitleChanged title ->
                this.OnTitleChanged
                |> Option.map (fun f -> f title)
    member this.Content with set(value) = maybeContent <- Some value

    override this.Dependencies() =
        maybeContent
        |> Option.map (fun content -> (0, content :> BuilderNode<'msg>))
        |> Option.toList

    override this.Create(dispatch: 'msg -> unit) =
        let maybeLayout =
            maybeContent
            |> Option.map (_.Layout)
        this.model <- create this.Attrs maybeLayout this.SignalMap dispatch

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
            this.model.AddContent(maybeContent.Value.Layout)
        | Some _, None ->
            // removed content
            this.model.RemoveContent()
        | Some _, Some _ -> // implicit "when x <> y" because of first case
            // changed content
            this.model.RemoveContent()
            this.model.AddContent(maybeContent.Value.Layout)

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
        this.model.Widget
