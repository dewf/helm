open System
open BuilderNode
open Org.Whatever.QtTesting
open Reactor
open Widgets
open Widgets.Menus
open type PaintResources // saves some typing in .DoPoint - pens, colors, brushes, etc

type Msg =
    | MouseDown of ev: Widget.MouseEvent
    | ExitAction

type State = {
    RectPositions: Common.Point list
}

let init () =
    let nextState = {
        RectPositions = []
    }
    nextState, Cmd.None

let update (state: State) (msg: Msg) =
    match msg with
    | ExitAction ->
        state, Cmd.QuitApplication
    | MouseDown ev ->
        let nextPositions =
            ev.Pos :: state.RectPositions
        let nextState =
            { state with RectPositions = nextPositions }
        nextState, Cmd.None
        
type PaintState(state: State) =
    inherit CustomWidget.PaintStateBase<State>(state)
    override this.StateEquals other =
        // optional override:
        // here we can consider only a subset of the total state, if we like (when deciding whether a redraw is necessary at all - this occurs during Attr diff phase)
        // default implementation compares the entire states
        state.RectPositions = other.RectPositions
    override this.ComputeUpdateArea prev =
        // optional override:
        // compare previous/this states to determine what changed and what areas need updating
        // or just redraw everything!
        CustomWidget.Everything
    override this.DoPaint widget painter rect =
        // required:
        // perform actual painting here with QPainter methods and resources (brushes, pens, colors, gradients, etc)
        // 'rect' param will probably become a Qt region or something more advanced eventually
        use black = Color.Create(Color.Constant.Black)
        use green = Color.Create(Color.Constant.Green)
        use pen = Pen.Create(green)
        painter.FillRect(widget.GetRect(), black)
        painter.SetPen(pen)
        for pos in state.RectPositions do
            painter.DrawRect(Common.Rect(X = pos.X - 20, Y = pos.Y - 20, Width = 40, Height = 40))
    
let view (state: State) =
    let custom =
        CustomWidget.Node(
            Attrs = [ CustomWidget.PaintState (PaintState state)],
            OnMousePress = MouseDown)
    let menuBar =
        MenuBar.Node(Menus = [
            Menu.Node(
                Attrs = [
                    Menu.Title "&File"
                ], Items = [
                    Action.Node(Attrs = [ Action.Text "E&xit" ], OnTriggered = (fun _ -> ExitAction))
                ])
            ])
    MainWindow.Node(
        Attrs = [ MainWindow.Title "Paint Testing!"; MainWindow.Size(800, 600) ],
        Content = custom,
        MenuBar = menuBar) :> IBuilderNode<Msg>
    
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
    let resultCode = innerApp argv
    Library.DumpTables()
    Library.Shutdown()
    resultCode
