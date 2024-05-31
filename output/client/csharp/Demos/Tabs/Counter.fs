module Tabs.Counter

open BuilderNode
open SubReactor
open Widgets
open BoxLayout

type Attr = unit
type Signal = unit

type State = {
    Count: int
}

type Msg =
    | Increment

let init () =
    { Count = 0 }, Cmd.None
    
let update (state: State) (msg: Msg) =
    match msg with
    | Increment ->
        { state with Count = state.Count + 1 }, Cmd.None
        
let view (state: State) =
    let label =
        Label.Node(Attrs = [
            Label.Text $"Count: {state.Count}"
            Label.Alignment Label.Center
        ])
    let button =
        PushButton.Node(Attrs = [ PushButton.Label "Increment" ], OnClicked = Increment)
    BoxLayout(
        Attrs = [ Direction Horizontal ],
        Items = [
            BoxItem.Create(label)
            BoxItem.Create(button)
        ])
    :> ILayoutNode<Msg>

type Node<'outerMsg>() =
    inherit LayoutReactorNode<'outerMsg, State, Msg, Attr, Signal>(init, nullAttrUpdate, update, view, nullDiffAttrs)
