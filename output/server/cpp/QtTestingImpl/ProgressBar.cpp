#include "generated/ProgressBar.h"

#include <QProgressBar>

#define THIS ((QProgressBar*)_this)

namespace ProgressBar
{
    void Handle_setRange(HandleRef _this, int32_t min, int32_t max) {
        THIS->setRange(min, max);
    }

    void Handle_setValue(HandleRef _this, int32_t value) {
        THIS->setValue(value);
    }

    void Handle_setTextVisible(HandleRef _this, bool visible) {
        THIS->setTextVisible(visible);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create() {
        return (HandleRef)new QProgressBar();
    }
}
