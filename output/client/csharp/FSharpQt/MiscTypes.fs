module FSharpQt.MiscTypes

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
        | Fixed -> Enums.SizePolicy.Policy.Fixed
        | Minimum -> Enums.SizePolicy.Policy.Minimum
        | Maximum -> Enums.SizePolicy.Policy.Maximum
        | Preferred -> Enums.SizePolicy.Policy.Preferred
        | MinimumExpanding -> Enums.SizePolicy.Policy.MinimumExpanding
        | Expanding -> Enums.SizePolicy.Policy.Expanding
        | Ignored -> Enums.SizePolicy.Policy.Ignored

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
        
// various enums needed before widget proxies below:

type ToolButtonStyle =
    | IconOnly
    | TextOnly
    | TextBesideIcon
    | TextUnderIcon
    | FollowStyle
with
    member internal this.QtValue =
        match this with
        | IconOnly -> Enums.ToolButtonStyle.IconOnly
        | TextOnly -> Enums.ToolButtonStyle.TextOnly
        | TextBesideIcon -> Enums.ToolButtonStyle.TextBesideIcon
        | TextUnderIcon -> Enums.ToolButtonStyle.TextUnderIcon
        | FollowStyle -> Enums.ToolButtonStyle.FollowStyle
    static member internal From (style: Enums.ToolButtonStyle) =
        match style with
        | Enums.ToolButtonStyle.IconOnly -> IconOnly
        | Enums.ToolButtonStyle.TextOnly -> TextOnly
        | Enums.ToolButtonStyle.TextBesideIcon -> TextBesideIcon
        | Enums.ToolButtonStyle.TextUnderIcon -> TextUnderIcon
        | Enums.ToolButtonStyle.FollowStyle -> FollowStyle
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
    
type DataRole =
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
    member this.QtRole =
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
    
[<RequireQualifiedAccess>]
type Variant =
    | Empty
    | String of str: string
with
    member this.QtValue =
        match this with
        | Empty -> Variant.Deferred.Empty() :> Org.Whatever.QtTesting.Variant.Deferred
        | String str -> Variant.Deferred.FromString(str)
    
// for utility widgets (synthetic layout widgets etc)

type internal NullWidgetHandler() =
    interface Widget.SignalHandler with
        member this.CustomContextMenuRequested pos =
            ()
        member this.WindowIconChanged icon =
            ()
        member this.WindowTitleChanged title =
            ()
        member this.Dispose() =
            ()
    
// for anything where we don't want users to be dealing with Org.Whatever.QtTesting namespace (generated C# code)
// generally these are for signals and callbacks of various kinds where the user might need to query some values

// but ideally the node/view API will use deferred stuff (eg Icon.Deferred) instead of proxies,
// because we don't want users to be responsible for lifetimes (disposal) on these things
// anything the user would have created on the stack, ideally shouldn't use a proxy

type WidgetProxy internal(handle: Widget.Handle) =
    // member val widget = widget
    member this.Rect =
        Rect.From(handle.GetRect())

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
        
type ModelIndexProxy internal(index: ModelIndex.Handle) =
    let x = 10
    
// experimenting for extreme cases:
   
type ProxyBase<'handle> internal() =
    member val internal Handle: 'handle = Unchecked.defaultof<'handle> with get, set

type PlainTextEditProxy() =
    inherit ProxyBase<PlainTextEdit.Handle>()
    internal new(handle: PlainTextEdit.Handle) =
        base.Handle <- handle
        PlainTextEditProxy()
    member this.ToPlainText () =
        this.Handle.ToPlainText()

// other =========================

type Icon private(deferred: Org.Whatever.QtTesting.Icon.Deferred) =
    member val internal QtValue = deferred
    new (filename: string) =
        let deferred =
            Icon.Deferred.FromFilename(filename)
        Icon(deferred)
    new (themeIcon: ThemeIcon) =
        let deferred =
            Icon.Deferred.FromThemeIcon(toQtThemeIcon themeIcon)
        Icon(deferred)

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
