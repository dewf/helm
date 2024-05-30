module HelperControls.DatePicker

open System
open System.Globalization
open BuilderNode
open SubReactor
open Widgets
open WithDialogs

type Value =
    | Empty
    | Invalid
    | Valid of dt: DateTime
    
type Signal =
    | ValueChanged of value: Value
    
type Attr =
    | Value of value: Value
    | Enabled of value: bool
    | DialogTitle of title: string
    
let keyFunc = function
    | Value _ -> 0
    | Enabled _ -> 1
    | DialogTitle _ -> 2
    
type State = {
    Enabled: bool
    Raw: string
    Value: Value
    DialogTitle: string
}

type Msg =
    | EditChanged of str: string
    | EditSubmitted
    | ShowCalendar
    | CalendarOp of op: DialogOp

let init () =
    let state = {
        Enabled = true
        Raw = ""
        Value = Empty
        DialogTitle = "Choose Date"
    }
    state, Cmd.None
    
let attrUpdate (state: State) (attr: Attr) =
    match attr with
    | Value value ->
        if value <> state.Value then
            match value with
            | Empty ->
                { state with Value = Empty; Raw = "" }
            | Invalid ->
                // unlikely to be assigned from outside except as a 2-way echo
                // we'd only get here if the parent component explicitly changed the value to invalid
                { state with Value = Invalid; Raw = "??invalid??" }
            | Valid dt ->
                { state with Value = Valid dt; Raw = dt.ToShortDateString() }
        else
            state
    | Enabled value ->
        { state with Enabled = value }
    | DialogTitle title ->
        { state with DialogTitle = title }
        
let tryParseDate (str: string) =
    match DateTime.TryParseExact(str, "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None) with
    | true, dt -> Some dt
    | _ -> None
    
let update (state: State) (msg: Msg) =
    match msg with
    | EditChanged str ->
        let nextValue =
            match tryParseDate str with
            | Some value ->
                Valid value
            | None ->
                if str = "" then
                    Empty
                else
                    Invalid
        let cmd =
            if nextValue <> state.Value then
                Cmd.Signal (ValueChanged nextValue)
            else
                Cmd.None
        { state with Raw = str; Value = nextValue }, cmd
    | EditSubmitted ->
        state, Cmd.None
    | ShowCalendar ->
        state, Cmd.DialogOp ("calendar", Exec)
    | CalendarOp op ->
        state, Cmd.DialogOp ("calendar", op)

let view (state: State) =
    let edit =
        LineEdit.Node(
            Attrs = [ LineEdit.Value state.Raw; LineEdit.Enabled state.Enabled ], OnChanged = EditChanged, OnReturnPressed = EditSubmitted)
    let button =
        PushButton.Node(Attrs = [ PushButton.Label "📅"; PushButton.Enabled state.Enabled ], OnClicked = ShowCalendar)
    let hbox =
        BoxLayout.Node(
            Attrs = [
                BoxLayout.Direction BoxLayout.Horizontal
                BoxLayout.Spacing 4
                BoxLayout.ContentsMargins (0, 0, 0, 0)
            ],
            Items = [ edit; button ])
    let dialog =
        let reject =
            PushButton.Node(Attrs = [ PushButton.Label "Reject" ], OnClicked = CalendarOp Reject)
        let accept =
            PushButton.Node(Attrs = [ PushButton.Label "Woot!" ], OnClicked = CalendarOp Accept)
        let layout =
            BoxLayout.Node(Attrs = [ BoxLayout.Direction BoxLayout.Vertical ], Items = [ reject; accept ])
        Dialog.Node(
            Attrs = [ Dialog.Size (320, 200); Dialog.Title state.DialogTitle ],
            Layout = layout)
    LayoutWithDialogs(hbox, [ "calendar", dialog ])
    :> ILayoutNode<Msg>
    
type DatePicker<'outerMsg>() =
    inherit LayoutReactorNode<'outerMsg, State, Msg, Attr, Signal>(init, attrUpdate, update, view, genericDiffAttrs keyFunc)
    let mutable onValueChanged: (Value -> 'outerMsg) option = None
    member this.OnValueChanged with set value = onValueChanged <- Some value
    override this.SignalMap (s: Signal) =
        match s with
        | ValueChanged value ->
            onValueChanged
            |> Option.map (fun f -> f value)
