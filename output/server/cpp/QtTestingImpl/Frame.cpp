#include "generated/Frame.h"

#include <QFrame>

#define THIS ((QFrame*)_this)

namespace Frame
{
    void Handle_setFrameShadow(HandleRef _this, Shadow shadow) {
        THIS->setFrameShadow((QFrame::Shadow)shadow);
    }

    void Handle_setFrameShape(HandleRef _this, Shape shape) {
        THIS->setFrameShape((QFrame::Shape)shape);
    }

    void Handle_setFrameStyle(HandleRef _this, Shape shape, Shadow shadow) {
        THIS->setFrameStyle((QFrame::Shape)shape | (QFrame::Shadow)shadow);
    }

    void Handle_setLineWidth(HandleRef _this, int32_t width) {
        THIS->setLineWidth(width);
    }

    void Handle_setMidLineWidth(HandleRef _this, int32_t width) {
        THIS->setMidLineWidth(width);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create() {
        return (HandleRef) new QFrame();
    }
}
