module FSharpQt.Widgets.PushButton

open FSharpQt.BuilderNode
open System
open Org.Whatever.QtTesting

open FSharpQt.Attrs

type Signal =
    | Clicked
    | ClickedWithState of state: bool // this is the full version of the Qt signal, but we offer simpler parameterless Clicked as well
    | Pressed
    | Released
    | Toggled of state: bool
    
type Attr =
    | AutoDefault of state: bool
    | Default of state: bool
    | Flat of state: bool
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
            | AutoDefault _ -> "pushbutton:autodefault"
            | Default _ -> "pushbutton:default"
            | Flat _ -> "pushbutton:flat"
        override this.ApplyTo (target: IAttrTarget) =
            match target with
            | :? PushButtonAttrTarget as buttonTarget ->
                let button =
                    buttonTarget.PushButton
                match this with
                | AutoDefault state ->
                    button.SetAutoDefault(state)
                | Default state ->
                    button.SetDefault(state)
                | Flat state ->
                    button.SetFlat(state)
            | _ ->
                printfn "warning: PushButton.Attr couldn't ApplyTo() unknown target type [%A]" target
    
type PushButtonProps() =
    inherit AbstractButton.AbstractButtonProps()
    
    let mutable attrs: IAttr list = []
    member this.PushButtonAttrs = attrs @ this.AbstractButtonAttrs
    
    member this.AutoDefault with set value =
        attrs <- AutoDefault value :: attrs
        
    member this.Default with set value =
        attrs <- Default value :: attrs
        
    member this.Flat with set value =
        attrs <- Flat value :: attrs
    
type private Model<'msg>(dispatch: 'msg -> unit) as this =
    let mutable button = PushButton.Create(this)
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable currentMask = enum<PushButton.SignalMask> 0
    
    // binding guards
    let mutable checked_ = false
    
    let signalDispatch (s: Signal) =
        signalMap s
        |> Option.iter dispatch
        
    member this.PushButton with get() = button
    member this.SignalMap with set value = signalMap <- value
    
    member this.SignalMask with set value =
        if value <> currentMask then
            button.SetSignalMask(value)
            currentMask <- value
    
    member this.ApplyAttrs(attrs: IAttr list) =
        for attr in attrs do
            attr.ApplyTo(this)
            
    interface PushButtonAttrTarget with
        // widget
        member this.Widget = button
        // abstractbutton:
        member this.AbstractButton = button
        member this.SetChecked newState =
            if newState <> checked_ then
                checked_ <- newState
                true
            else
                false
        // pushbutton:
        member this.PushButton = button
                
    interface PushButton.SignalHandler with
        member this.Clicked(checkState: bool) =
            signalDispatch (ClickedWithState checkState)
            signalDispatch Clicked
        member this.Pressed() =
            signalDispatch Pressed
        member this.Released() =
            signalDispatch Released
        member this.Toggled(checkState: bool) =
            printfn "toggled raw event, checkstate: %A" checkState
            checked_ <- checkState
            signalDispatch (Toggled checkState)
            
    interface IDisposable with
        member this.Dispose() =
            button.Dispose()

let private create (attrs: IAttr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (signalMask: PushButton.SignalMask) =
    let model = new Model<'msg>(dispatch)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private migrate (model: Model<'msg>) (attrs: IAttr list) (signalMap: Signal -> 'msg option) (signalMask: PushButton.SignalMask) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()

type PushButton<'msg>() =
    inherit PushButtonProps()
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    member this.Attrs = this.PushButtonAttrs
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
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

        override this.Create dispatch buildContext =
            this.model <- create this.Attrs signalMap dispatch signalMask
            
        override this.AttachDeps () =
            ()

        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> PushButton<'msg>)
            let nextAttrs = diffAttrs left'.Attrs this.Attrs |> createdOrChanged
            this.model <- migrate left'.model nextAttrs signalMap signalMask

        override this.Dispose() =
            (this.model :> IDisposable).Dispose()

        override this.Widget =
            this.model.PushButton
            
        override this.ContentKey =
            this.model.PushButton
            
        override this.Attachments =
            this.Attachments

