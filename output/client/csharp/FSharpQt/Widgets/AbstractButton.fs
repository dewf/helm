module FSharpQt.Widgets.AbstractButton

open FSharpQt.Attrs
open Org.Whatever.QtTesting
open FSharpQt.MiscTypes

type internal Signal =
    // inherited
    | WidgetSignal of signal: Widget.Signal
    // ours
    | Clicked
    | ClickedWithChecked of checked_: bool
    | Pressed
    | Released
    | Toggled of checked_: bool

type private Attr =
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
        override this.ApplyTo (target: IAttrTarget, maybePrev: IAttr option) =
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
                    abstractButton.SetCheckable(state)
                | Checked state ->
                    if buttonTarget.SetChecked(state) then
                        abstractButton.SetChecked(state)
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
    
type Props<'msg>() =
    inherit Widget.Props<'msg>()
    
    let mutable onClicked: 'msg option = None
    let mutable onClickedWithChecked: (bool -> 'msg) option = None
    let mutable onPressed: 'msg option = None
    let mutable onReleased: 'msg option = None
    let mutable onToggled: (bool -> 'msg) option = None

    member internal this.SignalMask = enum<AbstractButton.SignalMask> (int this._signalMask)

    member this.OnClicked with set value =
        onClicked <- Some value
        this.AddSignal(int AbstractButton.SignalMask.Clicked)
        
    member this.OnClickedWithChecked with set value =
        onClickedWithChecked <- Some value
        this.AddSignal(int AbstractButton.SignalMask.Clicked)
        
    member this.OnPressed with set value =
        onPressed <- Some value
        this.AddSignal(int AbstractButton.SignalMask.Pressed)
        
    member this.OnReleased with set value =
        onReleased <- Some value
        this.AddSignal(int AbstractButton.SignalMask.Released)
        
    member this.OnToggled with set value =
        onToggled <- Some value
        this.AddSignal(int AbstractButton.SignalMask.Toggled)

    member internal this.SignalMap = function
        | WidgetSignal signal ->
            (this :> Widget.Props<'msg>).SignalMap signal
        | Clicked -> onClicked
        | ClickedWithChecked checked_ ->
            onClickedWithChecked
            |> Option.map (fun f -> f checked_)
        | Pressed -> onPressed
        | Released -> onReleased
        | Toggled state ->
            onToggled
            |> Option.map (fun f -> f state)
    
    member this.AutoExclusive with set value =
        this.PushAttr(AutoExclusive value)
        
    member this.AutoRepeat with set value =
        this.PushAttr(AutoRepeat value)
        
    member this.AutoRepeatDelay with set value =
        this.PushAttr(AutoRepeatDelay value)
        
    member this.AutoRepeatInterval with set value =
        this.PushAttr(AutoRepeatInterval value)

    member this.Checkable with set value =
        this.PushAttr(Checkable value)
        
    member this.Checked with set value =
        this.PushAttr(Checked value)
        
    member this.Down with set value =
        this.PushAttr(Down value)
        
    member this.Icon with set value =
        this.PushAttr(IconAttr value)
        
    member this.IconSize with set value =
        this.PushAttr(IconSize value)
        
    member this.Shortcut with set value =
        this.PushAttr(Shortcut value)
        
    member this.Text with set value =
        this.PushAttr(Text value)
