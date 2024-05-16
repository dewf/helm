#include "generated/Menu.h"

#include <QMenu>

#define THIS ((QMenu*)_this)

namespace Menu
{
    void Handle_addAction(HandleRef _this, Action::HandleRef action) {
        THIS->addAction((QAction*)action);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create(std::string name) {
        return (HandleRef)new QMenu(name.c_str());
    }
}
