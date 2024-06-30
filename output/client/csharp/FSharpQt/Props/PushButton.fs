module FSharpQt.Props.PushButton

open FSharpQt.Attrs
open FSharpQt.Props.AbstractButton
open Org.Whatever.QtTesting

type Signal =
    | AbstractButtonSignal of signal: AbstractButton.Signal
    // no signals of our own :(
    
type Attr =
    | AutoDefault of state: bool
    | Default of state: bool
    | Flat of state: bool
with
    interface IAttr with
        override this.AttrEquals other =
            match other with
            | :? Attr as otherAttr ->
                this = otherAttr
            | _ ->
                false
        override this.Key =
            match this with
            | AutoDefault _ -> "pushbutton:autodefault"
            | Default _ -> "pushbutton:default"
            | Flat _ -> "pushbutton:flat"
        override this.ApplyTo (target: IAttrTarget) =
            match target with
            | :? PushButtonAttrTarget as buttonTarget ->
                let button =
                    buttonTarget.PushButton
                match this with
                | AutoDefault state ->
                    button.SetAutoDefault(state)
                | Default state ->
                    button.SetDefault(state)
                | Flat state ->
                    button.SetFlat(state)
            | _ ->
                printfn "warning: PushButton.Attr couldn't ApplyTo() unknown target type [%A]" target
    
type PushButtonProps<'msg>() =
    inherit AbstractButtonProps<'msg>()
    
    member internal this.SignalMask = enum<PushButton.SignalMask> (int this._signalMask)
    
    member internal this.SignalMap = function
        | AbstractButtonSignal signal ->
            (this :> AbstractButtonProps<'msg>).SignalMap signal
    
    member this.AutoDefault with set value =
        this.PushAttr(AutoDefault value)
        
    member this.Default with set value =
        this.PushAttr(Default value)
        
    member this.Flat with set value =
        this.PushAttr(Flat value)
 
