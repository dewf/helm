module FSharpQt.Widgets.Menus.MenuAction

open System
open FSharpQt.BuilderNode
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
    
let private keyFunc = function
    | Text _ -> 0
    | Enabled _ -> 1
    | Separator _ -> 2
    
let private diffAttrs =
    genericDiffAttrs keyFunc

type private Model<'msg>(dispatch: 'msg -> unit) as this =
    let mutable action = Action.Create(this)
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable currentMask = enum<Action.SignalMask> 0
    
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
                action.SetEnabled(state)
            | Separator state ->
                action.SetSeparator(state)
                
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

let private create (attrs: Attr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (signalMask: Action.SignalMask) =
    let model = new Model<'msg>(dispatch)
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
            
type MenuAction<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>
    
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
            this.model <- create this.Attrs signalMap dispatch signalMask
            
        override this.AttachDeps () =
            ()
            
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> MenuAction<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged
            this.model <- migrate left'.model nextAttrs signalMap signalMask
                
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()
            
        override this.Action =
            this.model.Action
            
        override this.ContentKey =
            (this :> IActionNode<'msg>).Action
            
        override this.Attachments =
            this.Attachments
