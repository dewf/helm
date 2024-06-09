module FSharpQt.Widgets.PushButton

open FSharpQt.BuilderNode
open System
open Org.Whatever.QtTesting

type Signal =
    | Clicked
    | ClickedWithState of state: bool // this is the full version of the Qt signal, but we offer simpler parameterless Clicked as well
    | Pressed
    | Released
    | Toggled of state: bool
    
type Attr =
    | Text of string
    | Checked of bool
    | Enabled of bool
    | Checkable of bool
    | AutoDefault of bool
    | MinWidth of width: int
    | MinHeight of width: int

let private keyFunc = function
    | Text _ -> 0
    | Checked _ -> 1
    | Enabled _ -> 2
    | Checkable _ -> 3
    | AutoDefault _ -> 4
    | MinWidth _ -> 5
    | MinHeight _ -> 6

let private diffAttrs =
    genericDiffAttrs keyFunc

type private Model<'msg>(dispatch: 'msg -> unit) as this =
    let mutable button = PushButton.Create(this)
    
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable currentMask = enum<PushButton.SignalMask> 0
    
    let dispatcher (s: Signal) =
        match signalMap s with
        | Some msg ->
            dispatch msg
        | None ->
            ()
        
    member this.Widget with get() = button
    member this.SignalMap with set value = signalMap <- value
    
    member this.SignalMask with set value =
        if value <> currentMask then
            button.SetSignalMask(value)
            currentMask <- value
    
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Text text ->
                button.SetText(text)
            | Checked state ->
                button.SetChecked(state)
            | Enabled state ->
                button.SetEnabled(state)
            | Checkable state ->
                button.SetCheckable(state)
            | AutoDefault state ->
                button.SetAutoDefault(state)
            | MinWidth width ->
                button.SetMinimumWidth(width)
            | MinHeight height ->
                button.SetMinimumHeight(height)
                
    interface PushButton.SignalHandler with
        member this.Clicked(checkState: bool) =
            dispatcher (ClickedWithState checkState)
            dispatcher Clicked
        member this.Pressed() =
            dispatcher Pressed
        member this.Released() =
            dispatcher Released
        member this.Toggled(checkState: bool) =
            dispatcher (Toggled checkState)
            
    interface IDisposable with
        member this.Dispose() =
            button.Dispose()

let private create (attrs: Attr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (initialMask: PushButton.SignalMask) =
    let model = new Model<'msg>(dispatch)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- initialMask
    model

let private migrate (model: Model<'msg>) (attrs: Attr list) (signalMap: Signal -> 'msg option) (signalMask: PushButton.SignalMask) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()

type PushButton<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>
    member val Attrs: Attr list = [] with get, set
    
    let mutable signalMask = enum<PushButton.SignalMask> 0
    
    let mutable onClicked: 'msg option = None
    let mutable onClickedWithState: (bool -> 'msg) option = None
    let mutable onPressed: 'msg option = None
    let mutable onReleased: 'msg option = None
    let mutable onToggled: (bool -> 'msg) option = None
    
    member this.OnClicked with set value =
        onClicked <- Some value
        signalMask <- signalMask ||| PushButton.SignalMask.Clicked  // clicked #1
        
    member this.OnClickedWithState with set value =
        onClickedWithState <- Some value
        signalMask <- signalMask ||| PushButton.SignalMask.Clicked  // clicked #2
        
    member this.OnPressed with set value =
        onPressed <- Some value
        signalMask <- signalMask ||| PushButton.SignalMask.Pressed
        
    member this.OnReleased with set value =
        onReleased <- Some value
        signalMask <- signalMask ||| PushButton.SignalMask.Released
        
    member this.OnToggled with set value =
        onToggled <- Some value
        signalMask <- signalMask ||| PushButton.SignalMask.Toggled
        
    let signalMap = function
        | Clicked -> onClicked
        | ClickedWithState checkState ->
            onClickedWithState
            |> Option.map (fun f -> f checkState)
        | Pressed -> onPressed
        | Released -> onReleased
        | Toggled state ->
            onToggled
            |> Option.map (fun f -> f state)
                
    interface IWidgetNode<'msg> with
        override this.Dependencies = []

        override this.Create(dispatch: 'msg -> unit) =
            this.model <- create this.Attrs signalMap dispatch signalMask

        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> PushButton<'msg>)
            let nextAttrs = diffAttrs left'.Attrs this.Attrs |> createdOrChanged
            this.model <- migrate left'.model nextAttrs signalMap signalMask

        override this.Dispose() =
            (this.model :> IDisposable).Dispose()

        override this.Widget =
            (this.model.Widget :> Widget.Handle)
            
        override this.ContentKey =
            (this :> IWidgetNode<'msg>).Widget
            
        override this.AttachedToWindow window =
            ()
