module Widgets.WindowSet

open BuilderNode

type Node<'msg>() =
    let mutable windows: (DepsKey * IWindowNode<'msg>) list = []
    
    member this.Windows
        with get() = windows
        and set value = windows <- value
        
    interface ITopLevelNode<'msg> with
        override this.Dependencies() =
            windows
            |> List.map (fun (key, window) -> key, window :> IBuilderNode<'msg>)
        override this.Create(dispatch: 'msg -> unit) =
            // no model, nothing to do
            ()
        override this.MigrateFrom(left: IBuilderNode<'msg>) =
            // no model, nothing to do
            ()
        override this.Dispose() =
            // etc
            ()
        override this.ContentKey =
            // does this even make sense?
            windows
