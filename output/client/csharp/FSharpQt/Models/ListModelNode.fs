module FSharpQt.Models.ListModelNode

open System
open FSharpQt.BuilderNode
open FSharpQt.Models.SimpleListModel
open FSharpQt.Models.TrackedRows
open FSharpQt.MiscTypes

type Signal = unit
    
type Attr<'row> =
    | Rows of rows: TrackedRows<'row>
    
let private keyFunc = function
    | Rows _ -> 0

// redeclared locally because of the dreaded "x would escape its scope" type errors
let diffAttrs (a1: Attr<'row> list) (a2: Attr<'row> list)  =
    let leftList = a1 |> List.map (fun a -> keyFunc a, a)
    let rightList = a2 |> List.map (fun a -> keyFunc a, a)
    let leftMap = leftList |> Map.ofList
    let rightMap = rightList |> Map.ofList

    let allKeys =
        (leftList @ rightList)
        |> List.map fst
        |> List.distinct
        |> List.sort

    allKeys
    |> List.choose (fun key ->
        let leftVal, rightVal = (Map.tryFind key leftMap, Map.tryFind key rightMap)
        match leftVal, rightVal with
        | Some left, Some right ->
            if left = right then None else Changed (left, right) |> Some
        | Some left, None ->
            Deleted left |> Some
        | None, Some right ->
            Created right |> Some
        | _ -> failwith "shouldn't happen")
    
// same as above
let createdOrChanged (changes: AttrChange<Attr<'row>> list) =
    changes
    |> List.choose (function | Created attr | Changed (_, attr) -> Some attr | _ -> None)


type Model<'msg,'row>(dispatch: 'msg -> unit, dataFunc: 'row -> DataRole -> Variant) =
    let listModel = new SimpleListModel<'row>([], dataFunc)
    
    member this.QtModel =
        listModel.QtModel
            
    member this.ApplyAttrs(attrs: Attr<'row> list) =
        for attr in attrs do
            match attr with
            | Rows rows ->
                for change in rows.Changes do
                    match change with
                    | Added(index, row) ->
                        listModel.AddRowAt(index, row)
                    | RangeAdded(index, rows) ->
                        listModel.AddRowsAt(index, rows)
    
    interface IDisposable with
        member this.Dispose() =
            (listModel :> IDisposable).Dispose()
            
let private create (attrs: Attr<'row> list) (dispatch: 'msg -> unit) (dataFunc: 'row -> DataRole -> Variant) =
    let model = new Model<'msg, 'row>(dispatch, dataFunc)
    model.ApplyAttrs attrs
    model

let private migrate (model: Model<'msg,'row>) (attrs: Attr<'row> list) =
    model.ApplyAttrs attrs
    model

let private dispose (model: Model<'msg,'row>) =
    (model :> IDisposable).Dispose()


type ListModelNode<'msg,'row>(dataFunc: 'row -> DataRole -> Variant) =
    [<DefaultValue>] val mutable model: Model<'msg,'row>

    member val Attrs: Attr<'row> list = [] with get, set
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    interface IModelNode<'msg> with
        override this.Dependencies = []

        override this.Create dispatch buildContext =
            this.model <- create this.Attrs dispatch dataFunc
            
        override this.AttachDeps () =
            ()

        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> ListModelNode<'msg,'row>)
            let nextAttrs = diffAttrs left'.Attrs this.Attrs |> createdOrChanged
            this.model <- migrate left'.model nextAttrs

        override this.Dispose() =
            (this.model :> IDisposable).Dispose()
            
        override this.QtModel =
            this.model.QtModel
            
        override this.ContentKey =
            this.model.QtModel
            
        override this.Attachments =
            this.Attachments
