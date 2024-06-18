module FSharpQt.InputEnums

open System.Collections.Generic
open Org.Whatever.QtTesting

type MouseButton =
    | LeftButton
    | RightButton
    | MiddleButton
    | OtherButton
with
    static member internal From (qtButton: Enums.MouseButton) =
        match qtButton with
        | Enums.MouseButton.None -> failwith "MouseButton.From - .None case - shoudln't happen"
        | Enums.MouseButton.Left -> LeftButton
        | Enums.MouseButton.Right -> RightButton
        | Enums.MouseButton.Middle -> MiddleButton
        | _ -> OtherButton // also handles other enum cases
    member internal this.QtValue =
        match this with
        | LeftButton -> Enums.MouseButton.Left
        | RightButton -> Enums.MouseButton.Right
        | MiddleButton -> Enums.MouseButton.Middle
        | OtherButton -> Enums.MouseButton.Other
    static member internal SetFrom (qtButtonSet: HashSet<Enums.MouseButton>) =
        (set qtButtonSet)
        |> Set.map MouseButton.From
    
type Modifier =
    | Shift
    | Control
    | Alt
    | Meta
with
    static member internal From (qtModifier: Enums.Modifier) =
        match qtModifier with
        | Enums.Modifier.Shift -> Shift
        | Enums.Modifier.Control -> Control
        | Enums.Modifier.Alt -> Alt
        | Enums.Modifier.Meta -> Meta
        | _ -> failwith "Modifier.From - unknown enum value (or .None, which shouldn't happen)"
    static member internal SetFrom (qtModifierSet: HashSet<Enums.Modifier>) =
        (set qtModifierSet)
        |> Set.map Modifier.From
    static member internal QtSetFrom (modifiers: Set<Modifier>) =
        HashSet<Enums.Modifier>(modifiers |> Set.map (_.QtValue))
    member internal this.QtValue =
        match this with
        | Shift -> Enums.Modifier.Shift
        | Control -> Enums.Modifier.Control
        | Alt -> Enums.Modifier.Alt
        | Meta -> Enums.Modifier.Meta

type StandardKey =
    | UnknownKey
    | HelpContents
    | WhatsThis
    | Open
    | Close
    | Save
    | New
    | Delete
    | Cut
    | Copy
    | Paste
    | Undo
    | Redo
    | Back
    | Forward
    | Refresh
    | ZoomIn
    | ZoomOut
    | Print
    | AddTab
    | NextChild
    | PreviousChild
    | Find
    | FindNext
    | FindPrevious
    | Replace
    | SelectAll
    | Bold
    | Italic
    | Underline
    | MoveToNextChar
    | MoveToPreviousChar
    | MoveToNextWord
    | MoveToPreviousWord
    | MoveToNextLine
    | MoveToPreviousLine
    | MoveToNextPage
    | MoveToPreviousPage
    | MoveToStartOfLine
    | MoveToEndOfLine
    | MoveToStartOfBlock
    | MoveToEndOfBlock
    | MoveToStartOfDocument
    | MoveToEndOfDocument
    | SelectNextChar
    | SelectPreviousChar
    | SelectNextWord
    | SelectPreviousWord
    | SelectNextLine
    | SelectPreviousLine
    | SelectNextPage
    | SelectPreviousPage
    | SelectStartOfLine
    | SelectEndOfLine
    | SelectStartOfBlock
    | SelectEndOfBlock
    | SelectStartOfDocument
    | SelectEndOfDocument
    | DeleteStartOfWord
    | DeleteEndOfWord
    | DeleteEndOfLine
    | InsertParagraphSeparator
    | InsertLineSeparator
    | SaveAs
    | Preferences
    | Quit
    | FullScreen
    | Deselect
    | DeleteCompleteLine
    | Backspace
    | Cancel
with
    member this.QtValue =
        match this with
        | UnknownKey -> KeySequence.StandardKey.UnknownKey
        | HelpContents -> KeySequence.StandardKey.HelpContents
        | WhatsThis -> KeySequence.StandardKey.WhatsThis
        | Open -> KeySequence.StandardKey.Open
        | Close -> KeySequence.StandardKey.Close
        | Save -> KeySequence.StandardKey.Save
        | New -> KeySequence.StandardKey.New
        | Delete -> KeySequence.StandardKey.Delete
        | Cut -> KeySequence.StandardKey.Cut
        | Copy -> KeySequence.StandardKey.Copy
        | Paste -> KeySequence.StandardKey.Paste
        | Undo -> KeySequence.StandardKey.Undo
        | Redo -> KeySequence.StandardKey.Redo
        | Back -> KeySequence.StandardKey.Back
        | Forward -> KeySequence.StandardKey.Forward
        | Refresh -> KeySequence.StandardKey.Refresh
        | ZoomIn -> KeySequence.StandardKey.ZoomIn
        | ZoomOut -> KeySequence.StandardKey.ZoomOut
        | Print -> KeySequence.StandardKey.Print
        | AddTab -> KeySequence.StandardKey.AddTab
        | NextChild -> KeySequence.StandardKey.NextChild
        | PreviousChild -> KeySequence.StandardKey.PreviousChild
        | Find -> KeySequence.StandardKey.Find
        | FindNext -> KeySequence.StandardKey.FindNext
        | FindPrevious -> KeySequence.StandardKey.FindPrevious
        | Replace -> KeySequence.StandardKey.Replace
        | SelectAll -> KeySequence.StandardKey.SelectAll
        | Bold -> KeySequence.StandardKey.Bold
        | Italic -> KeySequence.StandardKey.Italic
        | Underline -> KeySequence.StandardKey.Underline
        | MoveToNextChar -> KeySequence.StandardKey.MoveToNextChar
        | MoveToPreviousChar -> KeySequence.StandardKey.MoveToPreviousChar
        | MoveToNextWord -> KeySequence.StandardKey.MoveToNextWord
        | MoveToPreviousWord -> KeySequence.StandardKey.MoveToPreviousWord
        | MoveToNextLine -> KeySequence.StandardKey.MoveToNextLine
        | MoveToPreviousLine -> KeySequence.StandardKey.MoveToPreviousLine
        | MoveToNextPage -> KeySequence.StandardKey.MoveToNextPage
        | MoveToPreviousPage -> KeySequence.StandardKey.MoveToPreviousPage
        | MoveToStartOfLine -> KeySequence.StandardKey.MoveToStartOfLine
        | MoveToEndOfLine -> KeySequence.StandardKey.MoveToEndOfLine
        | MoveToStartOfBlock -> KeySequence.StandardKey.MoveToStartOfBlock
        | MoveToEndOfBlock -> KeySequence.StandardKey.MoveToEndOfBlock
        | MoveToStartOfDocument -> KeySequence.StandardKey.MoveToStartOfDocument
        | MoveToEndOfDocument -> KeySequence.StandardKey.MoveToEndOfDocument
        | SelectNextChar -> KeySequence.StandardKey.SelectNextChar
        | SelectPreviousChar -> KeySequence.StandardKey.SelectPreviousChar
        | SelectNextWord -> KeySequence.StandardKey.SelectNextWord
        | SelectPreviousWord -> KeySequence.StandardKey.SelectPreviousWord
        | SelectNextLine -> KeySequence.StandardKey.SelectNextLine
        | SelectPreviousLine -> KeySequence.StandardKey.SelectPreviousLine
        | SelectNextPage -> KeySequence.StandardKey.SelectNextPage
        | SelectPreviousPage -> KeySequence.StandardKey.SelectPreviousPage
        | SelectStartOfLine -> KeySequence.StandardKey.SelectStartOfLine
        | SelectEndOfLine -> KeySequence.StandardKey.SelectEndOfLine
        | SelectStartOfBlock -> KeySequence.StandardKey.SelectStartOfBlock
        | SelectEndOfBlock -> KeySequence.StandardKey.SelectEndOfBlock
        | SelectStartOfDocument -> KeySequence.StandardKey.SelectStartOfDocument
        | SelectEndOfDocument -> KeySequence.StandardKey.SelectEndOfDocument
        | DeleteStartOfWord -> KeySequence.StandardKey.DeleteStartOfWord
        | DeleteEndOfWord -> KeySequence.StandardKey.DeleteEndOfWord
        | DeleteEndOfLine -> KeySequence.StandardKey.DeleteEndOfLine
        | InsertParagraphSeparator -> KeySequence.StandardKey.InsertParagraphSeparator
        | InsertLineSeparator -> KeySequence.StandardKey.InsertLineSeparator
        | SaveAs -> KeySequence.StandardKey.SaveAs
        | Preferences -> KeySequence.StandardKey.Preferences
        | Quit -> KeySequence.StandardKey.Quit
        | FullScreen -> KeySequence.StandardKey.FullScreen
        | Deselect -> KeySequence.StandardKey.Deselect
        | DeleteCompleteLine -> KeySequence.StandardKey.DeleteCompleteLine
        | Backspace -> KeySequence.StandardKey.Backspace
        | Cancel -> KeySequence.StandardKey.Cancel

[<RequireQualifiedAccess>]
type Key =
    | Space
    | Any
    | Exclam
    | QuoteDbl
    | NumberSign
    | Dollar
    | Percent
    | Ampersand
    | Apostrophe
    | ParenLeft
    | ParenRight
    | Asterisk
    | Plus
    | Comma
    | Minus
    | Period
    | Slash
    | Key0
    | Key1
    | Key2
    | Key3
    | Key4
    | Key5
    | Key6
    | Key7
    | Key8
    | Key9
    | Colon
    | Semicolon
    | Less
    | Equal
    | Greater
    | Question
    | At
    | A
    | B
    | C
    | D
    | E
    | F
    | G
    | H
    | I
    | J
    | K
    | L
    | M
    | N
    | O
    | P
    | Q
    | R
    | S
    | T
    | U
    | V
    | W
    | X
    | Y
    | Z
    | BracketLeft
    | Backslash
    | BracketRight
    | AsciiCircum
    | Underscore
    | QuoteLeft
    | BraceLeft
    | Bar
    | BraceRight
    | AsciiTilde
    | nobreakspace
    | exclamdown
    | cent
    | sterling
    | currency
    | yen
    | brokenbar
    | section
    | diaeresis
    | copyright
    | ordfeminine
    | guillemotleft
    | notsign
    | hyphen
    | registered
    | macron
    | degree
    | plusminus
    | twosuperior
    | threesuperior
    | acute
    | micro
    | paragraph
    | periodcentered
    | cedilla
    | onesuperior
    | masculine
    | guillemotright
    | onequarter
    | onehalf
    | threequarters
    | questiondown
    | Agrave
    | Aacute
    | Acircumflex
    | Atilde
    | Adiaeresis
    | Aring
    | AE
    | Ccedilla
    | Egrave
    | Eacute
    | Ecircumflex
    | Ediaeresis
    | Igrave
    | Iacute
    | Icircumflex
    | Idiaeresis
    | ETH
    | Ntilde
    | Ograve
    | Oacute
    | Ocircumflex
    | Otilde
    | Odiaeresis
    | multiply
    | Ooblique
    | Ugrave
    | Uacute
    | Ucircumflex
    | Udiaeresis
    | Yacute
    | THORN
    | ssharp
    | division
    | ydiaeresis
    | Escape
    | Tab
    | Backtab
    | Backspace
    | Return
    | Enter
    | Insert
    | Delete
    | Pause
    | Print
    | SysReq
    | Clear
    | Home
    | End
    | Left
    | Up
    | Right
    | Down
    | PageUp
    | PageDown
    | Shift
    | Control
    | Meta
    | Alt
    | CapsLock
    | NumLock
    | ScrollLock
    | F1
    | F2
    | F3
    | F4
    | F5
    | F6
    | F7
    | F8
    | F9
    | F10
    | F11
    | F12
    | F13
    | F14
    | F15
    | F16
    | F17
    | F18
    | F19
    | F20
    | F21
    | F22
    | F23
    | F24
    | F25
    | F26
    | F27
    | F28
    | F29
    | F30
    | F31
    | F32
    | F33
    | F34
    | F35
    | Super_L
    | Super_R
    | Menu
    | Hyper_L
    | Hyper_R
    | Help
    | Direction_L
    | Direction_R
    | AltGr
    | Multi_key
    | Codeinput
    | SingleCandidate
    | MultipleCandidate
    | PreviousCandidate
    | Mode_switch
    | Kanji
    | Muhenkan
    | Henkan
    | Romaji
    | Hiragana
    | Katakana
    | Hiragana_Katakana
    | Zenkaku
    | Hankaku
    | Zenkaku_Hankaku
    | Touroku
    | Massyo
    | Kana_Lock
    | Kana_Shift
    | Eisu_Shift
    | Eisu_toggle
    | Hangul
    | Hangul_Start
    | Hangul_End
    | Hangul_Hanja
    | Hangul_Jamo
    | Hangul_Romaja
    | Hangul_Jeonja
    | Hangul_Banja
    | Hangul_PreHanja
    | Hangul_PostHanja
    | Hangul_Special
    | Dead_Grave
    | Dead_Acute
    | Dead_Circumflex
    | Dead_Tilde
    | Dead_Macron
    | Dead_Breve
    | Dead_Abovedot
    | Dead_Diaeresis
    | Dead_Abovering
    | Dead_Doubleacute
    | Dead_Caron
    | Dead_Cedilla
    | Dead_Ogonek
    | Dead_Iota
    | Dead_Voiced_Sound
    | Dead_Semivoiced_Sound
    | Dead_Belowdot
    | Dead_Hook
    | Dead_Horn
    | Dead_Stroke
    | Dead_Abovecomma
    | Dead_Abovereversedcomma
    | Dead_Doublegrave
    | Dead_Belowring
    | Dead_Belowmacron
    | Dead_Belowcircumflex
    | Dead_Belowtilde
    | Dead_Belowbreve
    | Dead_Belowdiaeresis
    | Dead_Invertedbreve
    | Dead_Belowcomma
    | Dead_Currency
    | Dead_a
    | Dead_A
    | Dead_e
    | Dead_E
    | Dead_i
    | Dead_I
    | Dead_o
    | Dead_O
    | Dead_u
    | Dead_U
    | Dead_Small_Schwa
    | Dead_Capital_Schwa
    | Dead_Greek
    | Dead_Lowline
    | Dead_Aboveverticalline
    | Dead_Belowverticalline
    | Dead_Longsolidusoverlay
    | Back
    | Forward
    | Stop
    | Refresh
    | VolumeDown
    | VolumeMute
    | VolumeUp
    | BassBoost
    | BassUp
    | BassDown
    | TrebleUp
    | TrebleDown
    | MediaPlay
    | MediaStop
    | MediaPrevious
    | MediaNext
    | MediaRecord
    | MediaPause
    | MediaTogglePlayPause
    | HomePage
    | Favorites
    | Search
    | Standby
    | OpenUrl
    | LaunchMail
    | LaunchMedia
    | Launch0
    | Launch1
    | Launch2
    | Launch3
    | Launch4
    | Launch5
    | Launch6
    | Launch7
    | Launch8
    | Launch9
    | LaunchA
    | LaunchB
    | LaunchC
    | LaunchD
    | LaunchE
    | LaunchF
    | MonBrightnessUp
    | MonBrightnessDown
    | KeyboardLightOnOff
    | KeyboardBrightnessUp
    | KeyboardBrightnessDown
    | PowerOff
    | WakeUp
    | Eject
    | ScreenSaver
    | WWW
    | Memo
    | LightBulb
    | Shop
    | History
    | AddFavorite
    | HotLinks
    | BrightnessAdjust
    | Finance
    | Community
    | AudioRewind
    | BackForward
    | ApplicationLeft
    | ApplicationRight
    | Book
    | CD
    | Calculator
    | ToDoList
    | ClearGrab
    | Close
    | Copy
    | Cut
    | Display
    | DOS
    | Documents
    | Excel
    | Explorer
    | Game
    | Go
    | iTouch
    | LogOff
    | Market
    | Meeting
    | MenuKB
    | MenuPB
    | MySites
    | News
    | OfficeHome
    | Option
    | Paste
    | Phone
    | Calendar
    | Reply
    | Reload
    | RotateWindows
    | RotationPB
    | RotationKB
    | Save
    | Send
    | Spell
    | SplitScreen
    | Support
    | TaskPane
    | Terminal
    | Tools
    | Travel
    | Video
    | Word
    | Xfer
    | ZoomIn
    | ZoomOut
    | Away
    | Messenger
    | WebCam
    | MailForward
    | Pictures
    | Music
    | Battery
    | Bluetooth
    | WLAN
    | UWB
    | AudioForward
    | AudioRepeat
    | AudioRandomPlay
    | Subtitle
    | AudioCycleTrack
    | Time
    | Hibernate
    | View
    | TopMenu
    | PowerDown
    | Suspend
    | ContrastAdjust
    | LaunchG
    | LaunchH
    | TouchpadToggle
    | TouchpadOn
    | TouchpadOff
    | MicMute
    | Red
    | Green
    | Yellow
    | Blue
    | ChannelUp
    | ChannelDown
    | Guide
    | Info
    | Settings
    | MicVolumeUp
    | MicVolumeDown
    | New
    | Open
    | Find
    | Undo
    | Redo
    | MediaLast
    | Select
    | Yes
    | No
    | Cancel
    | Printer
    | Execute
    | Sleep
    | Play
    | Zoom
    | Exit
    | Context1
    | Context2
    | Context3
    | Context4
    | Call
    | Hangup
    | Flip
    | ToggleCallHangup
    | VoiceDial
    | LastNumberRedial
    | Camera
    | CameraFocus
with
    member this.QtValue =
        match this with
        | Space -> Enums.Key.Key_Space
        | Any -> Enums.Key.Key_Any
        | Exclam -> Enums.Key.Key_Exclam
        | QuoteDbl -> Enums.Key.Key_QuoteDbl
        | NumberSign -> Enums.Key.Key_NumberSign
        | Dollar -> Enums.Key.Key_Dollar
        | Percent -> Enums.Key.Key_Percent
        | Ampersand -> Enums.Key.Key_Ampersand
        | Apostrophe -> Enums.Key.Key_Apostrophe
        | ParenLeft -> Enums.Key.Key_ParenLeft
        | ParenRight -> Enums.Key.Key_ParenRight
        | Asterisk -> Enums.Key.Key_Asterisk
        | Plus -> Enums.Key.Key_Plus
        | Comma -> Enums.Key.Key_Comma
        | Minus -> Enums.Key.Key_Minus
        | Period -> Enums.Key.Key_Period
        | Slash -> Enums.Key.Key_Slash
        | Key0 -> Enums.Key.Key_0
        | Key1 -> Enums.Key.Key_1
        | Key2 -> Enums.Key.Key_2
        | Key3 -> Enums.Key.Key_3
        | Key4 -> Enums.Key.Key_4
        | Key5 -> Enums.Key.Key_5
        | Key6 -> Enums.Key.Key_6
        | Key7 -> Enums.Key.Key_7
        | Key8 -> Enums.Key.Key_8
        | Key9 -> Enums.Key.Key_9
        | Colon -> Enums.Key.Key_Colon
        | Semicolon -> Enums.Key.Key_Semicolon
        | Less -> Enums.Key.Key_Less
        | Equal -> Enums.Key.Key_Equal
        | Greater -> Enums.Key.Key_Greater
        | Question -> Enums.Key.Key_Question
        | At -> Enums.Key.Key_At
        | A -> Enums.Key.Key_A
        | B -> Enums.Key.Key_B
        | C -> Enums.Key.Key_C
        | D -> Enums.Key.Key_D
        | E -> Enums.Key.Key_E
        | F -> Enums.Key.Key_F
        | G -> Enums.Key.Key_G
        | H -> Enums.Key.Key_H
        | I -> Enums.Key.Key_I
        | J -> Enums.Key.Key_J
        | K -> Enums.Key.Key_K
        | L -> Enums.Key.Key_L
        | M -> Enums.Key.Key_M
        | N -> Enums.Key.Key_N
        | O -> Enums.Key.Key_O
        | P -> Enums.Key.Key_P
        | Q -> Enums.Key.Key_Q
        | R -> Enums.Key.Key_R
        | S -> Enums.Key.Key_S
        | T -> Enums.Key.Key_T
        | U -> Enums.Key.Key_U
        | V -> Enums.Key.Key_V
        | W -> Enums.Key.Key_W
        | X -> Enums.Key.Key_X
        | Y -> Enums.Key.Key_Y
        | Z -> Enums.Key.Key_Z
        | BracketLeft -> Enums.Key.Key_BracketLeft
        | Backslash -> Enums.Key.Key_Backslash
        | BracketRight -> Enums.Key.Key_BracketRight
        | AsciiCircum -> Enums.Key.Key_AsciiCircum
        | Underscore -> Enums.Key.Key_Underscore
        | QuoteLeft -> Enums.Key.Key_QuoteLeft
        | BraceLeft -> Enums.Key.Key_BraceLeft
        | Bar -> Enums.Key.Key_Bar
        | BraceRight -> Enums.Key.Key_BraceRight
        | AsciiTilde -> Enums.Key.Key_AsciiTilde
        | nobreakspace -> Enums.Key.Key_nobreakspace
        // | exclamdown -> Enums.Key.Key_exclamdown
        // | cent -> Enums.Key.Key_cent
        // | sterling -> Enums.Key.Key_sterling
        // | currency -> Enums.Key.Key_currency
        // | yen -> Enums.Key.Key_yen
        // | brokenbar -> Enums.Key.Key_brokenbar
        // | section -> Enums.Key.Key_section
        // | diaeresis -> Enums.Key.Key_diaeresis
        // | copyright -> Enums.Key.Key_copyright
        // | ordfeminine -> Enums.Key.Key_ordfeminine
        // | guillemotleft -> Enums.Key.Key_guillemotleft
        // | notsign -> Enums.Key.Key_notsign
        // | hyphen -> Enums.Key.Key_hyphen
        // | registered -> Enums.Key.Key_registered
        // | macron -> Enums.Key.Key_macron
        // | degree -> Enums.Key.Key_degree
        // | plusminus -> Enums.Key.Key_plusminus
        // | twosuperior -> Enums.Key.Key_twosuperior
        // | threesuperior -> Enums.Key.Key_threesuperior
        // | acute -> Enums.Key.Key_acute
        // | micro -> Enums.Key.Key_micro
        // | paragraph -> Enums.Key.Key_paragraph
        // | periodcentered -> Enums.Key.Key_periodcentered
        // | cedilla -> Enums.Key.Key_cedilla
        // | onesuperior -> Enums.Key.Key_onesuperior
        // | masculine -> Enums.Key.Key_masculine
        // | guillemotright -> Enums.Key.Key_guillemotright
        // | onequarter -> Enums.Key.Key_onequarter
        // | onehalf -> Enums.Key.Key_onehalf
        // | threequarters -> Enums.Key.Key_threequarters
        // | questiondown -> Enums.Key.Key_questiondown
        // | Agrave -> Enums.Key.Key_Agrave
        // | Aacute -> Enums.Key.Key_Aacute
        // | Acircumflex -> Enums.Key.Key_Acircumflex
        // | Atilde -> Enums.Key.Key_Atilde
        // | Adiaeresis -> Enums.Key.Key_Adiaeresis
        // | Aring -> Enums.Key.Key_Aring
        // | AE -> Enums.Key.Key_AE
        // | Ccedilla -> Enums.Key.Key_Ccedilla
        // | Egrave -> Enums.Key.Key_Egrave
        // | Eacute -> Enums.Key.Key_Eacute
        // | Ecircumflex -> Enums.Key.Key_Ecircumflex
        // | Ediaeresis -> Enums.Key.Key_Ediaeresis
        // | Igrave -> Enums.Key.Key_Igrave
        // | Iacute -> Enums.Key.Key_Iacute
        // | Icircumflex -> Enums.Key.Key_Icircumflex
        // | Idiaeresis -> Enums.Key.Key_Idiaeresis
        // | ETH -> Enums.Key.Key_ETH
        // | Ntilde -> Enums.Key.Key_Ntilde
        // | Ograve -> Enums.Key.Key_Ograve
        // | Oacute -> Enums.Key.Key_Oacute
        // | Ocircumflex -> Enums.Key.Key_Ocircumflex
        // | Otilde -> Enums.Key.Key_Otilde
        // | Odiaeresis -> Enums.Key.Key_Odiaeresis
        // | multiply -> Enums.Key.Key_multiply
        // | Ooblique -> Enums.Key.Key_Ooblique
        // | Ugrave -> Enums.Key.Key_Ugrave
        // | Uacute -> Enums.Key.Key_Uacute
        // | Ucircumflex -> Enums.Key.Key_Ucircumflex
        // | Udiaeresis -> Enums.Key.Key_Udiaeresis
        // | Yacute -> Enums.Key.Key_Yacute
        // | THORN -> Enums.Key.Key_THORN
        // | ssharp -> Enums.Key.Key_ssharp
        // | division -> Enums.Key.Key_division
        // | ydiaeresis -> Enums.Key.Key_ydiaeresis
        // | Escape -> Enums.Key.Key_Escape
        // | Tab -> Enums.Key.Key_Tab
        // | Backtab -> Enums.Key.Key_Backtab
        // | Backspace -> Enums.Key.Key_Backspace
        // | Return -> Enums.Key.Key_Return
        // | Enter -> Enums.Key.Key_Enter
        // | Insert -> Enums.Key.Key_Insert
        // | Delete -> Enums.Key.Key_Delete
        // | Pause -> Enums.Key.Key_Pause
        // | Print -> Enums.Key.Key_Print
        // | SysReq -> Enums.Key.Key_SysReq
        // | Clear -> Enums.Key.Key_Clear
        // | Home -> Enums.Key.Key_Home
        // | End -> Enums.Key.Key_End
        // | Left -> Enums.Key.Key_Left
        // | Up -> Enums.Key.Key_Up
        // | Right -> Enums.Key.Key_Right
        // | Down -> Enums.Key.Key_Down
        // | PageUp -> Enums.Key.Key_PageUp
        // | PageDown -> Enums.Key.Key_PageDown
        // | Shift -> Enums.Key.Key_Shift
        // | Control -> Enums.Key.Key_Control
        // | Meta -> Enums.Key.Key_Meta
        // | Alt -> Enums.Key.Key_Alt
        // | CapsLock -> Enums.Key.Key_CapsLock
        // | NumLock -> Enums.Key.Key_NumLock
        // | ScrollLock -> Enums.Key.Key_ScrollLock
        // | F1 -> Enums.Key.Key_F1
        // | F2 -> Enums.Key.Key_F2
        // | F3 -> Enums.Key.Key_F3
        // | F4 -> Enums.Key.Key_F4
        // | F5 -> Enums.Key.Key_F5
        // | F6 -> Enums.Key.Key_F6
        // | F7 -> Enums.Key.Key_F7
        // | F8 -> Enums.Key.Key_F8
        // | F9 -> Enums.Key.Key_F9
        // | F10 -> Enums.Key.Key_F10
        // | F11 -> Enums.Key.Key_F11
        // | F12 -> Enums.Key.Key_F12
        // | F13 -> Enums.Key.Key_F13
        // | F14 -> Enums.Key.Key_F14
        // | F15 -> Enums.Key.Key_F15
        // | F16 -> Enums.Key.Key_F16
        // | F17 -> Enums.Key.Key_F17
        // | F18 -> Enums.Key.Key_F18
        // | F19 -> Enums.Key.Key_F19
        // | F20 -> Enums.Key.Key_F20
        // | F21 -> Enums.Key.Key_F21
        // | F22 -> Enums.Key.Key_F22
        // | F23 -> Enums.Key.Key_F23
        // | F24 -> Enums.Key.Key_F24
        // | F25 -> Enums.Key.Key_F25
        // | F26 -> Enums.Key.Key_F26
        // | F27 -> Enums.Key.Key_F27
        // | F28 -> Enums.Key.Key_F28
        // | F29 -> Enums.Key.Key_F29
        // | F30 -> Enums.Key.Key_F30
        // | F31 -> Enums.Key.Key_F31
        // | F32 -> Enums.Key.Key_F32
        // | F33 -> Enums.Key.Key_F33
        // | F34 -> Enums.Key.Key_F34
        // | F35 -> Enums.Key.Key_F35
        // | Super_L -> Enums.Key.Key_Super_L
        // | Super_R -> Enums.Key.Key_Super_R
        // | Menu -> Enums.Key.Key_Menu
        // | Hyper_L -> Enums.Key.Key_Hyper_L
        // | Hyper_R -> Enums.Key.Key_Hyper_R
        // | Help -> Enums.Key.Key_Help
        // | Direction_L -> Enums.Key.Key_Direction_L
        // | Direction_R -> Enums.Key.Key_Direction_R
        // | AltGr -> Enums.Key.Key_AltGr
        // | Multi_key -> Enums.Key.Key_Multi_key
        // | Codeinput -> Enums.Key.Key_Codeinput
        // | SingleCandidate -> Enums.Key.Key_SingleCandidate
        // | MultipleCandidate -> Enums.Key.Key_MultipleCandidate
        // | PreviousCandidate -> Enums.Key.Key_PreviousCandidate
        // | Mode_switch -> Enums.Key.Key_Mode_switch
        // | Kanji -> Enums.Key.Key_Kanji
        // | Muhenkan -> Enums.Key.Key_Muhenkan
        // | Henkan -> Enums.Key.Key_Henkan
        // | Romaji -> Enums.Key.Key_Romaji
        // | Hiragana -> Enums.Key.Key_Hiragana
        // | Katakana -> Enums.Key.Key_Katakana
        // | Hiragana_Katakana -> Enums.Key.Key_Hiragana_Katakana
        // | Zenkaku -> Enums.Key.Key_Zenkaku
        // | Hankaku -> Enums.Key.Key_Hankaku
        // | Zenkaku_Hankaku -> Enums.Key.Key_Zenkaku_Hankaku
        // | Touroku -> Enums.Key.Key_Touroku
        // | Massyo -> Enums.Key.Key_Massyo
        // | Kana_Lock -> Enums.Key.Key_Kana_Lock
        // | Kana_Shift -> Enums.Key.Key_Kana_Shift
        // | Eisu_Shift -> Enums.Key.Key_Eisu_Shift
        // | Eisu_toggle -> Enums.Key.Key_Eisu_toggle
        // | Hangul -> Enums.Key.Key_Hangul
        // | Hangul_Start -> Enums.Key.Key_Hangul_Start
        // | Hangul_End -> Enums.Key.Key_Hangul_End
        // | Hangul_Hanja -> Enums.Key.Key_Hangul_Hanja
        // | Hangul_Jamo -> Enums.Key.Key_Hangul_Jamo
        // | Hangul_Romaja -> Enums.Key.Key_Hangul_Romaja
        // | Hangul_Jeonja -> Enums.Key.Key_Hangul_Jeonja
        // | Hangul_Banja -> Enums.Key.Key_Hangul_Banja
        // | Hangul_PreHanja -> Enums.Key.Key_Hangul_PreHanja
        // | Hangul_PostHanja -> Enums.Key.Key_Hangul_PostHanja
        // | Hangul_Special -> Enums.Key.Key_Hangul_Special
        // | Dead_Grave -> Enums.Key.Key_Dead_Grave
        // | Dead_Acute -> Enums.Key.Key_Dead_Acute
        // | Dead_Circumflex -> Enums.Key.Key_Dead_Circumflex
        // | Dead_Tilde -> Enums.Key.Key_Dead_Tilde
        // | Dead_Macron -> Enums.Key.Key_Dead_Macron
        // | Dead_Breve -> Enums.Key.Key_Dead_Breve
        // | Dead_Abovedot -> Enums.Key.Key_Dead_Abovedot
        // | Dead_Diaeresis -> Enums.Key.Key_Dead_Diaeresis
        // | Dead_Abovering -> Enums.Key.Key_Dead_Abovering
        // | Dead_Doubleacute -> Enums.Key.Key_Dead_Doubleacute
        // | Dead_Caron -> Enums.Key.Key_Dead_Caron
        // | Dead_Cedilla -> Enums.Key.Key_Dead_Cedilla
        // | Dead_Ogonek -> Enums.Key.Key_Dead_Ogonek
        // | Dead_Iota -> Enums.Key.Key_Dead_Iota
        // | Dead_Voiced_Sound -> Enums.Key.Key_Dead_Voiced_Sound
        // | Dead_Semivoiced_Sound -> Enums.Key.Key_Dead_Semivoiced_Sound
        // | Dead_Belowdot -> Enums.Key.Key_Dead_Belowdot
        // | Dead_Hook -> Enums.Key.Key_Dead_Hook
        // | Dead_Horn -> Enums.Key.Key_Dead_Horn
        // | Dead_Stroke -> Enums.Key.Key_Dead_Stroke
        // | Dead_Abovecomma -> Enums.Key.Key_Dead_Abovecomma
        // | Dead_Abovereversedcomma -> Enums.Key.Key_Dead_Abovereversedcomma
        // | Dead_Doublegrave -> Enums.Key.Key_Dead_Doublegrave
        // | Dead_Belowring -> Enums.Key.Key_Dead_Belowring
        // | Dead_Belowmacron -> Enums.Key.Key_Dead_Belowmacron
        // | Dead_Belowcircumflex -> Enums.Key.Key_Dead_Belowcircumflex
        // | Dead_Belowtilde -> Enums.Key.Key_Dead_Belowtilde
        // | Dead_Belowbreve -> Enums.Key.Key_Dead_Belowbreve
        // | Dead_Belowdiaeresis -> Enums.Key.Key_Dead_Belowdiaeresis
        // | Dead_Invertedbreve -> Enums.Key.Key_Dead_Invertedbreve
        // | Dead_Belowcomma -> Enums.Key.Key_Dead_Belowcomma
        // | Dead_Currency -> Enums.Key.Key_Dead_Currency
        // | Dead_a -> Enums.Key.Key_Dead_a
        // | Dead_A -> Enums.Key.Key_Dead_A
        // | Dead_e -> Enums.Key.Key_Dead_e
        // | Dead_E -> Enums.Key.Key_Dead_E
        // | Dead_i -> Enums.Key.Key_Dead_i
        // | Dead_I -> Enums.Key.Key_Dead_I
        // | Dead_o -> Enums.Key.Key_Dead_o
        // | Dead_O -> Enums.Key.Key_Dead_O
        // | Dead_u -> Enums.Key.Key_Dead_u
        // | Dead_U -> Enums.Key.Key_Dead_U
        // | Dead_Small_Schwa -> Enums.Key.Key_Dead_Small_Schwa
        // | Dead_Capital_Schwa -> Enums.Key.Key_Dead_Capital_Schwa
        // | Dead_Greek -> Enums.Key.Key_Dead_Greek
        // | Dead_Lowline -> Enums.Key.Key_Dead_Lowline
        // | Dead_Aboveverticalline -> Enums.Key.Key_Dead_Aboveverticalline
        // | Dead_Belowverticalline -> Enums.Key.Key_Dead_Belowverticalline
        // | Dead_Longsolidusoverlay -> Enums.Key.Key_Dead_Longsolidusoverlay
        // | Back -> Enums.Key.Key_Back
        // | Forward -> Enums.Key.Key_Forward
        // | Stop -> Enums.Key.Key_Stop
        // | Refresh -> Enums.Key.Key_Refresh
        // | VolumeDown -> Enums.Key.Key_VolumeDown
        // | VolumeMute -> Enums.Key.Key_VolumeMute
        // | VolumeUp -> Enums.Key.Key_VolumeUp
        // | BassBoost -> Enums.Key.Key_BassBoost
        // | BassUp -> Enums.Key.Key_BassUp
        // | BassDown -> Enums.Key.Key_BassDown
        // | TrebleUp -> Enums.Key.Key_TrebleUp
        // | TrebleDown -> Enums.Key.Key_TrebleDown
        // | MediaPlay -> Enums.Key.Key_MediaPlay
        // | MediaStop -> Enums.Key.Key_MediaStop
        // | MediaPrevious -> Enums.Key.Key_MediaPrevious
        // | MediaNext -> Enums.Key.Key_MediaNext
        // | MediaRecord -> Enums.Key.Key_MediaRecord
        // | MediaPause -> Enums.Key.Key_MediaPause
        // | MediaTogglePlayPause -> Enums.Key.Key_MediaTogglePlayPause
        // | HomePage -> Enums.Key.Key_HomePage
        // | Favorites -> Enums.Key.Key_Favorites
        // | Search -> Enums.Key.Key_Search
        // | Standby -> Enums.Key.Key_Standby
        // | OpenUrl -> Enums.Key.Key_OpenUrl
        // | LaunchMail -> Enums.Key.Key_LaunchMail
        // | LaunchMedia -> Enums.Key.Key_LaunchMedia
        // | Launch0 -> Enums.Key.Key_Launch0
        // | Launch1 -> Enums.Key.Key_Launch1
        // | Launch2 -> Enums.Key.Key_Launch2
        // | Launch3 -> Enums.Key.Key_Launch3
        // | Launch4 -> Enums.Key.Key_Launch4
        // | Launch5 -> Enums.Key.Key_Launch5
        // | Launch6 -> Enums.Key.Key_Launch6
        // | Launch7 -> Enums.Key.Key_Launch7
        // | Launch8 -> Enums.Key.Key_Launch8
        // | Launch9 -> Enums.Key.Key_Launch9
        // | LaunchA -> Enums.Key.Key_LaunchA
        // | LaunchB -> Enums.Key.Key_LaunchB
        // | LaunchC -> Enums.Key.Key_LaunchC
        // | LaunchD -> Enums.Key.Key_LaunchD
        // | LaunchE -> Enums.Key.Key_LaunchE
        // | LaunchF -> Enums.Key.Key_LaunchF
        // | MonBrightnessUp -> Enums.Key.Key_MonBrightnessUp
        // | MonBrightnessDown -> Enums.Key.Key_MonBrightnessDown
        // | KeyboardLightOnOff -> Enums.Key.Key_KeyboardLightOnOff
        // | KeyboardBrightnessUp -> Enums.Key.Key_KeyboardBrightnessUp
        // | KeyboardBrightnessDown -> Enums.Key.Key_KeyboardBrightnessDown
        // | PowerOff -> Enums.Key.Key_PowerOff
        // | WakeUp -> Enums.Key.Key_WakeUp
        // | Eject -> Enums.Key.Key_Eject
        // | ScreenSaver -> Enums.Key.Key_ScreenSaver
        // | WWW -> Enums.Key.Key_WWW
        // | Memo -> Enums.Key.Key_Memo
        // | LightBulb -> Enums.Key.Key_LightBulb
        // | Shop -> Enums.Key.Key_Shop
        // | History -> Enums.Key.Key_History
        // | AddFavorite -> Enums.Key.Key_AddFavorite
        // | HotLinks -> Enums.Key.Key_HotLinks
        // | BrightnessAdjust -> Enums.Key.Key_BrightnessAdjust
        // | Finance -> Enums.Key.Key_Finance
        // | Community -> Enums.Key.Key_Community
        // | AudioRewind -> Enums.Key.Key_AudioRewind
        // | BackForward -> Enums.Key.Key_BackForward
        // | ApplicationLeft -> Enums.Key.Key_ApplicationLeft
        // | ApplicationRight -> Enums.Key.Key_ApplicationRight
        // | Book -> Enums.Key.Key_Book
        // | CD -> Enums.Key.Key_CD
        // | Calculator -> Enums.Key.Key_Calculator
        // | ToDoList -> Enums.Key.Key_ToDoList
        // | ClearGrab -> Enums.Key.Key_ClearGrab
        // | Close -> Enums.Key.Key_Close
        // | Copy -> Enums.Key.Key_Copy
        // | Cut -> Enums.Key.Key_Cut
        // | Display -> Enums.Key.Key_Display
        // | DOS -> Enums.Key.Key_DOS
        // | Documents -> Enums.Key.Key_Documents
        // | Excel -> Enums.Key.Key_Excel
        // | Explorer -> Enums.Key.Key_Explorer
        // | Game -> Enums.Key.Key_Game
        // | Go -> Enums.Key.Key_Go
        // | iTouch -> Enums.Key.Key_iTouch
        // | LogOff -> Enums.Key.Key_LogOff
        // | Market -> Enums.Key.Key_Market
        // | Meeting -> Enums.Key.Key_Meeting
        // | MenuKB -> Enums.Key.Key_MenuKB
        // | MenuPB -> Enums.Key.Key_MenuPB
        // | MySites -> Enums.Key.Key_MySites
        // | News -> Enums.Key.Key_News
        // | OfficeHome -> Enums.Key.Key_OfficeHome
        // | Option -> Enums.Key.Key_Option
        // | Paste -> Enums.Key.Key_Paste
        // | Phone -> Enums.Key.Key_Phone
        // | Calendar -> Enums.Key.Key_Calendar
        // | Reply -> Enums.Key.Key_Reply
        // | Reload -> Enums.Key.Key_Reload
        // | RotateWindows -> Enums.Key.Key_RotateWindows
        // | RotationPB -> Enums.Key.Key_RotationPB
        // | RotationKB -> Enums.Key.Key_RotationKB
        // | Save -> Enums.Key.Key_Save
        // | Send -> Enums.Key.Key_Send
        // | Spell -> Enums.Key.Key_Spell
        // | SplitScreen -> Enums.Key.Key_SplitScreen
        // | Support -> Enums.Key.Key_Support
        // | TaskPane -> Enums.Key.Key_TaskPane
        // | Terminal -> Enums.Key.Key_Terminal
        // | Tools -> Enums.Key.Key_Tools
        // | Travel -> Enums.Key.Key_Travel
        // | Video -> Enums.Key.Key_Video
        // | Word -> Enums.Key.Key_Word
        // | Xfer -> Enums.Key.Key_Xfer
        // | ZoomIn -> Enums.Key.Key_ZoomIn
        // | ZoomOut -> Enums.Key.Key_ZoomOut
        // | Away -> Enums.Key.Key_Away
        // | Messenger -> Enums.Key.Key_Messenger
        // | WebCam -> Enums.Key.Key_WebCam
        // | MailForward -> Enums.Key.Key_MailForward
        // | Pictures -> Enums.Key.Key_Pictures
        // | Music -> Enums.Key.Key_Music
        // | Battery -> Enums.Key.Key_Battery
        // | Bluetooth -> Enums.Key.Key_Bluetooth
        // | WLAN -> Enums.Key.Key_WLAN
        // | UWB -> Enums.Key.Key_UWB
        // | AudioForward -> Enums.Key.Key_AudioForward
        // | AudioRepeat -> Enums.Key.Key_AudioRepeat
        // | AudioRandomPlay -> Enums.Key.Key_AudioRandomPlay
        // | Subtitle -> Enums.Key.Key_Subtitle
        // | AudioCycleTrack -> Enums.Key.Key_AudioCycleTrack
        // | Time -> Enums.Key.Key_Time
        // | Hibernate -> Enums.Key.Key_Hibernate
        // | View -> Enums.Key.Key_View
        // | TopMenu -> Enums.Key.Key_TopMenu
        // | PowerDown -> Enums.Key.Key_PowerDown
        // | Suspend -> Enums.Key.Key_Suspend
        // | ContrastAdjust -> Enums.Key.Key_ContrastAdjust
        // | LaunchG -> Enums.Key.Key_LaunchG
        // | LaunchH -> Enums.Key.Key_LaunchH
        // | TouchpadToggle -> Enums.Key.Key_TouchpadToggle
        // | TouchpadOn -> Enums.Key.Key_TouchpadOn
        // | TouchpadOff -> Enums.Key.Key_TouchpadOff
        // | MicMute -> Enums.Key.Key_MicMute
        // | Red -> Enums.Key.Key_Red
        // | Green -> Enums.Key.Key_Green
        // | Yellow -> Enums.Key.Key_Yellow
        // | Blue -> Enums.Key.Key_Blue
        // | ChannelUp -> Enums.Key.Key_ChannelUp
        // | ChannelDown -> Enums.Key.Key_ChannelDown
        // | Guide -> Enums.Key.Key_Guide
        // | Info -> Enums.Key.Key_Info
        // | Settings -> Enums.Key.Key_Settings
        // | MicVolumeUp -> Enums.Key.Key_MicVolumeUp
        // | MicVolumeDown -> Enums.Key.Key_MicVolumeDown
        // | New -> Enums.Key.Key_New
        // | Open -> Enums.Key.Key_Open
        // | Find -> Enums.Key.Key_Find
        // | Undo -> Enums.Key.Key_Undo
        // | Redo -> Enums.Key.Key_Redo
        // | MediaLast -> Enums.Key.Key_MediaLast
        // | Select -> Enums.Key.Key_Select
        // | Yes -> Enums.Key.Key_Yes
        // | No -> Enums.Key.Key_No
        // | Cancel -> Enums.Key.Key_Cancel
        // | Printer -> Enums.Key.Key_Printer
        // | Execute -> Enums.Key.Key_Execute
        // | Sleep -> Enums.Key.Key_Sleep
        // | Play -> Enums.Key.Key_Play
        // | Zoom -> Enums.Key.Key_Zoom
        // | Exit -> Enums.Key.Key_Exit
        // | Context1 -> Enums.Key.Key_Context1
        // | Context2 -> Enums.Key.Key_Context2
        // | Context3 -> Enums.Key.Key_Context3
        // | Context4 -> Enums.Key.Key_Context4
        // | Call -> Enums.Key.Key_Call
        // | Hangup -> Enums.Key.Key_Hangup
        // | Flip -> Enums.Key.Key_Flip
        // | ToggleCallHangup -> Enums.Key.Key_ToggleCallHangup
        // | VoiceDial -> Enums.Key.Key_VoiceDial
        // | LastNumberRedial -> Enums.Key.Key_LastNumberRedial
        // | Camera -> Enums.Key.Key_Camera
        // | CameraFocus -> Enums.Key.Key_CameraFocus

