#include "generated/MainWindow.h"

#include <QMainWindow>

namespace MainWindow
{
    void Handle_setCentralWidget(HandleRef _this, Widget::HandleRef widget) {
        ((QMainWindow*)_this)->setCentralWidget((QWidget*)widget);
    }

    void Handle_dispose(HandleRef _this) {
        delete (QMainWindow*)_this;
    }

    HandleRef create(Widget::HandleRef parent, Kind kind) {
        Qt::WindowFlags windowFlags;
        switch (kind) {
        case Kind::Widget:
            windowFlags = Qt::WindowType::Widget;
            break;
        case Kind::Window:
            windowFlags = Qt::WindowType::Window;
            break;
        case Kind::Dialog:
            windowFlags = Qt::WindowType::Dialog;
            break;
        case Kind::Sheet:
            windowFlags = Qt::WindowType::Sheet;
            break;
        case Kind::Drawer:
            windowFlags = Qt::WindowType::Drawer;
            break;
        case Kind::Popup:
            windowFlags = Qt::WindowType::Popup;
            break;
        case Kind::Tool:
            windowFlags = Qt::WindowType::Tool;
            break;
        case Kind::ToolTip:
            windowFlags = Qt::WindowType::ToolTip;
            break;
        case Kind::SplashScreen:
            windowFlags = Qt::WindowType::SplashScreen;
            break;
        case Kind::SubWindow:
            windowFlags = Qt::WindowType::SubWindow;
            break;
        case Kind::ForeignWindow:
            windowFlags = Qt::WindowType::ForeignWindow;
            break;
        case Kind::CoverWindow:
            windowFlags = Qt::WindowType::CoverWindow;
            break;
        // default:
        }
        return (HandleRef)new QMainWindow((QWidget*)parent, windowFlags);
    }
}
