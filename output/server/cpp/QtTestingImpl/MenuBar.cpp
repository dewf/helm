#include "generated/MenuBar.h"

#include <QObject>
#include <QMenuBar>

#define THIS ((QMenuBar*)_this)

namespace MenuBar
{
    void Handle_clear(HandleRef _this) {
        THIS->clear();
    }

    void Handle_addMenu(HandleRef _this, Menu::HandleRef menu) {
        THIS->addMenu((QMenu*)menu);
    }

    void Handle_onTriggered(HandleRef _this, std::function<Handle::ActionDelegate> handler) {
        QObject::connect(
                THIS,
                &QMenuBar::triggered,
                THIS,
                [handler](QAction *action) {
                    handler((Action::HandleRef)action);
                });
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create() {
        return (HandleRef) new QMenuBar();
    }
}
