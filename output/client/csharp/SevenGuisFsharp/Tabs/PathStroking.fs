module Tabs.PathStroking

open FSharpQt.BuilderNode
open FSharpQt.Reactor
open FSharpQt.Widgets.BoxLayout
open FSharpQt.Widgets.GroupBox
open FSharpQt.Widgets.PushButton
open FSharpQt.Widgets.RadioButton
open FSharpQt.Widgets.Slider

type Signal = unit
type Attr = unit

type CapStyle =
    | Flat
    | Square
    | Round
    
type JoinStyle =
    | Bevel
    | Miter
    | SvgMiter
    | Round
    
type PenStyle =
    | Solid
    | Dash
    | Dot
    | DashDot
    | DashDotDot
    | CustomDash
    
type LineStyle =
    | Curves
    | Lines

type State = {
    CapStyle: CapStyle
    JoinStyle: JoinStyle
    PenStyle: PenStyle
    PenWidth: int
    LineStyle: LineStyle
    Animating: bool
}

type Msg =
    | SetCapStyle of style: CapStyle
    | SetJoinStyle of style: JoinStyle
    | SetPenStyle of style: PenStyle
    | SetPenWidth of width: int
    | SetLineStyle of style: LineStyle
    | SetAnimateState of value: bool

let init() =
    let state = {
        CapStyle = Flat
        JoinStyle = Bevel
        PenStyle = Solid
        PenWidth = 50
        LineStyle = Curves
        Animating = true
    }
    state, Cmd.None

let update (state: State) (msg: Msg) =
    let nextState =
        match msg with
        | SetCapStyle style ->
            { state with CapStyle = style }
        | SetJoinStyle style ->
            { state with JoinStyle = style }
        | SetPenStyle style ->
            { state with PenStyle = style }
        | SetPenWidth width ->
            { state with PenWidth = width }
        | SetLineStyle style ->
            { state with LineStyle = style }
        | SetAnimateState value ->
            { state with Animating = value }
    nextState, Cmd.None
    
let radioGroup (title: string) (items: (string * 'a) list) (setterMsgFunc: 'a -> Msg) (selected: 'a) =
    let buttons =
        items
        |> List.map (fun (label, value) ->
            let attrs =
                [ Text label; if value = selected then Checked true ]
            RadioButton(Attrs = attrs, OnClicked = setterMsgFunc value))
    let vbox =
        let items =
            buttons
            |> List.map BoxItem.Create
        BoxLayout(Attrs = [ Direction TopToBottom ], Items = items)
    GroupBox(Attrs = [ Title title ], Layout = vbox)
        
    
let view (state: State) =
    let capStyleGroup =
        let items =
            [ "Flat", Flat
              "Square", Square
              "Round", CapStyle.Round ]
        radioGroup "Cap Style" items SetCapStyle state.CapStyle
        
    let joinStyleGroup =
        let items =
            [ "Bevel", Bevel
              "Miter", Miter
              "SvgMiter", SvgMiter
              "Round", JoinStyle.Round ]
        radioGroup "Join Style" items SetJoinStyle state.JoinStyle
        
    let penStyleGroup =
        let items =
            [ "Solid", Solid
              "Dash", Dash
              "Dot", Dot
              "DashDot", DashDot
              "DashDotDot", DashDotDot
              "Custom", CustomDash ]
        radioGroup "Join Style" items SetPenStyle state.PenStyle
        
    let penWidthGroup =
        let slider =
            Slider(Attrs = [ Orientation Horizontal; Range (0, 500) ], OnValueChanged = SetPenWidth)
        let vbox =
            BoxLayout(Attrs = [ Direction TopToBottom ], Items = [ BoxItem.Create(slider) ])
        GroupBox(Attrs = [ Title "Pen Width" ], Layout = vbox)
        
    let lineStyleGroup =
        let items =
            [ "Curves", Curves
              "Lines", Lines ]
        radioGroup "Line Style" items SetLineStyle state.LineStyle
        
    let animateButton =
        PushButton(
            Attrs = [
                Checkable true
                FSharpQt.Widgets.PushButton.Text (if state.Animating then "Animating" else "Not Animating")
                FSharpQt.Widgets.PushButton.Checked state.Animating
            ], OnClickedWithState = SetAnimateState)
        
    BoxLayout(Attrs = [ Direction TopToBottom ],
              Items = [
                  BoxItem.Create(capStyleGroup)
                  BoxItem.Create(joinStyleGroup)
                  BoxItem.Create(penStyleGroup)
                  BoxItem.Create(penWidthGroup)
                  BoxItem.Create(lineStyleGroup)
                  BoxItem.Create(animateButton)
                  BoxItem.Stretch 1
              ])
    :> ILayoutNode<Msg>

type PathStroking<'outerMsg>() =
    inherit LayoutReactorNode<'outerMsg, State, Msg, Attr, Signal>(init, update, view)
