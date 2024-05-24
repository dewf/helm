module AppReactor

open System
open BuilderNode
open Org.Whatever.QtTesting

[<RequireQualifiedAccess>]
type Cmd<'msg> =
    | None
    | OfMsg of 'msg
    | QuitApplication
    | Batch of commands: Cmd<'msg> list
    
type Reactor<'state, 'msg>(init: unit -> 'state * Cmd<'msg>, update: 'state -> 'msg -> 'state * Cmd<'msg>, view: 'state -> IBuilderNode<'msg>, processCmd: Cmd<'msg> -> unit) =
    let initState, initCmd = init()
    let mutable state = initState
    let mutable root = view state
    let mutable inDispatch = false

    // dispatch isn't actually (supposed to be) recursive, but we do pass it as a parameter because it gets injected into all the widget models for callbacks
    // but we need to protect against reentrance, which is the purpose of the 'inDispatch' flag
    let rec dispatch (msg: 'msg) =
        if inDispatch then
            // already in dispatch, something fired an even when it shouldn't have
            // basically this acts as a global callback disabler, preventing them while we're handling one already
            ()
        else
            let prevRoot = root
            let nextState, cmd =
                update state msg
            state <- nextState
            root <- view state
            // prevent nested dispatching with a guard:
            inDispatch <- true
            diff dispatch (Some prevRoot) (Some root)
            inDispatch <- false
            // process command(s) after tree diff
            processCmd cmd
    do
        build dispatch root
        processCmd initCmd
        
    member this.ProcessMsg (msg: 'msg) =
        dispatch msg

    interface IDisposable with
        member this.Dispose() =
            // outside code has no concept of our inner tree, so we're responsible for disposing all of it
            disposeTree root
            
type AppReactor<'msg,'state>(init: unit -> 'state * Cmd<'msg>, update: 'state -> 'msg -> 'state * Cmd<'msg>, view: 'state -> IBuilderNode<'msg>) =
    [<DefaultValue>] val mutable reactor: Reactor<'state,'msg>
    member this.Run(argv: string array) =
        use app =
            Application.Create(argv)
        Application.SetStyle("Fusion")
        let rec processCmd = function
            | Cmd.None ->
                ()
            | Cmd.OfMsg msg ->
                this.reactor.ProcessMsg msg
            | Cmd.QuitApplication ->
                Application.Quit()
            | Cmd.Batch commands ->
                commands
                |> List.iter processCmd
        this.reactor <-
            new Reactor<'state,'msg>(init, update, view, processCmd)
        Application.Exec()
    interface IDisposable with
        member this.Dispose() =
            (this.reactor :> IDisposable).Dispose()
            
let createApplication (init: unit -> 'state * Cmd<'msg>) (update: 'state -> 'msg -> 'state * Cmd<'msg>) (view: 'state -> IBuilderNode<'msg>) =
    new AppReactor<'msg,'state>(init, update, view)

