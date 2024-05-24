module Tabs.TempConverter

open BuilderNode
open Reactor
open Widgets

type Msg =
    | SetCelsius of text: string
    | SetFahrenheit of text: string

type State = {
    CelsiusText: string
    FahrenText: string
} with
    static member Init = {
        CelsiusText = ""
        FahrenText = ""
    }

let init () =
    State.Init, SubCmd.None
    
let tryParseFloat (str: string) =
    match System.Single.TryParse str with
    | true, value ->
        Some (float value)
    | _ ->
        None
        
let update (state: State) (msg: Msg) =
    match msg with
    | SetCelsius text ->
        let nextState =
            if text.Length > 0 then
                let cValue =
                    tryParseFloat text
                let fText =
                    match cValue with
                    | Some celsius ->
                        let value =
                            (celsius * 9.0) / 5.0 + 32.0
                        sprintf "%.2f" value
                    | None ->
                        "<- invalid"
                { state with
                    CelsiusText = text
                    FahrenText = fText }
            else
                State.Init
        nextState, SubCmd.None
                
    | SetFahrenheit text ->
        let nextState =
            if text.Length > 0 then
                let fValue =
                    tryParseFloat text
                let cText =
                    match fValue with
                    | Some fahren ->
                        let value =
                            (fahren - 32.0) * 5.0 / 9.0
                        sprintf "%.2f" value
                    | None ->
                        "invalid ->"
                { state with
                    CelsiusText = cText
                    FahrenText = text }
            else
                State.Init
        nextState, SubCmd.None
        
let view (state: State) =
    let celsiusText =
        LineEdit.Node(Attrs = [ LineEdit.Value state.CelsiusText ], OnChanged = SetCelsius)
    let celsiusLabel =
        Label.Node(Attrs = [ Label.Text "Celsius = " ])
    let fahrenText =
        LineEdit.Node(Attrs = [ LineEdit.Value state.FahrenText ], OnChanged = SetFahrenheit)
    let fahrenLabel =
        Label.Node(Attrs = [ Label.Text "Fahrenheit" ])
    BoxLayout.Node(
        Attrs = [ BoxLayout.Direction BoxLayout.Horizontal ],
        Items = [ celsiusText; celsiusLabel; fahrenText; fahrenLabel ])
    :> ILayoutNode<Msg>

type Node<'outerMsg>() =
    inherit LayoutReactorNode<'outerMsg, State, Msg, unit, unit>(init, nullAttrUpdate, update, view, nullDiffAttrs)
