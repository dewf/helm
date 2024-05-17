module Reactor

open System
open BuilderNode

type Reactor<'state, 'msg>(init: unit -> 'state, update: 'state -> 'msg -> 'state, view: 'state -> BuilderNode<'msg>) =
    let mutable state: 'state = init()
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
            state <- update state msg
            root <- view state
            // prevent nested dispatching with a guard:
            inDispatch <- true
            diff dispatch (Some prevRoot) (Some root)
            inDispatch <- false
            
    do
        build dispatch root

    interface IDisposable with
        member this.Dispose() =
            // outside code has no concept of our inner tree, so we're responsible for disposing all of it
            disposeTree root
