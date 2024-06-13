module Program

open System

open FSharpQt
open BuilderNode
open FSharpQt.Widgets.Dialog
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
    | ShowNested

let init () =
    let nextState = {
        NothingYet = 0
    }
    nextState, Cmd.None
    
let update (state: State) (msg: Msg) =
    match msg with
    | DoSomething ->
        state, Cmd.Dialog ("dialog1", Show)
    | ShowNested ->
        state, Cmd.Dialog ("nested", Show)
    
let view (state: State) =
    let button =
        PushButton(Attrs = [ Text "ButtonText"; MinWidth 200 ], OnClicked = DoSomething)

    let layout =
        VBoxLayout(Items = [ BoxItem.Create(button) ])

    let dialog =
        let nested =
            Dialog(Attrs = [ Dialog.Title "Smaller!"; Dialog.Size (320, 240) ])
        let layout =
            let button =
                PushButton(Attrs = [ Text "Lauch Nested" ], OnClicked = ShowNested)
            VBoxLayout(Items = [ BoxItem.Create(button) ])
        Dialog(Attrs = [ Dialog.Title "Awhooo"; Dialog.Size (640, 480) ], Layout = layout, Attachments = [ "nested", Attachment.Dialog nested ])

    MainWindow(Attrs = [ Title "Wooooot" ], Content = layout, Attachments = [ "dialog1", Attachment.Dialog dialog ])
    :> IBuilderNode<Msg>
    
[<EntryPoint>]
[<STAThread>]
let main argv =
    use app =
        createApplication init update view
    app.Run argv
