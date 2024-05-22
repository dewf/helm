#include "generated/Timer.h"

#include <QTimer>
#include <QObject>
#include <chrono>

#define THIS ((QTimer*)_this)

namespace Timer
{
    void Handle_setInterval(HandleRef _this, int32_t msec) {
        THIS->setInterval(msec);
    }

    void Handle_setSingleShot(HandleRef _this, bool state) {
        THIS->setSingleShot(state);
    }

    void Handle_start(HandleRef _this, int32_t msec) {
        THIS->start(msec);
    }

    void Handle_start(HandleRef _this) {
        THIS->start();
    }

    void Handle_stop(HandleRef _this) {
        THIS->stop();
    }

    void Handle_onTimeout(HandleRef _this, std::function<VoidDelegate> handler) {
        QObject::connect(
                THIS,
                &QTimer::timeout,
                THIS,
                handler);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create() {
        return (HandleRef) new QTimer();
    }

    void singleShot(int32_t msec, std::function<VoidDelegate> handler) {
        auto interval = std::chrono::milliseconds(msec);
        QTimer::singleShot(interval, handler);
    }
}
