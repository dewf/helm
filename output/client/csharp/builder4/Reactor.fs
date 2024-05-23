module Reactor

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

[<RequireQualifiedAccess>]
type SubCmd<'msg,'signal> =
    | None
    | OfMsg of 'msg
    | Signal of 'signal
    | Batch of commands: SubCmd<'msg,'signal> list

type SubReactor<'state, 'attr, 'msg, 'signal, 'root when 'root :> IBuilderNode<'msg>>(
                    init: unit -> 'state * SubCmd<'msg,'signal>,
                    attrUpdate: 'state -> 'attr -> 'state,
                    update: 'state -> 'msg -> 'state * SubCmd<'msg,'signal>,
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
            diff dispatch (Some (prevRoot :> IBuilderNode<'msg>)) (Some (root :> IBuilderNode<'msg>))
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
    
    member this.ProcessMsg (msg: 'msg) =
        dispatch msg

    interface IDisposable with
        member this.Dispose() =
            // outside code has no concept of our inner tree, so we're responsible for disposing all of it
            disposeTree root


[<AbstractClass>]    
type ReactorNodeBase<'outerMsg,'state,'msg,'attr,'signal,'root when 'root :> IBuilderNode<'msg>>(
                init: unit -> 'state * SubCmd<'msg, 'signal>,
                attrUpdate: 'state -> 'attr -> 'state,
                update: 'state -> 'msg -> 'state * SubCmd<'msg, 'signal>,
                view: 'state -> 'root,
                diffAttrs: 'attr list -> 'attr list -> AttrChange<'attr> list
                ) =
    [<DefaultValue>] val mutable reactor: SubReactor<'state,'attr,'msg,'signal,'root>
    member val Attrs: 'attr list = [] with get, set
    abstract member SignalMap: 'signal -> 'outerMsg option
    
    interface IBuilderNode<'outerMsg> with
        override this.Dependencies() = []
        override this.Create(dispatch: 'outerMsg -> unit) =
            let rec processCmd (cmd: SubCmd<'msg, 'signal>) =
                match cmd with
                | SubCmd.None ->
                    ()
                | SubCmd.OfMsg msg ->
                    // note, will break if used by init() - 'reactor' variable hasn't been set!
                    this.reactor.ProcessMsg(msg)
                | SubCmd.Signal signal ->
                    match this.SignalMap signal with
                    | Some outerMsg ->
                        dispatch outerMsg
                    | None ->
                        ()
                | SubCmd.Batch commands ->
                    commands
                    |> List.iter processCmd
            this.reactor <- new SubReactor<'state,'attr,'msg,'signal,'root>(init, attrUpdate, update, view, processCmd)
            this.reactor.ApplyAttrs(this.Attrs)
        override this.MigrateFrom(left: IBuilderNode<'outerMsg>) =
            let left' = (left :?> ReactorNodeBase<'outerMsg,'state,'msg,'attr,'signal,'root>)
            let nextAttrs = diffAttrs left'.Attrs this.Attrs |> createdOrChanged
            this.reactor <- left'.reactor
            this.reactor.ApplyAttrs(nextAttrs)
        override this.Dispose() =
            (this.reactor :> IDisposable).Dispose()
        override this.ContentKey =
            this.reactor.Root.ContentKey
    
[<AbstractClass>]
type WidgetReactorNode<'outerMsg,'state,'msg,'attr,'signal>(
                init: unit -> 'state * SubCmd<'msg, 'signal>,
                attrUpdate: 'state -> 'attr -> 'state,
                update: 'state -> 'msg -> 'state * SubCmd<'msg, 'signal>,
                view: 'state -> IWidgetNode<'msg>,
                diffAttrs: 'attr list -> 'attr list -> AttrChange<'attr> list
                ) =
    inherit ReactorNodeBase<'outerMsg,'state,'msg,'attr,'signal,IWidgetNode<'msg>>(init, attrUpdate, update, view, diffAttrs)
    
    interface IWidgetNode<'outerMsg> with
        override this.Widget =
            this.reactor.Root.Widget

[<AbstractClass>]
type LayoutReactorNode<'outerMsg,'state,'msg,'attr,'signal>(
                init: unit -> 'state * SubCmd<'msg, 'signal>,
                attrUpdate: 'state -> 'attr -> 'state,
                update: 'state -> 'msg -> 'state * SubCmd<'msg, 'signal>,
                view: 'state -> ILayoutNode<'msg>,
                diffAttrs: 'attr list -> 'attr list -> AttrChange<'attr> list
                ) =
    inherit ReactorNodeBase<'outerMsg,'state,'msg,'attr,'signal,ILayoutNode<'msg>>(init, attrUpdate, update, view, diffAttrs)
    let mutable maybeSyntheticParent: Widget.Handle option = None

    interface ILayoutNode<'outerMsg> with
        override this.Layout =
            this.reactor.Root.Layout
        override this.Widget =
            match maybeSyntheticParent with
            | Some widget ->
                widget
            | None ->
                let widget = Widget.Create()
                widget.SetLayout((this :> ILayoutNode<'outerMsg>).Layout)
                maybeSyntheticParent <- Some widget
                widget
                
        // TODO: still need a Dispose mechanism that works up the hierarchy ...
        
[<AbstractClass>]
type WindowReactorNode<'outerMsg,'state,'msg,'attr,'signal>(
                init: unit -> 'state * SubCmd<'msg, 'signal>,
                attrUpdate: 'state -> 'attr -> 'state,
                update: 'state -> 'msg -> 'state * SubCmd<'msg, 'signal>,
                view: 'state -> IWindowNode<'msg>,
                diffAttrs: 'attr list -> 'attr list -> AttrChange<'attr> list
                ) =
    inherit ReactorNodeBase<'outerMsg,'state,'msg,'attr,'signal,IWindowNode<'msg>>(init, attrUpdate, update, view, diffAttrs)
    
    interface IWindowNode<'outerMsg> with
        override this.WindowWidget =
            this.reactor.Root.WindowWidget
