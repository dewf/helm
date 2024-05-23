cmake_minimum_required(VERSION 3.14)

project(QtTestingImpl LANGUAGES CXX)

set(CMAKE_AUTOUIC ON)
set(CMAKE_AUTOMOC ON)
set(CMAKE_AUTORCC ON)
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

find_package(QT NAMES Qt6 Qt5 REQUIRED COMPONENTS Widgets)
find_package(Qt${QT_VERSION_MAJOR} REQUIRED COMPONENTS Widgets)

add_library(QtTestingImpl STATIC
    ../support/NativeImplServer.cpp
    ../generated/Library.cpp

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

    ../generated/Painter.h
    ../generated/Painter_wrappers.cpp
    ../Painter.cpp

    ../generated/PaintResources.h
    ../generated/PaintResources_wrappers.cpp
    ../PaintResourcesInternal.h                      # needed for internal struct definitions!
    ../PaintResources.cpp

    ../generated/PlainTextEdit.h
    ../generated/PlainTextEdit_wrappers.cpp
    ../PlainTextEdit.cpp

    ../generated/PushButton.h
    ../generated/PushButton_wrappers.cpp
    ../PushButton.cpp

    ../generated/Signal.h
    ../generated/Signal_wrappers.cpp
    ../Signal.cpp

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
)

target_link_libraries(QtTestingImpl PRIVATE Qt${QT_VERSION_MAJOR}::Widgets)

target_compile_definitions(QtTestingImpl PRIVATE QTTESTINGIMPL_LIBRARY)
