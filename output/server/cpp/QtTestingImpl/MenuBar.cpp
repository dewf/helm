#include "generated/MenuBar.h"

#include <QMenuBar>

#define THIS ((QMenuBar*)_this)

namespace MenuBar
{
    void Handle_addMenu(HandleRef _this, Menu::HandleRef menu) {
        THIS->addMenu((QMenu*)menu);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create() {
        return (HandleRef) new QMenuBar();
    }
}
