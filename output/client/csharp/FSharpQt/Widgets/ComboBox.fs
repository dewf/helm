module FSharpQt.Widgets.ComboBox

open System
open FSharpQt.BuilderNode
open Org.Whatever.QtTesting

open FSharpQt.Attrs
open FSharpQt.Props.ComboBox

let someIfPositive (i: int) =
    if i >= 0 then Some i else None
    
type private Model<'msg>(dispatch: 'msg -> unit) as this =
    let mutable combo = ComboBox.Create(this)
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable currentMask = enum<ComboBox.SignalMask> 0
    
    // binding guards:
    let mutable currentIndex: int option = None
    let mutable currentText: string option = None
    
    let signalDispatch (s: Signal) =
        match signalMap s with
        | Some msg ->
            dispatch msg
        | None ->
            ()
        
    member this.Widget with get() = combo
    member this.SignalMap with set value = signalMap <- value
    
    member this.SignalMask with set value =
        if value <> currentMask then
            combo.SetSignalMask(value)
            currentMask <- value
    
    member this.ApplyAttrs(attrs: IAttr list) =
        for attr in attrs do
            attr.ApplyTo(this)
            
    interface ComboBoxAttrTarget with
        member this.Widget = combo
        member this.ComboBox = combo
        member this.Clear() =
            currentIndex <- None
            currentText <- None
        member this.SetCurrentIndex newIndex =
            if newIndex <> currentIndex then
                currentIndex <- newIndex
                true
            else
                false
        member this.SetCurrentText newText =
            if newText <> currentText then
                currentText <- newText
                true
            else
                false
                        
    interface ComboBox.SignalHandler with
        override this.Activated index =
            signalDispatch (someIfPositive index |> Activated)
        override this.CurrentIndexChanged index =
            signalDispatch (someIfPositive index |> CurrentIndexChanged)
        override this.CurrentTextChanged text =
            signalDispatch (CurrentTextChanged text)
        override this.EditTextChanged text =
            signalDispatch (EditTextChanged text)
        override this.Highlighted index =
            signalDispatch (someIfPositive index |> Highlighted)
        override this.TextActivated text =
            signalDispatch (TextActivated text)
        override this.TextHighlighted text =
            signalDispatch (TextHighlighted text)
                        
    interface IDisposable with
        member this.Dispose() =
            combo.Dispose()

let private create (attrs: IAttr list) (signalMap: Signal -> 'msg option) (dispatch: 'msg -> unit) (signalMask: ComboBox.SignalMask) =
    let model = new Model<'msg>(dispatch)
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private migrate (model: Model<'msg>) (attrs: IAttr list) (signalMap: Signal -> 'msg option) (signalMask: ComboBox.SignalMask) =
    model.ApplyAttrs attrs
    model.SignalMap <- signalMap
    model.SignalMask <- signalMask
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()
    

type ComboBox<'msg>() =
    inherit Props<'msg>()
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
            let left' = (left :?> ComboBox<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged
            this.model <- migrate left'.model nextAttrs this.SignalMap this.SignalMask
                
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()
            
        override this.Widget =
            (this.model.Widget :> Widget.Handle)
            
        override this.ContentKey =
            (this :> IWidgetNode<'msg>).Widget
            
        override this.Attachments =
            this.Attachments
