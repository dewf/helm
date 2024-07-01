module FakeThing

open FSharpQt
open FSharpQt.Attrs
open FSharpQt.BuilderNode
open FSharpQt.Reactor
open FSharpQt.Widgets.PushButton

type Signal = unit

type Attr =
    | Suffix of text: string
    
let private attrKey = function
    | Suffix _ -> 0

type State = {
    Count: int
    Suffix: string option
}

let private attrUpdate (state: State) (attr: Attr) =
    match attr with
    | Suffix text ->
        { state with Suffix = Some text }

type Msg =
    | DoSomething

let init() =
    { Count = 0; Suffix = None }, Cmd.None

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
    
type EasyAttrs(values: Attr list, uniqueKey: string, keyFunc: Attr -> int, attrUpdate: State -> Attr -> State) =
    member val Values = values
    interface IAttr with
        override this.AttrEquals other =
            match other with
            | :? EasyAttrs as otherAttr ->
                values = otherAttr.Values
            | _ ->
                false
        override this.Key =
            sprintf "easyattrs:%s" uniqueKey
        override this.ApplyTo(target: IAttrTarget, maybePrev: IAttr option) =
            match target with
            | :? ComponentStateTarget<State> as componentTarget ->
                let changedAttrs =
                    match maybePrev with
                    | Some prevIAttr ->
                        match prevIAttr with
                        | :? EasyAttrs as prev ->
                            (genericDiffAttrs keyFunc) prev.Values values
                            |> BuilderNode.createdOrChanged
                        | _ ->
                            failwith "EasyAttrs previous value type match error - did you use a truly unique key?"
                    | None ->
                        // no previous values, use them all
                        values
                let nextState =
                    (componentTarget.State, changedAttrs)
                    ||> List.fold attrUpdate
                componentTarget.Update(nextState)
            | _ ->
                failwith "nope"

type FakeThing<'outerMsg>() =
    inherit WidgetReactorNode<'outerMsg,State,Msg,Signal>(init, update, view)
    
    member this.Attrs2 with set value =
        this.PushAttr(EasyAttrs(value, "fakethingattrs", attrKey, attrUpdate))
