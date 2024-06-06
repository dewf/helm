#include "generated/RadioButton.h"

#include <QRadioButton>

#define THIS ((QRadioButton*)_this)

namespace RadioButton
{
    void Handle_setChecked(HandleRef _this, bool checked_) {
        THIS->setChecked(checked_);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create() {
        return (HandleRef) new QRadioButton();
    }
}
