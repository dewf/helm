module Tabs.Counter

open BuilderNode
open Reactor
open Widgets

type Msg =
    | Increment

type State = {
    Count: int
}

let init () =
    { Count = 0 }, SubCmd.None
    
let update (state: State) (msg: Msg) =
    match msg with
    | Increment ->
        { state with Count = state.Count + 1 }, SubCmd.None
        
let view (state: State) =
    let label =
        Label.Node(Attrs = [ Label.Text $"Count: {state.Count}" ])
    let button =
        PushButton.Node(Attrs = [ PushButton.Label "Increment" ], OnClicked = Increment)
    BoxLayout.Node(
        Attrs = [ BoxLayout.Direction BoxLayout.Horizontal ],
        Items = [ label; button ])
    :> ILayoutNode<Msg>

type Node<'outerMsg>() =
    inherit LayoutReactorNode<'outerMsg, State, Msg, unit, unit>(init, nullAttrUpdate, update, view, nullDiffAttrs)
