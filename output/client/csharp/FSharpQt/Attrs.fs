module FSharpQt.Attrs

open Org.Whatever.QtTesting

type IAttrTarget =
    interface
    end
    
type IAttr =
    interface
        abstract member Key: string
        abstract member AttrEquals: IAttr -> bool
        abstract member ApplyTo: IAttrTarget * IAttr option -> unit    // 2nd argument: previous value, if any
    end
    
type AttrDiffResult =
    | Created of attr: IAttr
    | Changed of prev: IAttr * next: IAttr
    | Deleted of prev: IAttr
    
let diffAttrs (left: IAttr list) (right: IAttr list) =
    let leftList = left |> List.map (fun attr -> attr.Key, attr)
    let rightList = right |> List.map (fun attr -> attr.Key, attr)
    let leftMap = leftList |> Map.ofList
    let rightMap = rightList |> Map.ofList

    let allKeys =
        (leftList @ rightList)
        |> List.map fst
        |> List.distinct
        // |> List.sort        // I think we used to do this for numeric key ordering? but we don't use those anymore

    allKeys
    |> List.choose (fun key ->
        match leftMap.TryFind key, rightMap.TryFind key with
        | Some left, Some right ->
            if left.AttrEquals right then
                None
            else
                Changed (left, right) |> Some
        | Some left, None ->
            Deleted left |> Some
        | None, Some right ->
            Created right |> Some
        | _ ->
            failwith "can't happen")

let createdOrChanged (changes: AttrDiffResult list) =
    changes
    |> List.choose (function
        | Created attr ->
            Some (None, attr)
        | Changed (prev, attr) ->
            Some (Some prev, attr)
        | _ -> None)
    
// ======= move these to another module? ===========================================

type PropsRoot() =
    // internal attribute-from-properties storage that will be shared by subclasses (eg [Root] -> Widget -> AbstractButton -> PushButton)
    // needs to be reversed before use to maintain the order that was originally assigned
    // we do it this way (all subclasses sharing this single list) precisely to preserve the consumer-supplied order
    let mutable _attrs: IAttr list = []
    member this.Attrs = _attrs |> List.rev
    member internal this.PushAttr(attr: IAttr) =
        _attrs <- attr :: _attrs
        
    member val internal _signalMask = 0L with get, set
    member internal this.AddSignal(flag: int64) =
        this._signalMask <- this._signalMask ||| flag
        
[<AbstractClass>]
type ModelCoreRoot() =
    interface IAttrTarget
    member this.ApplyAttrs(attrs: (IAttr option * IAttr) list) =
        for maybePrev, attr in attrs do
            attr.ApplyTo(this, maybePrev)
        
// this interface doesn't really do anything, just tags our objects as relevant to this purpose
// nicer than just 'Object'
type internal ISignalMapFunc =
    interface
        abstract member Nothing: int
    end
    
[<AbstractClass>]
type internal SignalMapFuncBase<'signal,'msg>(func: 'signal -> 'msg option) =
    member val Func = func
    interface ISignalMapFunc with
        member this.Nothing = 0

// for cases where we have no signals (eg PushButton)
type internal NullSignalMapFunc() =
    interface ISignalMapFunc with
        member this.Nothing = 0

// ====================================================================
// various interfaces for accessing qobjects/widgets, + 2-way binding guard setters where applicable
// if you want to support a given type of attribute, you have to implement the target interface
// reasonable enough!

type internal LayoutAttrTarget =
    interface
        inherit IAttrTarget
        abstract member Layout: Layout.Handle
    end
    
type internal BoxLayoutAttrTarget =
    interface
        inherit LayoutAttrTarget
        abstract member BoxLayout: BoxLayout.Handle
    end

type internal WidgetAttrTarget =
    interface
        inherit IAttrTarget
        abstract member Widget: Widget.Handle
        // abstract member SetWindowIcon: Icon -> bool
        // abstract member SetWindowTitle: string -> bool
    end
    
type internal FrameAttrTarget =
    interface
        inherit WidgetAttrTarget
        abstract member Frame: Frame.Handle
    end
    
type internal LabelAttrTarget =
    interface
        inherit FrameAttrTarget
        abstract member Label: Label.Handle
    end
    
type internal AbstractButtonAttrTarget =
    interface
        inherit WidgetAttrTarget
        abstract member AbstractButton: AbstractButton.Handle
        abstract member SetChecked: bool -> bool
        abstract member SetDown: bool -> bool
    end
    
type internal PushButtonAttrTarget =
    interface
        inherit AbstractButtonAttrTarget
        abstract member PushButton: PushButton.Handle
    end
    
type internal RadioButtonAttrTarget =
    interface
        inherit AbstractButtonAttrTarget
        abstract member RadioButton: RadioButton.Handle
    end
    
type internal ComboBoxAttrTarget =
    interface
        inherit WidgetAttrTarget
        abstract member ComboBox: ComboBox.Handle
        abstract member Clear: unit -> unit // same as next 2 individually, I guess
        abstract member SetCurrentIndex: int option -> bool
        abstract member SetCurrentText: string option -> bool
    end
    
type internal LineEditAttrTarget =
    interface
        inherit WidgetAttrTarget
        abstract member LineEdit: LineEdit.Handle
        abstract member SetText: string -> bool     // binding guards
        abstract member SetCursorPos: int -> bool
    end
    
type internal MenuAttrTarget =
    interface
        inherit WidgetAttrTarget
        abstract member Menu: Menu.Handle
    end
    
type internal MenuBarAttrTarget =
    interface
        inherit WidgetAttrTarget
        abstract member MenuBar: MenuBar.Handle
    end
    
type internal ActionAttrTarget =
    interface
        inherit IAttrTarget
        abstract member Action: Action.Handle
        abstract member SetEnabled: bool -> bool   // return value: internal value did change
        abstract member SetCheckable: bool -> bool
        abstract member SetChecked: bool -> bool
    end
  
type internal SortFilterProxyModelAttrTarget =
    interface
        inherit IAttrTarget
        abstract member ProxyModel: SortFilterProxyModel.Handle
    end

type internal DialogAttrTarget =
    interface
        inherit WidgetAttrTarget
        abstract member Dialog: Dialog.Handle
    end

type internal FileDialogAttrTarget =
    interface
        inherit DialogAttrTarget
        abstract member FileDialog: FileDialog.Handle
    end
