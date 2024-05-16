#include "generated/MainWindow.h"

#include <QMainWindow>

#define THIS ((QMainWindow*)_this)

namespace MainWindow
{
    void Handle_setCentralWidget(HandleRef _this, Widget::HandleRef widget) {
        THIS->setCentralWidget((QWidget*)widget);
    }

    void Handle_setMenuBar(HandleRef _this, MenuBar::HandleRef menubar) {
        THIS->setMenuBar((QMenuBar*)menubar);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create() {
        return (HandleRef)new QMainWindow();
    }
}
