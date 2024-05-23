#include "generated/Label.h"

#include <QLabel>

#define THIS ((QLabel*)_this)

namespace Label
{
    void Handle_setText(HandleRef _this, std::string text) {
        THIS->setText(text.c_str());
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create() {
        return (HandleRef) new QLabel();
    }
}
