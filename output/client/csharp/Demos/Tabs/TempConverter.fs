module Tabs.TempConverter

open BuilderNode
open SubReactor
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
    State.Init, Cmd.None
    
let tryParseFloat (str: string) =
    match System.Single.TryParse str with
    | true, value ->
        Some (float value)
    | _ ->
        None
        
let celsiusToFahren (cText: string) =
    if cText.Length > 0 then
        match tryParseFloat cText with
        | Some celsius ->
            let fahren =
                (celsius * 9.0) / 5.0 + 32.0
            sprintf "%.2f" fahren
        | None ->
            "<- invalid"
    else
        ""
        
let fahrenToCelsius (fText: string) =
    if fText.Length > 0 then
        match tryParseFloat fText with
        | Some fahren ->
            let celsius =
                (fahren - 32.0) * 5.0 / 9.0
            sprintf "%.2f" celsius
        | None ->
            "invalid ->"
    else
        ""
        
let update (state: State) (msg: Msg) =
    match msg with
    | SetCelsius text ->
        let nextState =
            { state with CelsiusText = text; FahrenText = celsiusToFahren text }
        nextState, Cmd.None
                
    | SetFahrenheit text ->
        let nextState =
            { state with FahrenText = text; CelsiusText = fahrenToCelsius text }
        nextState, Cmd.None
        
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
