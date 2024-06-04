module Program

open System

open FSharpQt
open BuilderNode
open Reactor
open FSharpQt.Widgets.WindowSet
open FSharpQt.Widgets
open BoxLayout
open Label
open PushButton
open MainWindow

open Tabs.Counter
open Tabs.DropTesting
open Tabs.TempConverter
open Tabs.FlightBooker
open Tabs.TimerPage
open Tabs.CRUD
open Tabs.CircleDrawer

open Org.Whatever.QtTesting

[<RequireQualifiedAccess>]
type GuiKind =
    // 7guis
    | Counter
    | TempConverter
    | FlightBooker
    | TimerPage
    | CRUD
    | CircleDrawer
    | Spreadsheet
    // misc
    | DropTesting
    
type GuiInstance = {
    Key: int
    Kind: GuiKind
}

type State = {
    NextKey: int
    Instances: GuiInstance list
}

type Msg =
    | MainWindowClosed
    | LaunchInstance of kind: GuiKind
    | InstanceClosed of key: int

let init () =
    let nextState = {
        NextKey = 1
        Instances = []
    }
    nextState, Cmd.None

let update (state: State) (msg: Msg) =
    match msg with
    | MainWindowClosed ->
        state, Cmd.Signal QuitApplication
    | LaunchInstance kind ->
        let nextState =
            let nextInstances =
                let instance =
                    { Key = state.NextKey; Kind = kind }
                instance :: state.Instances
            { state with Instances = nextInstances; NextKey = state.NextKey + 1 }
        nextState, Cmd.None
    | InstanceClosed key ->
        let nextState =
            let nextInstances =
                let index =
                    state.Instances
                    |> List.findIndex (fun inst -> inst.Key = key)
                state.Instances
                |> List.removeAt index
            { state with Instances = nextInstances }
        nextState, Cmd.None
    
let view (state: State) =
    let topLabel =
        Label(Attrs = [ Label.Text "7GUIs"; Alignment Left ]) :> IWidgetNode<Msg>
        
    let topButtons =
        let buttons =
            [ "Counter", GuiKind.Counter
              "TempConv", GuiKind.TempConverter
              "FlightBooker", GuiKind.FlightBooker
              "Timer", GuiKind.TimerPage
              "CRUD", GuiKind.CRUD
              "CircleDrawer", GuiKind.CircleDrawer
              "Spreadsheet", GuiKind.Spreadsheet ]
            |> List.map (fun (name, kind) ->
                let enabled =
                    kind <> GuiKind.Spreadsheet
                PushButton(Attrs = [ Text name; Enabled enabled ], OnClicked = LaunchInstance kind))
        let items =
            buttons
            |> List.map BoxItem.Create
        BoxLayout(Attrs = [ Direction TopToBottom; ContentsMargins (0, 0, 0, 0) ], Items = items)
        
    let bottomLabel =
        Label(Attrs = [ Label.Text "Misc"; Alignment Left ])
        
    let bottomButtons =
        let buttons =
            [ "DropTesting", GuiKind.DropTesting ]
            |> List.map (fun (name, kind) ->
                PushButton(Attrs = [ Text name ], OnClicked = LaunchInstance kind))
        let items =
            buttons
            |> List.map BoxItem.Create
        BoxLayout(Attrs = [ Direction TopToBottom; ContentsMargins (0, 0, 0, 0) ], Items = items)
        
    let vbox =
        let items = [
            // listed explicitly (vs. List.map) to avoid casting to :> IWidgetNode above
            BoxItem.Create(topLabel)
            BoxItem.Create(topButtons)
            BoxItem.Create(bottomLabel)
            BoxItem.Create(bottomButtons)
        ]
        BoxLayout(Attrs = [ Direction TopToBottom ], Items = items)
        
    let mainWindow =
        let window =
            MainWindow(
                Attrs = [ Title "7GUIs in F#/Qt" ],
                Content = vbox,
                OnClosed = MainWindowClosed)
        IntKey 0, window :> IWindowNode<Msg>
        
    let instanceWindows =
        state.Instances
        |> List.map (fun inst ->
            let title, node =
                match inst.Kind with
                | GuiKind.Counter -> "Counter", Counter() :> ILayoutNode<Msg>
                | GuiKind.TempConverter -> "Temperature Converter", TempConverter()
                | GuiKind.FlightBooker -> "Flight Booker", FlightBooker()
                | GuiKind.TimerPage -> "Timer", TimerPage()
                | GuiKind.CRUD -> "CRUD", CRUDPage()
                | GuiKind.CircleDrawer -> "Circle Drawer", CircleDrawer()
                | GuiKind.Spreadsheet ->
                    failwith "not yet implemented"
                | GuiKind.DropTesting -> "Drop Testing", DropTesting()
            let window =
                MainWindow(Attrs = [ Title title ], Content = node, OnClosed = InstanceClosed inst.Key)
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
