module AppReactor

open System
open BuilderNode
open Org.Whatever.QtTesting

type DialogOps =
    | Exec
    | Accept
    | Reject
    
[<RequireQualifiedAccess>]
type Cmd<'msg> =
    | None
    | OfMsg of 'msg
    | QuitApplication
    | Batch of commands: Cmd<'msg> list
    | DialogOp of name: string * op: DialogOps
    
type Reactor<'state, 'msg>(init: unit -> 'state * Cmd<'msg>, update: 'state -> 'msg -> 'state * Cmd<'msg>, view: 'state -> IBuilderNode<'msg>, processCmd: Reactor<'state, 'msg> -> Cmd<'msg> -> unit) as this =
    let initState, initCmd = init()
    let mutable state = initState
    let mutable root = view state
    let mutable disableDispatch = false
    let mutable dialogMap = Map.empty<string, IDialogNode<'msg>>
    
    let updateDialogMap() =
        let rec recInner (soFar: Map<string, IDialogNode<'msg>>) (node: IBuilderNode<'msg>) =
            // first process dependencies
            let soFar =
                (soFar, node.Dependencies())
                ||> List.fold (fun acc (_, node) -> recInner acc node)
            // now this node
            match node with
            | :? IDialogParent<'msg> as dlgParent ->
                (soFar, dlgParent.AttachedDialogs)
                ||> List.fold (fun acc (name, node) -> acc.Add (name, node))
            | _ ->
                soFar
        dialogMap <- recInner Map.empty root

    // dispatch isn't actually (supposed to be) recursive, but we do pass it as a parameter because it gets injected into all the widget models for callbacks
    // but we need to protect against reentrance, which is the purpose of the 'nowDiffing' flag
    let rec dispatch (msg: 'msg) =
        if disableDispatch then
            // still diffing, something fired an event when it shouldn't have
            // basically this acts as a global callback disabler, preventing them while we're migrating the tree
            ()
        else
            let prevRoot = root
            let nextState, cmd =
                update state msg
            state <- nextState
            root <- view state
            // prevent nested dispatching with a guard:
            disableDispatch <- true
            diff dispatch (Some prevRoot) (Some root)
            disableDispatch <- false
            //
            updateDialogMap()
            // process command(s) after tree diff
            processCmd this cmd
    do
        build dispatch root
        updateDialogMap()
        processCmd this initCmd
        
    member this.ProcessMsg (msg: 'msg) =
        dispatch msg
        
    member this.DialogOp (name: string) (op: DialogOps) =
        match dialogMap.TryFind name with
        | Some node ->
            match op with
            | Exec -> node.Dialog.Exec()
            | Accept -> node.Dialog.Accept()
            | Reject -> node.Dialog.Reject()
        | None ->
            printfn "Reactor.ExecDialog: couldn't find dialog '%s'" name
            
    interface IDisposable with
        member this.Dispose() =
            printfn "Reactor type .Dispose()"
            // outside code has no concept of our inner tree, so we're responsible for disposing all of it
            disposeTree root
            
type AppReactor<'msg,'state>(init: unit -> 'state * Cmd<'msg>, update: 'state -> 'msg -> 'state * Cmd<'msg>, view: 'state -> IBuilderNode<'msg>) =
    member this.Run(argv: string array) =
        use app =
            Application.Create(argv)
        Application.SetStyle("Fusion")
        let rec processCmd (reactor: Reactor<'state,'msg>) = function
            | Cmd.None ->
                ()
            | Cmd.OfMsg msg ->
                reactor.ProcessMsg msg
            | Cmd.QuitApplication ->
                Application.Quit()
            | Cmd.Batch commands ->
                commands
                |> List.iter (processCmd reactor)
            | Cmd.DialogOp (name, op) ->
                reactor.DialogOp name op
        use reactor =
            new Reactor<'state,'msg>(init, update, view, processCmd)
        Application.Exec()
        // reactor should be disposed, then app (QApplication instance)
    interface IDisposable with
        member this.Dispose() =
            printfn "AppReactor .Dispose() (nothing to do, currently)"
            
let createApplication (init: unit -> 'state * Cmd<'msg>) (update: 'state -> 'msg -> 'state * Cmd<'msg>) (view: 'state -> IBuilderNode<'msg>) =
    new AppReactor<'msg,'state>(init, update, view)

