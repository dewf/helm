module FSharpQt.Props.PropsRoot

open FSharpQt.Attrs

type PropsRoot() =
    // internal attribute-from-properties storage that will be shared by subclasses (eg [Root] -> Widget -> AbstractButton -> PushButton)
    // needs to be reversed before use to maintain the order that was originally assigned
    // we do it this way (all subclasses sharing this single list) precisely to preserve the consumer-supplied order
    member val internal _attrs: IAttr list = [] with get, set
    member internal this.PushAttr(attr: IAttr) =
        this._attrs <- attr :: this._attrs
        
    member val internal _signalMask = 0L with get, set
    member internal this.AddSignal(flag: int64) =
        this._signalMask <- this._signalMask ||| flag
