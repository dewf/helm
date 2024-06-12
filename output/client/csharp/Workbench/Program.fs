module Program

open System

open FSharpQt
open BuilderNode
open FSharpQt.Widgets.Dialog
open FSharpQt.WithDialogs
open Reactor

open FSharpQt.Widgets
open MainWindow
open PushButton
open BoxLayout

type State = {
    NothingYet: int
}

type Msg =
    | DoSomething

let init () =
    let nextState = {
        NothingYet = 0
    }
    nextState, Cmd.None
    
let update (state: State) (msg: Msg) =
    match msg with
    | DoSomething ->
        state, Cmd.Dialog ("dialog1", Show)
    
let view (state: State) =
    let button =
        PushButton(Attrs = [ Text "ButtonText"; MinWidth 200 ], OnClicked = DoSomething)
    let layout =
        VBoxLayout(Items = [ BoxItem.Create(button) ])
    let mainWindow =
        MainWindow(Attrs = [ Title "Wooooot" ], Content = layout)
    let dialog =
        Dialog(Attrs = [ Dialog.Title "Awhooo"; Dialog.Size (320, 240) ])
        
    WindowWithDialogs(mainWindow, [ "dialog1", dialog ])
    :> IBuilderNode<Msg>
    
[<EntryPoint>]
[<STAThread>]
let main argv =
    use app =
        createApplication init update view
    app.Run argv
