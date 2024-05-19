module Reactor

open System
open BuilderNode

[<RequireQualifiedAccess>]
type Cmd<'msg> =
    | None
    | QuitApplication
    | Batch of commands: Cmd<'msg> list
    
type Reactor<'state, 'msg>(init: unit -> 'state * Cmd<'msg>, update: 'state -> 'msg -> 'state * Cmd<'msg>, view: 'state -> BuilderNode<'msg>, processCmd: Cmd<'msg> -> unit) =
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

    interface IDisposable with
        member this.Dispose() =
            // outside code has no concept of our inner tree, so we're responsible for disposing all of it
            disposeTree root
            

[<RequireQualifiedAccess>]
type SubCmd<'msg,'signal> =
    | None
    | Signal of 'signal
    | Batch of commands: SubCmd<'msg,'signal> list

type SubReactor<'state, 'attr, 'msg, 'signal, 'root when 'root :> BuilderNode<'msg>>(
                    init: unit -> 'state * SubCmd<'msg,'signal>,
                    update: 'state -> 'msg -> 'state * SubCmd<'msg,'signal>,
                    attrUpdate: 'state -> 'attr -> 'state,
                    view: 'state -> 'root,
                    processCmd: SubCmd<'msg,'signal> -> unit) =
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
            diff dispatch (Some (prevRoot :> BuilderNode<'msg>)) (Some (root :> BuilderNode<'msg>))
            inDispatch <- false
            // process command(s) after tree diff
            processCmd cmd
    do
        build dispatch root
        processCmd initCmd
        
    member this.Root =
        root
        
    member this.ApplyAttrs (attrs: 'attr list) =
        let prevRoot = root
        let nextState =
            (state, attrs)
            ||> List.fold attrUpdate
        state <- nextState
        root <- view state
        // prevent any dispatching
        inDispatch <- true
        diff dispatch (Some prevRoot) (Some root)
        inDispatch <- false
        // no commands allowed in attr update (for now)

    interface IDisposable with
        member this.Dispose() =
            // outside code has no concept of our inner tree, so we're responsible for disposing all of it
            disposeTree root
