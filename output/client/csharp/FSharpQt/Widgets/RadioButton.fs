module FSharpQt.Widgets.RadioButton

open FSharpQt.BuilderNode
open System
open Org.Whatever.QtTesting

type Signal =
    | Clicked
    | ClickedWithState of state: bool
    | Pressed
    | Released
    | Toggled of state: bool

type Attr =
    | Text of text: string
    | Checked of state: bool
    | Enabled of enabled: bool

let private keyFunc = function
    | Text _ -> 0
    | Checked _ -> 1
    | Enabled _ -> 2

let private diffAttrs =
    genericDiffAttrs keyFunc

type private Model<'msg>(dispatch: 'msg -> unit) as this =
    let mutable radioButton = RadioButton.Create(this)
    
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable currentMask = enum<RadioButton.SignalMask> 0
    
    // 2-way binding guard
    let mutable lastState = false
    
    let dispatcher (s: Signal) =
        match signalMap s with
        | Some msg ->
            dispatch msg
        | None ->
            ()
        
    member this.Widget with get() = radioButton
    member this.SignalMap with set value = signalMap <- value
    
    member this.SignalMask with set value =
        if value <> currentMask then
            radioButton.SetSignalMask(value)
            currentMask <- value
    
    member this.ApplyAttrs(attrs: Attr list) =
        for attr in attrs do
            match attr with
            | Text text ->
                radioButton.SetText(text)
            | Checked state ->
                // 2-way binding guard
                if state <> lastState then
                    radioButton.SetChecked(state)
                    lastState <- state
            | Enabled state ->
                radioButton.SetEnabled(state)
                
    interface RadioButton.SignalHandler with
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
            radioButton.Dispose()

let private create (attrs: Attr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (initialMask: RadioButton.SignalMask) =
    let model = new Model<'msg>(dispatch)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- initialMask
    model

let private migrate (model: Model<'msg>) (attrs: Attr list) (signalMap: Signal -> 'msg option) (signalMask: RadioButton.SignalMask) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()

type RadioButton<'msg>() =
    [<DefaultValue>] val mutable private model: Model<'msg>
    member val Attrs: Attr list = [] with get, set
    
    let mutable signalMask = enum<RadioButton.SignalMask> 0
    
    let mutable onClicked: 'msg option = None
    let mutable onClickedWithState: (bool -> 'msg) option = None
    let mutable onPressed: 'msg option = None
    let mutable onReleased: 'msg option = None
    let mutable onToggled: (bool -> 'msg) option = None
    
    member this.OnClicked with set value =
        onClicked <- Some value
        signalMask <- signalMask ||| RadioButton.SignalMask.Clicked  // clicked #1
        
    member this.OnClickedWithState with set value =
        onClickedWithState <- Some value
        signalMask <- signalMask ||| RadioButton.SignalMask.Clicked  // clicked #2
        
    member this.OnPressed with set value =
        onPressed <- Some value
        signalMask <- signalMask ||| RadioButton.SignalMask.Pressed
        
    member this.OnReleased with set value =
        onReleased <- Some value
        signalMask <- signalMask ||| RadioButton.SignalMask.Released
        
    member this.OnToggled with set value =
        onToggled <- Some value
        signalMask <- signalMask ||| RadioButton.SignalMask.Toggled
        
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
            let left' = (left :?> RadioButton<'msg>)
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
