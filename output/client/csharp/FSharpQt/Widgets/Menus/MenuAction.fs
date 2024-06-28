module FSharpQt.Widgets.Menus.MenuAction

open System
open FSharpQt.BuilderNode
open Org.Whatever.QtTesting
open FSharpQt.MiscTypes

open FSharpQt.Attrs

type Signal =
    | Changed
    | CheckableChanged of checkable: bool
    | EnabledChanged of enabled: bool
    | Hovered
    | Toggled of checked_: bool
    | Triggered of checked_: bool
    | VisibleChanged
    
type private Model<'msg>(dispatch: 'msg -> unit, maybeContainingWindow: Widget.Handle option) as this =
    let owner =
        match maybeContainingWindow with
        | Some handle ->
            // printfn "MenuAction being created with owner [%A]" handle
            handle
        | None ->
            printfn "MenuAction created with no owner"
            null
    let mutable action = Action.Create(owner, this)
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable currentMask = enum<Action.SignalMask> 0
    
    // 2-way binding guards for anything with a signal
    // see the note in LineEdit's ApplyAttrs
    let mutable enabled = true
    let mutable checkable = false
    let mutable checked_ = false
    
    let signalDispatch (s: Signal) =
        match signalMap s with
        | Some msg ->
            dispatch msg
        | None ->
            ()
        
    member this.Action with get() = action
    member this.SignalMap with set value = signalMap <- value
    member this.SignalMask with set value =
        if value <> currentMask then
            action.SetSignalMask(value)
            currentMask <- value
            
    member this.ApplyAttrs(attrs: IAttr list) =
        for attr in attrs do
            attr.ApplyTo(this)
            
    // I guess we would have to implement any interface up the whole hierarchy? hmmm
    // but if we could inherit Models (as we'd like for signals), it would ease some of that trouble
    interface ActionAttrTarget with
        override this.Action = action
        override this.SetEnabled state =
            if state <> enabled then
                enabled <- state
                true
            else
                false
        override this.SetCheckable state =
            if checkable <> state then
                checkable <- state
                true
            else
                false
        override this.SetChecked state =
            if checked_ <> state then
                checked_ <- state
                true
            else
                false
    
    interface Action.SignalHandler with
        override this.Changed () =
            signalDispatch Changed
        override this.CheckableChanged newState =
            checkable <- newState
            signalDispatch (CheckableChanged newState)
        override this.EnabledChanged newState =
            enabled <- newState
            signalDispatch (EnabledChanged newState)
        override this.Hovered () =
            signalDispatch Hovered
        override this.Toggled newState =
            checked_ <- newState
            signalDispatch (Toggled newState)
        override this.Triggered checked_ =
            signalDispatch (Triggered checked_)
        override this.VisibleChanged () =
            signalDispatch VisibleChanged
                
    interface IDisposable with
        member this.Dispose() =
            action.Dispose()

let private create (attrs: IAttr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (signalMask: Action.SignalMask) (maybeContainingWindow: Widget.Handle option) =
    let model = new Model<'msg>(dispatch, maybeContainingWindow)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private migrate (model: Model<'msg>) (attrs: IAttr list) (signalMap: Signal -> 'msg option) (signalMask: Action.SignalMask) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()
    
type BuildState =
    | Init
    | Created
    | DepsAttached
    | Migrated
    | Disposed
            
type MenuAction<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>
    let mutable buildState = Init
    
    member val Attrs: IAttr list = [] with get, set
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    let mutable signalMask = enum<Action.SignalMask> 0
    
    let mutable onChanged: 'msg option = None
    let mutable onCheckableChanged: (bool -> 'msg) option = None
    let mutable onEnabledChanged: (bool -> 'msg) option = None
    let mutable onHovered: 'msg option = None
    let mutable onToggled: (bool -> 'msg) option = None
    let mutable onTriggered: (bool -> 'msg) option = None
    let mutable onVisibleChanged: 'msg option = None
    
    member this.OnChanged with set value =
        onChanged <- Some value
        signalMask <- signalMask ||| Action.SignalMask.Changed
        
    member this.OnCheckableChanged with set value =
        onCheckableChanged <- Some value
        signalMask <- signalMask ||| Action.SignalMask.CheckableChanged
        
    member this.OnEnabledChanged with set value =
        onEnabledChanged <- Some value
        signalMask <- signalMask ||| Action.SignalMask.EnabledChanged
        
    member this.OnHovered with set value =
        onHovered <- Some value
        signalMask <- signalMask ||| Action.SignalMask.Hovered
        
    member this.OnToggled with set value =
        onToggled <- Some value
        signalMask <- signalMask ||| Action.SignalMask.Toggled
        
    member this.OnTriggered with set value =
        onTriggered <- Some value
        signalMask <- signalMask ||| Action.SignalMask.Triggered
        
    member this.OnVisibleChanged with set value =
        onVisibleChanged <- Some value
        signalMask <- signalMask ||| Action.SignalMask.VisibleChanged
    
    let signalMap = function
        | Changed ->
            onChanged
        | CheckableChanged value ->
            onCheckableChanged
            |> Option.map (fun f -> f value)
        | EnabledChanged value ->
            onEnabledChanged
            |> Option.map (fun f -> f value)
        | Hovered ->
            onHovered
        | Toggled value ->
            onToggled
            |> Option.map (fun f -> f value)
        | Triggered value ->
            onTriggered
            |> Option.map (fun f -> f value)
        | VisibleChanged ->
            onVisibleChanged
                
    interface IActionNode<'msg> with
        override this.Dependencies = []
        
        override this.Create dispatch buildContext =
            // since Actions can be dependencies of multiple nodes (eg MainWindow, menus, context menus, toolbars, etc),
            // we need to protect against rebuilding the model each time
            match buildState with
            | Init ->
                this.model <- create this.Attrs signalMap dispatch signalMask buildContext.ContainingWindow
                buildState <- Created
            | _ ->
                // ignore
                ()
            
        override this.AttachDeps () =
            // see .Create node
            match buildState with
            | Created ->
                // not that we're using this, but for any future use ...
                buildState <- DepsAttached
            | _ ->
                // ignore
                ()
            
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            // see .Create node
            match buildState with
            | Init ->
                // note migration always occurs straight from Init
                let left' = (left :?> MenuAction<'msg>)
                let nextAttrs =
                    diffAttrs left'.Attrs this.Attrs
                    |> createdOrChanged
                this.model <- migrate left'.model nextAttrs signalMap signalMask
                buildState <- Migrated
            | _ ->
                // ignore
                ()
                
        override this.Dispose() =
            match buildState with
            | Disposed ->
                // already disposed, do nothing
                ()
            | _ ->
                (this.model :> IDisposable).Dispose()
                buildState <- Disposed
            
        override this.Action =
            this.model.Action
            
        override this.ContentKey =
            (this :> IActionNode<'msg>).Action
            
        override this.Attachments =
            this.Attachments

[<RequireQualifiedAccess>]
type internal AttrValue =
    | Text of text: string
    | Enabled of state: bool    // why does QAction redeclare this? ohhh it's a QObject, not a widget
    | Separator of state: bool
    | Checkable of state: bool
    | Checked of state: bool
    | IconAttr of icon: Icon    // + "Attr" to prevent annoying collisions
    | IconText of text: string
    | Shortcut of seq: KeySequence
    | StatusTip of tip: string
    | ToolTip of tip: string
            
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
            | AttrValue.Text _ -> "action:text"
            | AttrValue.Enabled _ -> "action:enabled"
            | AttrValue.Separator _ -> "action:separator"
            | AttrValue.Checkable _ -> "action:checkable"
            | AttrValue.Checked _ -> "action:checked"
            | AttrValue.IconAttr _ -> "action:iconattr"
            | AttrValue.IconText _ -> "action:icontext"
            | AttrValue.Shortcut _ -> "action:shortcut"
            | AttrValue.StatusTip _ -> "action:statustip"
            | AttrValue.ToolTip _ -> "action:tooltip"
        override this.ApplyTo (target: IAttrTarget) =
            match target with
            | :? ActionAttrTarget as actionTarget ->
                let action =
                    actionTarget.Action
                match value with
                | AttrValue.Text text ->
                    action.SetText(text)
                | AttrValue.Enabled state ->
                    if actionTarget.SetEnabled(state) then
                        action.SetEnabled(state)
                | AttrValue.Separator state ->
                    action.SetSeparator(state)
                | AttrValue.Checkable state ->
                    if actionTarget.SetCheckable(state) then
                        action.SetCheckable(state)
                | AttrValue.Checked state ->
                    if actionTarget.SetChecked(state) then
                        action.SetChecked(state)
                | AttrValue.IconAttr icon ->
                    action.SetIcon(icon.QtValue)
                | AttrValue.IconText text ->
                    action.SetIconText(text)
                | AttrValue.Shortcut seq ->
                    action.SetShortcut(seq.QtValue)
                | AttrValue.StatusTip tip ->
                    action.SetStatusTip(tip)
                | AttrValue.ToolTip tip ->
                    action.SetToolTip(tip)
            | _ ->
                printfn "warning: MenuAction.Attr couldn't ApplyTo() unknown object type [%A]" target

type Text(text: string) =
    inherit Attr(AttrValue.Text(text))
    
type Separator(state: bool) =
    inherit Attr(AttrValue.Separator(state))

type Checkable(state: bool) =
    inherit Attr(AttrValue.Checkable(state))
    
type Checked(state: bool) =
    inherit Attr(AttrValue.Checked(state))

type IconAttr(icon: Icon) =
    inherit Attr(AttrValue.IconAttr(icon))

type IconText(text: string) =
    inherit Attr(AttrValue.IconText(text))

type Shortcut(seq: KeySequence) =
    inherit Attr(AttrValue.Shortcut(seq))

type StatusTip(tip: string) =
    inherit Attr(AttrValue.StatusTip(tip))

type ToolTip(tip: string) =
    inherit Attr(AttrValue.ToolTip(tip))
