module Widgets.ListWidget

open System
open BuilderNode
open Org.Whatever.QtTesting

type Signal =
    | CurrentRowChanged of index: int option
    | ItemSelectionChanged of indices: int list

type SelectionMode =
    | NotAllowed
    | Single
    | Extended
    
type Attr =
    | Items of items: string list
    | SelectionMode of mode: SelectionMode
    | CurrentRow of maybeIndex: int option
    
let private attrKey = function
    | Items _ -> 0
    | SelectionMode _ -> 1
    | CurrentRow _ -> 2

let private diffAttrs =
    genericDiffAttrs attrKey

type private Model<'msg>(dispatch: 'msg -> unit) =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable listWidget = ListWidget.Create()
    do
        let signalDispatch (s: Signal) =
            match signalMap s with
            | Some msg ->
                dispatch msg
            | None ->
                ()
        listWidget.OnCurrentRowChanged (fun index ->
            let value =
                if index >= 0 then
                    Some index
                else
                    None
            signalDispatch (CurrentRowChanged value))
        listWidget.OnItemSelectionChanged (fun _ ->
            let indices =
                listWidget.SelectedIndices()
                |> Array.toList
            signalDispatch (ItemSelectionChanged indices))
    member this.Widget with get() = listWidget
    member this.SignalMap with set(value) = signalMap <- value
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Items items ->
                listWidget.SetItems (items |> Array.ofList)
            | SelectionMode mode ->
                match mode with
                | NotAllowed -> ListWidget.SelectionMode.None
                | Single -> ListWidget.SelectionMode.Single
                | Extended -> ListWidget.SelectionMode.Extended
                |> listWidget.SetSelectionMode
            | CurrentRow maybeIndex ->
                listWidget.SetCurrentRow(maybeIndex |> Option.defaultValue -1)
                
    interface IDisposable with
        member this.Dispose() =
            listWidget.Dispose()

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
    [<DefaultValue>] val mutable private model: Model<'msg>
    member val Attrs: Attr list = [] with get, set
    let mutable onCurrentRowChanged: (int option -> 'msg) option = None
    let mutable onItemSelectionChanged: (int list -> 'msg) option = None
    member this.OnCurrentRowChanged with set value = onCurrentRowChanged <- Some value
    member this.OnItemSelectionChanged with set value = onItemSelectionChanged <- Some value
    member private this.SignalMap
        with get() = function
            | CurrentRowChanged maybeIndex ->
                onCurrentRowChanged
                |> Option.map (fun f -> f maybeIndex)
            | ItemSelectionChanged indices ->
                onItemSelectionChanged
                |> Option.map (fun f -> f indices)
                
    interface IWidgetNode<'msg> with
        override this.Dependencies = []
        override this.Create(dispatch: 'msg -> unit) =
            this.model <- create this.Attrs this.SignalMap dispatch
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> Node<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged
            this.model <-
                migrate left'.model nextAttrs this.SignalMap
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()
        override this.Widget =
            (this.model.Widget :> Widget.Handle)
        override this.ContentKey =
            (this :> IWidgetNode<'msg>).Widget
        override this.AttachedToWindow window =
            ()
