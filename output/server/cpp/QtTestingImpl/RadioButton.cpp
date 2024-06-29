#include "generated/RadioButton.h"

#include <QRadioButton>
#include <utility>

#include "util/SignalStuff.h"

#define THIS ((RadioButtonWithHandler*)_this)

namespace RadioButton
{
    class RadioButtonWithHandler : public QRadioButton {
        Q_OBJECT
    private:
        std::shared_ptr<SignalHandler> handler;
        SignalMask lastMask = 0;
        std::vector<SignalMapItem<SignalMaskFlags>> signalMap = {
            { SignalMaskFlags::Clicked, SIGNAL(clicked(bool)), SLOT(onClicked(bool)) },
            { SignalMaskFlags::Pressed, SIGNAL(pressed()), SLOT(onPressed()) },
            { SignalMaskFlags::Released, SIGNAL(released()), SLOT(onReleased()) },
            { SignalMaskFlags::Toggled, SIGNAL(toggled(bool)), SLOT(onToggled(bool)) }
        };
    public:
        explicit RadioButtonWithHandler(std::shared_ptr<SignalHandler> handler) : handler(std::move(handler)) {}

        void setSignalMask(SignalMask newMask) {
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

    void Handle_setSignalMask(HandleRef _this, SignalMask mask) {
        THIS->setSignalMask(mask);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create(std::shared_ptr<SignalHandler> handler) {
        return (HandleRef) new RadioButtonWithHandler(std::move(handler));
    }
}

#include "RadioButton.moc"
