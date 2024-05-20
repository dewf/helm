open System
open BuilderNode
open Org.Whatever.QtTesting
open Reactor
open Widgets
open Widgets.Menus

type Msg =
    | CoolHappening of string
    | ExitTriggered
    | EditChanged of string
    | EditActivated
    
type State = {
    EditValue: string
    ButtonLabel: string
    ActivationCount: int
}

let init () =
    { EditValue = ""
      ButtonLabel = "Init Label"
      ActivationCount = 0
      }, Cmd.None

let update (state: State) (msg: Msg) =
    match msg with
    | CoolHappening str ->
        printfn "Cool happening!!! [%s]" str
        state, Cmd.None
    | ExitTriggered ->
        state, Cmd.QuitApplication
    | EditChanged str ->
        { state with EditValue = str }, Cmd.None
    | EditActivated ->
        { state with
            ButtonLabel = state.EditValue
            EditValue = ""
            ActivationCount = state.ActivationCount + 1 }, Cmd.None
    
let view (state: State) =
    let tabs =
        let page1 =
            PushButton.Node(Attrs = [PushButton.Label "PAGE 01"])
        let page2 =
            PushButton.Node(Attrs = [PushButton.Label "PAGE 02"])
        let page3 =
            let edit =
                LineEdit.Node(
                    Attrs = [
                        LineEdit.Value state.EditValue
                    ],
                    OnChanged = EditChanged,
                    OnReturnPressed = EditActivated)
            let list =
                ListWidget.Node(Attrs = [
                    let items =
                        [ for i in 0 .. 99 -> $"%02d{i}" ]
                    ListWidget.Items items
                ])
            BoxLayout.Node(Attrs = [BoxLayout.Direction BoxLayout.Vertical],
                           Items = [edit; list])
        let page4 =
            let label =
                sprintf "%s - %02d" state.ButtonLabel state.ActivationCount
            CoolPanel.Node(Attrs = [CoolPanel.ButtonLabel label], OnSomethingHappened = CoolHappening)
        TabWidget.Node(Pages = [
            "Page 1", page1
            "Page 2", page2
            "Page 3", page3
            "Page 4", page4
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
        | Cmd.None ->
            ()
        | Cmd.OfMsg msg ->
            // how about moving all this stuff into an 'AppReactor' class?
            printfn "!!! root level doesn't yet support Cmd.OfMsg, need to reorganize some things"
        | Cmd.QuitApplication ->
            Application.Quit()
        | Cmd.Batch commands ->
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
