#include "generated/Label.h"

#include <QLabel>
#include "util/convert.h"

#define THIS ((QLabel*)_this)

namespace Label
{
    void Handle_setText(HandleRef _this, std::string text) {
        THIS->setText(text.c_str());
    }

    void Handle_setAlignment(HandleRef _this, uint32_t align) {
        THIS->setAlignment((Qt::AlignmentFlag)align);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create() {
        return (HandleRef) new QLabel();
    }
}
