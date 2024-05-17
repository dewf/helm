#include "generated/Action.h"

#include <QObject>
#include <QAction>

#define THIS ((QAction*)_this)

namespace Action
{
    std::string Handle_getText(HandleRef _this) {
        return THIS->text().toStdString();
    }

    void Handle_setText(HandleRef _this, std::string text) {
        THIS->setText(text.c_str());
    }

    void Handle_setSeparator(HandleRef _this, bool state) {
        THIS->setSeparator(state);
    }

    void Handle_setEnabled(HandleRef _this, bool state) {
        THIS->setEnabled(state);
    }

    void Handle_onTriggered(HandleRef _this, std::function<BoolDelegate> handler) {
        QObject::connect(
            THIS,
            &QAction::triggered,
            THIS,
            handler);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create() {
        return (HandleRef)new QAction();
    }

    HandleRef create(std::string text) {
        return (HandleRef)new QAction(text.c_str());
    }
}
