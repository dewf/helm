module Program

open System

open FSharpQt
open BuilderNode
open AppReactor
open FSharpQt.Widgets.WindowSet
open Tabs.CircleDrawer
open FSharpQt.Widgets
open BoxLayout
open PushButton
open MainWindow
open Tabs.Counter
open Tabs.TempConverter
open Tabs.FlightBooker
open Tabs.TimerPage
open Tabs.CRUD

open Org.Whatever.QtTesting

type GuiKind =
    | Counter2
    | TempConverter2
    | FlightBooker2
    | TimerPage2
    | CRUD2
    | CircleDrawer2
    
type GuiInstance = {
    Key: int
    Kind: GuiKind
}

type State = {
    NextKey: int
    Instances: GuiInstance list
}

type Msg =
    | OpenDialog
    | AcceptDialog
    | DialogClosed of accepted: bool
    | LaunchInstance of kind: GuiKind

let init () =
    let nextState = {
        NextKey = 1
        Instances = []
    }
    nextState, Cmd.None

let update (state: State) (msg: Msg) =
    match msg with
    | OpenDialog ->
        state, Cmd.DialogOp ("testing", Exec)
    | AcceptDialog ->
        state, Cmd.DialogOp ("testing", Accept)
    | DialogClosed accepted ->
        printfn "dlg accepted: [%A]" accepted
        state, Cmd.None
    | LaunchInstance kind ->
        let nextState =
            let nextInstances =
                let instance =
                    { Key = state.NextKey; Kind = kind }
                instance :: state.Instances
            { state with Instances = nextInstances; NextKey = state.NextKey + 1 }
        nextState, Cmd.None
    
let view (state: State) =
    let pairs = [
        "Counter", Counter2
        "TempConv", TempConverter2
        "FlightBooker", FlightBooker2
        "Timer", TimerPage2
        "CRUD", CRUD2
        "CircleDrawer", CircleDrawer2
    ]
    let buttons =
        pairs
        |> List.map (fun (name, kind) ->
            PushButton(Attrs = [ Text name ], OnClicked = LaunchInstance kind))
    let vbox =
        let items =
            buttons
            |> List.map BoxItem.Create
        BoxLayout(Attrs = [ Direction TopToBottom ], Items = items)
    let mainWindow =
        let window =
            // TODO: close entire app on main window close
            MainWindow(
                Attrs = [ Title "7GUIs in F#/Qt" ],
                Content = vbox)
        IntKey 0, window :> IWindowNode<Msg>
    let instanceWindows =
        state.Instances
        |> List.map (fun inst ->
            let title, node =
                match inst.Kind with
                | Counter2 -> "Counter", Counter() :> ILayoutNode<Msg>
                | TempConverter2 -> "Temperature Converter", TempConverter()
                | FlightBooker2 -> "Flight Booker", FlightBooker()
                | TimerPage2 -> "Timer", TimerPage()
                | CRUD2 -> "CRUD", CRUDPage()
                | CircleDrawer2 -> "Circle Drawer", CircleDrawer()
            let window =
                // TODO: close/hide event needs to remove window from instance list
                MainWindow(Attrs = [ Title title ], Content = node)
            IntKey inst.Key, window :> IWindowNode<Msg>)
    WindowSet(Windows = mainWindow :: instanceWindows)
    :> IBuilderNode<Msg>
    
[<EntryPoint>]
[<STAThread>]
let main argv =
    // initialize NativeImpl library
    Library.Init()
    // begin F#Qt proper
    let exitCode =
        // notice the 'use' inside of here, because we want it to .Dispose the AppReactor when leaving this block/binding
        use app =
            createApplication init update view
        app.Run argv
    // NativeImpl shutdown
    Library.DumpTables()
    Library.Shutdown()
    exitCode
