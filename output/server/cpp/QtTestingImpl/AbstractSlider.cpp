#include "generated/AbstractSlider.h"

#include <QObject>
#include <QAbstractSlider>

#define THIS ((QAbstractSlider*)_this)

namespace AbstractSlider
{
    void Handle_setOrientation(HandleRef _this, Orientation orient) {
        THIS->setOrientation((Qt::Orientation)orient);
    }

    void Handle_setRange(HandleRef _this, int32_t min, int32_t max) {
        THIS->setRange(min, max);
    }

    void Handle_setValue(HandleRef _this, int32_t value) {
        THIS->setValue(value);
    }

    void Handle_setSingleStep(HandleRef _this, int32_t step) {
        THIS->setSingleStep(step);
    }

    void Handle_setPageStep(HandleRef _this, int32_t pageStep) {
        THIS->setPageStep(pageStep);
    }

    void Handle_setTracking(HandleRef _this, bool value) {
        THIS->setTracking(value);
    }

    void Handle_onValueChanged(HandleRef _this, std::function<IntDelegate> handler) {
        QObject::connect(
                THIS,
                &QAbstractSlider::valueChanged,
                THIS,
                handler);
    }

    void Handle_dispose(HandleRef _this) {
        printf("AbstractSlider deleted directly (vs. via sublcass), why?\n");
        delete THIS;
    }
}
