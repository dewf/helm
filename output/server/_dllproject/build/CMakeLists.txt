cmake_minimum_required(VERSION 3.14)

if(WIN32)
    #set(CMAKE_PREFIX_PATH "C:/Qt/6.6.3/msvc2019_64/lib/cmake")
    set(CMAKE_PREFIX_PATH "C:/Qt/6.7.1/msvc2019_64/lib/cmake")
elseif (APPLE)
    set(CMAKE_PREFIX_PATH "/Users/daniel/Qt/6.7.2/macos/lib/cmake")
    set(CMAKE_OSX_ARCHITECTURES "x86_64;arm64")
endif()

project(QtTestingServer LANGUAGES CXX)

set(CMAKE_AUTOUIC ON)
set(CMAKE_AUTOMOC ON)
set(CMAKE_AUTORCC ON)
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

find_package(QT NAMES Qt6 Qt5 REQUIRED COMPONENTS Widgets)
find_package(Qt${QT_VERSION_MAJOR} REQUIRED COMPONENTS Widgets)

if (WIN32)
    add_library(QtTestingServer SHARED
            # QtTestingServer_global.h
            # qttestingserver.cpp
            # qttestingserver.h
            ../dllmain.cpp
            ../core/NativeImplCore.cpp
            ../DirectExport.h
            ../DirectExport.cpp
    )
else()
    add_library(QtTestingServer SHARED
            # QtTestingServer_global.h
            # qttestingserver.cpp
            # qttestingserver.h
#            ../dllmain.cpp
            ../core/NativeImplCore.cpp
            ../DirectExport.h
            ../DirectExport.cpp
    )
endif()

add_subdirectory("../../cpp/QtTestingImpl/build" "${CMAKE_CURRENT_BINARY_DIR}/QtTestingImpl")

target_link_libraries(QtTestingServer PRIVATE Qt${QT_VERSION_MAJOR}::Widgets "QtTestingImpl")

target_compile_definitions(QtTestingServer PRIVATE QTTESTINGSERVER_LIBRARY)

if (WIN32 AND NOT DEFINED CMAKE_TOOLCHAIN_FILE)
    set(DEBUG_SUFFIX)
    if (MSVC AND CMAKE_BUILD_TYPE MATCHES "Debug")
        set(DEBUG_SUFFIX "d")
    endif ()
    set(QT_INSTALL_PATH "${CMAKE_PREFIX_PATH}")
    if (NOT EXISTS "${QT_INSTALL_PATH}/bin")
        set(QT_INSTALL_PATH "${QT_INSTALL_PATH}/..")
        if (NOT EXISTS "${QT_INSTALL_PATH}/bin")
            set(QT_INSTALL_PATH "${QT_INSTALL_PATH}/..")
        endif ()
    endif ()
    if (EXISTS "${QT_INSTALL_PATH}/plugins/platforms/qwindows${DEBUG_SUFFIX}.dll")
        add_custom_command(TARGET ${PROJECT_NAME} POST_BUILD
                COMMAND ${CMAKE_COMMAND} -E make_directory
                "$<TARGET_FILE_DIR:${PROJECT_NAME}>/plugins/platforms/")
        add_custom_command(TARGET ${PROJECT_NAME} POST_BUILD
                COMMAND ${CMAKE_COMMAND} -E copy
                "${QT_INSTALL_PATH}/plugins/platforms/qwindows${DEBUG_SUFFIX}.dll"
                "$<TARGET_FILE_DIR:${PROJECT_NAME}>/plugins/platforms/")
    endif ()
    foreach (QT_LIB Core Gui Widgets)
        add_custom_command(TARGET ${PROJECT_NAME} POST_BUILD
                COMMAND ${CMAKE_COMMAND} -E copy
                "${QT_INSTALL_PATH}/bin/Qt6${QT_LIB}${DEBUG_SUFFIX}.dll"
                "$<TARGET_FILE_DIR:${PROJECT_NAME}>")
    endforeach (QT_LIB)
endif ()
