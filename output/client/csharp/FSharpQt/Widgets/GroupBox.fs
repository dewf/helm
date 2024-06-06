module FSharpQt.Widgets.GroupBox

open FSharpQt.BuilderNode
open System
open Org.Whatever.QtTesting

type Signal = unit

type Attr =
    | Title of title: string
    
let keyFunc = function
    | Title _ -> 0
    
let private diffAttrs =
    genericDiffAttrs keyFunc

type private Model<'msg>(dispatch: 'msg -> unit, maybeLayout: Layout.Handle option) =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable groupBox = GroupBox.Create()
    do
        let signalDispatch (s: Signal) =
            match signalMap s with
            | Some msg ->
                dispatch msg
            | None ->
                ()
        // no signals yet
        
        maybeLayout
        |> Option.iter groupBox.SetLayout
        
    member this.Widget with get() = groupBox
    member this.SignalMap with set value = signalMap <- value
    member this.ApplyAttrs (attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Title title ->
                groupBox.SetTitle(title)
                
    interface IDisposable with
        member this.Dispose() =
            groupBox.Dispose()
            
    member this.RemoveLayout() =
        let existing =
            groupBox.GetLayout()
        existing.RemoveAll()
        groupBox.SetLayout(null)
        
    member this.AddLayout(layout: Layout.Handle) =
        groupBox.SetLayout(layout)
            

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

type GroupBox<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    let mutable maybeLayout: ILayoutNode<'msg> option = None
    member this.Layout with set value = maybeLayout <- Some value
    
    member val Attrs: Attr list = [] with get, set
    member private this.SignalMap = (fun _ -> None)
    
    member private this.MigrateContent (changeMap: Map<DepsKey, DepsChange>) =
        match changeMap.TryFind (StrKey "layout") with
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
            // neither side had a layout
            ()
            
    interface IWidgetNode<'msg> with
        override this.Dependencies =
            maybeLayout
            |> Option.map (fun content -> (StrKey "layout", content :> IBuilderNode<'msg>))
            |> Option.toList
        
        override this.Create(dispatch: 'msg -> unit) =
            let maybeLayoutHandle =
                maybeLayout
                |> Option.map (_.Layout)
            this.model <- create this.Attrs this.SignalMap dispatch maybeLayoutHandle

        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> GroupBox<'msg>)
            let nextAttrs = diffAttrs left'.Attrs this.Attrs |> createdOrChanged
            this.model <- migrate left'.model nextAttrs this.SignalMap
            this.MigrateContent (depsChanges |> Map.ofList)
            
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()

        override this.Widget =
            (this.model.Widget :> Widget.Handle)
            
        override this.ContentKey =
            (this :> IWidgetNode<'msg>).Widget
            
        override this.AttachedToWindow window =
            ()
          
