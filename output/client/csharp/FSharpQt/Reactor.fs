module FSharpQt.Reactor

open BuilderNode
open System
open FSharpQt.BuilderNode
open FSharpQt.MiscTypes
open Org.Whatever.QtTesting

type DialogOp<'msg> =
    | Exec
    | ExecAt of p: Point
    | ExecWithResult of msgFunc: (int -> 'msg)
    | ExecAtPointWithResult of p: Point * msgFunc: (int -> 'msg)
    | Show
    | ShowAt of p: Point
    | Accept
    | Reject

[<RequireQualifiedAccess>]
type Cmd<'msg,'signal> =
    | None
    | Msg of 'msg
    | Signal of 'signal
    | Batch of commands: Cmd<'msg,'signal> list
    | Dialog of name: string * op: DialogOp<'msg>
    | ShowMenu of name: string * loc: Point
    | Async of block: Async<'msg>
    | Sub of subFunc: (('msg -> unit) -> unit)
    
let asyncPerform (block: Async<'a>) (mapper: 'a -> 'msg) =
    async {
        let! result = block
        return mapper result
    }
    
let nullAttrUpdate (state: 'state) (attr: 'attr) =
    state
    
type RelativeAttachment<'msg> =
    | AttachedDialog of node: IDialogNode<'msg> * relativeTo: Widget.Handle option
    | AttachedPopup of node: IMenuNode<'msg> * relativeTo: Widget.Handle
    
type Reactor<'state, 'attr, 'msg, 'signal, 'root when 'root :> IBuilderNode<'msg>>(
                    init: unit -> 'state * Cmd<'msg,'signal>,
                    attrUpdate: 'state -> 'attr -> 'state,
                    update: 'state -> 'msg -> 'state * Cmd<'msg,'signal>,
                    view: 'state -> 'root,
                    processSignal: 'signal -> unit,
                    buildContext: BuilderContext<'msg>) as this =
    let initState, initCmd = init()
    let mutable state = initState
    let mutable root = view state
    let mutable disableDispatch = false
    let mutable attachMap = Map.empty<string, RelativeAttachment<'msg>>
    
    let mutable disposed = false
    
    let updateAttachments() =
        let rec recInner (soFar: Map<string, RelativeAttachment<'msg>>) (node: IBuilderNode<'msg>) =
            // first process dependencies
            let soFar =
                (soFar, node.Dependencies)
                ||> List.fold (fun acc (_, node) -> recInner acc node)
            // now this node's attachments
            (soFar, node.Attachments)
            ||> List.fold (fun acc (id, attach) ->
                match attach with
                | Attachment.NonVisual nonVisualNode ->
                    recInner acc nonVisualNode
                | Attachment.Dialog dialogNode ->
                    // first process anything beneath (subdialogs! ack)
                    let acc =
                        recInner acc dialogNode
                    // then this particular attachment
                    let maybeWidget =
                        match node with
                        | :? IWidgetNode<'msg> as widgetNode -> Some widgetNode.Widget
                        | :? IWindowNode<'msg> as windowNode -> Some windowNode.WindowWidget
                        | :? IDialogNode<'msg> as dialogNode -> Some dialogNode.Dialog
                        | _ -> None
                    acc.Add(id, AttachedDialog(dialogNode, maybeWidget))
                | Attachment.Menu menuNode ->
                    // deeper attachments
                    let acc =
                        recInner acc menuNode
                    // then this particular attachment
                    let widget =
                        match node with
                        | :? IWidgetNode<'msg> as widgetNode -> widgetNode.Widget
                        | :? IWindowNode<'msg> as windowNode -> windowNode.WindowWidget
                        | :? IDialogNode<'msg> as dialogNode -> dialogNode.Dialog :> Widget.Handle
                        | _ -> failwith "Reactor .updateAttachments() - menu was not attached to something with a widget handle"
                    acc.Add(id, AttachedPopup(menuNode, widget))
                )
        attachMap <- recInner Map.empty root

    let rec dispatch (msg: 'msg) =
        if disableDispatch then
            // currently diffing, something tried to dispatch due to an attribute change (generally a Qt widget emitting a signal due to a method call / property change)
            // basically this acts as a global callback disabler, preventing them while we're handling one already
            ()
        else
            let prevRoot = root
            let nextState, cmd =
                update state msg
            state <- nextState
            root <- view state
            // prevent diff-triggered dispatching with a guard:
            disableDispatch <- true
            diff dispatch (Some (prevRoot :> IBuilderNode<'msg>)) (Some (root :> IBuilderNode<'msg>)) buildContext
            disableDispatch <- false
            //
            updateAttachments()
            // process command(s) after tree diff
            processCmd cmd
    and
        processCmd (cmd: Cmd<'msg,'signal>) =
            match cmd with
            | Cmd.None ->
                ()
            | Cmd.Msg msg ->
                // should this be deferred via .ExecuteOnMainThread? would the recursion be a problem for any reason?
                dispatch msg
            | Cmd.Signal signal ->
                processSignal signal
            | Cmd.Batch commands ->
                commands
                |> List.iter processCmd
            | Cmd.Dialog (name, op) ->
                this.DialogOp name op
            | Cmd.ShowMenu (name, loc) ->
                this.PopMenu name loc
            | Cmd.Async block ->
                async {
                    let! msg = block
                    Application.ExecuteOnMainThread(fun _ -> dispatch msg)
                } |> Async.Start
            | Cmd.Sub subFunc ->
                let safeDispatch msg =
                    // don't do anything except on the UI thread!
                    let inner _ =
                        if not disposed then
                            dispatch msg
                        else
                            printfn "Cmd.Sub - attempted to dispatch [%A] on a disposed reactor" msg
                    Application.ExecuteOnMainThread(inner)
                subFunc safeDispatch
    do
        build dispatch root
        updateAttachments()
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
        // prevent dispatching while diffing
        disableDispatch <- true
        diff dispatch (Some prevRoot) (Some root) buildContext
        disableDispatch <- false
        //
        updateAttachments()
        // no commands allowed in attr update (for now)
    
    member this.DialogOp (name: string) (op: DialogOp<'msg>) =
        match attachMap.TryFind name with
        | Some node ->
            match node with
            | AttachedDialog (node, maybeWidget) ->
                let moveTo (p: Point) =
                    match maybeWidget with
                    | Some widget ->
                        let abs = widget.MapToGlobal(p.QtValue)
                        node.Dialog.Move(abs)
                    | None ->
                        ()
                match op with
                | Exec ->
                    node.Dialog.Exec()
                    |> ignore
                | ExecAt p ->
                    moveTo p
                    node.Dialog.Exec()
                    |> ignore
                | ExecWithResult msgFunc ->
                    let result =
                        node.Dialog.Exec()
                    dispatch (msgFunc result)
                | ExecAtPointWithResult (p, msgFunc) ->
                    moveTo p
                    let result =
                        node.Dialog.Exec()
                    dispatch (msgFunc result)
                | Show ->
                    node.Dialog.Show()
                | ShowAt p ->
                    moveTo p
                    node.Dialog.Show()
                | Accept -> node.Dialog.Accept()
                | Reject -> node.Dialog.Reject()
            | _ ->
                printfn "Cmd.DialogOp - found a node but it wasn't a dialog node (are you using the same name twice?)"
        | None ->
            printfn "SubReactor.DialogOp: couldn't find dialog '%s'" name
            
    member this.PopMenu (name: string) (loc: Point) =
        match attachMap.TryFind name with
        | Some node ->
            match node with
            | AttachedPopup (node, relativeTo) ->
                let loc' =
                    relativeTo.MapToGlobal(loc.QtValue)
                node.Menu.Popup(loc')
            | _ ->
                printfn "Cmd.PopMenu - found a node but it wasn't a menu node (are you using the same name twice?)"
        | None ->
            printfn "SubReactor.PopMenu: couldn't find popup '%s'" name
        
    interface IDisposable with
        member this.Dispose() =
            // set ASAP to stop subscription dispatches after disposal:
            disposed <- true
            // outside code has no concept of our inner tree, so we're responsible for disposing all of it
            disposeTree root
            
[<AbstractClass>]    
type ReactorNodeBase<'outerMsg,'state,'msg,'attr,'signal,'root when 'root :> IBuilderNode<'msg>>(
                init: unit -> 'state * Cmd<'msg, 'signal>,
                attrUpdate: 'state -> 'attr -> 'state,
                update: 'state -> 'msg -> 'state * Cmd<'msg, 'signal>,
                view: 'state -> 'root,
                diffAttrs: 'attr list -> 'attr list -> AttrChange<'attr> list
                ) =
    [<DefaultValue>] val mutable reactor: Reactor<'state,'attr,'msg,'signal,'root>
    member val Attrs: 'attr list = [] with get, set
    
    abstract member SignalMap: 'signal -> 'outerMsg option
    default this.SignalMap _ = None
    
    interface IBuilderNode<'outerMsg> with
        override this.Dependencies = []
        override this.Create2 dispatch buildContext =
            let processSignal signal =
                match this.SignalMap signal with
                | Some outerMsg ->
                    dispatch outerMsg
                | None ->
                    ()
            let buildContext =
                // we need to rebind this, because 'outerMsg and 'msg are different
                { ContainingWindow = buildContext.ContainingWindow }
            this.reactor <- new Reactor<'state,'attr,'msg,'signal,'root>(init, attrUpdate, update, view, processSignal, buildContext)
            this.reactor.ApplyAttrs(this.Attrs)
        override this.AttachDeps () =
            ()
        override this.MigrateFrom (left: IBuilderNode<'outerMsg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> ReactorNodeBase<'outerMsg,'state,'msg,'attr,'signal,'root>)
            let nextAttrs = diffAttrs left'.Attrs this.Attrs |> createdOrChanged
            this.reactor <- left'.reactor
            this.reactor.ApplyAttrs(nextAttrs)
        override this.Dispose() =
            (this.reactor :> IDisposable).Dispose()
        override this.ContentKey =
            this.reactor.Root.ContentKey
        override this.Attachments = []
            
    
[<AbstractClass>]
type WidgetReactorNode<'outerMsg,'state,'msg,'attr,'signal>(
                init: unit -> 'state * Cmd<'msg, 'signal>,
                attrUpdate: 'state -> 'attr -> 'state,
                update: 'state -> 'msg -> 'state * Cmd<'msg, 'signal>,
                view: 'state -> IWidgetNode<'msg>,
                diffAttrs: 'attr list -> 'attr list -> AttrChange<'attr> list
                ) =
    inherit ReactorNodeBase<'outerMsg,'state,'msg,'attr,'signal,IWidgetNode<'msg>>(init, attrUpdate, update, view, diffAttrs)
    new(init, update, view) =
        WidgetReactorNode(init, nullAttrUpdate, update, view, nullDiffAttrs)
    interface IWidgetNode<'outerMsg> with
        override this.Widget =
            this.reactor.Root.Widget

[<AbstractClass>]
type LayoutReactorNode<'outerMsg,'state,'msg,'attr,'signal>(
                init: unit -> 'state * Cmd<'msg, 'signal>,
                attrUpdate: 'state -> 'attr -> 'state,
                update: 'state -> 'msg -> 'state * Cmd<'msg, 'signal>,
                view: 'state -> ILayoutNode<'msg>,
                diffAttrs: 'attr list -> 'attr list -> AttrChange<'attr> list
                ) =
    inherit ReactorNodeBase<'outerMsg,'state,'msg,'attr,'signal,ILayoutNode<'msg>>(init, attrUpdate, update, view, diffAttrs)
    new(init, update, view) =
        LayoutReactorNode(init, nullAttrUpdate, update, view, nullDiffAttrs)
    interface ILayoutNode<'outerMsg> with
        override this.Layout =
            this.reactor.Root.Layout
        
[<AbstractClass>]
type WindowReactorNode<'outerMsg,'state,'msg,'attr,'signal>(
                init: unit -> 'state * Cmd<'msg, 'signal>,
                attrUpdate: 'state -> 'attr -> 'state,
                update: 'state -> 'msg -> 'state * Cmd<'msg, 'signal>,
                view: 'state -> IWindowNode<'msg>,
                diffAttrs: 'attr list -> 'attr list -> AttrChange<'attr> list
                ) =
    inherit ReactorNodeBase<'outerMsg,'state,'msg,'attr,'signal,IWindowNode<'msg>>(init, attrUpdate, update, view, diffAttrs)
    new(init, update, view) =
        WindowReactorNode(init, nullAttrUpdate, update, view, nullDiffAttrs)
    interface IWindowNode<'outerMsg> with
        override this.WindowWidget =
            this.reactor.Root.WindowWidget

// root-level AppReactor stuff ============================================================

type AppSignal =
    | QuitApplication

type AppReactor<'msg,'state>(init: unit -> 'state * Cmd<'msg,AppSignal>, update: 'state -> 'msg -> 'state * Cmd<'msg,AppSignal>, view: 'state -> IBuilderNode<'msg>) =
    [<DefaultValue>] val mutable reactor: Reactor<'state,unit,'msg,AppSignal,IBuilderNode<'msg>>
    do
        Library.Init()
        
    member this.Run(argv: string array) =
        use app =
            Application.Create(argv)
        let processSignal signal =
            match signal with
            | QuitApplication ->
                Application.Quit()
        let context =
            { ContainingWindow = None }
        this.reactor <- new Reactor<'state,unit,'msg,AppSignal,IBuilderNode<'msg>>(init, nullAttrUpdate, update, view, processSignal, context)
        Application.Exec()
        
    interface IDisposable with
        member this.Dispose() =
            (this.reactor :> IDisposable).Dispose()
            Library.DumpTables()
            Library.Shutdown()
            
let createApplication (init: unit -> 'state * Cmd<'msg,AppSignal>) (update: 'state -> 'msg -> 'state * Cmd<'msg,AppSignal>) (view: 'state -> IBuilderNode<'msg>) =
    new AppReactor<'msg,'state>(init, update, view)
