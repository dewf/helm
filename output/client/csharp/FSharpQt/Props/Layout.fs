module FSharpQt.Props.Layout

open FSharpQt.Attrs
open FSharpQt.Props.PropsRoot
open Org.Whatever.QtTesting

type SizeConstraint =
    | SetDefaultConstraint
    | SetNoConstraint
    | SetMinimumSize
    | SetFixedSize
    | SetMaximumSize
    | SetMinAndMaxSize
with
    member this.QtValue =
        match this with
        | SetDefaultConstraint -> Layout.SizeConstraint.SetDefaultConstraint
        | SetNoConstraint -> Layout.SizeConstraint.SetNoConstraint
        | SetMinimumSize -> Layout.SizeConstraint.SetMinimumSize
        | SetFixedSize -> Layout.SizeConstraint.SetFixedSize
        | SetMaximumSize -> Layout.SizeConstraint.SetMaximumSize
        | SetMinAndMaxSize -> Layout.SizeConstraint.SetMinAndMaxSize
    
type internal Attr =
    | Enabled of enabled: bool
    | Spacing of spacing: int
    | ContentsMargins of left: int * top: int * right: int * bottom: int
    | SizeConstraint of value: SizeConstraint
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
            | Enabled _ -> "layout:enabled"
            | Spacing _ -> "layout:spacing"
            | ContentsMargins _ -> "layout:contentsmargins"
            | SizeConstraint _ -> "layout:sizeconstraint"
        override this.ApplyTo (target: IAttrTarget) =
            match target with
            | :? LayoutAttrTarget as layoutTarget ->
                let layout =
                    layoutTarget.Layout
                match this with
                | Enabled enabled ->
                    layout.SetEnabled(enabled)
                | Spacing spacing ->
                    layout.SetSpacing(spacing)
                | ContentsMargins(left, top, right, bottom) ->
                    layout.SetContentsMargins(left, top, right, bottom)
                | SizeConstraint value ->
                    layout.SetSizeConstraint(value.QtValue)
            | _ ->
                printfn "warning: Layout.Attr couldn't ApplyTo() unknown target type [%A]" target

type Props<'msg>() =
    inherit PropsRoot()
    
    member this.Enabled with set value =
        this.PushAttr(Enabled value)
        
    member this.Spacing with set value =
        this.PushAttr(Spacing value)
        
    member this.ContentsMargins with set value =
        this.PushAttr(ContentsMargins value)
        
    member this.SizeConstraint with set value =
        this.PushAttr(SizeConstraint value)
