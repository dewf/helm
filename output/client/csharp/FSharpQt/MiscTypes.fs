﻿module FSharpQt.MiscTypes

open System
open Org.Whatever.QtTesting
open FSharpQt.InputEnums

type SizePolicy =
    | Fixed
    | Minimum
    | Maximum
    | Preferred
    | MinimumExpanding
    | Expanding
    | Ignored
with
    member this.QtValue =
        match this with
        | Fixed -> SizePolicy.Policy.Fixed
        | Minimum -> SizePolicy.Policy.Minimum
        | Maximum -> SizePolicy.Policy.Maximum
        | Preferred -> SizePolicy.Policy.Preferred
        | MinimumExpanding -> SizePolicy.Policy.MinimumExpanding
        | Expanding -> SizePolicy.Policy.Expanding
        | Ignored -> SizePolicy.Policy.Ignored

type Alignment =
    | Left
    | Leading
    | Right
    | Trailing
    | HCenter
    | Justify
    | Absolute
    | Top
    | Bottom
    | VCenter
    | Baseline
    | Center
with
    member internal this.QtValue =
        match this with
        | Left -> Enums.Alignment.AlignLeft
        | Leading -> Enums.Alignment.AlignLeading
        | Right -> Enums.Alignment.AlignRight
        | Trailing -> Enums.Alignment.AlignTrailing
        | HCenter -> Enums.Alignment.AlignHCenter
        | Justify -> Enums.Alignment.AlignJustify
        | Absolute -> Enums.Alignment.AlignAbsolute
        | Top -> Enums.Alignment.AlignTop
        | Bottom -> Enums.Alignment.AlignBottom
        | VCenter ->Enums.Alignment.AlignVCenter
        | Baseline -> Enums.Alignment.AlignBaseline
        | Center -> Enums.Alignment.AlignCenter
        
type Orientation =
    | Horizontal
    | Vertical
with
    member this.QtValue =
        match this with
        | Horizontal -> Enums.Orientation.Horizontal
        | Vertical -> Enums.Orientation.Vertical
    static member From (o: Enums.Orientation) =
        match o with
        | Enums.Orientation.Horizontal -> Horizontal
        | Enums.Orientation.Vertical -> Vertical
        | _ -> failwith "Orientation.From - unknown input value"
    
type Point = {
    X: int
    Y: int
} with
    static member From (x, y) =
        { X = x; Y = y }
    static member internal From(p: Common.Point) =
        { X = p.X; Y = p.Y }
    member internal this.QtValue =
        Common.Point(X = this.X, Y = this.Y)
        
type PointF = {
    X: double
    Y: double
} with
    static member From (x, y) =
        { X = x; Y = y }
    static member From(p: Point) =
        { X = double p.X; Y = p.Y }
    static member internal From(p: Common.Point) =
        { X = p.X; Y = p.Y }
    static member internal From(p: Common.PointF) =
        { X = p.X; Y = p.Y }
    member internal this.QtValue =
        Common.PointF(X = this.X, Y = this.Y)
        
type Size = {
    Width: int
    Height: int
} with
    static member Invalid =
        // useful in some places
        { Width = -1; Height = -1 }
    static member Zero =
        { Width = 0; Height = 0 }
    static member From (w, h) =
        { Width = w; Height = h }
    static member internal From (sz: Common.Size) =
        { Width = sz.Width; Height = sz.Height }
    member internal this.QtValue =
        Common.Size(Width = this.Width, Height = this.Height)
        
type Rect = {
    X: int
    Y: int
    Width: int
    Height: int
} with
    static member From (x, y, width, height) =
        { X = x; Y = y; Width = width; Height = height }
    static member From (size: Size) =
        { X = 0; Y = 0; Width = size.Width; Height = size.Height }
    static member internal From(rect: Common.Rect) =
        { X = rect.X; Y = rect.Y; Width = rect.Width; Height = rect.Height }
    member internal this.QtValue =
        Common.Rect(X = this.X, Y = this.Y, Width = this.Width, Height = this.Height)

type RectF = {
    X: double
    Y: double
    Width: double
    Height: double
} with
    static member From (x, y, width, height) =
        { X = x; Y = y; Width = width; Height = height }
    static member From (size: Size) =
        { X = 0; Y = 0; Width = size.Width; Height = size.Height }
    static member internal From(rect: Common.Rect) =
        { X = double rect.X; Y = rect.Y; Width = rect.Width; Height = rect.Height }
    static member internal From(rect: Common.RectF) =
        { X = rect.X; Y = rect.Y; Width = rect.Width; Height = rect.Height }
    member internal this.QtValue =
        Common.RectF(X = this.X, Y = this.Y, Width = this.Width, Height = this.Height)
        
type WindowModality =
    | NonModal
    | WindowModal
    | ApplicationModal
with
    member internal this.QtValue =
        match this with
        | NonModal -> Enums.WindowModality.NonModal
        | WindowModal -> Enums.WindowModality.WindowModal
        | ApplicationModal -> Enums.WindowModality.ApplicationModal
        
type ContextMenuPolicy =
    | NoContextMenu
    | DefaultContextMenu
    | ActionsContextMenu
    | CustomContextMenu
    | PreventContextMenu
with
    member internal this.QtValue =
        match this with
        | NoContextMenu -> Enums.ContextMenuPolicy.NoContextMenu
        | DefaultContextMenu -> Enums.ContextMenuPolicy.DefaultContextMenu
        | ActionsContextMenu -> Enums.ContextMenuPolicy.ActionsContextMenu
        | CustomContextMenu -> Enums.ContextMenuPolicy.CustomContextMenu
        | PreventContextMenu -> Enums.ContextMenuPolicy.PreventContextMenu
        
type CursorMoveStyle =
    | LogicalMoveStyle
    | VisualModeStyle
with
    member internal this.QtValue =
        match this with
        | LogicalMoveStyle -> Enums.CursorMoveStyle.LogicalMoveStyle
        | VisualModeStyle -> Enums.CursorMoveStyle.VisualMoveStyle
        
type DropAction =
    | Ignore
    | Copy
    | Move
    | Link
with
    static member internal From (qtDropAction: Enums.DropAction) =
        match qtDropAction with
        | Enums.DropAction.Ignore -> Ignore
        | Enums.DropAction.Copy -> Copy
        | Enums.DropAction.Move -> Move
        | Enums.DropAction.Link -> Link
        | _ -> failwith "DropAction.From - unhandled DropAction case (only move/copy/link supported)"
    member internal this.QtValue =
        match this with
        | Ignore -> Enums.DropAction.Ignore
        | Copy -> Enums.DropAction.Copy
        | Move -> Enums.DropAction.Move
        | Link -> Enums.DropAction.Link
    static member internal SetFrom (qtDropActionSet: Enums.DropActionSet) =
        let pairs = [
            // Enums.DropActionSet.Ignore, Ignore // 0 value
            Enums.DropActionSet.Copy, Copy
            Enums.DropActionSet.Move, Move
            Enums.DropActionSet.Link, Link
        ]
        (Set.empty<DropAction>, pairs)
        ||> List.fold (fun acc (flag, action) ->
            if qtDropActionSet.HasFlag flag then
                acc.Add(action)
            else
                acc)
    static member internal QtSetFrom (actions: DropAction seq) =
        (enum<Enums.DropActionSet> 0, actions)
        ||> Seq.fold (fun acc action ->
            let flag =
                match action with
                | Ignore -> Enums.DropActionSet.Ignore // 0 value, meaningless in a set
                | Copy -> Enums.DropActionSet.Copy
                | Move -> Enums.DropActionSet.Move
                | Link -> Enums.DropActionSet.Link
            acc ||| flag)

type TextFormat =
    | PlainText
    | RichText
    | AutoText
    | MarkdownText
with
    member internal this.QtValue =
        match this with
        | PlainText -> Enums.TextFormat.PlainText
        | RichText -> Enums.TextFormat.RichText
        | AutoText -> Enums.TextFormat.AutoText
        | MarkdownText -> Enums.TextFormat.MarkdownText
        
type TextInteractionFlag =
    | TextSelectableByMouse
    | TextSelectableByKeyboard
    | LinksAccessibleByMouse
    | LinksAccessibleByKeyboard
    | TextEditable
    | TextEditorInteraction
    | TextBrowserInteraction
with
    static member internal QtSetFrom (flags: TextInteractionFlag seq) =
        (enum<Enums.TextInteractionFlags> 0, flags)
        ||> Seq.fold (fun acc item ->
            let flag =
                match item with
                | TextSelectableByMouse -> Enums.TextInteractionFlags.TextSelectableByMouse
                | TextSelectableByKeyboard -> Enums.TextInteractionFlags.TextSelectableByMouse
                | LinksAccessibleByMouse -> Enums.TextInteractionFlags.TextSelectableByMouse
                | LinksAccessibleByKeyboard -> Enums.TextInteractionFlags.TextSelectableByMouse
                | TextEditable -> Enums.TextInteractionFlags.TextSelectableByMouse
                | TextEditorInteraction -> Enums.TextInteractionFlags.TextSelectableByMouse
                | TextBrowserInteraction -> Enums.TextInteractionFlags.TextSelectableByMouse
            acc ||| flag)
        
type CaseSensitivity =
    | CaseInsensitive
    | CaseSensitive
with
    static member internal From (qtValue: Enums.CaseSensitivity) =
        match qtValue with
        | Enums.CaseSensitivity.CaseInsensitive -> CaseInsensitive
        | Enums.CaseSensitivity.CaseSensitive -> CaseSensitive
        | _ -> failwithf "CaseSensitivity.From: unknown input value [%d]" (int qtValue)
    member internal this.QtValue =
        match this with
        | CaseInsensitive -> Enums.CaseSensitivity.CaseInsensitive
        | CaseSensitive -> Enums.CaseSensitivity.CaseSensitive
        
type TextElideMode =
    | ElideLeft
    | ElideRight
    | ElideMiddle
    | ElideNone
with
    member internal this.QtValue =
        match this with
        | ElideLeft -> Enums.TextElideMode.ElideLeft
        | ElideRight -> Enums.TextElideMode.ElideRight
        | ElideMiddle -> Enums.TextElideMode.ElideMiddle
        | ElideNone -> Enums.TextElideMode.ElideNone

type ShortcutContext =
    | WidgetShortcut
    | WindowShortcut
    | ApplicationShortcut
    | WidgetWithChildrenShortcut
with
    member internal this.QtValue =
        match this with
        | WidgetShortcut -> Enums.ShortcutContext.WidgetShortcut
        | WindowShortcut -> Enums.ShortcutContext.WindowShortcut
        | ApplicationShortcut -> Enums.ShortcutContext.ApplicationShortcut
        | WidgetWithChildrenShortcut -> Enums.ShortcutContext.WidgetWithChildrenShortcut
        
// in C++ Qt this is a class,
// will decide over time what it needs to be here ...
module TextOption =
    type WrapMode =
        | NoWrap
        | WordWrap
        | ManualWrap
        | WrapAnywhere
        | WrapAtWordBoundaryOrAnywhere
    with
        member internal this.QtValue =
            match this with
            | NoWrap -> TextOption.WrapMode.NoWrap
            | WordWrap -> TextOption.WrapMode.WordWrap
            | ManualWrap -> TextOption.WrapMode.ManualWrap
            | WrapAnywhere -> TextOption.WrapMode.WrapAnywhere
            | WrapAtWordBoundaryOrAnywhere -> TextOption.WrapMode.WrapAtWordBoundaryOrAnywhere
            
type TimerType =
    | PreciseTimer
    | CoarseTimer
    | VeryCoarseTimer
with
    member internal this.QtValue =
        match this with
        | PreciseTimer -> Enums.TimerType.PreciseTimer
        | CoarseTimer -> Enums.TimerType.CoarseTimer
        | VeryCoarseTimer -> Enums.TimerType.VeryCoarseTimer
        
type ToolBarArea =
    | LeftToolBarArea
    | RightToolBarArea
    | TopToolBarArea
    | BottomToolBarArea
with
    static member internal NoToolBarAreas = Set.empty<ToolBarArea>
    static member internal AllToolBarAreas =
        [ LeftToolBarArea; RightToolBarArea; TopToolBarArea; BottomToolBarArea ]
        |> Set.ofList
    member internal this.QtValue =
        match this with
        | LeftToolBarArea -> Enums.ToolBarAreas.LeftToolBarArea
        | RightToolBarArea -> Enums.ToolBarAreas.RightToolBarArea
        | TopToolBarArea -> Enums.ToolBarAreas.TopToolBarArea
        | BottomToolBarArea -> Enums.ToolBarAreas.BottomToolBarArea
        // | AllToolBarAreas -> Enums.ToolBarAreas.AllToolBarAreas
        // | NoToolBarArea -> Enums.ToolBarAreas.NoToolBarArea
    static member internal QtSetFrom (values: ToolBarArea seq) =
        (enum<Enums.ToolBarAreas> 0, values)
        ||> Seq.fold (fun acc item ->
            acc ||| item.QtValue)
    static member internal SetFrom (qtValues: Enums.ToolBarAreas) =
        let pairs = [
            Enums.ToolBarAreas.LeftToolBarArea, LeftToolBarArea
            Enums.ToolBarAreas.RightToolBarArea, RightToolBarArea
            Enums.ToolBarAreas.TopToolBarArea, TopToolBarArea
            Enums.ToolBarAreas.BottomToolBarArea, BottomToolBarArea
        ]
        (Set.empty<ToolBarArea>, pairs)
        ||> List.fold (fun acc (flag, value) ->
            if qtValues.HasFlag(flag) then
                acc.Add(value)
            else
                acc)

type ToolButtonStyle =
    | ToolButtonIconOnly
    | ToolButtonTextOnly
    | ToolButtonTextBesideIcon
    | ToolButtonTextUnderIcon
    | ToolButtonFollowStyle
with
    member internal this.QtValue =
        match this with
        | ToolButtonIconOnly -> Enums.ToolButtonStyle.ToolButtonIconOnly
        | ToolButtonTextOnly -> Enums.ToolButtonStyle.ToolButtonTextOnly
        | ToolButtonTextBesideIcon -> Enums.ToolButtonStyle.ToolButtonTextBesideIcon
        | ToolButtonTextUnderIcon -> Enums.ToolButtonStyle.ToolButtonTextUnderIcon
        | ToolButtonFollowStyle -> Enums.ToolButtonStyle.ToolButtonFollowStyle
    static member internal From (style: Enums.ToolButtonStyle) =
        match style with
        | Enums.ToolButtonStyle.ToolButtonIconOnly -> ToolButtonIconOnly
        | Enums.ToolButtonStyle.ToolButtonTextOnly -> ToolButtonTextOnly
        | Enums.ToolButtonStyle.ToolButtonTextBesideIcon -> ToolButtonTextBesideIcon
        | Enums.ToolButtonStyle.ToolButtonTextUnderIcon -> ToolButtonTextUnderIcon
        | Enums.ToolButtonStyle.ToolButtonFollowStyle -> ToolButtonFollowStyle
        | _ -> failwith "ToolButtonStyle.From - unknown input value"
    
type ThemeIcon =
    | AddressBookNew = 0
    | ApplicationExit = 1
    | AppointmentNew = 2
    | CallStart = 3
    | CallStop = 4
    | ContactNew = 5
    | DocumentNew = 6
    | DocumentOpen = 7
    | DocumentOpenRecent = 8
    | DocumentPageSetup = 9
    | DocumentPrint = 10
    | DocumentPrintPreview = 11
    | DocumentProperties = 12
    | DocumentRevert = 13
    | DocumentSave = 14
    | DocumentSaveAs = 15
    | DocumentSend = 16
    | EditClear = 17
    | EditCopy = 18
    | EditCut = 19
    | EditDelete = 20
    | EditFind = 21
    | EditPaste = 22
    | EditRedo = 23
    | EditSelectAll = 24
    | EditUndo = 25
    | FolderNew = 26
    | FormatIndentLess = 27
    | FormatIndentMore = 28
    | FormatJustifyCenter = 29
    | FormatJustifyFill = 30
    | FormatJustifyLeft = 31
    | FormatJustifyRight = 32
    | FormatTextDirectionLtr = 33
    | FormatTextDirectionRtl = 34
    | FormatTextBold = 35
    | FormatTextItalic = 36
    | FormatTextUnderline = 37
    | FormatTextStrikethrough = 38
    | GoDown = 39
    | GoHome = 40
    | GoNext = 41
    | GoPrevious = 42
    | GoUp = 43
    | HelpAbout = 44
    | HelpFaq = 45
    | InsertImage = 46
    | InsertLink = 47
    | InsertText = 48
    | ListAdd = 49
    | ListRemove = 50
    | MailForward = 51
    | MailMarkImportant = 52
    | MailMarkRead = 53
    | MailMarkUnread = 54
    | MailMessageNew = 55
    | MailReplyAll = 56
    | MailReplySender = 57
    | MailSend = 58
    | MediaEject = 59
    | MediaPlaybackPause = 60
    | MediaPlaybackStart = 61
    | MediaPlaybackStop = 62
    | MediaRecord = 63
    | MediaSeekBackward = 64
    | MediaSeekForward = 65
    | MediaSkipBackward = 66
    | MediaSkipForward = 67
    | ObjectRotateLeft = 68
    | ObjectRotateRight = 69
    | ProcessStop = 70
    | SystemLockScreen = 71
    | SystemLogOut = 72
    | SystemSearch = 73
    | SystemReboot = 74
    | SystemShutdown = 75
    | ToolsCheckSpelling = 76
    | ViewFullscreen = 77
    | ViewRefresh = 78
    | ViewRestore = 79
    | WindowClose = 80
    | WindowNew = 81
    | ZoomFitBest = 82
    | ZoomIn = 83
    | ZoomOut = 84
    | AudioCard = 85
    | AudioInputMicrophone = 86
    | Battery = 87
    | CameraPhoto = 88
    | CameraVideo = 89
    | CameraWeb = 90
    | Computer = 91
    | DriveHarddisk = 92
    | DriveOptical = 93
    | InputGaming = 94
    | InputKeyboard = 95
    | InputMouse = 96
    | InputTablet = 97
    | MediaFlash = 98
    | MediaOptical = 99
    | MediaTape = 100
    | MultimediaPlayer = 101
    | NetworkWired = 102
    | NetworkWireless = 103
    | Phone = 104
    | Printer = 105
    | Scanner = 106
    | VideoDisplay = 107
    | AppointmentMissed = 108
    | AppointmentSoon = 109
    | AudioVolumeHigh = 110
    | AudioVolumeLow = 111
    | AudioVolumeMedium = 112
    | AudioVolumeMuted = 113
    | BatteryCaution = 114
    | BatteryLow = 115
    | DialogError = 116
    | DialogInformation = 117
    | DialogPassword = 118
    | DialogQuestion = 119
    | DialogWarning = 120
    | FolderDragAccept = 121
    | FolderOpen = 122
    | FolderVisiting = 123
    | ImageLoading = 124
    | ImageMissing = 125
    | MailAttachment = 126
    | MailUnread = 127
    | MailRead = 128
    | MailReplied = 129
    | MediaPlaylistRepeat = 130
    | MediaPlaylistShuffle = 131
    | NetworkOffline = 132
    | PrinterPrinting = 133
    | SecurityHigh = 134
    | SecurityLow = 135
    | SoftwareUpdateAvailable = 136
    | SoftwareUpdateUrgent = 137
    | SyncError = 138
    | SyncSynchronizing = 139
    | UserAvailable = 140
    | UserOffline = 141
    | WeatherClear = 142
    | WeatherClearNight = 143
    | WeatherFewClouds = 144
    | WeatherFewCloudsNight = 145
    | WeatherFog = 146
    | WeatherShowers = 147
    | WeatherSnow = 148
    | WeatherStorm = 149
    
let internal toQtThemeIcon (icon: ThemeIcon) =
    enum<Icon.ThemeIcon> (int icon)
    
type ItemDataRole =
    | DisplayRole
    | DecorationRole
    | EditRole
    | ToolTipRole
    | StatusTipRole
    | WhatsThisRole
    | FontRole
    | TextAlignmentRole
    | BackgroundRole
    | ForegroundRole
    | CheckStateRole
    | AccessibleTextRole
    | AccessibleDescriptionRole
    | SizeHintRole
    | InitialSortOrderRole
    | DisplayPropertyRole
    | DecorationPropertyRole
    | ToolTipPropertyRole
    | StatusTipPropertyRole
    | WhatsThisPropertyRole
    | UserRole of value: int   // over 0x0100
with
    member this.QtValue =
        match this with
        | DisplayRole -> Enums.ItemDataRole.DisplayRole
        | DecorationRole -> Enums.ItemDataRole.DecorationRole
        | EditRole -> Enums.ItemDataRole.EditRole
        | ToolTipRole -> Enums.ItemDataRole.ToolTipRole
        | StatusTipRole -> Enums.ItemDataRole.StatusTipRole
        | WhatsThisRole -> Enums.ItemDataRole.WhatsThisRole
        | FontRole -> Enums.ItemDataRole.FontRole
        | TextAlignmentRole -> Enums.ItemDataRole.TextAlignmentRole
        | BackgroundRole -> Enums.ItemDataRole.BackgroundRole
        | ForegroundRole -> Enums.ItemDataRole.ForegroundRole
        | CheckStateRole -> Enums.ItemDataRole.CheckStateRole
        | AccessibleTextRole -> Enums.ItemDataRole.AccessibleTextRole
        | AccessibleDescriptionRole -> Enums.ItemDataRole.AccessibleDescriptionRole
        | SizeHintRole -> Enums.ItemDataRole.SizeHintRole
        | InitialSortOrderRole -> Enums.ItemDataRole.InitialSortOrderRole
        | DisplayPropertyRole -> Enums.ItemDataRole.DisplayPropertyRole
        | DecorationPropertyRole -> Enums.ItemDataRole.DecorationPropertyRole
        | ToolTipPropertyRole -> Enums.ItemDataRole.ToolTipPropertyRole
        | StatusTipPropertyRole -> Enums.ItemDataRole.StatusTipPropertyRole
        | WhatsThisPropertyRole -> Enums.ItemDataRole.WhatsThisPropertyRole
        | UserRole value -> enum<Enums.ItemDataRole> value
    static member From (role: Enums.ItemDataRole) =
        match role with
        | Enums.ItemDataRole.DisplayRole -> DisplayRole
        | Enums.ItemDataRole.DecorationRole -> DecorationRole
        | Enums.ItemDataRole.EditRole -> EditRole
        | Enums.ItemDataRole.ToolTipRole -> ToolTipRole
        | Enums.ItemDataRole.StatusTipRole -> StatusTipRole
        | Enums.ItemDataRole.WhatsThisRole -> WhatsThisRole
        | Enums.ItemDataRole.FontRole -> FontRole
        | Enums.ItemDataRole.TextAlignmentRole -> TextAlignmentRole
        | Enums.ItemDataRole.BackgroundRole -> BackgroundRole
        | Enums.ItemDataRole.ForegroundRole -> ForegroundRole
        | Enums.ItemDataRole.CheckStateRole -> CheckStateRole
        | Enums.ItemDataRole.AccessibleTextRole -> AccessibleTextRole
        | Enums.ItemDataRole.AccessibleDescriptionRole -> AccessibleDescriptionRole
        | Enums.ItemDataRole.SizeHintRole -> SizeHintRole
        | Enums.ItemDataRole.InitialSortOrderRole -> InitialSortOrderRole
        | Enums.ItemDataRole.DisplayPropertyRole -> DisplayPropertyRole
        | Enums.ItemDataRole.DecorationPropertyRole -> DecorationPropertyRole
        | Enums.ItemDataRole.ToolTipPropertyRole -> ToolTipPropertyRole
        | Enums.ItemDataRole.StatusTipPropertyRole -> StatusTipPropertyRole
        | Enums.ItemDataRole.WhatsThisPropertyRole -> WhatsThisPropertyRole
        | _ ->
            let value = int role
            if value >= 0x100 then
                UserRole value
            else
                failwithf "DataRole.From: unknown input value [%d]" value
                
// // for utility widgets (synthetic layout widgets etc)
// NEW: just use Widget.CreateNoHandler()
// type internal NullWidgetHandler() =
//     interface Widget.SignalHandler with
//         member this.Destroyed obj =
//             ()
//         member this.ObjectNameChanged name =
//             ()
//         member this.CustomContextMenuRequested pos =
//             ()
//         member this.WindowIconChanged icon =
//             ()
//         member this.WindowTitleChanged title =
//             ()
//         member this.Dispose() =
//             ()
    
// for anything where we don't want users to be dealing with Org.Whatever.QtTesting namespace (generated C# code)
// generally these are for signals and callbacks of various kinds where the user might need to query some values

// but ideally the node/view API will use deferred stuff (eg Icon.Deferred) instead of proxies,
// because we don't want users to be responsible for lifetimes (disposal) on these things
// anything the user would have created on the stack, ideally shouldn't use a proxy

type QObjectProxy internal(handle: Object.Handle) =
    let x = 10

type WidgetProxy internal(handle: Widget.Handle) =
    // member val widget = widget
    member this.Rect =
        Rect.From(handle.Rect())
        
type AbstractButtonProxy internal(handle: AbstractButton.Handle) =
    let x = 10

type ActionProxy internal(action: Action.Handle) =
    // not sure what methods/props will be useful yet
    let x = 10

type IconProxy internal(icon: Icon.Handle) =
    // just put this here due to a signal needing it
    // user-created icons (for node construction) are further below
    let x = 10
    
type DockWidgetProxy internal(widget: DockWidget.Handle) =
    let x = 10
    
type MimeDataProxy internal(qMimeData: Widget.MimeData) =
    member val qMimeData = qMimeData
    member this.HasFormat(mimeType: string) =
        qMimeData.HasFormat(mimeType)
    member this.Text =
        qMimeData.Text()
    member this.Urls =
        qMimeData.Urls()
        

// might merge these ModelIndex types in the future, going to see how it plays out
// kind of don't like putting IDisposable on the proxy/deferred versions since they aren't owned, and then get IDE warnings about using 'new' etc
// but we'll see. juggling 3 different ones for different purposes seems kind of crazy (when they all represent a C++ QModelIndex in the end)
// even 'from scratch' they need to be created from a model somewhere, so would the 'new' be that big a deal?

type ModelIndexProxy internal(index: ModelIndex.Handle) =
    member val internal Index = index
    member this.IsValid =
        index.IsValid()
    member this.Row =
        index.Row()
    member this.Column =
        index.Column()
    
type ModelIndexOwned internal(index: Org.Whatever.QtTesting.ModelIndex.OwnedHandle) =
    let mutable disposed = false
    
    override this.Finalize() =
        (this :> IDisposable).Dispose()
        
    interface IDisposable with
        member this.Dispose() =
            if not disposed then
                index.Dispose()
                disposed <- true
                
    member this.IsValid =
        index.IsValid()
    member this.Row =
        index.Row()
    member this.Column =
        index.Column()
        
type ModelIndexDeferred private(deferred: ModelIndex.Deferred) =
    member val internal QtValue = deferred
    internal new(owned: ModelIndex.OwnedHandle) =
        ModelIndexDeferred(ModelIndex.Deferred.FromOwned(owned))
    internal new(handle: ModelIndex.Handle) =
        ModelIndexDeferred(ModelIndex.Deferred.FromHandle(handle))
        
// persistent model index
        
type PersistentModelIndexProxy internal(index: PersistentModelIndex.Handle) =
    member val internal Index = index

// see notes on ModelIndex* stuff above, similar will apply here
// maybe we need a "stack types" module or something?
type SizePolicyDeferred private(deferred: Org.Whatever.QtTesting.SizePolicy.Deferred) =
    member val internal QtValue = deferred
    
// other =========================

type Icon private(deferred: Org.Whatever.QtTesting.Icon.Deferred) =
    member val internal QtValue = deferred
    new() =
        let deferred =
            Icon.Deferred.Empty()
        Icon(deferred)
    new (filename: string) =
        let deferred =
            Icon.Deferred.FromFilename(filename)
        Icon(deferred)
    new (themeIcon: ThemeIcon) =
        let deferred =
            Icon.Deferred.FromThemeIcon(toQtThemeIcon themeIcon)
        Icon(deferred)
        
type ColorConstant =
    | Black
    | White
    | DarkGray
    | Gray
    | LightGray
    | Red
    | Green
    | Blue
    | Cyan
    | Magenta
    | Yellow
    | DarkRed
    | DarkGreen
    | DarkBlue
    | DarkCyan
    | DarkMagenta
    | DarkYellow
    | Transparent
with
    member internal this.QtValue =
        match this with
        | Black -> PaintResources.Color.Constant.Black
        | White -> PaintResources.Color.Constant.White
        | DarkGray -> PaintResources.Color.Constant.DarkGray
        | Gray -> PaintResources.Color.Constant.Gray
        | LightGray -> PaintResources.Color.Constant.LightGray
        | Red -> PaintResources.Color.Constant.Red
        | Green -> PaintResources.Color.Constant.Green
        | Blue -> PaintResources.Color.Constant.Blue
        | Cyan -> PaintResources.Color.Constant.Cyan
        | Magenta -> PaintResources.Color.Constant.Magenta
        | Yellow -> PaintResources.Color.Constant.Yellow
        | DarkRed -> PaintResources.Color.Constant.DarkRed
        | DarkGreen -> PaintResources.Color.Constant.DarkGreen
        | DarkBlue -> PaintResources.Color.Constant.DarkBlue
        | DarkCyan -> PaintResources.Color.Constant.DarkCyan
        | DarkMagenta -> PaintResources.Color.Constant.Magenta
        | DarkYellow -> PaintResources.Color.Constant.Yellow
        | Transparent -> PaintResources.Color.Constant.Transparent
        
type Color private(deferred: PaintResources.Color.Deferred) =
    member val internal QtValue = deferred
    new(constant: ColorConstant) =
        Color(PaintResources.Color.Deferred.FromConstant(constant.QtValue))
    new(r: int, g: int, b: int) =
        Color(PaintResources.Color.Deferred.FromRGB(r, g, b))
    new(r: int, g: int, b: int, a: int) =
        Color(PaintResources.Color.Deferred.FromRGBA(r, g, b, a))
    new(r: float, g: float, b: float) =
        Color(PaintResources.Color.Deferred.FromRGBF(float32 r, float32 g, float32 b))
    new(r: float, g: float, b: float, a: float) =
        Color(PaintResources.Color.Deferred.FromRGBAF(float32 r, float32 g, float32 b, float32 a))

type KeySequence private(deferred: Org.Whatever.QtTesting.KeySequence.Deferred) =
    member val internal QtValue = deferred
    new(str: string) =
        let deferred =
            KeySequence.Deferred.FromString(str)
        KeySequence(deferred)
    new(stdKey: StandardKey) =
        let deferred =
            KeySequence.Deferred.FromStandard(toQtStandardKey stdKey)
        KeySequence(deferred)
    new(key: Key, ?modifiers: Modifier list) =
        let deferred =
            let mods =
                defaultArg modifiers []
                |> Set.ofList
            KeySequence.Deferred.FromKey(toQtKey key, Modifier.QtSetFrom mods)
        KeySequence(deferred)
    
[<RequireQualifiedAccess>]
type Variant =
    | Empty
    | String of str: string
    | Int of value: int
    | Icon of icon: Icon
    | Color of color: Color
with
    member this.QtValue =
        match this with
        | Empty -> Variant.Deferred.Empty() :> Org.Whatever.QtTesting.Variant.Deferred
        | String str -> Variant.Deferred.FromString(str)
        | Int value -> Variant.Deferred.FromInt(value)
        | Icon icon -> Variant.Deferred.FromIcon(icon.QtValue)
        | Color color -> Variant.Deferred.FromColor(color.QtValue)
        
type RegexOption =
    | CaseInsensitive
    | DotMatchesEverything
    | Multiline
    | ExtendedPatternSyntax
    | InvertedGreediness
    | DontCapture
    | UseUnicodeProperties
with
    static member QtSetFrom (options: RegexOption seq) =
        (enum<RegularExpression.PatternOptions> 0, options)
        ||> Seq.fold (fun acc option ->
            let flag =
                match option with
                | CaseInsensitive -> RegularExpression.PatternOptions.CaseInsensitiveOption
                | DotMatchesEverything -> RegularExpression.PatternOptions.DotMatchesEverythingOption
                | Multiline -> RegularExpression.PatternOptions.MultilineOption
                | ExtendedPatternSyntax -> RegularExpression.PatternOptions.ExtendedPatternSyntaxOption
                | InvertedGreediness -> RegularExpression.PatternOptions.InvertedGreedinessOption
                | DontCapture -> RegularExpression.PatternOptions.DontCaptureOption
                | UseUnicodeProperties -> RegularExpression.PatternOptions.UseUnicodePropertiesOption
            acc ||| flag)
    
type Regex private(deferred: RegularExpression.Deferred) =
    member val internal QtValue = deferred
    new() =
        Regex(RegularExpression.Deferred.Empty())
    new(pattern: string, ?options: RegexOption seq)=
        let deferred =
            let options' =
                defaultArg options Set.empty
                |> RegexOption.QtSetFrom
            RegularExpression.Deferred.Regex(pattern, options')
        Regex(deferred)
