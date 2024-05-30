module Widgets.ComboBox

open System
open BuilderNode
open Org.Whatever.QtTesting

type Signal =
    | Selected of index: int option
    
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
    let mutable combo = ComboBox.Create()
    let mutable selectedIndex: int option = None
    do
        let signalDispatch (s: Signal) =
            match signalMap s with
            | Some msg ->
                dispatch msg
            | None ->
                ()
        combo.OnCurrentIndexChanged (fun i ->
            let value =
                if i >= 0 then
                    Some i
                else
                    None
            selectedIndex <- value
            signalDispatch (Selected value))
    member this.Widget with get() = combo
    member this.SignalMap with set(value) = signalMap <- value
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Items items ->
                combo.Clear()
                combo.SetItems(items |> Array.ofList)
            | SelectedIndex maybeIndex ->
                // short-circuit identical values, see LineEdit comments for explanation why
                if maybeIndex <> selectedIndex then
                    selectedIndex <- maybeIndex
                    match maybeIndex with
                    | Some value ->
                        combo.SetCurrentIndex(value)
                    | None ->
                        combo.SetCurrentIndex(-1)
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
    [<DefaultValue>] val mutable private model: Model<'msg>
    member val Attrs: Attr list = [] with get, set
    let mutable onSelected: (int option -> 'msg) option = None
    member this.OnSelected
        with set value = onSelected <- Some value
    member private this.SignalMap
        with get() = function
            | Selected maybeArgs ->
                onSelected
                |> Option.map (fun f -> f maybeArgs)
                
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
            
