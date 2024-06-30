#include "generated/PushButton.h"

#include <QPushButton>
#include "util/SignalStuff.h"
#include "util/convert.h"

#define THIS ((PushButtonWithHandler*)_this)

namespace PushButton
{
    class PushButtonWithHandler : public QPushButton {
        Q_OBJECT
    private:
        std::shared_ptr<SignalHandler> handler;
        SignalMask lastMask = 0;
        std::vector<SignalMapItem<SignalMaskFlags>> signalMap = {
            // Widget
            { SignalMaskFlags::CustomContextMenuRequested, SIGNAL(customContextMenuRequested(QPoint)), SLOT(onCustomContextMenuRequested(QPoint)) },
            { SignalMaskFlags::WindowIconChanged, SIGNAL(windowIconChanged(QIcon)), SLOT(onWindowIconChanged(QIcon)) },
            { SignalMaskFlags::WindowTitleChanged, SIGNAL(windowTitleChanged(QString)), SLOT(onWindowTitleChanged(QString)) },
            // AbstractButton
            { SignalMaskFlags::Clicked, SIGNAL(clicked(bool)), SLOT(onClicked(bool)) },
            { SignalMaskFlags::Pressed, SIGNAL(pressed()), SLOT(onPressed()) },
            { SignalMaskFlags::Released, SIGNAL(released()), SLOT(onReleased()) },
            { SignalMaskFlags::Toggled, SIGNAL(toggled(bool)), SLOT(onToggled(bool)) }
            // none for PushButton
        };
    public:
        explicit PushButtonWithHandler(std::shared_ptr<SignalHandler> handler) : handler(std::move(handler)) {}
        void setSignalMask(SignalMask newMask) {
            if (newMask != lastMask) {
                processChanges(lastMask, newMask, signalMap, this);
                lastMask = newMask;
            }
        }
    public slots:
        // Widget:
        void onCustomContextMenuRequested(const QPoint& pos) {
            handler->customContextMenuRequested(toPoint(pos));
        }
        void onWindowIconChanged(const QIcon& icon) {
            handler->windowIconChanged((Icon::HandleRef)&icon);
        }
        void onWindowTitleChanged(const QString& title) {
            handler->windowTitleChanged(title.toStdString());
        }
        // AbstractButton:
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

    void Handle_setAutoDefault(HandleRef _this, bool value) {
        THIS->setAutoDefault(value);
    }

    void Handle_setDefault(HandleRef _this, bool value) {
        THIS->setDefault(value);
    }

    void Handle_setFlat(HandleRef _this, bool value) {
        THIS->setFlat(value);
    }

    void Handle_setSignalMask(HandleRef _this, SignalMask mask) {
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
