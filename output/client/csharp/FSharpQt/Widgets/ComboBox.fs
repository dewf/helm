module FSharpQt.Widgets.ComboBox

open System
open FSharpQt.BuilderNode
open FSharpQt.MiscTypes
open FSharpQt.Widgets.Widget
open Org.Whatever.QtTesting

open FSharpQt.Attrs

type Signal =
    | Activated of index: int option
    | CurrentIndexChanged of index: int option
    | CurrentTextChanged of text: string
    | EditTextChanged of text: string
    | Highlighted of index: int option
    | TextActivated of text: string
    | TextHighlighted of text: string
    
type InsertPolicy =
    | NoInsert
    | InsertAtTop
    | InsertAtCurrent
    | InsertAtBottom
    | InsertAfterCurrent
    | InsertBeforeCurrent
    | InsertAlphabetically
with
    member this.QtValue =
        match this with
        | NoInsert -> ComboBox.InsertPolicy.NoInsert
        | InsertAtTop -> ComboBox.InsertPolicy.InsertAtTop
        | InsertAtCurrent -> ComboBox.InsertPolicy.InsertAtCurrent
        | InsertAtBottom -> ComboBox.InsertPolicy.InsertAtBottom
        | InsertAfterCurrent -> ComboBox.InsertPolicy.InsertAfterCurrent
        | InsertBeforeCurrent -> ComboBox.InsertPolicy.InsertBeforeCurrent
        | InsertAlphabetically -> ComboBox.InsertPolicy.InsertAlphabetically
        
type SizeAdjustPolicy =
    | AdjustToContents
    | AdjustToContentsOnFirstShow
    | AdjustToMinimumContentsLengthWithIcon
with
    member this.QtValue =
        match this with
        | AdjustToContents -> ComboBox.SizeAdjustPolicy.AdjustToContents
        | AdjustToContentsOnFirstShow -> ComboBox.SizeAdjustPolicy.AdjustToContentsOnFirstShow
        | AdjustToMinimumContentsLengthWithIcon -> ComboBox.SizeAdjustPolicy.AdjustToMinimumContentsLengthWithIcon
    
type internal Attr =
    | CurrentIndex of maybeIndex: int option
    | CurrentText of maybeText: string option
    | DuplicatesEnabled of enabled: bool
    | Editable of editable: bool
    | Frame of hasFrame: bool
    | IconSize of size: Size
    | InsertPolicy of policy: InsertPolicy
    | MaxCount of count: int
    | MaxVisibleItems of count: int
    | MinimumContentsLength of length: int
    | ModelColumn of column: int
    | PlaceholderText of text: string
    | SizeAdjustPolicy of policy: SizeAdjustPolicy
    // ours:
    | StringItems of items: string list
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
            | CurrentIndex _ -> "combobox:currentindex"
            | CurrentText _ -> "combobox:currenttext"
            | DuplicatesEnabled _ -> "combobox:duplicatesenabled"
            | Editable _ -> "combobox:editable"
            | Frame _ -> "combobox:hasframe"
            | IconSize _ -> "combobox:iconsize"
            | InsertPolicy _ -> "combobox:insertpolicy"
            | MaxCount _ -> "combobox:maxcount"
            | MaxVisibleItems _ -> "combobox:maxvisibleitems"
            | MinimumContentsLength _ -> "combobox:minimumcontentslength"
            | ModelColumn _ -> "combobox:modelcolumn"
            | PlaceholderText _ -> "combobox:placeholdertext"
            | SizeAdjustPolicy _ -> "combobox:sizeadjustpolicy"
            | StringItems _ -> "combobox:stringitems"
        override this.ApplyTo (target: IAttrTarget) =
            match target with
            | :? ComboBoxAttrTarget as comboTarget ->
                let comboBox =
                    comboTarget.ComboBox
                match this with
                | CurrentIndex maybeIndex ->
                    if comboTarget.SetCurrentIndex maybeIndex then
                        comboBox.SetCurrentIndex(maybeIndex |> Option.defaultValue -1)
                | CurrentText maybeText ->
                    if comboTarget.SetCurrentText maybeText then
                        comboBox.SetCurrentText(maybeText |> Option.defaultValue "")
                | DuplicatesEnabled enabled ->
                    comboBox.SetDuplicatesEnabled(enabled)
                | Editable editable ->
                    comboBox.SetEditable(editable)
                | Frame hasFrame ->
                    comboBox.SetFrame(hasFrame)
                | IconSize size ->
                    comboBox.SetIconSize(size.QtValue)
                | InsertPolicy policy ->
                    comboBox.SetInsertPolicy(policy.QtValue)
                | MaxCount count ->
                    comboBox.SetMaxCount(count)
                | MaxVisibleItems count ->
                    comboBox.SetMaxVisibleItems(count)
                | MinimumContentsLength length ->
                    comboBox.SetMinimumContentsLength(length)
                | ModelColumn column ->
                    comboBox.SetModelColumn(column)
                | PlaceholderText text ->
                    comboBox.SetPlaceholderText(text)
                | SizeAdjustPolicy policy ->
                    comboBox.SetSizeAdjustPolicy(policy.QtValue)
                | StringItems items ->
                    comboTarget.Clear()
                    comboBox.Clear()
                    comboBox.AddItems(items |> Array.ofList)
            | _ ->
                printfn "warning: ComboBox.Attr couldn't ApplyTo() unknown target type [%A]" target
                
type ComboBoxProps() =
    inherit WidgetProps()
    
    member this.CurrentIndex with set value =
        this.PushAttr(CurrentIndex value)

    member this.CurrentText with set value =
        this.PushAttr(CurrentText value)

    member this.DuplicatesEnabled with set value =
        this.PushAttr(DuplicatesEnabled value)

    member this.Editable with set value =
        this.PushAttr(Editable value)

    member this.Frame with set value =
        this.PushAttr(Frame value)

    member this.IconSize with set value =
        this.PushAttr(IconSize value)

    member this.InsertPolicy with set value =
        this.PushAttr(InsertPolicy value)

    member this.MaxCount with set value =
        this.PushAttr(MaxCount value)

    member this.MaxVisibleItems with set value =
        this.PushAttr(MaxVisibleItems value)

    member this.MinimumContentsLength with set value =
        this.PushAttr(MinimumContentsLength value)

    member this.ModelColumn with set value =
        this.PushAttr(ModelColumn value)

    member this.PlaceholderText with set value =
        this.PushAttr(PlaceholderText value)

    member this.SizeAdjustPolicy with set value =
        this.PushAttr(SizeAdjustPolicy value)
        
    member this.StringItems with set value =
        this.PushAttr(StringItems value)
        
    
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
    inherit ComboBoxProps()
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    member this.Attrs = this._attrs |> List.rev
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    let mutable signalMask = enum<ComboBox.SignalMask> 0
    
    let mutable onActivated: (int option -> 'msg) option = None
    let mutable onCurrentIndexChanged: (int option -> 'msg) option = None
    let mutable onCurrentTextChanged: (string -> 'msg) option = None
    let mutable onEditTextChanged: (string -> 'msg) option = None
    let mutable onHighlighted: (int option -> 'msg) option = None
    let mutable onTextActivated: (string -> 'msg) option = None
    let mutable onTextHighlighted: (string -> 'msg) option = None
    
    member this.OnActivated with set value =
        onActivated <- Some value
        signalMask <- signalMask ||| ComboBox.SignalMask.Activated
    member this.OnCurrentIndexChanged with set value =
        onCurrentIndexChanged <- Some value
        signalMask <- signalMask ||| ComboBox.SignalMask.CurrentIndexChanged
    member this.OnCurrentTextChanged with set value =
        onCurrentTextChanged <- Some value
        signalMask <- signalMask ||| ComboBox.SignalMask.CurrentTextChanged
    member this.OnEditTextChanged with set value =
        onEditTextChanged <- Some value
        signalMask <- signalMask ||| ComboBox.SignalMask.EditTextChanged
    member this.OnHighlighted with set value =
        onHighlighted <- Some value
        signalMask <- signalMask ||| ComboBox.SignalMask.Highlighted
    member this.OnTextActivated with set value =
        onTextActivated <- Some value
        signalMask <- signalMask ||| ComboBox.SignalMask.TextActivated
    member this.OnTextHighlighted with set value =
        onTextHighlighted <- Some value
        signalMask <- signalMask ||| ComboBox.SignalMask.TextHighlighted
        
    let signalMap = function
        | Activated index ->
            onActivated
            |> Option.map (fun f -> f index)
        | CurrentIndexChanged index ->
            onCurrentIndexChanged
            |> Option.map (fun f -> f index)
        | CurrentTextChanged text ->
            onCurrentTextChanged
            |> Option.map (fun f -> f text)
        | EditTextChanged text ->
            onEditTextChanged
            |> Option.map (fun f -> f text)
        | Highlighted index ->
            onHighlighted
            |> Option.map (fun f -> f index)
        | TextActivated text ->
            onTextActivated
            |> Option.map (fun f -> f text)
        | TextHighlighted text ->
            onTextHighlighted
            |> Option.map (fun f -> f text)
                
    interface IWidgetNode<'msg> with
        override this.Dependencies = []
        
        override this.Create dispatch buildContext =
            this.model <- create this.Attrs signalMap dispatch signalMask
            
        override this.AttachDeps () =
            ()
            
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> ComboBox<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged
            this.model <- migrate left'.model nextAttrs signalMap signalMask
                
        override this.Dispose() =
            (this.model :> IDisposable).Dispose()
            
        override this.Widget =
            (this.model.Widget :> Widget.Handle)
            
        override this.ContentKey =
            (this :> IWidgetNode<'msg>).Widget
            
        override this.Attachments =
            this.Attachments
