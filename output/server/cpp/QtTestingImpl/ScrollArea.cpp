#include "generated/ScrollArea.h"

#include <QScrollArea>

#define THIS ((QScrollArea*)_this)

namespace ScrollArea
{
    void Handle_setWidget(HandleRef _this, Widget::HandleRef widget) {
        THIS->setWidget((QWidget*)widget);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create() {
        return (HandleRef) new QScrollArea();
    }
}
