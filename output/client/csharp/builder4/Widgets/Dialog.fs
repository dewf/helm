module Widgets.Dialog

open System
open BuilderNode
open Org.Whatever.QtTesting

type Signal =
    | Accepted
    | Rejected
    
type Attr =
    | NoneYet

let private diffAttrs =
    genericDiffAttrs (function
        | NoneYet -> 0)

type private Model<'msg>(dispatch: 'msg -> unit, maybeLayout: Layout.Handle option, maybeParent: Widget.Handle option) =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable dialog =
        match maybeParent with
        | Some parent ->
            Dialog.Create(parent)
        | None ->
            Dialog.Create()
    do
        let signalDispatch (s: Signal) =
            match signalMap s with
            | Some msg ->
                dispatch msg
            | None ->
                ()
        
        dialog.OnAccepted(fun _ -> signalDispatch Accepted)
        dialog.OnRejected(fun _ -> signalDispatch Rejected)
        
        maybeLayout
        |> Option.iter dialog.SetLayout
        
    member this.Dialog with get() = dialog
    member this.SignalMap with set value = signalMap <- value
    member this.ApplyAttrs (attrs: Attr list) =
        for attr in attrs do
            match attr with
            | NoneYet ->
                ()
    
    interface IDisposable with
        member this.Dispose() =
            dialog.Dispose()
    
    member this.RemoveLayout() =
        let existing =
            dialog.GetLayout()
        existing.RemoveAll()
        dialog.SetLayout(null)
        
    member this.AddLayout (layout: Layout.Handle) =
        dialog.SetLayout(layout)

let private create (attrs: Attr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (maybeLayout: Layout.Handle option) (maybeParent: Widget.Handle option) =
    let model = new Model<'msg>(dispatch, maybeLayout, maybeParent)
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
    let mutable maybeParent: IWidgetNode<'msg> option = None
    let mutable onAccepted: 'msg option = None
    let mutable onRejected: 'msg option = None
    member private this.MaybeLayout = maybeLayout
    member private this.MaybeParent = maybeParent
    member this.Layout with set value = maybeLayout <- Some value
    member this.__ParentPrivate with set value = maybeParent <- Some value // used for dialog modal code
    member this.OnAccepted with set value = onAccepted <- Some value
    member this.OnRejected with set value = onRejected <- Some value
    
    [<DefaultValue>] val mutable private model: Model<'msg>
    member val Attrs: Attr list = [] with get, set
    
    member private this.SignalMap
        with get() = function
            | Accepted -> onAccepted
            | Rejected -> onRejected
            
    member private this.MigrateContent (chamgeMap: Map<DepsKey, DepsChange>) =
        match chamgeMap.TryFind (StrKey "layout") with
        | Some change ->
            match change with
            | Unchanged ->
                ()
            | Added ->
                this.model.AddLayout(maybeLayout.Value.Layout)
            | Removed ->
                this.model.RemoveLayout()
            | Swapped ->
                this.model.RemoveLayout()
                this.model.AddLayout(maybeLayout.Value.Layout)
        | None ->
            // neither side had layout
            ()
        // layout only - parent can't be changed after creation
    
    interface IDialogNode<'msg> with
        override this.Dependencies() =
            // maybeParent not a dependency, would create circular problems (and is intended only for internal modal use)
            maybeLayout
            |> Option.map (fun content -> (StrKey "layout", content :> IBuilderNode<'msg>))
            |> Option.toList
            
        override this.Create(dispatch: 'msg -> unit) =
            let maybeParentHandle =
                maybeParent
                |> Option.map (_.Widget)
            let maybeLayoutHandle =
                maybeLayout
                |> Option.map (_.Layout)
            this.model <- create this.Attrs this.SignalMap dispatch maybeLayoutHandle maybeParentHandle
            
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> Node<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged
            this.model <- migrate left'.model nextAttrs this.SignalMap
            this.MigrateContent (depsChanges |> Map.ofList)
            
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()
            
        override this.Dialog =
            this.model.Dialog
            
        override this.ContentKey =
            (this :> IDialogNode<'msg>).ContentKey
