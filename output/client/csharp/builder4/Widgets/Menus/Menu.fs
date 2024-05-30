module Widgets.Menus.Menu

open System
open BuilderNode
open Org.Whatever.QtTesting

type Signal =
    | AboutToShow

type Attr =
    | Title of title: string
let private attrKey = function
    | Title _ -> 0
    
let private diffAttrs =
    genericDiffAttrs attrKey
    
type private Model<'msg>(dispatch: 'msg -> unit, items: Action.Handle list) =
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable menu = Menu.Create()
    do
        for item in items do
            menu.AddAction(item)
            
        let signalDispatch (s: Signal) =
            match signalMap s with
            | Some msg ->
                dispatch msg
            | None ->
                ()
        menu.OnAboutToShow (fun _ ->
            signalDispatch AboutToShow)
    member this.Menu with get() = menu
    member this.SignalMap with set value = signalMap <- value
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Title title ->
                menu.SetTitle(title)
    interface IDisposable with
        member this.Dispose() =
            menu.Dispose()
    member this.Refill(items: Action.Handle list) =
        menu.Clear()
        for item in items do
            menu.AddAction(item)
        

let private create (attrs: Attr list) (items: Action.Handle list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) =
    let model = new Model<'msg>(dispatch, items)
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
    let mutable items: IActionNode<'msg> list = []

    [<DefaultValue>] val mutable private model: Model<'msg>
    member val Attrs: Attr list = [] with get, set
    let mutable onAboutToShow: 'msg option = None
    member this.OnAboutToShow
        with set value =
            onAboutToShow <- Some value
    member private this.SignalMap
        with get() = function
            | AboutToShow ->
                onAboutToShow
    member this.Items
        with get() = items
        and set value = items <- value
        
    member private this.MigrateContent (leftMenu: Node<'msg>) =
        let leftItems =
            leftMenu.Items
            |> List.map (_.ContentKey)
        let thisItems =
            items
            |> List.map (_.ContentKey)
        if leftItems <> thisItems then
            let actions =
                items
                |> List.map (_.Action)
            this.model.Refill(actions)
        else
            ()
        
    interface IMenuNode<'msg> with
        override this.Dependencies() =
            // see long note on same BoxLayout method
            items
            |> List.mapi (fun i item -> (IntKey i, item :> IBuilderNode<'msg>))
        override this.Create(dispatch: 'msg -> unit) =
            let actionHandles =
                items
                |> List.map (_.Action)
            this.model <- create this.Attrs actionHandles this.SignalMap dispatch
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let leftNode = (left :?> Node<'msg>)
            let nextAttrs =
                diffAttrs leftNode.Attrs this.Attrs
                |> createdOrChanged
            this.model <- migrate leftNode.model nextAttrs this.SignalMap
            this.MigrateContent(leftNode)
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()
        override this.Menu =
            this.model.Menu
        override this.ContentKey =
            (this :> IMenuNode<'msg>).Menu
        override this.AttachedToWindow window =
            ()
            

let make (attrs: Attr list) (items: IActionNode<'msg> list) =
    Node(Attrs = attrs, Items = items)
