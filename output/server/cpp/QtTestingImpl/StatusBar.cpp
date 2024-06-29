#include "generated/StatusBar.h"

#include <QStatusBar>
#include <utility>
#include "util/SignalStuff.h"

#define THIS ((StatusBarWithHandler*)_this)

namespace StatusBar
{
    class StatusBarWithHandler : public QStatusBar {
        Q_OBJECT
    private:
        std::shared_ptr<SignalHandler> handler;
        SignalMask lastMask = 0;
        std::vector<SignalMapItem<SignalMaskFlags>> signalMap = {
            { SignalMaskFlags::MessageChanged, SIGNAL(messageChanged(QString)), SLOT(onMessageChanged(QString)) },
        };
    public:
        explicit StatusBarWithHandler(std::shared_ptr<SignalHandler> handler) : handler(std::move(handler)) {}
        void setSignalMask(SignalMask newMask) {
            if (newMask != lastMask) {
                processChanges(lastMask, newMask, signalMap, this);
                lastMask = newMask;
            }
        }
    public slots:
        void onMessageChanged(const QString& message) {
            handler->messageChanged(message.toStdString());
        };
    };

    void Handle_clearMessage(HandleRef _this) {
        THIS->clearMessage();
    }

    void Handle_showMessage(HandleRef _this, std::string message, int32_t timeout) {
        THIS->showMessage(QString::fromStdString(message), timeout);
    }

    void Handle_setSignalMask(HandleRef _this, SignalMask mask) {
        THIS->setSignalMask(mask);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create(std::shared_ptr<SignalHandler> handler) {
        return (HandleRef) new StatusBarWithHandler(std::move(handler));
    }
}

#include "StatusBar.moc"
