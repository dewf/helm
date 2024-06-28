module FSharpQt.Widgets.AbstractButton

open FSharpQt.Attrs
open FSharpQt.MiscTypes

[<RequireQualifiedAccess>]
type internal AttrValue =
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

[<AbstractClass>]
type Attr internal(value: AttrValue) =
    member val private Value = value
    interface IAttr with
        override this.AttrEquals other =
            match other with
            | :? Attr as attr ->
                this.Value = attr.Value
            | _ ->
                false
        override this.Key =
            match value with
            | AttrValue.AutoExclusive _ -> "abstractbutton:autoexclusive"
            | AttrValue.AutoRepeat _ -> "abstractbutton:autorepeat"
            | AttrValue.AutoRepeatDelay _ -> "abstractbutton:repeatdelay"
            | AttrValue.AutoRepeatInterval _ -> "abstractbutton:repeatinterval"
            | AttrValue.Checkable _ -> "abstractbutton:checkable"
            | AttrValue.Checked _ -> "abstractbutton:checked"
            | AttrValue.Down _ -> "abstractbutton:down"
            | AttrValue.IconAttr _ -> "abstractbutton:iconattr"
            | AttrValue.IconSize _ -> "abstractbutton:iconsize"
            | AttrValue.Shortcut _ -> "abstractbutton:shortcut"
            | AttrValue.Text _ -> "abstractbutton:text"
        override this.ApplyTo (target: IAttrTarget) =
            match target with
            | :? AbstractButtonAttrTarget as buttonTarget ->
                let abstractButton =
                    buttonTarget.AbstractButton
                match value with
                | AttrValue.AutoExclusive state ->
                    abstractButton.SetAutoExclusive(state)
                | AttrValue.AutoRepeat state ->
                    abstractButton.SetAutoRepeat(state)
                | AttrValue.AutoRepeatDelay delay ->
                    abstractButton.SetAutoRepeatDelay(delay)
                | AttrValue.AutoRepeatInterval interval ->
                    abstractButton.SetAutoRepeatInterval(interval)
                | AttrValue.Checkable state ->
                    abstractButton.SetCheckable(state)
                | AttrValue.Checked state ->
                    if buttonTarget.SetChecked(state) then
                        abstractButton.SetChecked(state)
                | AttrValue.Down state ->
                    abstractButton.SetDown(state)
                | AttrValue.IconAttr icon ->
                    abstractButton.SetIcon(icon.QtValue)
                | AttrValue.IconSize size ->
                    abstractButton.SetIconSize(size.QtValue)
                | AttrValue.Shortcut seq ->
                    abstractButton.SetShortcut(seq.QtValue)
                | AttrValue.Text text ->
                    abstractButton.SetText(text)
            | _ ->
                printfn "warning: AbstractButton.Attr couldn't ApplyTo() unknown target type [%A]" target

type AutoExclusive(state: bool) =
    inherit Attr(AttrValue.AutoExclusive(state))
    
type AutoRepeat(state: bool) =
    inherit Attr(AttrValue.AutoRepeat(state))

type AutoRepeatDelay(delay: int) =
    inherit Attr(AttrValue.AutoRepeatDelay(delay))
  
type AutoRepeatInterval(interval: int) =
    inherit Attr(AttrValue.AutoRepeatInterval(interval))

type Checkable(state: bool) =
    inherit Attr(AttrValue.Checkable(state))

type Checked(state: bool) =
    inherit Attr(AttrValue.Checked(state))
    
type Down(state: bool) =
    inherit Attr(AttrValue.Down(state))
    
type IconAttr(icon: Icon) =
    inherit Attr(AttrValue.IconAttr(icon))
    
type IconSize(size: Size) =
    inherit Attr(AttrValue.IconSize(size))
    
type Shortcut(seq: KeySequence) =
    inherit Attr(AttrValue.Shortcut(seq))
    
type Text(text: string) =
    inherit Attr(AttrValue.Text(text))
 
