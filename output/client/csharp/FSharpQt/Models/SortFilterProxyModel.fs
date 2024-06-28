module FSharpQt.Models.SortFilterProxyModel

open FSharpQt.BuilderNode
open System
open Org.Whatever.QtTesting
open FSharpQt.MiscTypes
open FSharpQt.ModelBindings

open FSharpQt.Attrs

type Signal =
    | AutoAcceptChildRowsChanged of autoAcceptChildRows: bool

type private Model<'msg>(dispatch: 'msg -> unit) as this =
    let mutable proxyModel = SortFilterProxyModel.Create(this)
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable currentMask = enum<SortFilterProxyModel.SignalMask> 0
    
    let signalDispatch (s: Signal) =
        signalMap s
        |> Option.iter dispatch
        
    member this.ProxyModel = proxyModel
    member this.SignalMap with set value = signalMap <- value
    
    member this.SignalMask with set value =
        if value <> currentMask then
            proxyModel.SetSignalMask(value)
            currentMask <- value
            
    member this.AddSourceModel (model: AbstractItemModel.Handle) =
        proxyModel.SetSourceModel(model)
        
    member this.RemoveSourceModel () =
        // well if it gets deleted (as a dependency), won't that delete from the listView automatically?
        ()
    
    member this.ApplyAttrs(attrs: IAttr list) =
        for attr in attrs do
            attr.ApplyTo(this)
            
    interface SortFilterProxyModelAttrTarget with
        member this.ProxyModel = proxyModel
                
    interface SortFilterProxyModel.SignalHandler with
        member this.AutoAcceptChildRowsChanged value =
            signalDispatch (AutoAcceptChildRowsChanged value)
            
    interface IDisposable with
        member this.Dispose() =
            proxyModel.Dispose()


let private create (attrs: IAttr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (signalMask: SortFilterProxyModel.SignalMask) =
    let model = new Model<'msg>(dispatch)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private migrate (model: Model<'msg>) (attrs: IAttr list) (signalMap: Signal -> 'msg option) (signalMask: SortFilterProxyModel.SignalMask) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()


type SortFilterProxyModel<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    member val Attrs: IAttr list = [] with get, set
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    let mutable maybeModelBinding: AbstractProxyModelBinding option = None
    member this.ModelBinding with set value = maybeModelBinding <- Some value
    
    let mutable maybeSourceModel: IModelNode<'msg> option = None
    member this.SourceModel with set value = maybeSourceModel <- Some value
    
    let mutable signalMask = enum<SortFilterProxyModel.SignalMask> 0
    
    let mutable onAutoAcceptChildRowsChanged: (bool -> 'msg) option = None
    
    member this.OnAutoAcceptChildRowsChanged with set value =
        onAutoAcceptChildRowsChanged <- Some value
        signalMask <- signalMask ||| SortFilterProxyModel.SignalMask.AutoAcceptChildRowsChanged
        
    let signalMap = function
        | AutoAcceptChildRowsChanged value ->
            onAutoAcceptChildRowsChanged
            |> Option.map (fun f -> f value)
            
    member this.MigrateDeps (changeMap: Map<DepsKey, DepsChange>) =
        match changeMap.TryFind (StrKey "source") with
        | Some change ->
            match change with
            | Unchanged ->
                ()
            | Added ->
                this.model.AddSourceModel(maybeSourceModel.Value.QtModel)
            | Removed ->
                this.model.RemoveSourceModel()
            | Swapped ->
                this.model.RemoveSourceModel()
                this.model.AddSourceModel(maybeSourceModel.Value.QtModel)
        | None ->
            // neither side had one
            ()
    
    interface IModelNode<'msg> with
        override this.Dependencies =
            maybeSourceModel
            |> Option.map (fun node -> StrKey "source", node :> IBuilderNode<'msg>)
            |> Option.toList

        override this.Create dispatch buildContext =
            this.model <- create this.Attrs signalMap dispatch signalMask
            // assign the method proxy if one is requested
            maybeModelBinding
            |> Option.iter (fun mp -> mp.Handle <- this.model.ProxyModel)
            
        override this.AttachDeps () =
            maybeSourceModel
            |> Option.iter (fun node ->
                this.model.AddSourceModel(node.QtModel))

        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> SortFilterProxyModel<'msg>)
            let nextAttrs = diffAttrs left'.Attrs this.Attrs |> createdOrChanged
            this.model <- migrate left'.model nextAttrs signalMap signalMask
            this.MigrateDeps(depsChanges |> Map.ofList)

        override this.Dispose() =
            (this.model :> IDisposable).Dispose()

        override this.QtModel =
            this.model.ProxyModel
            
        override this.ContentKey =
            this.model.ProxyModel
            
        override this.Attachments =
            this.Attachments
            
[<RequireQualifiedAccess>]
type internal AttrValue =
    | FilterRegex of regex: Regex
    | FilterKeyColumn of column: int option

[<AbstractClass>]
type Attr internal(value: AttrValue) =
    member val private Value = value
    interface IAttr with
        override this.AttrEquals other =
            match other with
            | :? Attr as attr ->
                this.Value = attr.Value
            | _ ->
                false
        override this.Key =
            match value with
            | AttrValue.FilterRegex _ -> "sortfilterproxymodel:filterregex"
            | AttrValue.FilterKeyColumn _ -> "sortfilterproxymodel:filterkeycolumn"
        override this.ApplyTo (target: IAttrTarget) =
            match target with
            | :? SortFilterProxyModelAttrTarget as proxyModeltarget ->
                let proxyModel =
                    proxyModeltarget.ProxyModel
                match value with
                | AttrValue.FilterRegex regex ->
                    proxyModel.SetFilterRegularExpression(regex.QtValue)
                | AttrValue.FilterKeyColumn column ->
                    proxyModel.SetFilterKeyColumn(column |> Option.defaultValue -1)
            | _ ->
                printfn "warning: SortFilterProxyModel.Attr couldn't ApplyTo() unknown object type [%A]" target

type FilterRegex(regex: Regex) =
    inherit Attr(AttrValue.FilterRegex(regex))

type FilterKeyColumn(column: int option) =
    inherit Attr(AttrValue.FilterKeyColumn(column))
