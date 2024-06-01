#include "generated/Menu.h"

#include <QObject>
#include <QMenu>

#include "util/convert.h"

#define THIS ((QMenu*)_this)

namespace Menu
{
    void Handle_clear(HandleRef _this) {
        THIS->clear();
    }

    void Handle_setTitle(HandleRef _this, std::string title) {
        THIS->setTitle(title.c_str());
    }

    void Handle_addAction(HandleRef _this, Action::HandleRef action) {
        THIS->addAction((QAction*)action);
    }

    void Handle_popup(HandleRef _this, Point p) {
        THIS->popup(toQPoint(p));
    }

    void Handle_onAboutToShow(HandleRef _this, std::function<VoidDelegate> handler) {
        QObject::connect(
                THIS,
                &QMenu::aboutToShow,
                THIS,
                handler);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create() {
        return (HandleRef)new QMenu();
    }

    HandleRef create(std::string title) {
        return (HandleRef)new QMenu(title.c_str());
    }
}
