module FSharpQt.Props.BoxLayout

open FSharpQt.Attrs
open Org.Whatever.QtTesting

// no signals yet
type Signal = unit

type DirectionValue =
    | LeftToRight
    | RightToLeft
    | TopToBottom
    | BottomToTop
with
    member this.QtValue =
        match this with
        | LeftToRight -> BoxLayout.Direction.LeftToRight
        | RightToLeft -> BoxLayout.Direction.RightToLeft
        | TopToBottom -> BoxLayout.Direction.TopToBottom
        | BottomToTop -> BoxLayout.Direction.BottomToTop

type Attr =
    | Direction of dir: DirectionValue
with
    interface IAttr with
        override this.AttrEquals other =
            match other with
            | :? Attr as attrOther ->
                this = attrOther
            | _ ->
                false
        override this.Key =
            match this with
            | Direction dir -> "boxlayout:direction"
        override this.ApplyTo (target: IAttrTarget) =
            match target with
            | :? BoxLayoutAttrTarget as boxTarget ->
                let boxLayout =
                    boxTarget.BoxLayout
                match this with
                | Direction dir ->
                    boxLayout.SetDirection(dir.QtValue)
            | _ ->
                printfn "warning: BoxLayout.Attr couldn't ApplyTo() unknown target type [%A]" target
                
type Props<'msg>() =
    inherit Layout.Props<'msg>()
    
    member this.SignalMap: Signal -> 'msg option = (fun _ -> None)
    
    member this.Direction with set value =
        this.PushAttr(Direction value)
