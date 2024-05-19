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
    AvailableStyles: string list
    EditEnabled: bool
    TextValue: string
    LastActivated: string option
    ExtraButtonCount: int
    ComboSelection: int option
}

let init () =
    { AvailableStyles = Application.AvailableStyles() |> Array.toList
      EditEnabled = true
      TextValue = ""
      LastActivated = None
      ExtraButtonCount = 0
      ComboSelection = None }, Cmd.Noop

let update (state: State) (msg: Msg) =
    match msg with
    | ExitTriggered ->
        state, Cmd.QuitApplication
    | EditChanged str ->
        { state with TextValue = str }, Cmd.Noop
    | EditActivated ->
        { state with LastActivated = Some state.TextValue }, Cmd.Noop
    | ToggleEdit ->
        { state with EditEnabled = not state.EditEnabled }, Cmd.Noop
    | AddButton ->
        { state with ExtraButtonCount = state.ExtraButtonCount + 1 }, Cmd.Noop
    | ExtraPush i ->
        printfn "hello from extra button %02d" i
        state, Cmd.Noop
    | ComboChanged maybeValue ->
        let nextTextValue =
            match maybeValue with
            | Some index ->
                sprintf "you selected %d" index
            | None ->
                state.TextValue
        { state with ComboSelection = maybeValue; TextValue = nextTextValue }, Cmd.Noop
    
let view (state: State) =
    let tabs =
        let page1 =
            PushButton.Node(Attrs = [PushButton.Label "PAGE 01"])
        let page2 =
            PushButton.Node(Attrs = [PushButton.Label "PAGE 02"])
        let page3 =
            let edit =
                LineEdit.Node()
            let list =
                ListWidget.Node(Attrs = [
                    let items =
                        [ for i in 0 .. 99 -> $"%02d{i}" ]
                    ListWidget.Items items
                ])
            BoxLayout.Node(Attrs = [BoxLayout.Direction BoxLayout.Vertical],
                           Items = [edit; list])
        TabWidget.Node(Pages = [
            "Page 1", page1
            "Page 2", page2
            "Page 3", page3
        ])
    let menuBar =
        let fileMenu =
            let items = [
                Action.Node(Attrs = [Action.Text "- nothing -"]) :> ActionNode<Msg>
                Action.Node(Attrs = [Action.Separator true])
                Action.Node(Attrs = [Action.Text "E&xit"], OnTriggered = (fun _ -> ExitTriggered))
            ]
            Menu.Node(Attrs = [Menu.Title "&File"], Items = items)
        MenuBar.Node(Menus = [ fileMenu ])
    let window = 
        MainWindow.Node(
            Attrs = [MainWindow.Title "Very Nice!"; MainWindow.Visible true], // MainWindow.Size (800, 600);
            MenuBar = menuBar,
            Content = tabs) // PushButton.Node(Attrs = [PushButton.Label "nice"])
    window :> BuilderNode<Msg>
    
let innerApp (argv: string array) =
    use app =
        Application.Create(argv)
    Application.SetStyle("Fusion")
    // Application.SetStyle("Windows")
    let rec processCmd = function
        | Noop ->
            ()
        | QuitApplication ->
            Application.Quit()
        | Batch commands ->
            commands
            |> List.iter processCmd
    use reactor =
        // this binding will keep reactor alive for the entire lifetime of this method (until Application.Exec() exits), correct??
        // because the connection between what's going on in Reactor, and the state of Qt is completely implicit / behind-the-scenes
        // if it got garbage-collected or something that would be bad news :)
        new Reactor<State,Msg>(init, update, view, processCmd)
    Application.Exec()

[<EntryPoint>]
[<STAThread>]
let main argv =
    Library.Init()
    let code = innerApp argv
    Library.DumpTables()
    Library.Shutdown()
    code
