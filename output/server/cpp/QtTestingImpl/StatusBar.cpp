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
        uint32_t lastMask = 0;
        std::vector<SignalMapItem<SignalMask>> signalMap = {
            { SignalMask::MessageChanged, SIGNAL(messageChanged(QString)), SLOT(onMessageChanged(QString)) },
        };
    public:
        explicit StatusBarWithHandler(std::shared_ptr<SignalHandler> handler) : handler(std::move(handler)) {}
        void setSignalMask(uint32_t newMask) {
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
        THIS->showMessage(message.c_str(), timeout);
    }

    void Handle_setSignalMask(HandleRef _this, uint32_t mask) {
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