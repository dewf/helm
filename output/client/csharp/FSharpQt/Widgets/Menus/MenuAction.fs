module FSharpQt.Widgets.Menus.MenuAction

open System
open FSharpQt.BuilderNode
open FSharpQt.MiscTypes
open Org.Whatever.QtTesting

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
    | Enabled of state: bool
    | Separator of state: bool
    | Checkable of state: bool
    | Checked of state: bool
    | Icon of icon: IconProxy  // "proxy" for now ...
    | IconText of text: string
    | Shortcut of seq: KeySequenceProxy
    | StatusTip of tip: string
    | ToolTip of tip: string
    
let private keyFunc = function
    | Text _ -> 0
    | Enabled _ -> 1
    | Separator _ -> 2
    | Checkable _ -> 3
    | Checked _ -> 4
    | Icon _ -> 5
    | IconText _ -> 6
    | Shortcut _ -> 7
    | StatusTip _ -> 8
    | ToolTip _ -> 9
    
let private diffAttrs =
    genericDiffAttrs keyFunc

type private Model<'msg>(dispatch: 'msg -> unit, maybeContainingWindow: Widget.Handle option) as this =
    let owner =
        match maybeContainingWindow with
        | Some handle ->
            printfn "MenuAction being created with owner [%A]" handle
            handle
        | None ->
            printfn "MenuAction created with no owner :("
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
    
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Text text ->
                action.SetText(text)
            | Enabled state ->
                if state <> enabled then
                    enabled <- state
                    action.SetEnabled(state)
            | Separator state ->
                action.SetSeparator(state)
            | Checkable state ->
                if state <> checkable then
                    checkable <- state
                    action.SetCheckable(state)
            | Checked state ->
                if state <> checked_ then
                    checked_ <- state
                    action.SetChecked(state)
            | Icon icon ->
                action.SetIcon(icon.Handle)
            | IconText text ->
                action.SetIconText(text)
            | Shortcut seq ->
                action.SetShortcut(seq.Handle)
            | StatusTip tip ->
                action.SetStatusTip(tip)
            | ToolTip tip ->
                action.SetToolTip(tip)
                
    interface Action.SignalHandler with
        override this.Changed () =
            signalDispatch Changed
        override this.CheckableChanged checkable =
            signalDispatch (CheckableChanged checkable)
        override this.EnabledChanged enabled =
            signalDispatch (EnabledChanged enabled)
        override this.Hovered () =
            signalDispatch Hovered
        override this.Toggled checked_ =
            signalDispatch (Toggled checked_)
        override this.Triggered checked_ =
            signalDispatch (Triggered checked_)
        override this.VisibleChanged () =
            signalDispatch VisibleChanged
                
    interface IDisposable with
        member this.Dispose() =
            action.Dispose()

let private create (attrs: Attr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (signalMask: Action.SignalMask) (maybeContainingWindow: Widget.Handle option) =
    let model = new Model<'msg>(dispatch, maybeContainingWindow)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private migrate (model: Model<'msg>) (attrs: Attr list) (signalMap: Signal -> 'msg option) (signalMask: Action.SignalMask) =
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
    
    member val Attrs: Attr list = [] with get, set
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
