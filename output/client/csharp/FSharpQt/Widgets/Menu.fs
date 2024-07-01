module FSharpQt.Widgets.Menu

open System
open FSharpQt
open FSharpQt.BuilderNode
open Microsoft.FSharp.Core
open Org.Whatever.QtTesting
open Extensions

open FSharpQt.Attrs
open FSharpQt.MiscTypes

type Signal =
    | AboutToHide
    | AboutToShow
    | Hovered of action: ActionProxy
    | Triggered of action: ActionProxy
    
type Attr =
    | IconAttr of icon: Icon
    | SeparatorsCollapsible of state: bool
    | TearOffEnabled of state: bool
    | Title of title: string
    | ToolTipsVisible of visible: bool
with
    interface IAttr with
        override this.AttrEquals other =
            match other with
            | :? Attr as otherAttr ->
                this = otherAttr
            | _ ->
                false
        override this.Key =
            match this with
            | IconAttr _ -> "menu:icon"
            | SeparatorsCollapsible _ -> "menu:separatorscollapsible"
            | TearOffEnabled _ -> "menu:tearoffenabled"
            | Title _ -> "menu:title"
            | ToolTipsVisible _ -> "menu:tooltipsvisible"
        override this.ApplyTo (target: IAttrTarget, maybePrev: IAttr option) =
            match target with
            | :? MenuAttrTarget as attrTarget ->
                let menu =
                    attrTarget.Menu
                match this with
                | IconAttr icon ->
                    menu.SetIcon(icon.QtValue)
                | SeparatorsCollapsible state ->
                    menu.SetSeparatorsCollapsible(state)
                | TearOffEnabled state ->
                    menu.SetTearOffEnabled(state)
                | Title title ->
                    menu.SetTitle(title)
                | ToolTipsVisible visible ->
                    menu.SetToolTipsVisible(visible)
            | _ ->
                printfn "warning: Menu.Attr couldn't ApplyTo() unknown object type [%A]" target
                
type Props<'msg>() =
    inherit Widget.Props<'msg>()
    
    let mutable onAboutToHide: 'msg option = None
    let mutable onAboutToShow: 'msg option = None
    let mutable onHovered: (ActionProxy -> 'msg) option = None
    let mutable onTriggered: (ActionProxy -> 'msg) option = None
    
    member internal this.SignalMask = enum<Menu.SignalMask> (int this._signalMask)
    
    member this.OnAboutToHide with set value =
        onAboutToHide <- Some value
        this.AddSignal(int Menu.SignalMask.AboutToHide)
    member this.OnAboutToShow with set value =
        onAboutToShow <- Some value
        this.AddSignal(int Menu.SignalMask.AboutToShow)
    member this.OnHovered with set value =
        onHovered <- Some value
        this.AddSignal(int Menu.SignalMask.Hovered)
    member this.OnTriggered with set value =
        onTriggered <- Some value
        this.AddSignal(int Menu.SignalMask.Triggered)
    
    member internal this.SignalMap = function
        | AboutToHide ->
            onAboutToHide
        | AboutToShow ->
            onAboutToShow
        | Hovered action ->
            onHovered
            |> Option.map (fun f -> f action)
        | Triggered action ->
            onTriggered
            |> Option.map (fun f -> f action)
    
    member this.Icon with set value =
        this.PushAttr(IconAttr value)
        
    member this.SeparatorsCollapsible with set value =
        this.PushAttr(SeparatorsCollapsible value)
        
    member this.TearOffEnabled with set value =
        this.PushAttr(TearOffEnabled value)
        
    member this.Title with set value =
        this.PushAttr(Title value)
        
    member this.ToolTipsVisible with set value =
        this.PushAttr(ToolTipsVisible value)
    
type internal ItemInternal<'msg> =
    | ActionItem of action: IActionNode<'msg>
    | Separator
    | Nothing
    
type MenuItem<'msg> internal(item: ItemInternal<'msg>) =
    new(action: IActionNode<'msg>) =
        MenuItem(ActionItem action)
    new(?separator: bool) =
        match defaultArg separator true with
        | true -> MenuItem(Separator)
        | false -> MenuItem(Nothing)
    // internal stufF:
    member internal this.ContentKey =
        match item with
        | ActionItem action -> action.ContentKey
        | Separator -> Separator
        | Nothing -> Nothing
    member internal this.MaybeNode =
        match item with
        | ActionItem action -> Some action
        | Separator -> None
        | Nothing -> None
    member internal this.AddTo (menu: Menu.Handle) =
        match item with
        | ActionItem action ->
            menu.AddAction(action.Action)
        | Separator ->
            menu.AddSeparator()
            |> ignore
        | Nothing ->
            ()
        
type Separator<'msg>() =
    inherit MenuItem<'msg>(ItemInternal.Separator)
    
type private Model<'msg>(dispatch: 'msg -> unit) as this =
    let mutable menu = Menu.Create(this)
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable currentMask = enum<Menu.SignalMask> 0
    
    let signalDispatch (s: Signal) =
        match signalMap s with
        | Some msg ->
            dispatch msg
        | None ->
            ()
            
    member this.Menu = menu
    member this.SignalMap with set value = signalMap <- value

    member this.SignalMask with set value =
        if value <> currentMask then
            menu.SetSignalMask(value)
            currentMask <- value
    
    member this.ApplyAttrs(attrs: (IAttr option * IAttr) list) =
        for maybePrev, attr in attrs do
            attr.ApplyTo(this, maybePrev)
                
    member this.Refill(items: MenuItem<'msg> list) =
        menu.Clear()
        for item in items do
            item.AddTo(menu)
            
    interface MenuAttrTarget with
        member this.Widget = menu
        member this.Menu = menu
        
    interface Menu.SignalHandler with
        override this.AboutToHide() =
            signalDispatch AboutToHide
        override this.AboutToShow() =
            signalDispatch AboutToShow
        override this.Hovered action =
            signalDispatch (ActionProxy action |> Hovered)
        override this.Triggered action =
            signalDispatch (ActionProxy action |> Triggered)
            
    interface IDisposable with
        member this.Dispose() =
            menu.Dispose()

let private create (attrs: IAttr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (signalMask: Menu.SignalMask) =
    let model = new Model<'msg>(dispatch)
    model.ApplyAttrs (attrs |> List.map (fun attr -> None, attr))
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private migrate (model: Model<'msg>) (attrs: (IAttr option * IAttr) list) (signalMap: Signal -> 'msg option) (signalMask: Menu.SignalMask) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()

type Menu<'msg>() =
    inherit Props<'msg>()
    [<DefaultValue>] val mutable private model: Model<'msg>

    member val Items: MenuItem<'msg> list = [] with get, set
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    member private this.MigrateContent (leftMenu: Menu<'msg>) =
        let leftItems =
            leftMenu.Items
            |> List.map (_.ContentKey)
        let thisItems =
            this.Items
            |> List.map (_.ContentKey)
        if leftItems <> thisItems then
            this.model.Refill(this.Items)
        else
            ()
        
    interface IMenuNode<'msg> with
        override this.Dependencies =
            // see long note on same BoxLayout method
            this.Items
            |> List.zipWithIndex
            |> List.choose (fun (i, item) ->
                item.MaybeNode
                |> Option.map (fun node -> IntKey i, node :> IBuilderNode<'msg>))
            
        override this.Create dispatch buildContext =
            this.model <- create this.Attrs this.SignalMap dispatch this.SignalMask
            
        override this.AttachDeps () =
            this.model.Refill(this.Items)
            
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let leftNode = (left :?> Menu<'msg>)
            let nextAttrs =
                diffAttrs leftNode.Attrs this.Attrs
                |> createdOrChanged
            this.model <- migrate leftNode.model nextAttrs this.SignalMap this.SignalMask
            this.MigrateContent(leftNode)
            
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()
            
        override this.Menu =
            this.model.Menu
            
        override this.ContentKey =
            (this :> IMenuNode<'msg>).Menu
            
        override this.Popup point =
            this.model.Menu.Popup point
            
        override this.Attachments =
            this.Attachments
