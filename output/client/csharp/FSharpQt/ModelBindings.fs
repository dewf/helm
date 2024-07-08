module FSharpQt.ModelBindings

open MiscTypes
open Org.Whatever.QtTesting

type ModelBindingBase<'handle> internal() =
    member val internal Handle: 'handle = Unchecked.defaultof<'handle> with get, set
    
// should these be declared in the modules, as <module>.ModelBinding ?

type AbstractProxyModelBinding() =
    inherit ModelBindingBase<AbstractProxyModel.Handle>()
    internal new(handle: AbstractProxyModel.Handle) =
        base.Handle <- handle
        AbstractProxyModelBinding()
    member this.MapToSource (proxyIndex: ModelIndexProxy) =
        let ret = this.Handle.MapToSource(ModelIndex.Deferred.FromHandle(proxyIndex.Index))
        new ModelIndexOwned(ret)
