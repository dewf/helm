module Tabs.FlightBooker.DateEdit

open System
open BuilderNode
open SubReactor
open Widgets

type Signal =
    | DateChanged of dt: DateTime
    
type Attr =
    | Value of dt: DateTime
    | Enabled of value: bool
    
let keyFunc = function
    | Value _ -> 0
    | Enabled _ -> 1
    
let diffAttrs =
    genericDiffAttrs keyFunc
    
type State = {
    Enabled: bool
    Raw: string
    MaybeParsed: DateTime option
}

type Msg =
    | EditChanged of str: string

let init () =
    let dt =
        DateTime.Now
    let state = {
        Enabled = true
        MaybeParsed = Some dt
        Raw = string dt
    }
    state, Cmd.None
    
let attrUpdate (state: State) (attr: Attr) =
    match attr with
    | Value dt ->
        let nextState =
            { state with
                MaybeParsed = Some dt
                Raw = string dt }
        nextState
    | Enabled value ->
        { state with Enabled = value }
    
let update (state: State) (msg: Msg) =
    // attempt to parse raw text into date
    // update background color of edit if bad
    state, Cmd.None

let view (state: State) =
    let edit =
        LineEdit.Node(
            Attrs = [ LineEdit.Value state.Raw; LineEdit.Enabled state.Enabled ], OnChanged = EditChanged)
    edit :> IWidgetNode<Msg>
    // let box =
    //     BoxLayout.Node(
    //         Attrs = [ BoxLayout.Direction BoxLayout.Vertical ],
    //         Items = [ edit ])
    // box :> ILayoutNode<Msg>
    
type Node<'outerMsg>() =
    inherit WidgetReactorNode<'outerMsg, State, Msg, Attr, Signal>(init, attrUpdate, update, view, diffAttrs)

