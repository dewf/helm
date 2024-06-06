module Tabs.PathStroking.PathStroking

open FSharpQt
open FSharpQt.BuilderNode
open FSharpQt.Reactor
open FSharpQt.Widgets.BoxLayout
open FSharpQt.Widgets.GroupBox
open FSharpQt.Widgets.PushButton
open FSharpQt.Widgets.RadioButton
open FSharpQt.Widgets.Slider

open Painting
open Renderer

type Signal = unit
type Attr = unit

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
    | SetAnimating of value: bool

let init() =
    let state = {
        CapStyle = Flat
        JoinStyle = Bevel
        PenStyle = SolidLine
        PenWidth = 2
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
        | SetAnimating value ->
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
            [ "Solid", SolidLine
              "Dash", DashLine
              "Dot", DotLine
              "DashDot", DashDotLine
              "DashDotDot", DashDotDotLine
              "Custom", CustomDashLine ]
        radioGroup "Join Style" items SetPenStyle state.PenStyle
        
    let penWidthGroup =
        let slider =
            Slider(
                Attrs = [
                    Orientation Horizontal
                    Range (1, 20)
                    Value state.PenWidth
                ], OnValueChanged = SetPenWidth)
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
            ], OnClickedWithState = SetAnimating)
        
    let rightPanel =
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
    let renderer =
        PathStrokeRenderer(
            Attrs = [
                CapStyle state.CapStyle
                JoinStyle state.JoinStyle
                PenStyle state.PenStyle
                PenWidth state.PenWidth
                LineStyle state.LineStyle
                Animating state.Animating
            ])
    
    BoxLayout(Attrs = [ Direction LeftToRight ],
              Items = [
                  BoxItem.Create(renderer, 1)
                  BoxItem.Create(rightPanel)
              ])
    :> ILayoutNode<Msg>

type PathStroking<'outerMsg>() =
    inherit LayoutReactorNode<'outerMsg, State, Msg, Attr, Signal>(init, update, view)
