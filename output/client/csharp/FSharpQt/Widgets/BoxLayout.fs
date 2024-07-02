module FSharpQt.Widgets.BoxLayout

open System
open Org.Whatever.QtTesting
open FSharpQt.BuilderNode
open FSharpQt.Extensions

open FSharpQt.MiscTypes
open FSharpQt.Attrs

// no signals

type Direction =
    | LeftToRight
    | RightToLeft
    | TopToBottom
    | BottomToTop
with
    member this.QtValue =
        match this with
        | LeftToRight -> BoxLayout.Direction.LeftToRight
        | RightToLeft -> BoxLayout.Direction.RightToLeft
        | TopToBottom -> BoxLayout.Direction.TopToBottom
        | BottomToTop -> BoxLayout.Direction.BottomToTop

type private Attr =
    | Direction of dir: Direction
with
    interface IAttr with
        override this.AttrEquals other =
            match other with
            | :? Attr as attrOther ->
                this = attrOther
            | _ ->
                false
        override this.Key =
            match this with
            | Direction _ -> "boxlayout:direction"
        override this.ApplyTo (target: IAttrTarget, maybePrev: IAttr option) =
            match target with
            | :? BoxLayoutAttrTarget as boxTarget ->
                let boxLayout =
                    boxTarget.BoxLayout
                match this with
                | Direction dir ->
                    boxLayout.SetDirection(dir.QtValue)
            | _ ->
                printfn "warning: BoxLayout.Attr couldn't ApplyTo() unknown target type [%A]" target
                
type Props<'msg>() =
    inherit Layout.Props<'msg>()
    
    member internal this.SignalMapList =
        // prepend to parent signal map funcs
        NullSignalMapFunc() :> ISignalMapFunc :: base.SignalMapList
    
    member this.Direction with set value =
        this.PushAttr(Direction value)

[<RequireQualifiedAccess>]
type internal ItemKey<'msg> =
    // used for internal comparisons, since we can't compare builder node interfaces against each other, we use the ContentKeys
    | WidgetItem of content: ContentKey * maybeStretch: int option * maybeAlign: Alignment option
    | LayoutItem of content: ContentKey * maybeStretch: int option
    | Spacer of sp: int
    | Stretch of stretch: int
    | Ignore
    
type internal InternalItem<'msg> =
    | WidgetItem of w: IWidgetNode<'msg> * maybeStretch: int option * maybeAlign: Alignment option
    | LayoutItem of l: ILayoutNode<'msg> * maybeStretch: int option
    | Spacer of sp: int
    | Stretch of stretch: int
    | Ignore
with
    member this.Key =
        match this with
        | WidgetItem(w, stretch, align) ->
            ItemKey.WidgetItem (w.ContentKey, stretch, align)
        | LayoutItem(l, stretch) ->
            ItemKey.LayoutItem (l.ContentKey, stretch)
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
        | WidgetItem(w, _, _) -> w :> IBuilderNode<'msg> |> Some
        | LayoutItem(l, _) -> l :> IBuilderNode<'msg> |> Some
        | Spacer _ -> None
        | Stretch _ -> None
        | Ignore -> None
    
    new(w: IWidgetNode<'msg>, ?stretch: int, ?align: Alignment) =
        BoxItem(WidgetItem (w, stretch, align))
        
    new(l: ILayoutNode<'msg>, ?stretch: int) =
        BoxItem(LayoutItem (l, stretch))
        
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
        
let private addItem (box: BoxLayout.Handle) (item: InternalItem<'msg>) =
    match item with
    | WidgetItem(w, maybeStretch, maybeAlign) ->
        match maybeStretch, maybeAlign with
        | None, None ->
            box.AddWidget(w.Widget)
        | None, Some align ->
            box.AddWidget(w.Widget, 0, align.QtValue)
        | Some stretch, None ->
            box.AddWidget(w.Widget, stretch)
        | Some stretch, Some align ->
            box.AddWidget(w.Widget, stretch, align.QtValue)
    | LayoutItem(l, maybeStretch) ->
        match maybeStretch with
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
        
type ModelCore<'msg>(dispatch: 'msg -> unit) =
    inherit Layout.ModelCore<'msg>(dispatch: 'msg -> unit)
    let mutable boxLayout: BoxLayout.Handle = null
    // no signalmap or mask needed
    
    member this.BoxLayout
        with get() =
            boxLayout
        and set value =
            // must assign base version(s) as well!!
            this.Layout <- value
            boxLayout <- value
            
    member internal this.SignalMaps with set (mapFuncList: ISignalMapFunc list) =
        match mapFuncList with
        | h :: etc ->
            match h with
            | :? NullSignalMapFunc ->
                // nothing to do
                ()
            | _ ->
                failwith "BoxLayout.ModelCore.SignalMaps: wrong func type"
            // assign the remainder
            base.SignalMaps <- etc
        | _ ->
            failwith "BoxLayout.ModelCore: signal map assignment didn't have a head element"
            
    interface BoxLayoutAttrTarget with
        // since these attr targets inherit, we shouldn't actually have to implement the parent getters
        // (because one of the first things we do, is set the parent's widget value to ours)
        override this.Layout = boxLayout
        override this.BoxLayout = boxLayout
        
    interface IDisposable with
        member this.Dispose() =
            boxLayout.Dispose()

type private Model<'msg>(dispatch: 'msg -> unit, initialDirection: BoxLayout.Direction) as this =
    inherit ModelCore<'msg>(dispatch)
    let boxLayout = BoxLayout.Create(initialDirection)
    do
        this.BoxLayout <- boxLayout
        
    member this.ApplyAttrs(attrs: (IAttr option * IAttr) list) =
        for maybePrev, attr in attrs do
            attr.ApplyTo(this, maybePrev)

    member this.AttachDeps (items: BoxItem<'msg> list) =
        for item in items do
            addItem boxLayout item.Item
    
    member this.Refill(items: BoxItem<'msg> list) =
        boxLayout.RemoveAll()
        for item in items do
            addItem boxLayout item.Item

let private create (attrs: IAttr list) (signalMaps: ISignalMapFunc list) (dispatch: 'msg -> unit) (initialDirection: BoxLayout.Direction) =
    let model = new Model<'msg>(dispatch, initialDirection)
    model.ApplyAttrs (attrs |> List.map (fun attr -> None, attr))
    model.SignalMaps <- signalMaps
    // no signals, so no mask
    // (unless we inherit from QObject in the future)
    model

let private migrate (model: Model<'msg>) (attrs: (IAttr option * IAttr) list) (signalMaps: ISignalMapFunc list) =
    model.ApplyAttrs attrs
    model.SignalMaps <- signalMaps
    // no signals, so no mask
    // (unless we inherit from QObject in the future)
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()
    
type BoxLayoutBase<'msg>(initialDirection: BoxLayout.Direction) =
    inherit Props<'msg>()
    [<DefaultValue>] val mutable private model: Model<'msg>
    
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
            this.model <- create this.Attrs this.SignalMapList dispatch initialDirection
            
        override this.AttachDeps () =
            this.model.AttachDeps this.Items
        
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> BoxLayoutBase<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged
            this.model <- migrate left'.model nextAttrs this.SignalMapList
            this.MigrateContent(left')
                
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()

        override this.Layout =
            this.model.Layout
            
        override this.ContentKey =
            this.model.Layout
            
        override this.Attachments = this.Attachments

type BoxLayout<'msg>() =
    inherit BoxLayoutBase<'msg>(BoxLayout.Direction.TopToBottom)
    
type VBoxLayout<'msg>() =
    inherit BoxLayoutBase<'msg>(BoxLayout.Direction.TopToBottom)
    
type HBoxLayout<'msg>() =
    inherit BoxLayoutBase<'msg>(BoxLayout.Direction.LeftToRight)
