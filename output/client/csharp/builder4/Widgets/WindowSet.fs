module Widgets.WindowSet

open BuilderNode

type Node<'msg>() =
    inherit TopLevelNode<'msg>()
    let mutable windows: (int * WindowNode<'msg>) list = []
    
    member this.Windows
        with get() = windows
        and set value = windows <- value
        
    override this.Dependencies() =
        windows
        |> List.map (fun (key, window) -> key, window :> BuilderNode<'msg>)
        
    override this.Create(dispatch: 'msg -> unit) =
        // no model, nothing to do
        ()
        
    override this.MigrateFrom(left: BuilderNode<'msg>) =
        // no model, nothing to do
        ()
        
    override this.Dispose() =
        // etc
        ()
        
    override this.ContentKey =
        // does this even make sense?
        windows
  