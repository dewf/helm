cmake_minimum_required(VERSION 3.14)

project(QtTestingImpl LANGUAGES CXX)

set(CMAKE_AUTOUIC ON)
set(CMAKE_AUTOMOC ON)
set(CMAKE_AUTORCC ON)
set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

find_package(QT NAMES Qt6 Qt5 REQUIRED COMPONENTS Widgets)
find_package(Qt${QT_VERSION_MAJOR} REQUIRED COMPONENTS Widgets)

add_library(QtTestingImpl STATIC
    ../support/NativeImplServer.cpp
    ../generated/Library.cpp

    ../generated/AbstractButton.h
    ../generated/AbstractButton_wrappers.cpp
    ../AbstractButton.cpp

    ../generated/AbstractScrollArea.h
    ../generated/AbstractScrollArea_wrappers.cpp
    ../AbstractScrollArea.cpp

    ../generated/AbstractSlider.h
    ../generated/AbstractSlider_wrappers.cpp
    ../AbstractSlider.cpp

    ../generated/Action.h
    ../generated/Action_wrappers.cpp
    ../Action.cpp

    ../generated/Application.h
    ../generated/Application_wrappers.cpp
    ../Application.cpp

    ../generated/BoxLayout.h
    ../generated/BoxLayout_wrappers.cpp
    ../BoxLayout.cpp

    ../generated/ComboBox.h
    ../generated/ComboBox_wrappers.cpp
    ../ComboBox.cpp

    ../generated/Common.h
    ../generated/Common_wrappers.cpp
    ../Common.cpp

    ../generated/Dialog.h
    ../generated/Dialog_wrappers.cpp
    ../Dialog.cpp

    ../generated/DockWidget.h
    ../generated/DockWidget_wrappers.cpp
    ../DockWidget.cpp

    ../generated/Enums.h
    ../generated/Enums_wrappers.cpp
    ../Enums.cpp

    ../generated/FileDialog.h
    ../generated/FileDialog_wrappers.cpp
    ../FileDialog.cpp

    ../generated/Frame.h
    ../generated/Frame_wrappers.cpp
    ../Frame.cpp

    ../generated/GridLayout.h
    ../generated/GridLayout_wrappers.cpp
    ../GridLayout.cpp

    ../generated/GroupBox.h
    ../generated/GroupBox_wrappers.cpp
    ../GroupBox.cpp

    ../generated/Icon.h
    ../generated/Icon_wrappers.cpp
    ../IconInternal.h                           # needed for internal struct definitions (when accessing icons from other modules)
    ../Icon.cpp

    ../generated/Label.h
    ../generated/Label_wrappers.cpp
    ../Label.cpp

    ../generated/Layout.h
    ../generated/Layout_wrappers.cpp
    ../Layout.cpp

    ../generated/LineEdit.h
    ../generated/LineEdit_wrappers.cpp
    ../LineEdit.cpp

    ../generated/ListWidget.h
    ../generated/ListWidget_wrappers.cpp
    ../ListWidget.cpp

    ../generated/MainWindow.h
    ../generated/MainWindow_wrappers.cpp
    ../MainWindow.cpp

    ../generated/Menu.h
    ../generated/Menu_wrappers.cpp
    ../Menu.cpp

    ../generated/MenuBar.h
    ../generated/MenuBar_wrappers.cpp
    ../MenuBar.cpp

    ../generated/MessageBox.h
    ../generated/MessageBox_wrappers.cpp
    ../MessageBox.cpp

    ../generated/Painter.h
    ../generated/Painter_wrappers.cpp
    ../Painter.cpp

    ../generated/PaintResources.h
    ../generated/PaintResources_wrappers.cpp
    ../PaintResourcesInternal.h                      # needed for internal struct definitions
    ../PaintResources.cpp

    ../generated/PlainTextEdit.h
    ../generated/PlainTextEdit_wrappers.cpp
    ../PlainTextEdit.cpp

    ../generated/ProgressBar.h
    ../generated/ProgressBar_wrappers.cpp
    ../ProgressBar.cpp

    ../generated/PushButton.h
    ../generated/PushButton_wrappers.cpp
    ../PushButton.cpp

    ../generated/RadioButton.h
    ../generated/RadioButton_wrappers.cpp
    ../RadioButton.cpp

    ../generated/ScrollArea.h
    ../generated/ScrollArea_wrappers.cpp
    ../ScrollArea.cpp

    ../generated/Signal.h
    ../generated/Signal_wrappers.cpp
    ../Signal.cpp

    ../generated/Slider.h
    ../generated/Slider_wrappers.cpp
    ../Slider.cpp

    ../generated/TabWidget.h
    ../generated/TabWidget_wrappers.cpp
    ../TabWidget.cpp

    ../generated/Timer.h
    ../generated/Timer_wrappers.cpp
    ../Timer.cpp

    ../generated/Widget.h
    ../generated/Widget_wrappers.cpp
    ../Widget.cpp

    ../util/convert.h
    ../util/convert.cpp
    ../util/SignalStuff.h
)

target_link_libraries(QtTestingImpl PRIVATE Qt${QT_VERSION_MAJOR}::Widgets)

target_compile_definitions(QtTestingImpl PRIVATE QTTESTINGIMPL_LIBRARY)
