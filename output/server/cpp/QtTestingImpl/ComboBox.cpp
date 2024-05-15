#include "generated/ComboBox.h"

#include <QComboBox>

namespace ComboBox
{
    void Handle_todo(HandleRef _this) {
    }

    void Handle_dispose(HandleRef _this) {
        delete (QComboBox*)_this;
    }

    HandleRef create() {
        return (HandleRef)new QComboBox();
    }
}
