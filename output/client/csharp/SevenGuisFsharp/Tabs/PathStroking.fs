module Tabs.PathStroking

open FSharpQt.BuilderNode
open FSharpQt.Reactor
open FSharpQt.Widgets.BoxLayout
open FSharpQt.Widgets.PushButton

type Signal = unit
type Attr = unit

type State = {
    NothingYet: int
}

type Msg = unit

let init() =
    { NothingYet = 0 }, Cmd.None

let update (state: State) (msg: Msg) =
    state, Cmd.None
    
let view (state: State) =
    let button =
        PushButton(Attrs = [ Text "Woot" ])
    BoxLayout(Attrs = [ Direction TopToBottom ], Items = [ BoxItem.Create(button) ])
    :> ILayoutNode<Msg>

type PathStroking<'outerMsg>() =
    inherit LayoutReactorNode<'outerMsg, State, Msg, Attr, Signal>(init, update, view)
