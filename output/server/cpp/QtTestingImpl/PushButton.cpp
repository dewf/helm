#include "generated/PushButton.h"

#include <QObject>
#include <QPushButton>
#include <utility>

#include "util/SignalStuff.h"

#define THIS ((PushButtonWithHandler*)_this)

namespace PushButton
{
    class PushButtonWithHandler : public QPushButton {
        Q_OBJECT
    private:
        std::shared_ptr<SignalHandler> handler;
        uint32_t lastMask = 0;
        std::vector<SignalMapItem<SignalMask>> signalMap = {
                { SignalMask::Clicked, SIGNAL(clicked(bool)), SLOT(onClicked(bool)) },
                { SignalMask::Pressed, SIGNAL(pressed()), SLOT(onPressed()) },
                { SignalMask::Released, SIGNAL(released()), SLOT(onReleased()) },
                { SignalMask::Toggled, SIGNAL(toggled(bool)), SLOT(onToggled(bool)) }
        };
    public:
        explicit PushButtonWithHandler(std::shared_ptr<SignalHandler> handler) : handler(std::move(handler)) {}

        void setSignalMask(uint32_t newMask) {
            if (newMask != lastMask) {
                processChanges(lastMask, newMask, signalMap, this);
                lastMask = newMask;
            }
        }
    public slots:
        void onClicked(bool checkState) {
            handler->clicked(checkState);
        }
        void onPressed() {
            handler->pressed();
        }
        void onReleased() {
            handler->released();
        }
        void onToggled(bool checkState) {
            handler->toggled(checkState);
        }
    };

    void Handle_setText(HandleRef _this, std::string label) {
        THIS->setText(label.c_str());
    }

    void Handle_setAutoDefault(HandleRef _this, bool value) {
        THIS->setAutoDefault(value);
    }

    void Handle_setSignalMask(HandleRef _this, uint32_t mask) {
        THIS->setSignalMask(mask);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create(std::shared_ptr<SignalHandler> handler) {
        return (HandleRef) new PushButtonWithHandler(std::move(handler));
    }
}

#include "PushButton.moc"
