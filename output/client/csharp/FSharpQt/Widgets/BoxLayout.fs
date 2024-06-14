module FSharpQt.Widgets.BoxLayout

open System
open FSharpQt.BuilderNode
open Org.Whatever.QtTesting
open FSharpQt.Extensions

// no signals yet

type DirectionValue =
    | TopToBottom
    | LeftToRight

type Attr =
    | Direction of dir: DirectionValue
    | Spacing of spacing: int
    | ContentsMargins of left: int * top: int * right: int * bottom: int
    
let private keyFunc = function
    | Direction _ -> 0
    | Spacing _ -> 1
    | ContentsMargins _ -> 2
    
let private diffAttrs =
    genericDiffAttrs keyFunc
    
type ItemInfo = {
    Stretch: int option
    Align: Common.Alignment option  // irrelevant for layout items
}

[<RequireQualifiedAccess>]
type internal ItemKey<'msg> =
    // used for internal comparisons, since we can't compare builder node interfaces against each other, we use the ContentKeys
    | WidgetItem of content: Object * info: ItemInfo
    | LayoutItem of content: Object * info: ItemInfo
    | Spacer of sp: int
    | Stretch of stretch: int
    | Ignore
    
type internal InternalItem<'msg> =
    | WidgetItem of w: IWidgetNode<'msg> * info: ItemInfo
    | LayoutItem of l: ILayoutNode<'msg> * info: ItemInfo
    | Spacer of sp: int
    | Stretch of stretch: int
    | Ignore
with
    member this.Key =
        match this with
        | WidgetItem(w, info) ->
            ItemKey.WidgetItem (w.ContentKey, info)
        | LayoutItem(l, info) ->
            ItemKey.LayoutItem (l.ContentKey, info)
        | Spacer sp ->
            ItemKey.Spacer sp
        | Stretch stretch ->
            ItemKey.Stretch stretch
        | Ignore ->
            ItemKey.Ignore
        
type BoxItem<'msg> private(item: InternalItem<'msg>) =
    member val internal Item = item
    
    member this.MaybeNode =
        match item with
        | WidgetItem(w, _) -> w :> IBuilderNode<'msg> |> Some
        | LayoutItem(l, _) -> l :> IBuilderNode<'msg> |> Some
        | Spacer _ -> None
        | Stretch _ -> None
        | Ignore -> None
    
    new(w: IWidgetNode<'msg>, ?stretch: int, ?align: Common.Alignment) =
        let info =
            { Stretch = defaultArg (Some stretch) None
              Align = defaultArg (Some align) None }
        BoxItem(WidgetItem (w, info))
        
    new(l: ILayoutNode<'msg>, ?stretch: int) =
        let info =
            { Stretch = defaultArg (Some stretch) None
              Align = None }
        BoxItem(LayoutItem (l, info))
        
    new(?stretch: int, ?spacer: int) =
        match stretch with
        | Some value ->
            // spacer ignored, only one valid at a time
            BoxItem(Stretch value)
        | None ->
            match spacer with
            | Some value ->
                BoxItem(Spacer value)
            | None ->
                BoxItem(Ignore)
                
    // these are annoying because you have to do BoxItem<_>.Whatever etc
    // hence the ctor above with optionals for stretch and spacer
    static member Spacer(sp: int) =
        BoxItem(Spacer(sp))
        
    static member Stretch(stretch: int) =
        BoxItem(Stretch(stretch))
        
    member internal this.InternalKey =
        item.Key
        
let internal addItem (box: BoxLayout.Handle) (item: InternalItem<'msg>) =
    match item with
    | WidgetItem(w, info) ->
        match info.Stretch, info.Align with
        | None, None ->
            box.AddWidget(w.Widget)
        | None, Some align ->
            box.AddWidget(w.Widget, 0, align)
        | Some stretch, None ->
            box.AddWidget(w.Widget, stretch)
        | Some stretch, Some align ->
            box.AddWidget(w.Widget, stretch, align)
    | LayoutItem(l, info) ->
        match info.Stretch with
        | Some stretch ->
            box.AddLayout(l.Layout, stretch)
        | None ->
            box.AddLayout(l.Layout)
    | Spacer sp ->
        box.AddSpacing(sp)
    | Stretch stretch ->
        box.AddStretch(stretch)
    | Ignore ->
        ()

type private Model<'msg>(dispatch: 'msg -> unit, initialDirection: BoxLayout.Direction) =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable box = BoxLayout.Create(initialDirection)

    member this.Layout with get() = box
    member this.SignalMap with set value = signalMap <- value
    
    member this.AttachDeps (items: BoxItem<'msg> list) =
        for item in items do
            addItem box item.Item
    
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Direction ov ->
                let dir =
                    match ov with
                    | TopToBottom -> BoxLayout.Direction.TopToBottom
                    | LeftToRight -> BoxLayout.Direction.LeftToRight
                box.SetDirection(dir)
            | Attr.Spacing spacing ->
                box.SetSpacing(spacing)
            | ContentsMargins (left, top, right, bottom) ->
                box.SetContentsMargins (left, top, right, bottom)
                
    interface IDisposable with
        member this.Dispose() =
            box.Dispose()
            
    member this.Refill(items: BoxItem<'msg> list) =
        box.RemoveAll()
        for item in items do
            addItem box item.Item

let private create (attrs: Attr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (initialDirection: BoxLayout.Direction) =
    let model = new Model<'msg>(dispatch, initialDirection)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model

let private migrate (model: Model<'msg>) (attrs: Attr list) (signalMap: Signal -> 'msg option) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()
    
type BoxLayoutBase<'msg>(initialDirection: BoxLayout.Direction) =
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    let signalMap = (fun _ -> None)
    
    member val Attrs: Attr list = [] with get, set
    member val Items: BoxItem<'msg> list = [] with get, set
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
        
    member private this.MigrateContent(leftBox: BoxLayoutBase<'msg>) =
        let leftContents =
            leftBox.Items
            |> List.map (_.InternalKey)
        let thisContents =
            this.Items
            |> List.map (_.InternalKey)
        if leftContents <> thisContents then
            this.model.Refill(this.Items)
        else
            ()
        
    interface ILayoutNode<'msg> with
        override this.Dependencies =
            // because the indices are generated here, based on items order,
            // it prevents the possibility of the "user" (app developer) from being able to reorder existing items without them being destroyed/recreated entirely
            // but I don't think that's a very common use case, to be reordering anything in a vbox/hbox, except maybe adding things at the end (which should work fine)
            // if user-reordering was a common use case, then the (API) user would have to provide item keys / IDs as part of the item list
            // we'll do that for example with top-level windows in the app window order, so that windows can be added/removed without forcing a rebuild of existing windows
            this.Items
            |> List.zipWithIndex
            |> List.choose (fun (i, item) ->
                item.MaybeNode
                |> Option.map (fun node -> (IntKey i, node)))
            
        override this.Create dispatch buildContext =
            this.model <- create this.Attrs signalMap dispatch initialDirection
            
        override this.AttachDeps () =
            this.model.AttachDeps this.Items
        
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> BoxLayoutBase<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged
            this.model <- migrate left'.model nextAttrs signalMap
            this.MigrateContent(left')
                
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()

        override this.Layout =
            (this.model.Layout :> Layout.Handle)
            
        override this.ContentKey =
            (this :> ILayoutNode<'msg>).Layout
            
        override this.Attachments = this.Attachments

type BoxLayout<'msg>() =
    inherit BoxLayoutBase<'msg>(BoxLayout.Direction.TopToBottom)
    
type VBoxLayout<'msg>() =
    inherit BoxLayoutBase<'msg>(BoxLayout.Direction.TopToBottom)
    
type HBoxLayout<'msg>() =
    inherit BoxLayoutBase<'msg>(BoxLayout.Direction.LeftToRight)

