module Program

open System

open FSharpQt
open BuilderNode
open Reactor

open FSharpQt.Widgets
open FSharpQt.WithDialogs

open MainWindow
open PushButton
open BoxLayout
open FileDialog

type State = {
    NothingYet: int
}

type Msg =
    | ShowDialog
    | FileSelected of file: string

let init () =
    let nextState = {
        NothingYet = 0
    }
    nextState, Cmd.None

let update (state: State) (msg: Msg) =
    match msg with
    | ShowDialog ->
        state, Cmd.Dialog ("open", Exec)
    | FileSelected file ->
        printfn "file: %A" file
        state, Cmd.None
    
let view (state: State) =
    let mainWindow =
        let layout =
            let button =
                PushButton(
                    Attrs = [
                        Text "show"
                        MinWidth 200
                    ], OnClicked = ShowDialog)
            VBoxLayout(Attrs = [ ContentsMargins (10, 10, 10, 10) ],
                      Items = [ BoxItem.Create(button) ])
        MainWindow(Attrs = [ Title "Hello" ], Content = layout)
        
    let openDialog =
        FileDialog(
            Attrs = [
                FileMode ExistingFile
                NameFilter "Images (*.png *.xpm *.jpg)"
                ViewMode Detail
            ], OnFileSelected = FileSelected)
        
    WindowWithDialogs(mainWindow, [ "open", openDialog ])
    :> IBuilderNode<Msg>
    
[<EntryPoint>]
[<STAThread>]
let main argv =
    use app =
        createApplication init update view
    app.Run argv
