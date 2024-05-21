module Widgets.Widget

open System
open BuilderNode
open Org.Whatever.QtTesting

// no signals yet

type Attr =
    | Visible of state: bool
    
let private keyFunc = function
    | Visible _ -> 0
    
let private diffAttrs =
    genericDiffAttrs keyFunc

type private Model<'msg>(dispatch: 'msg -> unit, maybeLayout: Layout.Handle option) =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable widget = Widget.Create()
    do
        let signalDispatch (s: Signal) =
            match signalMap s with
            | Some msg ->
                dispatch msg
            | None ->
                ()
                
        // no signals yet
        
        maybeLayout
        |> Option.iter widget.SetLayout
        
        // I guess show it initially, in case it's being used as a top-levle
        widget.Show()
    member this.Widget with get() = widget
    member this.SignalMap with set(value) = signalMap <- value
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Visible state ->
                widget.SetVisible(state)
    interface IDisposable with
        member this.Dispose() =
            widget.Dispose()
    member this.RemoveLayout() =
        let existing =
            widget.GetLayout()
        existing.RemoveAll()
        widget.SetLayout(null)
    member this.AddLayout(layout: Layout.Handle) =
        widget.SetLayout(layout)
        
let private create (attrs: Attr list) (maybeLayout: Layout.Handle option) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) =
    let model = new Model<'msg>(dispatch, maybeLayout)
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
    let mutable maybeLayout: ILayoutNode<'msg> option = None
    member private this.MaybeLayout = maybeLayout // need to be able to access from migration (does this need to be a function?)

    [<DefaultValue>] val mutable private model: Model<'msg>
    member val Attrs: Attr list = [] with get, set
    member private this.SignalMap
        with get() = (fun _ -> None)
    member this.Layout with set value = maybeLayout <- Some value
    
    member private this.MigrateContent(leftFrame: Node<'msg>) =
        let leftContentKey =
            leftFrame.MaybeLayout
            |> Option.map (_.ContentKey)
        let contentKey =
            maybeLayout
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
            this.model.AddLayout(maybeLayout.Value.Layout)
        | Some _, None ->
            // removed content
            this.model.RemoveLayout()
        | Some _, Some _ -> // implicit "when x <> y" because of first case
            // changed content
            this.model.RemoveLayout()
            this.model.AddLayout(maybeLayout.Value.Layout)
    
    interface IWidgetNode<'msg> with
        override this.Dependencies() =
            maybeLayout
            |> Option.map (fun content -> (StrKey "layout", content :> IBuilderNode<'msg>))
            |> Option.toList
  
        override this.Create(dispatch: 'msg -> unit) =
            let maybeLayoutHandle =
                maybeLayout
                |> Option.map (_.Layout)
            this.model <- create this.Attrs maybeLayoutHandle this.SignalMap dispatch

        override this.MigrateFrom(left: IBuilderNode<'msg>) =
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
            
        override this.ContentKey =
            (this :> IWidgetNode<'msg>).ContentKey
