module Tabs.FlightBooker.DatePicker

open System
open BuilderNode
open SubReactor
open Widgets

type Value =
    | Invalid
    | Valid of dt: DateTime
    
type Signal =
    | ValueChanged of value: Value
    
type Attr =
    | Value of dt: DateTime
    | Enabled of value: bool
    
let keyFunc = function
    | Value _ -> 0
    | Enabled _ -> 1
    
type State = {
    Enabled: bool
    Raw: string
    Parsed: Value
}

type Msg =
    | EditChanged of str: string
    | EditSubmitted
    | ShowCalendar

let init () =
    let dt =
        DateTime.Now
    let state = {
        Enabled = true
        Raw = dt.ToShortDateString()
        Parsed = Valid dt
    }
    state, Cmd.None
    
let attrUpdate (state: State) (attr: Attr) =
    match attr with
    | Value dt ->
        { state with
            Parsed = Valid dt
            Raw = dt.ToShortDateString() }
    | Enabled value ->
        { state with Enabled = value }
        
let tryParseDate (str: string) =
    match DateTime.TryParse str with // Exact(str, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None)
    | true, dt -> Some dt
    | _ -> None
    
let update (state: State) (msg: Msg) =
    match msg with
    | EditChanged str ->
        let nextParsed =
            match tryParseDate str with
            | Some value ->
                Valid value
            | None ->
                Invalid
        let cmd =
            if nextParsed <> state.Parsed then
                Cmd.Signal (ValueChanged nextParsed)
            else
                Cmd.None
        { state with Raw = str; Parsed = nextParsed }, cmd
    | EditSubmitted ->
        state, Cmd.None
    | ShowCalendar ->
        state, Cmd.None

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
    hbox :> ILayoutNode<Msg>
    
type Node<'outerMsg>() =
    inherit LayoutReactorNode<'outerMsg, State, Msg, Attr, Signal>(init, attrUpdate, update, view, genericDiffAttrs keyFunc)
    let mutable onValueChanged: (Value -> 'outerMsg) option = None
    member this.OnValueChanged with set value = onValueChanged <- Some value
    override this.SignalMap (s: Signal) =
        match s with
        | ValueChanged value ->
            onValueChanged
            |> Option.map (fun f -> f value)
