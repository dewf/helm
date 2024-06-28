module FSharpQt.Widgets.AbstractButton

open FSharpQt.Attrs
open FSharpQt.MiscTypes
open FSharpQt.Widgets

type internal Attr =
    | AutoExclusive of state: bool
    | AutoRepeat of state: bool
    | AutoRepeatDelay of delay: int
    | AutoRepeatInterval of interval: int
    | Checkable of state: bool
    | Checked of state: bool
    | Down of state: bool
    | IconAttr of icon: Icon
    | IconSize of size: Size
    | Shortcut of seq: KeySequence
    | Text of text: string
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
            | AutoExclusive _ -> "abstractbutton:autoexclusive"
            | AutoRepeat _ -> "abstractbutton:autorepeat"
            | AutoRepeatDelay _ -> "abstractbutton:repeatdelay"
            | AutoRepeatInterval _ -> "abstractbutton:repeatinterval"
            | Checkable _ -> "abstractbutton:checkable"
            | Checked _ -> "abstractbutton:checked"
            | Down _ -> "abstractbutton:down"
            | IconAttr _ -> "abstractbutton:iconattr"
            | IconSize _ -> "abstractbutton:iconsize"
            | Shortcut _ -> "abstractbutton:shortcut"
            | Text _ -> "abstractbutton:text"
        override this.ApplyTo (target: IAttrTarget) =
            match target with
            | :? AbstractButtonAttrTarget as buttonTarget ->
                let abstractButton =
                    buttonTarget.AbstractButton
                match this with
                | AutoExclusive state ->
                    abstractButton.SetAutoExclusive(state)
                | AutoRepeat state ->
                    abstractButton.SetAutoRepeat(state)
                | AutoRepeatDelay delay ->
                    abstractButton.SetAutoRepeatDelay(delay)
                | AutoRepeatInterval interval ->
                    abstractButton.SetAutoRepeatInterval(interval)
                | Checkable state ->
                    printfn "setting checkable"
                    abstractButton.SetCheckable(state)
                | Checked state ->
                    printfn "setting checked"
                    if buttonTarget.SetChecked(state) then
                        printfn "button target successfully setChecked(%A)" state
                        abstractButton.SetChecked(state)
                    else
                        printfn "button target refused change"
                | Down state ->
                    abstractButton.SetDown(state)
                | IconAttr icon ->
                    abstractButton.SetIcon(icon.QtValue)
                | IconSize size ->
                    abstractButton.SetIconSize(size.QtValue)
                | Shortcut seq ->
                    abstractButton.SetShortcut(seq.QtValue)
                | Text text ->
                    abstractButton.SetText(text)
            | _ ->
                printfn "warning: AbstractButton.Attr couldn't ApplyTo() unknown target type [%A]" target
    
type AbstractButtonProps() =
    inherit Widget.WidgetProps()
    
    let mutable attrs: IAttr list = []
    member this.AbstractButtonAttrs = attrs @ this.WidgetAttrs
    
    member this.AutoExclusive with set value =
        attrs <- AutoExclusive value :: attrs
        
    member this.AutoRepeat with set value =
        attrs <- AutoRepeat value :: attrs
        
    member this.Checkable with set value =
        attrs <- Checkable value :: attrs
        
    member this.Checked with set value =
        attrs <- Checked value :: attrs
        
    member this.Text with set value =
        attrs <- Text value :: attrs
