module FSharpQt.Widgets.Widget

open System
open FSharpQt.BuilderNode
open Org.Whatever.QtTesting

open FSharpQt.MiscTypes
open FSharpQt.Attrs

type internal Signal =
    | CustomContextMenuRequested of pos: Point
    | WindowIconChanged of icon: IconProxy
    | WindowTitleChanged of title: string

type FocusPolicy =
    | NoFocus
    | TabFocus
    | ClickFocus
    | StrongFocus
    | WheelFocus
with
    member internal this.QtValue =
        match this with
        | NoFocus -> Enums.FocusPolicy.NoFocus
        | TabFocus -> Enums.FocusPolicy.TabFocus
        | ClickFocus -> Enums.FocusPolicy.ClickFocus
        | StrongFocus -> Enums.FocusPolicy.StrongFocus
        | WheelFocus -> Enums.FocusPolicy.WheelFocus
        
type InputMethodHint =
    | HiddenText
    | SensitiveData
    | NoAutoUppercase
    | PreferNumbers
    | PreferUppercase
    | PreferLowercase
    | NoPredictiveText
    | Date
    | Time
    | PreferLatin
    | MultiLine
    | NoEditMenu
    | NoTextHandles
    | DigitsOnly
    | FormattedNumbersOnly
    | UppercaseOnly
    | LowercaseOnly
    | DialableCharactersOnly
    | EmailCharactersOnly
    | UrlCharactersOnly
    | LatinOnly
    | ExclusiveInputMask
with
    static member QtSetFrom (hints: InputMethodHint seq) =
        (LanguagePrimitives.EnumOfValue<uint, Enums.InputMethodHints> 0u, hints)
        ||> Seq.fold (fun acc hint ->
            let flag =
                match hint with
                | HiddenText -> Enums.InputMethodHints.ImhHiddenText
                | SensitiveData -> Enums.InputMethodHints.ImhSensitiveData
                | NoAutoUppercase -> Enums.InputMethodHints.ImhNoAutoUppercase
                | PreferNumbers -> Enums.InputMethodHints.ImhPreferNumbers
                | PreferUppercase -> Enums.InputMethodHints.ImhPreferUppercase
                | PreferLowercase -> Enums.InputMethodHints.ImhPreferLowercase
                | NoPredictiveText -> Enums.InputMethodHints.ImhNoPredictiveText
                | Date -> Enums.InputMethodHints.ImhDate
                | Time -> Enums.InputMethodHints.ImhTime
                | PreferLatin -> Enums.InputMethodHints.ImhPreferLatin
                | MultiLine -> Enums.InputMethodHints.ImhMultiLine
                | NoEditMenu -> Enums.InputMethodHints.ImhNoEditMenu
                | NoTextHandles -> Enums.InputMethodHints.ImhNoTextHandles
                | DigitsOnly -> Enums.InputMethodHints.ImhDigitsOnly
                | FormattedNumbersOnly -> Enums.InputMethodHints.ImhFormattedNumbersOnly
                | UppercaseOnly -> Enums.InputMethodHints.ImhUppercaseOnly
                | LowercaseOnly -> Enums.InputMethodHints.ImhLowercaseOnly
                | DialableCharactersOnly -> Enums.InputMethodHints.ImhDialableCharactersOnly
                | EmailCharactersOnly -> Enums.InputMethodHints.ImhEmailCharactersOnly
                | UrlCharactersOnly -> Enums.InputMethodHints.ImhUrlCharactersOnly
                | LatinOnly -> Enums.InputMethodHints.ImhLatinOnly
                | ExclusiveInputMask -> Enums.InputMethodHints.ImhExclusiveInputMask
            acc ||| flag)
   
type LayoutDirection =
    | LeftToRight
    | RightToLeft
    | LayoutDirectionAuto
with
    member this.QtValue =
        match this with
        | LeftToRight -> Enums.LayoutDirection.LeftToRight
        | RightToLeft -> Enums.LayoutDirection.RightToLeft
        | LayoutDirectionAuto -> Enums.LayoutDirection.LayoutDirectionAuto
        
type WindowFlag =
    | Widget
    | Window
    | Dialog
    | Sheet
    | Drawer
    | Popup
    | Tool
    | ToolTip
    | SplashScreen
    | Desktop
    | SubWindow
    | ForeignWindow
    | CoverWindow
    | WindowType_Mask
    | MSWindowsFixedSizeDialogHint
    | MSWindowsOwnDC
    | BypassWindowManagerHint
    | X11BypassWindowManagerHint
    | FramelessWindowHint
    | WindowTitleHint
    | WindowSystemMenuHint
    | WindowMinimizeButtonHint
    | WindowMaximizeButtonHint
    | WindowMinMaxButtonsHint
    | WindowContextHelpButtonHint
    | WindowShadeButtonHint
    | WindowStaysOnTopHint
    | WindowTransparentForInput
    | WindowOverridesSystemGestures
    | WindowDoesNotAcceptFocus
    | MaximizeUsingFullscreenGeometryHint
    | CustomizeWindowHint
    | WindowStaysOnBottomHint
    | WindowCloseButtonHint
    | MacWindowToolBarButtonHint
    | BypassGraphicsProxyWidget
    | NoDropShadowWindowHint
    | WindowFullscreenButtonHint
with
    static member QtSetFrom (flags: WindowFlag seq) =
        (LanguagePrimitives.EnumOfValue<uint, Enums.WindowFlags> 0u, flags)
        ||> Seq.fold (fun acc wf ->
            let flag =
                match wf with
                | Widget -> Enums.WindowFlags.Widget
                | Window -> Enums.WindowFlags.Window
                | Dialog -> Enums.WindowFlags.Dialog
                | Sheet -> Enums.WindowFlags.Sheet
                | Drawer -> Enums.WindowFlags.Drawer
                | Popup -> Enums.WindowFlags.Popup
                | Tool -> Enums.WindowFlags.Tool
                | ToolTip -> Enums.WindowFlags.ToolTip
                | SplashScreen -> Enums.WindowFlags.SplashScreen
                | Desktop -> Enums.WindowFlags.Desktop
                | SubWindow -> Enums.WindowFlags.SubWindow
                | ForeignWindow -> Enums.WindowFlags.ForeignWindow
                | CoverWindow -> Enums.WindowFlags.CoverWindow
                | WindowType_Mask -> Enums.WindowFlags.WindowType_Mask
                | MSWindowsFixedSizeDialogHint -> Enums.WindowFlags.MSWindowsFixedSizeDialogHint
                | MSWindowsOwnDC -> Enums.WindowFlags.MSWindowsOwnDC
                | BypassWindowManagerHint -> Enums.WindowFlags.BypassWindowManagerHint
                | X11BypassWindowManagerHint -> Enums.WindowFlags.X11BypassWindowManagerHint
                | FramelessWindowHint -> Enums.WindowFlags.FramelessWindowHint
                | WindowTitleHint -> Enums.WindowFlags.WindowTitleHint
                | WindowSystemMenuHint -> Enums.WindowFlags.WindowSystemMenuHint
                | WindowMinimizeButtonHint -> Enums.WindowFlags.WindowMinimizeButtonHint
                | WindowMaximizeButtonHint -> Enums.WindowFlags.WindowMaximizeButtonHint
                | WindowMinMaxButtonsHint -> Enums.WindowFlags.WindowMinMaxButtonsHint
                | WindowContextHelpButtonHint -> Enums.WindowFlags.WindowContextHelpButtonHint
                | WindowShadeButtonHint -> Enums.WindowFlags.WindowShadeButtonHint
                | WindowStaysOnTopHint -> Enums.WindowFlags.WindowStaysOnTopHint
                | WindowTransparentForInput -> Enums.WindowFlags.WindowTransparentForInput
                | WindowOverridesSystemGestures -> Enums.WindowFlags.WindowOverridesSystemGestures
                | WindowDoesNotAcceptFocus -> Enums.WindowFlags.WindowDoesNotAcceptFocus
                | MaximizeUsingFullscreenGeometryHint -> Enums.WindowFlags.MaximizeUsingFullscreenGeometryHint
                | CustomizeWindowHint -> Enums.WindowFlags.CustomizeWindowHint
                | WindowStaysOnBottomHint -> Enums.WindowFlags.WindowStaysOnBottomHint
                | WindowCloseButtonHint -> Enums.WindowFlags.WindowCloseButtonHint
                | MacWindowToolBarButtonHint -> Enums.WindowFlags.MacWindowToolBarButtonHint
                | BypassGraphicsProxyWidget -> Enums.WindowFlags.BypassGraphicsProxyWidget
                | NoDropShadowWindowHint -> Enums.WindowFlags.NoDropShadowWindowHint
                | WindowFullscreenButtonHint -> Enums.WindowFlags.WindowFullscreenButtonHint
            acc ||| flag)

type private Attr =
    | AcceptDrops of accept: bool
    | AccessibleDescription of desc: string
    | AccessibleName of name: string
    | AutoFillBackground of state: bool
    | BaseSize of size: Size
    | ContextMenuPolicy of policy: ContextMenuPolicy
    | Enabled of enabled: bool
    | FocusPolicy of policy: FocusPolicy
    | Geometry of rect: Rect
    | InputMethodHints of hints: InputMethodHint seq
    | LayoutDirection of direction: LayoutDirection
    | MaximumHeight of height: int
    | MaximumWidth of width: int
    | MaximumSize of size: Size
    | MinimumHeight of height: int
    | MinimumSize of size: Size
    | MinimumWidth of width: int
    | MouseTracking of enabled: bool
    | Position of pos: Point            // move
    | Size of size: Size                // resize
    | SizeIncrement of size: Size
    | SizePolicy of policy: SizePolicyDeferred
    | SizePolicy2 of hPolicy: SizePolicy * vPolicy: SizePolicy
    | StatusTip of tip: string
    | StyleSheet of styles: string
    | TabletTracking of enabled: bool
    | ToolTip of tip: string
    | ToolTipDuration of msecs: int
    | UpdatesEnabled of enabled: bool
    | Visible of visible: bool
    | WhatsThis of text: string
    | WindowFilePath of path: string
    | WindowFlags of flags: WindowFlag seq
    | WindowIcon of icon: Icon
    | WindowModality of modality: WindowModality
    | WindowModified of modified: bool
    | WindowOpacity of opacity: double
    | WindowTitle of title: string
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
            | AcceptDrops _ -> "widget:AcceptDrops"
            | AccessibleDescription _ -> "widget:AccessibleDescription"
            | AccessibleName _ -> "widget:AccessibleName"
            | AutoFillBackground _ -> "widget:AutoFillBackground"
            | BaseSize _ -> "widget:BaseSize"
            | ContextMenuPolicy _ -> "widget:ContextMenuPolicy"
            | Enabled _ -> "widget:Enabled"
            | FocusPolicy _ -> "widget:FocusPolicy"
            | Geometry _ -> "widget:Geometry"
            | InputMethodHints _ -> "widget:InputMethodHints"
            | LayoutDirection _ -> "widget:LayoutDirection"
            | MaximumHeight _ -> "widget:MaximumHeight"
            | MaximumWidth _ -> "widget:MaximumWidth"
            | MaximumSize _ -> "widget:MaximumSize"
            | MinimumHeight _ -> "widget:MinimumHeight"
            | MinimumSize _ -> "widget:MinimumSize"
            | MinimumWidth _ -> "widget:MinimumWidth"
            | MouseTracking _ -> "widget:MouseTracking"
            | Position _ -> "widget:Position"
            | Size _ -> "widget:Size"
            | SizeIncrement _ -> "widget:SizeIncrement"
            | SizePolicy _ -> "widget:SizePolicy"
            | SizePolicy2 _ -> "widget:SizePolicy2"
            | StatusTip _ -> "widget:StatusTip"
            | StyleSheet _ -> "widget:StyleSheet"
            | TabletTracking _ -> "widget:TabletTracking"
            | ToolTip _ -> "widget:ToolTip"
            | ToolTipDuration _ -> "widget:ToolTipDuration"
            | UpdatesEnabled _ -> "widget:UpdatesEnabled"
            | Visible _ -> "widget:Visible"
            | WhatsThis _ -> "widget:WhatsThis"
            | WindowFilePath _ -> "widget:WindowFilePath"
            | WindowFlags _ -> "widget:WindowFlags"
            | WindowIcon _ -> "widget:WindowIcon"
            | WindowModality _ -> "widget:WindowModality"
            | WindowModified _ -> "widget:WindowModified"
            | WindowOpacity _ -> "widget:WindowOpacity"
            | WindowTitle _ -> "widget:WindowTitle"
        override this.ApplyTo (target: IAttrTarget, maybePrev: IAttr option) =
            match target with
            | :? WidgetAttrTarget as widgetTarget ->
                let widget =
                    widgetTarget.Widget
                match this with
                | AcceptDrops accept ->
                    widget.SetAcceptDrops(accept)
                | AccessibleDescription desc ->
                    widget.SetAccessibleDescription(desc)
                | AccessibleName name ->
                    widget.SetAccessibleName(name)
                | AutoFillBackground state ->
                    widget.SetAutoFillBackground(state)
                | BaseSize size ->
                    widget.SetBaseSize(size.QtValue)
                | ContextMenuPolicy policy ->
                    widget.SetContextMenuPolicy(policy.QtValue)
                | Enabled enabled ->
                    widget.SetEnabled(enabled)
                | FocusPolicy policy ->
                    widget.SetFocusPolicy(policy.QtValue)
                | Geometry rect ->
                    widget.SetGeometry(rect.QtValue)
                | InputMethodHints hints ->
                    widget.SetInputMethodHints(hints |> InputMethodHint.QtSetFrom)
                | LayoutDirection direction ->
                    widget.SetLayoutDirection(direction.QtValue)
                | MaximumHeight height ->
                    widget.SetMaximumHeight(height)
                | MaximumWidth width ->
                    widget.SetMaximumWidth(width)
                | MaximumSize size ->
                    widget.SetMaximumSize(size.QtValue)
                | MinimumHeight height ->
                    widget.SetMaximumHeight(height)
                | MinimumSize size ->
                    widget.SetMinimumSize(size.QtValue)
                | MinimumWidth width ->
                    widget.SetMinimumWidth(width)
                | MouseTracking enabled ->
                    widget.SetMouseTracking(enabled)
                | Position pos ->
                    widget.Move(pos.QtValue)
                | Size size ->
                    widget.Resize(size.QtValue)
                | SizeIncrement size ->
                    widget.SetSizeIncrement(size.QtValue)
                | SizePolicy policy ->
                    widget.SetSizePolicy(policy.QtValue)
                | SizePolicy2(hPolicy, vPolicy) ->
                    widget.SetSizePolicy(hPolicy.QtValue, vPolicy.QtValue)
                | StatusTip tip ->
                    widget.SetStatusTip(tip)
                | StyleSheet styles ->
                    widget.SetStyleSheet(styles)
                | TabletTracking enabled ->
                    widget.SetTabletTracking(enabled)
                | ToolTip tip ->
                    widget.SetToolTip(tip)
                | ToolTipDuration msecs ->
                    widget.SetToolTipDuration(msecs)
                | UpdatesEnabled enabled ->
                    widget.SetUpdatesEnabled(enabled)
                | Visible visible ->
                    widget.SetVisible(visible)
                | WhatsThis text ->
                    widget.SetWhatsThis(text)
                | WindowFilePath path ->
                    widget.SetWindowFilePath(path)
                | WindowFlags flags ->
                    widget.SetWindowFlags(flags |> WindowFlag.QtSetFrom)
                | WindowIcon icon ->
                    widget.SetWindowIcon(icon.QtValue)
                | WindowModality modality ->
                    widget.SetWindowModality(modality.QtValue)
                | WindowModified modified ->
                    widget.SetWindowModified(modified)
                | WindowOpacity opacity ->
                    widget.SetWindowOpacity(opacity)
                | WindowTitle title ->
                    widget.SetWindowTitle(title)
            | _ ->
                printfn "warning: Widget.Attr couldn't ApplyTo() unknown target type [%A]" target

type private SignalMapFunc<'msg>(func) =
    inherit SignalMapFuncBase<Signal,'msg>(func)

type Props<'msg>() =
    inherit PropsRoot()
    
    let mutable onCustomContextMenuRequested: (Point -> 'msg) option = None
    let mutable onWindowIconChanged: (IconProxy -> 'msg) option = None
    let mutable onWindowTitleChanged: (string -> 'msg) option = None

    member internal this.SignalMask = enum<Widget.SignalMask> (int this._signalMask)
    
    member this.OnCustomContextMenuRequested with set value =
        onCustomContextMenuRequested <- Some value
        this.AddSignal(int Widget.SignalMask.CustomContextMenuRequested)
        
    member this.OnWindowIconChanged with set value =
        onWindowIconChanged <- Some value
        this.AddSignal(int Widget.SignalMask.WindowIconChanged)
        
    member this.OnWindowTitleChanged with set value =
        onWindowTitleChanged <- Some value
        this.AddSignal(int Widget.SignalMask.WindowTitleChanged)
        
    // TODO: will remove this completely once all the widgets are done
    member internal this.SignalMap_REMOVE = function
        | CustomContextMenuRequested pos ->
            onCustomContextMenuRequested
            |> Option.map (fun f -> f pos)
        | WindowIconChanged icon ->
            onWindowIconChanged
            |> Option.map (fun f -> f icon)
        | WindowTitleChanged title ->
            onWindowTitleChanged
            |> Option.map (fun f -> f title)
        
    member internal this.SignalMapList =
        let thisFunc = function
            | CustomContextMenuRequested pos ->
                onCustomContextMenuRequested
                |> Option.map (fun f -> f pos)
            | WindowIconChanged icon ->
                onWindowIconChanged
                |> Option.map (fun f -> f icon)
            | WindowTitleChanged title ->
                onWindowTitleChanged
                |> Option.map (fun f -> f title)
        // if we weren't at root level (eg a Widget subclass),
        // we'd do thisFunc :: base.SignalMapFuncs
        [ SignalMapFunc(thisFunc) :> ISignalMapFunc ]
        
    member this.AcceptDrops with set value =
        this.PushAttr(AcceptDrops value)
        
    member this.AccessibleDescription with set value =
        this.PushAttr(AccessibleDescription value)
        
    member this.AccessibleName with set value =
        this.PushAttr(AccessibleName value)
        
    member this.AutoFillBackground with set value =
        this.PushAttr(AutoFillBackground value)
        
    member this.BaseSize with set value =
        this.PushAttr(BaseSize value)
        
    member this.ContextMenuPolicy with set value =
        this.PushAttr(ContextMenuPolicy value)
        
    member this.Enabled with set value =
        this.PushAttr(Enabled value)
        
    member this.FocusPolicy with set value =
        this.PushAttr(FocusPolicy value)
        
    member this.Geometry with set value =
        this.PushAttr(Geometry value)
        
    member this.InputMethodHints with set value =
        this.PushAttr(InputMethodHints value)
        
    member this.LayoutDirection with set value =
        this.PushAttr(LayoutDirection value)
        
    member this.MaximumHeight with set value =
        this.PushAttr(MaximumHeight value)
        
    member this.MaximumWidth with set value =
        this.PushAttr(MaximumWidth value)
        
    member this.MaximumSize with set value =
        this.PushAttr(MaximumSize value)
        
    member this.MinimumHeight with set value =
        this.PushAttr(MinimumHeight value)
        
    member this.MinimumSize with set value =
        this.PushAttr(MinimumSize value)
        
    member this.MinimumWidth with set value =
        this.PushAttr(MinimumWidth value)
        
    member this.MouseTracking with set value =
        this.PushAttr(MouseTracking value)
        
    member this.Position with set value =
        this.PushAttr(Position value)
        
    member this.Size with set value =
        this.PushAttr(Size value)
        
    member this.SizeIncrement with set value =
        this.PushAttr(SizeIncrement value)
        
    member this.SizePolicy with set value =
        this.PushAttr(SizePolicy value)
        
    member this.SizePolicy2 with set value =
        this.PushAttr(SizePolicy2 value)
                      
    member this.StatusTip with set value =
        this.PushAttr(StatusTip value)
        
    member this.StyleSheet with set value =
        this.PushAttr(StyleSheet value)
        
    member this.TabletTracking with set value =
        this.PushAttr(TabletTracking value)
        
    member this.ToolTip with set value =
        this.PushAttr(ToolTip value)
        
    member this.ToolTipDuration with set value =
        this.PushAttr(ToolTipDuration value)
        
    member this.UpdatesEnabled with set value =
        this.PushAttr(UpdatesEnabled value)
        
    member this.Visible with set value =
        this.PushAttr(Visible value)
        
    member this.WhatsThis with set value =
        this.PushAttr(WhatsThis value)
        
    member this.WindowFilePath with set value =
        this.PushAttr(WindowFilePath value)
        
    member this.WindowFlags with set value =
        this.PushAttr(WindowFlags value)
        
    member this.WindowIcon with set value =
        this.PushAttr(WindowIcon value)
        
    member this.WindowModality with set value =
        this.PushAttr(WindowModality value)
        
    member this.WindowModified with set value =
        this.PushAttr(WindowModified value)
        
    member this.WindowOpacity with set value =
        this.PushAttr(WindowOpacity value)
        
    member this.WindowTitle with set value =
        this.PushAttr(WindowTitle value)
        
type ModelCore<'msg>(dispatch: 'msg -> unit) =
    let mutable widget: Widget.Handle = null
    let mutable signalMap: Signal -> 'msg option = (fun _ -> None)
    let mutable currentMask = enum<Widget.SignalMask> 0
    // binding guards
    let mutable lastTitle = ""

    let signalDispatch (s: Signal) =
        signalMap s
        |> Option.iter dispatch
        
    member this.Widget
        with get() =
            widget
        and set value =
            widget <- value
    
    member internal this.SignalMaps with set (mapFuncList: ISignalMapFunc list) =
        match mapFuncList with
        | h :: _ ->
            match h with
            | :? SignalMapFunc<'msg> as smf ->
                signalMap <- smf.Func
            | _ ->
                failwith "Widget.ModelCore.SignalMaps: wrong func type"
            // no base class to assign the rest to
            // base.SignalMaps <- etc
        | _ ->
            failwith "Widget.ModelCore: signal map assignment didn't have a head element"
    
    member this.SignalMask with set value =
        if value <> currentMask then
            // we don't need to invoke the base version, the most derived widget handles the full signal stack from all super classes (at the C++/C# levels)
            widget.SetSignalMask(value)
            currentMask <- value
    
    interface WidgetAttrTarget with
        override this.Widget = widget
        // TODO: add both binding guards
        // override this.SetWindowIcon newIcon =
        //     ...
        // override this.SetWindowTitle newTitle =
        //     if newTitle <> lastTitle then
        //         lastTitle <- newTitle
        //         true
        //     else
        //         false
        
    interface Widget.SignalHandler with
        member this.CustomContextMenuRequested pos =
            signalDispatch (Point.From pos |> CustomContextMenuRequested)
        member this.WindowIconChanged icon =
            // TODO: guard
            signalDispatch (IconProxy(icon) |> WindowIconChanged)
        member this.WindowTitleChanged title =
            lastTitle <- title
            signalDispatch (WindowTitleChanged title)
            
    interface IDisposable with
        member this.Dispose() =
            widget.Dispose()

type private Model<'msg>(dispatch: 'msg -> unit) as this =
    inherit ModelCore<'msg>(dispatch)
    do
        // tried supplying this as a ctor parameter but caused issues :(
        this.Widget <- Widget.Create(this)
    
    member this.ApplyAttrs(attrs: (IAttr option * IAttr) list) =
        for maybePrev, attr in attrs do
            attr.ApplyTo(this, maybePrev)
            
    member this.RemoveLayout() =
        // the only way the layout's going to change is if it's deleted as a dependency
        // ... so is any of this even necessary? won't the layout remove itself in its dtor?
        let existing =
            this.Widget.GetLayout()
        existing.RemoveAll()
        this.Widget.SetLayout(null)
        
    member this.AddLayout(layout: Layout.Handle) =
        this.Widget.SetLayout(layout)
        
let private create (attrs: IAttr list) (signalMaps: ISignalMapFunc list) (dispatch: 'msg -> unit) (signalMask: Widget.SignalMask) =
    let model = new Model<'msg>(dispatch)
    model.ApplyAttrs (attrs |> List.map (fun attr -> None, attr))
    model.SignalMaps <- signalMaps
    model.SignalMask <- signalMask
    model

let private migrate (model: Model<'msg>) (attrs: (IAttr option * IAttr) list) (signalMaps: ISignalMapFunc list) (signalMask: Widget.SignalMask) =
    model.ApplyAttrs attrs
    model.SignalMaps <- signalMaps
    model.SignalMask <- signalMask
    model

let private dispose (model: Model<'msg>) =
    (model :> IDisposable).Dispose()

type Widget<'msg>() =
    inherit Props<'msg>()
    [<DefaultValue>] val mutable private model: Model<'msg>
    
    member val Attachments: (string * Attachment<'msg>) list = [] with get, set
    
    let mutable maybeLayout: ILayoutNode<'msg> option = None
    member this.Layout with set value = maybeLayout <- Some value
    
    member private this.MigrateContent (changeMap: Map<DepsKey, DepsChange>) =
        match changeMap.TryFind (StrKey "layout") with
        | Some change ->
            match change with
            | Unchanged ->
                ()
            | Added ->
                this.model.AddLayout(maybeLayout.Value.Layout)
            | Removed ->
                this.model.RemoveLayout()
            | Swapped ->
                this.model.RemoveLayout()
                this.model.AddLayout(maybeLayout.Value.Layout)
        | None ->
            // neither side had a layout
            ()
    
    interface IWidgetNode<'msg> with
        override this.Dependencies =
            maybeLayout
            |> Option.map (fun content -> (StrKey "layout", content :> IBuilderNode<'msg>))
            |> Option.toList
  
        override this.Create dispatch buildContext =
            this.model <- create this.Attrs this.SignalMapList dispatch this.SignalMask
            
        override this.AttachDeps () =
            maybeLayout
            |> Option.iter (fun node -> this.model.AddLayout(node.Layout))
            
        override this.MigrateFrom (left: IBuilderNode<'msg>) (depsChanges: (DepsKey * DepsChange) list) =
            let left' = (left :?> Widget<'msg>)
            let nextAttrs =
                diffAttrs left'.Attrs this.Attrs
                |> createdOrChanged
            this.model <- migrate left'.model nextAttrs this.SignalMapList this.SignalMask
            this.MigrateContent (depsChanges |> Map.ofList)

        override this.Dispose() =
            (this.model :> IDisposable).Dispose()
            
        override this.Widget =
            this.model.Widget
            
        override this.ContentKey =
            this.model.Widget
            
        override this.Attachments =
            this.Attachments

