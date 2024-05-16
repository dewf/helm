module Box

open System
open BuilderNode
open Org.Whatever.QtTesting

type private Signal = // just a placeholder for the future
    | NoneYet

type DirectionValue =
    | Vertical
    | Horizontal

type Attr =
    | Direction of dir: DirectionValue
    | Spacing of spacing: int
    
let private keyFunc = function
    | Direction _ -> 0
    | Spacing _ -> 1
    
let private diffAttrs =
    genericDiffAttrs keyFunc

type private Model<'msg>(dispatch: 'msg -> unit, items: Widget.Handle list) =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable box = BoxLayout.Create(BoxLayout.Direction.TopToBottom)
    do
        // no signals yet
        for item in items do
            box.AddWidget(item)
    member this.Layout with get() = box
    member this.SignalMap with set(value) = signalMap <- value
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Direction ov ->
                let dir =
                    match ov with
                    | Vertical -> BoxLayout.Direction.TopToBottom
                    | Horizontal -> BoxLayout.Direction.LeftToRight
                box.SetDirection(dir)
            | Spacing spacing ->
                box.SetSpacing(spacing)
    interface IDisposable with
        member this.Dispose() =
            box.Dispose()
    member this.Refill(items: Widget.Handle list) =
        box.RemoveAll()
        for item in items do
            box.AddWidget(item)

let private create (attrs: Attr list) (items: Widget.Handle list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) =
    let model = new Model<'msg>(dispatch, items)
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
    inherit LayoutNode<'msg>()
    let mutable items: WidgetNode<'msg> list = []

    [<DefaultValue>] val mutable private model: Model<'msg>
    member val Attrs: Attr list = [] with get, set
    member val private SignalMap: Signal -> 'msg option = (fun _ -> None) with get, set // just pass through to model
    member this.Items
        with get() = items
        and set(value) = items <- value
    
    override this.Dependencies() =
        // because the indices are generated here, based on items order,
        // it prevents the possibility of the "user" (app developer) from being able to reorder existing items without them being destroyed/recreated entirely
        // but I don't think that's a very common use case, to be reordering anything in a vbox/hbox, except maybe adding things at the end (which should work fine)
        // if user-reordering was a common use case, then the user would have to provide item keys / IDs as part of the item list
        // we'll do that for example with top-level windows in the app window order, so that windows can be added/removed without forcing a rebuild of existing windows
        items
        |> List.mapi (fun i item -> (i, item :> BuilderNode<'msg>))
        
    override this.Create(dispatch: 'msg -> unit) =
        let widgets =
            items
            |> List.map (_.Widget)
        this.model <- create this.Attrs widgets this.SignalMap dispatch
        
    member private this.MigrateContent(leftBox: Node<'msg>) =
        let leftContents =
            leftBox.Items
            |> List.map (_.ContentKey)
        let thisContents =
            items
            |> List.map (_.ContentKey)
        if leftContents <> thisContents then
            let widgets =
                items
                |> List.map (_.Widget)
            this.model.Refill(widgets)
        else
            ()
            
    override this.MigrateFrom(left: BuilderNode<'msg>) =
        let left' = (left :?> Node<'msg>)
        let nextAttrs =
            diffAttrs left'.Attrs this.Attrs
            |> createdOrChanged
        this.model <- migrate left'.model nextAttrs this.SignalMap
        this.MigrateContent(left')
            
    override this.Dispose() =
        (this.model :> IDisposable).Dispose()

    override this.Layout =
        (this.model.Layout :> Layout.Handle)

let make (attrs: Attr list) (items: WidgetNode<'msg> list) =
    Node(Attrs = attrs, Items = items)
