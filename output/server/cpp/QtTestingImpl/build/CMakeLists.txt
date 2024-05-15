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

    ../generated/Application.h
    ../generated/Application_Wrappers.cpp
    ../Application.cpp

    ../generated/BoxLayout.h
    ../generated/BoxLayout_Wrappers.cpp
    ../BoxLayout.cpp

    ../generated/ComboBox.h
    ../generated/ComboBox_Wrappers.cpp
    ../ComboBox.cpp

    ../generated/Common.h
    ../generated/Common_Wrappers.cpp
    ../Common.cpp

    ../generated/Layout.h
    ../generated/Layout_Wrappers.cpp
    ../Layout.cpp

    ../generated/LineEdit.h
    ../generated/LineEdit_Wrappers.cpp
    ../LineEdit.cpp

    ../generated/MainWindow.h
    ../generated/MainWindow_Wrappers.cpp
    ../MainWindow.cpp

    ../generated/PlainTextEdit.h
    ../generated/PlainTextEdit_Wrappers.cpp
    ../PlainTextEdit.cpp

    ../generated/PushButton.h
    ../generated/PushButton_Wrappers.cpp
    ../PushButton.cpp

    ../generated/Signal.h
    ../generated/Signal_Wrappers.cpp
    ../Signal.cpp

    ../generated/Widget.h
    ../generated/Widget_Wrappers.cpp
    ../Widget.cpp

    ../util/convert.h
    ../util/convert.cpp
)

target_link_libraries(QtTestingImpl PRIVATE Qt${QT_VERSION_MAJOR}::Widgets)

target_compile_definitions(QtTestingImpl PRIVATE QTTESTINGIMPL_LIBRARY)
