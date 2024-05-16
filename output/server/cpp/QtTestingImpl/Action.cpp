#include "generated/Action.h"

#include <QObject>
#include <QAction>

#define THIS ((QAction*)_this)

namespace Action
{
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

    HandleRef create(std::string title) {
        return (HandleRef)new QAction(title.c_str());
    }
}
