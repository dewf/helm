module FSharpQt.Widgets.MenuAction

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

type Attr =
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
            | Text _ -> "action:text"
            | Enabled _ -> "action:enabled"
            | Separator _ -> "action:separator"
            | Checkable _ -> "action:checkable"
            | Checked _ -> "action:checked"
            | IconAttr _ -> "action:iconattr"
            | IconText _ -> "action:icontext"
            | Shortcut _ -> "action:shortcut"
            | StatusTip _ -> "action:statustip"
            | ToolTip _ -> "action:tooltip"
        override this.ApplyTo (target: IAttrTarget, maybePrev: IAttr option) =
            match target with
            | :? ActionAttrTarget as attrTarget ->
                let action =
                    attrTarget.Action
                match this with
                | Text text ->
                    action.SetText(text)
                | Enabled state ->
                    if attrTarget.SetEnabled(state) then
                        action.SetEnabled(state)
                | Separator state ->
                    action.SetSeparator(state)
                | Checkable state ->
                    if attrTarget.SetCheckable(state) then
                        action.SetCheckable(state)
                | Checked state ->
                    if attrTarget.SetChecked(state) then
                        action.SetChecked(state)
                | IconAttr icon ->
                    action.SetIcon(icon.QtValue)
                | IconText text ->
                    action.SetIconText(text)
                | Shortcut seq ->
                    action.SetShortcut(seq.QtValue)
                | StatusTip tip ->
                    action.SetStatusTip(tip)
                | ToolTip tip ->
                    action.SetToolTip(tip)
            | _ ->
                printfn "warning: MenuAction.Attr couldn't ApplyTo() unknown object type [%A]" target
                
type private SignalMapFunc<'msg>(func) =
    inherit SignalMapFuncBase<Signal,'msg>(func)
                
type Props<'msg>() =
    inherit QObject.Props<'msg>()
    
    let mutable onChanged: 'msg option = None
    let mutable onCheckableChanged: (bool -> 'msg) option = None
    let mutable onEnabledChanged: (bool -> 'msg) option = None
    let mutable onHovered: 'msg option = None
    let mutable onToggled: (bool -> 'msg) option = None
    let mutable onTriggered: (bool -> 'msg) option = None
    let mutable onVisibleChanged: 'msg option = None
    
    member internal this.SignalMask = enum<Action.SignalMask> (int this._signalMask)
    
    member this.OnChanged with set value =
        onChanged <- Some value
        this.AddSignal(int Action.SignalMask.Changed)
        
    member this.OnCheckableChanged with set value =
        onCheckableChanged <- Some value
        this.AddSignal(int Action.SignalMask.CheckableChanged)
        
    member this.OnEnabledChanged with set value =
        onEnabledChanged <- Some value
        this.AddSignal(int Action.SignalMask.EnabledChanged)
        
    member this.OnHovered with set value =
        onHovered <- Some value
        this.AddSignal(int Action.SignalMask.Hovered)
        
    member this.OnToggled with set value =
        onToggled <- Some value
        this.AddSignal(int Action.SignalMask.Toggled)
        
    member this.OnTriggered with set value =
        onTriggered <- Some value
        this.AddSignal(int Action.SignalMask.Triggered)
        
    member this.OnVisibleChanged with set value =
        onVisibleChanged <- Some value
        this.AddSignal(int Action.SignalMask.VisibleChanged)
    
    member internal this.SignalMapList =
        let thisFunc = function
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
        // we inherit from Object, so prepend to its signals
        SignalMapFunc(thisFunc) :> ISignalMapFunc :: base.SignalMapList
    
    member this.Text with set value =
        this.PushAttr(Text value)
        
    member this.Enabled with set value =
        this.PushAttr(Enabled value)
        
    member this.Separator with set value =
        this.PushAttr(Separator value)
        
    member this.Checkable with set value =
        this.PushAttr(Checkable value)
        
    member this.Checked with set value =
        this.PushAttr(Checked value)
        
    member this.Icon with set value =
        this.PushAttr(IconAttr value)
        
    member this.IconText with set value =
        this.PushAttr(IconText value)
        
    member this.Shortcut with set value =
        this.PushAttr(Shortcut value)
        
    member this.StatusTip with set value =
        this.PushAttr(StatusTip value)
        
    member this.ToolTip with set value =
        this.PushAttr(ToolTip value)
        
type ModelCore<'msg>(dispatch: 'msg -> unit) =
    inherit QObject.ModelCore<'msg>(dispatch)
    let mutable action: Action.Handle = null
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable currentMask = enum<Action.SignalMask> 0
    // 2-way binding guards:
    let mutable enabled = true
    let mutable checkable = false
    let mutable checked_ = false
    
    let signalDispatch (s: Signal) =
        signalMap s
        |> Option.iter dispatch
        
    member this.Action
        with get() = action
        and set value =
            this.Object <- value
            action <- value
            
    member internal this.SignalMaps with set (mapFuncList: ISignalMapFunc list) =
        match mapFuncList with
        | h :: etc ->
            match h with
            | :? SignalMapFunc<'msg> as smf ->
                signalMap <- smf.Func
            | _ ->
                failwith "MenuAction.ModelCore.SignalMaps: wrong func type"
            // assign the remainder up the hierarchy
            base.SignalMaps <- etc
        | _ ->
            failwith "MenuAction.ModelCore: signal map assignment didn't have a head element"
            
    member this.SignalMask with set value =
        if value <> currentMask then
            // we don't need to invoke the base version, the most derived widget handles the full signal stack from all super classes (at the C++/C# levels)
            action.SetSignalMask(value)
            currentMask <- value
            
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
        // object =========================
        member this.Destroyed(obj: Object.Handle) =
            (this :> Object.SignalHandler).Destroyed(obj)
        member this.ObjectNameChanged(name: string) =
            (this :> Object.SignalHandler).ObjectNameChanged(name)
        // action =========================
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
    
type private Model<'msg>(dispatch: 'msg -> unit, maybeContainingWindow: Widget.Handle option) as this =
    inherit ModelCore<'msg>(dispatch)
    let owner =
        match maybeContainingWindow with
        | Some handle ->
            // printfn "MenuAction being created with owner [%A]" handle
            handle
        | None ->
            printfn "MenuAction created with no owner"
            null
    let mutable action = Action.Create(owner, this)
    do
        this.Action <- action

let private create (attrs: IAttr list) (signalMaps: ISignalMapFunc list) (dispatch: 'msg -> unit) (signalMask: Action.SignalMask) (maybeContainingWindow: Widget.Handle option) =
    let model = new Model<'msg>(dispatch, maybeContainingWindow)
    model.ApplyAttrs (attrs |> List.map (fun attr -> None, attr))
    model.SignalMaps <- signalMaps
    model.SignalMask <- signalMask
    model

let private migrate (model: Model<'msg>) (attrs: (IAttr option * IAttr) list) (signalMaps: ISignalMapFunc list) (signalMask: Action.SignalMask) =
    model.ApplyAttrs attrs
    model.SignalMaps <- signalMaps
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
    inherit Props<'msg>()
    [<DefaultValue>] val mutable private model: Model<'msg>
    let mutable buildState = Init
    
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    interface IActionNode<'msg> with
        override this.Dependencies = []
        
        override this.Create dispatch buildContext =
            // since Actions can be dependencies of multiple nodes (eg MainWindow, menus, context menus, toolbars, etc),
            // we need to protect against rebuilding the model each time
            match buildState with
            | Init ->
                this.model <- create this.Attrs this.SignalMapList dispatch this.SignalMask buildContext.ContainingWindow
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
                this.model <- migrate left'.model nextAttrs this.SignalMapList this.SignalMask
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
                dispose this.model
                buildState <- Disposed
            
        override this.Action =
            this.model.Action
            
        override this.ContentKey =
            (this :> IActionNode<'msg>).Action
            
        override this.Attachments =
            this.Attachments
