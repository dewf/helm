module Program

open System

open FSharpQt
open BuilderNode
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
    | NextPrimes of batch: int64 list
    | Cancel
    | Done

let init () =
    let nextState = {
        NothingYet = 0
    }
    nextState, Cmd.None
    
let nextPrimesList (prev: int64 list) =
    let rec findNext (current: int64) =
        let divisibleByPrevious =
            prev
            |> List.exists (fun prevPrime -> current % prevPrime = 0)
        if not divisibleByPrevious then
            current
        else
            findNext (current + 1L)
    match prev with
    | last :: _ ->
        findNext (last + 1L) :: prev
    | _ ->
        failwith "nope"
    
let update (state: State) (msg: Msg) =
    match msg with
    | DoSomething ->
        let subFunc dispatch =
            async {
                let mutable primes = [ 2L ]
                let mutable lastCount = 1
                while primes.Head < 300_000L do // not token.IsCancellationRequested && 
                    primes <- nextPrimesList primes
                    if primes.Length - lastCount >= 1000 then
                        primes
                        |> List.take 1000
                        |> NextPrimes
                        |> dispatch
                        lastCount <- primes.Length
                dispatch Done
            } |> (fun block -> Async.Start block)
        state, Cmd.Sub subFunc
    | NextPrimes batch ->
        printfn "primes batch: %A" batch
        state, Cmd.None
    | Cancel ->
        state, Cmd.None
    | Done ->
        printfn "@@@@@@ found all @@@@@@"
        state, Cmd.None
    
let view (state: State) =
    let mainWindow =
        let layout =
            let button =
                PushButton(
                    Attrs = [
                        Text "do something"
                        MinWidth 200
                    ], OnClicked = DoSomething)
            let cancel =
                PushButton(
                    Attrs = [
                        Text "cancel"
                        MinWidth 200
                    ], OnClicked = Cancel)
            VBoxLayout(
                      Attrs = [ ContentsMargins (10, 10, 10, 10) ],
                      Items = [
                          BoxItem.Create(button)
                          BoxItem.Create(cancel)
                      ])
        MainWindow(Attrs = [ Title "Hello" ], Content = layout)
        
    mainWindow
    :> IBuilderNode<Msg>
    
[<EntryPoint>]
[<STAThread>]
let main argv =
    use app =
        createApplication init update view
    app.Run argv
