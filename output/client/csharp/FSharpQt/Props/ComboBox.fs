module FSharpQt.Props.ComboBox

open FSharpQt.Attrs
open FSharpQt.MiscTypes
open Org.Whatever.QtTesting

type Signal =
    // inherited
    | WidgetSignal of signal: Widget.Signal
    // ours
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
                
type Props<'msg>() =
    inherit Widget.Props<'msg>()

    let mutable onActivated: (int option -> 'msg) option = None
    let mutable onCurrentIndexChanged: (int option -> 'msg) option = None
    let mutable onCurrentTextChanged: (string -> 'msg) option = None
    let mutable onEditTextChanged: (string -> 'msg) option = None
    let mutable onHighlighted: (int option -> 'msg) option = None
    let mutable onTextActivated: (string -> 'msg) option = None
    let mutable onTextHighlighted: (string -> 'msg) option = None

    member internal this.SignalMask = enum<ComboBox.SignalMask> (int this._signalMask)

    member this.OnActivated with set value =
        onActivated <- Some value
        this.AddSignal(int ComboBox.SignalMask.Activated)
        // signalMask <- signalMask ||| ComboBox.SignalMask.Activated
        
    member this.OnCurrentIndexChanged with set value =
        onCurrentIndexChanged <- Some value
        this.AddSignal(int ComboBox.SignalMask.CurrentIndexChanged)
        // signalMask <- signalMask ||| ComboBox.SignalMask.CurrentIndexChanged
        
    member this.OnCurrentTextChanged with set value =
        onCurrentTextChanged <- Some value
        this.AddSignal(int ComboBox.SignalMask.CurrentTextChanged)
        // signalMask <- signalMask ||| ComboBox.SignalMask.CurrentTextChanged
        
    member this.OnEditTextChanged with set value =
        onEditTextChanged <- Some value
        this.AddSignal(int ComboBox.SignalMask.EditTextChanged)
        // signalMask <- signalMask ||| ComboBox.SignalMask.EditTextChanged
        
    member this.OnHighlighted with set value =
        onHighlighted <- Some value
        this.AddSignal(int ComboBox.SignalMask.Highlighted)
        // signalMask <- signalMask ||| ComboBox.SignalMask.Highlighted
        
    member this.OnTextActivated with set value =
        onTextActivated <- Some value
        this.AddSignal(int ComboBox.SignalMask.TextActivated)
        // signalMask <- signalMask ||| ComboBox.SignalMask.TextActivated
        
    member this.OnTextHighlighted with set value =
        onTextHighlighted <- Some value
        this.AddSignal(int ComboBox.SignalMask.TextHighlighted)
        // signalMask <- signalMask ||| ComboBox.SignalMask.TextHighlighted
        
    member internal this.SignalMap = function
        | WidgetSignal signal ->
            (this :> Widget.Props<'msg>).SignalMap signal
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
    
