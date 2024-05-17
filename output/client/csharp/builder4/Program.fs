open System
open BuilderNode
open Org.Whatever.QtTesting
open Reactor
open Widgets
open Widgets.Menus

type Msg =
    | ExitTriggered
    | ToggleEdit
    | EditChanged of string
    | EditActivated
    | AddButton
    | ExtraPush of int
    | ComboChanged of maybeValue: int option
    
type State = {
    EditEnabled: bool
    TextValue: string
    LastActivated: string option
    ExtraButtonCount: int
    ComboSelection: int option
}

let init() =
    { EditEnabled = true
      TextValue = ""
      LastActivated = None
      ExtraButtonCount = 0
      ComboSelection = None }

let update (state: State) (msg: Msg): State =
    match msg with
    | ExitTriggered ->
        printfn "EXITED!!"
        state
    | EditChanged str ->
        { state with TextValue = str }
    | EditActivated ->
        { state with LastActivated = Some state.TextValue }
    | ToggleEdit ->
        { state with EditEnabled = not state.EditEnabled }
    | AddButton ->
        { state with ExtraButtonCount = state.ExtraButtonCount + 1 }
    | ExtraPush i ->
        printfn "hello from extra button %02d" i
        state
    | ComboChanged maybeValue ->
        let nextTextValue =
            match maybeValue with
            | Some index ->
                sprintf "you selected %d" index
            | None ->
                state.TextValue
        { state with ComboSelection = maybeValue; TextValue = nextTextValue }
    
let view (state: State) =
    let edit =
        LineEdit.Node(Attrs = [LineEdit.Value state.TextValue; LineEdit.Enabled state.EditEnabled], OnChanged = EditChanged, OnActivated = EditActivated)
    let disableButton =
        let label =
            if state.EditEnabled then
                "disable text box"
            else
                "enable text box"
        Button.make label ToggleEdit
    let addButton =
        let enabled =
            state.ExtraButtonCount < 5
        Button.Node(Attrs = [Button.Label "Add"; Button.Enabled enabled], OnClicked = AddButton)
    let extraButtons =
        if state.ExtraButtonCount > 0 then
            seq {
                for i in 1..state.ExtraButtonCount do
                    let label =
                        sprintf "extra button %02d" i
                    Button.make label (ExtraPush i) :> WidgetNode<Msg>
            } |> Seq.toList
        else
            []
    let combo =
        let items =
            ["item 01"; "item 02"; "item03"]
        ComboBox.Node(Attrs = [ComboBox.Items items; ComboBox.SelectedIndex (Some 0)], OnSelected = ComboChanged)
    let box =
        BoxLayout.Node(
            Attrs = [BoxLayout.Direction BoxLayout.Vertical; BoxLayout.Spacing 10],
            Items = [ edit; disableButton; combo; addButton ] @ extraButtons)
    let title =
        match state.LastActivated with
        | Some value -> sprintf "last activation [%s]" value
        | None -> "..."
    let menuBar =
        let fileMenu =
            let items = [
                Action.Node(Attrs = [Action.Text "- nothing -"]) :> ActionNode<Msg>
                Action.Node(Attrs = [Action.Text "E&xit"], OnTriggered = (fun _ -> ExitTriggered))
            ]
            Menu.Node(Attrs = [Menu.Title "&File"], Items = items)
        MenuBar.Node(Menus = [ fileMenu ])
    let window = 
        Window.Node(
            Attrs = [Window.Title title; Window.Size (800, 600); Window.Visible true],
            MenuBar = menuBar,
            Content = box)
    window :> BuilderNode<Msg>
    
let innerApp (argv: string array) =
    use app =
        Application.Create(argv)
    Application.SetStyle("Fusion")
    use reactor =
        // this binding will keep reactor alive for the entire lifetime of this method (until Application.Exec() exits), correct??
        // because the connection between what's going on in Reactor, and the state of Qt is completely implicit / behind-the-scenes
        // if it got garbage-collected or something that would be bad news :)
        new Reactor<State,Msg>(init, update, view)
    Application.Exec()

[<EntryPoint>]
[<STAThread>]
let main argv =
    Library.Init()
    let code = innerApp argv
    Library.DumpTables()
    Library.Shutdown()
    code
