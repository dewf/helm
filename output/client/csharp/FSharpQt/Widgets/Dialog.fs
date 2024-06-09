module FSharpQt.Widgets.Dialog

open System
open FSharpQt.BuilderNode
open Org.Whatever.QtTesting

type Signal =
    | Closed of accepted: bool
    
type Modality =
    | WindowModal
    | AppModal
    
type Attr =
    | Size of width: int * height: int
    | Title of title: string
    | Modality of modality: Modality
    
let private keyFunc = function
    | Size _ -> 0
    | Title _ -> 1
    | Modality _ -> 2

let private diffAttrs =
    genericDiffAttrs keyFunc

type private Model<'msg>(dispatch: 'msg -> unit, maybeLayout: Layout.Handle option) =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable dialog = Dialog.Create()
    do
        let signalDispatch (s: Signal) =
            match signalMap s with
            | Some msg ->
                dispatch msg
            | None ->
                ()
        
        dialog.OnAccepted(fun _ -> signalDispatch (Closed true))
        dialog.OnRejected(fun _ -> signalDispatch (Closed false))
        
        maybeLayout
        |> Option.iter dialog.SetLayout
        
    member this.Dialog with get() = dialog
    member this.SignalMap with set value = signalMap <- value
    member this.ApplyAttrs (attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Size (w, h) ->
                dialog.Resize(w, h)
            | Title title ->
                dialog.SetWindowTitle title
            | Modality modality ->
                let qModality =
                    match modality with
                    | WindowModal -> Widget.WindowModality.WindowModal
                    | AppModal -> Widget.WindowModality.ApplicationModal
                dialog.SetWindowModality(qModality)
    
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

let private create (attrs: Attr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (maybeLayout: Layout.Handle option) =
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

type Dialog<'msg>() =
    let mutable maybeLayout: ILayoutNode<'msg> option = None
    let mutable onClosed: (bool -> 'msg) option = None
    member private this.MaybeLayout = maybeLayout
    member this.Layout with set value = maybeLayout <- Some value
    member this.OnClosed with set value = onClosed <- Some value
    
    [<DefaultValue>] val mutable private model: Model<'msg>
    member val Attrs: Attr list = [] with get, set
    member private this.SignalMap = function
        | Closed accepted ->
            onClosed
            |> Option.map (fun f -> f accepted)
            
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
        override this.Dependencies =
            // maybeParent not a dependency, would create circular problems (and is intended only for internal modal use)
            maybeLayout
            |> Option.map (fun content -> (StrKey "layout", content :> IBuilderNode<'msg>))
            |> Option.toList
            
        override this.Create(dispatch: 'msg -> unit) =
            let maybeLayoutHandle =
                maybeLayout
                |> Option.map (_.Layout)
            this.model <- create this.Attrs this.SignalMap dispatch maybeLayoutHandle
            
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> Dialog<'msg>)
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
            (this :> IDialogNode<'msg>).Dialog
            
        override this.AttachedToWindow window =
            this.model.Dialog.SetParentDialogFlags(window)
