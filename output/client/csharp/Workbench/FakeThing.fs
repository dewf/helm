module FakeThing

open FSharpQt.BuilderNode
open FSharpQt.Reactor
open FSharpQt.Widgets.PushButton

type Signal = unit

type Attr =
    | Suffix of text: string
    
let attrKey = function
    | Suffix _ -> 0

type State = {
    Count: int
    Suffix: string option
}

type Msg =
    | DoSomething

let init() =
    { Count = 0; Suffix = None }, Cmd.None
    
let attrUpdate (state: State) (attr: Attr) =
    match attr with
    | Suffix text ->
        { state with Suffix = Some text }
        
let update (state: State) (msg: Msg) =
    match msg with
    | DoSomething ->
        { state with Count = state.Count + 1 }, Cmd.None

let view (state: State) =
    let suffixText =
        match state.Suffix with
        | Some suffix -> sprintf " suffix: <%s>" suffix
        | None -> ""
    PushButton(Text = $"FAKE THING BUTTON [{state.Count}]{suffixText}", OnClicked = DoSomething)
    :> IWidgetNode<Msg>
    

type FakeThing<'outerMsg>() =
    inherit WidgetReactorNode<'outerMsg,State,Msg,Signal>(init, update, view)
    
    member this.Attrs with set value =
        this.PushAttr(EasyAttrs(value, "fakethingattrs", attrKey, attrUpdate))
