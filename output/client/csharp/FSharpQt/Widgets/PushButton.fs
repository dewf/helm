module FSharpQt.Widgets.PushButton

open FSharpQt.BuilderNode
open System
open Org.Whatever.QtTesting

open FSharpQt.Attrs
open FSharpQt.Props
open FSharpQt.Props.PushButton

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
            signalDispatch (AbstractButton.Signal.Clicked |> AbstractButtonSignal)
            signalDispatch (AbstractButton.Signal.ClickedWithChecked checkState |> AbstractButtonSignal)
        member this.Pressed() =
            signalDispatch (AbstractButton.Signal.Pressed |> AbstractButtonSignal)
        member this.Released() =
            signalDispatch (AbstractButton.Signal.Released |> AbstractButtonSignal)
        member this.Toggled(checkState: bool) =
            checked_ <- checkState
            signalDispatch (AbstractButton.Signal.Toggled checkState |> AbstractButtonSignal)
            
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
    inherit PushButtonProps<'msg>()
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    member this.Attrs = this._attrs |> List.rev
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    interface IWidgetNode<'msg> with
        override this.Dependencies = []

        override this.Create dispatch buildContext =
            this.model <- create this.Attrs this.SignalMap dispatch this.SignalMask
            
        override this.AttachDeps () =
            ()

        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> PushButton<'msg>)
            let nextAttrs = diffAttrs left'.Attrs this.Attrs |> createdOrChanged
            this.model <- migrate left'.model nextAttrs this.SignalMap this.SignalMask

        override this.Dispose() =
            (this.model :> IDisposable).Dispose()

        override this.Widget =
            this.model.PushButton
            
        override this.ContentKey =
            this.model.PushButton
            
        override this.Attachments =
            this.Attachments

