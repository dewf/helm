#include "generated/GroupBox.h"

#include <QGroupBox>

#define THIS ((QGroupBox*)_this)

namespace GroupBox {
    void Handle_setTitle(HandleRef _this, std::string title) {
        THIS->setTitle(title.c_str());
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create() {
        return (HandleRef) new QGroupBox();
    }
}
