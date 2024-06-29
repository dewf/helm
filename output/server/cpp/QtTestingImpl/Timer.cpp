#include "generated/Timer.h"

#include <QTimer>
#include <QObject>
#include <chrono>
#include <utility>

#include "util/SignalStuff.h"

#define THIS ((TimerWithHandler*)_this)

namespace Timer
{
    class TimerWithHandler : public QTimer {
        Q_OBJECT
    private:
        std::shared_ptr<SignalHandler> handler;
        SignalMask lastMask = 0;
        std::vector<SignalMapItem<SignalMaskFlags>> signalMap = {
            { SignalMaskFlags::Timeout, SIGNAL(timeout()), SLOT(onTimeout()) },
        };
    public:
        explicit TimerWithHandler(std::shared_ptr<SignalHandler> handler) : handler(std::move(handler)) {}
        void setSignalMask(SignalMask newMask) {
            if (newMask != lastMask) {
                processChanges(lastMask, newMask, signalMap, this);
                lastMask = newMask;
            }
        }
    public slots:
        void onTimeout() {
            handler->timeout();
        };
    };

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

    void Handle_setSignalMask(HandleRef _this, SignalMask mask) {
        THIS->setSignalMask(mask);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create(std::shared_ptr<SignalHandler> handler) {
        return (HandleRef) new TimerWithHandler(std::move(handler));
    }
}

#include "Timer.moc"
