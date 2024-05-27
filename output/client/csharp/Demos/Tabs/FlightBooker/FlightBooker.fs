module Tabs.FlightBooker.FlightBooker

open System
open BuilderNode
open SubReactor
open Widgets

type Signal = unit
type Attr = unit

type Mode =
    | OneWay
    | RoundTrip

type State = {
    Mode: Mode
    DepartDate: DateTime option
    LeaveDate: DateTime option
}

type Msg =
    | ComboChanged of maybeIndex: int option

let init() =
    { Mode = OneWay
      DepartDate = Some DateTime.Now
      LeaveDate = None }, Cmd.None
    
let update (state: State) (msg: Msg) =
    match msg with
    | ComboChanged maybeIndex ->
        let nextMode = 
            match maybeIndex with
            | Some 0 -> OneWay
            | Some 1 -> RoundTrip
            | _ -> failwith "nope"
        { state with Mode = nextMode }, Cmd.None

let view (state: State) =
    let combo =
        let selectedIndex =
            match state.Mode with
            | OneWay -> 0
            | RoundTrip -> 1
        let items = [
            "One Way"
            "Round Trip"
        ]
        // PushButton.Node(Attrs = [ PushButton.Label "just testing" ])
        ComboBox.Node(Attrs = [ ComboBox.Items items; ComboBox.SelectedIndex (Some selectedIndex) ], OnSelected = ComboChanged)
    let edit1 =
        DateEdit.Node()
    let edit2 =
        DateEdit.Node(Attrs = [ DateEdit.Enabled (state.Mode = RoundTrip) ])
    let bookButton =
        PushButton.Node(Attrs = [ PushButton.Label "Book Trip"; PushButton.Enabled false ])
    BoxLayout.Node(
        Attrs = [ BoxLayout.Direction BoxLayout.Vertical ],
        Items = [ combo; edit1; edit2; bookButton ])
    :> ILayoutNode<Msg>
    
type Node<'outerMsg>() =
    inherit LayoutReactorNode<'outerMsg, State, Msg, unit, unit>(init, nullAttrUpdate, update, view, nullDiffAttrs)
