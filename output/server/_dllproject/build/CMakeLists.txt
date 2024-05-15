cmake_minimum_required(VERSION 3.14)

project(QtTestingServer LANGUAGES CXX)

set(CMAKE_AUTOUIC ON)
set(CMAKE_AUTOMOC ON)
set(CMAKE_AUTORCC ON)
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

find_package(QT NAMES Qt6 Qt5 REQUIRED COMPONENTS Widgets)
find_package(Qt${QT_VERSION_MAJOR} REQUIRED COMPONENTS Widgets)

add_library(QtTestingServer SHARED
  # QtTestingServer_global.h
  # qttestingserver.cpp
  # qttestingserver.h
  ../dllmain.cpp
  ../core/NativeImplCore.cpp
  ../DirectExport.h
  ../DirectExport.cpp
)

add_subdirectory("../../cpp/QtTestingImpl/build" "${CMAKE_CURRENT_BINARY_DIR}/QtTestingImpl")

target_link_libraries(QtTestingServer PRIVATE Qt${QT_VERSION_MAJOR}::Widgets "QtTestingImpl")

target_compile_definitions(QtTestingServer PRIVATE QTTESTINGSERVER_LIBRARY)
