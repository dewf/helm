#include "generated/Slider.h"

#include <QSlider>

#define THIS ((QSlider*)_this)

namespace Slider
{
    void Handle_setTickPosition(HandleRef _this, TickPosition tpos) {
        THIS->setTickPosition((QSlider::TickPosition)tpos);
    }

    void Handle_setTickInterval(HandleRef _this, int32_t interval) {
        THIS->setTickInterval(interval);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create() {
        return (HandleRef)new QSlider();
    }
}
