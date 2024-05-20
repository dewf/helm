open System
open BuilderNode
open Org.Whatever.QtTesting
open Reactor
open Widgets
open Widgets.Menus

type Msg =
    | ExitAction
    
type State = {
    Placeholder: int
}

let init () =
    let nextState =
        { Placeholder = 0 }
    nextState, Cmd.None

let update (state: State) (msg: Msg) =
    match msg with
    | ExitAction ->
        state, Cmd.QuitApplication
    
let view (state: State) =
    // TODO: detect when a given node has been attached to 2+ places in a single graph
    // since nodes are stateful, this would no doubt cause havoc
    let menuBar() =
        MenuBar.Node(Menus = [
            Menu.Node(Attrs = [
                    Menu.Title "&File"
                ], Items = [
                    Action.Node(Attrs = [ Action.Text "E&xit" ], OnTriggered = (fun _ -> ExitAction))
                ])
            ])
    let window01 =
        MainWindow.Node(
            Attrs = [
                MainWindow.Title "Window 01"
                MainWindow.Size (800, 600)
                MainWindow.Visible true
            ], MenuBar = menuBar())
    let window02 =
        MainWindow.Node(
            Attrs = [
                MainWindow.Title "Window 02"
                MainWindow.Size (800, 600)
                MainWindow.Visible true
            ], MenuBar = menuBar())
    let window03 =
        MainWindow.Node(
            Attrs = [
                MainWindow.Title "Window 03"
                MainWindow.Size (800, 600)
                MainWindow.Visible true
            ], MenuBar = menuBar())
    WindowSet.Node(
        Windows = [
            StrKey "one", window01
            StrKey "two", window02
            StrKey "three", window03
        ]) :> BuilderNode<Msg>
    
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
