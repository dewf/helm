#include "generated/Action.h"

#include <QObject>
#include <QAction>

#include "util/SignalStuff.h"
#include "IconInternal.h"
#include "KeySequenceInternal.h"

#define THIS ((ActionWithHandler*)_this)

namespace Action
{
    class ActionWithHandler : public QAction {
        Q_OBJECT
    private:
        std::shared_ptr<SignalHandler> handler;
        uint32_t lastMask = 0;
        std::vector<SignalMapItem<SignalMask>> signalMap = {
            { SignalMask::Changed, SIGNAL(changed()), SLOT(onChanged) },
            { SignalMask::CheckableChanged, SIGNAL(checkableChanged(bool)), SLOT(onCheckableChanged(bool)) },
            { SignalMask::EnabledChanged, SIGNAL(enabledChanged(bool)), SLOT(onEnabledChanged(bool) )},
            { SignalMask::Hovered, SIGNAL(hovered()), SLOT(onHovered()) },
            { SignalMask::Toggled, SIGNAL(toggled(bool)), SLOT(onToggled(bool)) },
            { SignalMask::Triggered, SIGNAL(triggered(bool)), SLOT(onTriggered(bool)) },
            { SignalMask::VisibleChanged, SIGNAL(visibleChanged()), SLOT(onVisibleChanged()) }
        };
    public:
        explicit ActionWithHandler(std::shared_ptr<SignalHandler> handler) : handler(std::move(handler)) {}
        void setSignalMask(uint32_t newMask) {
            if (newMask != lastMask) {
                processChanges(lastMask, newMask, signalMap, this);
                lastMask = newMask;
            }
        }
    public slots:
        void onChanged() {
            handler->changed();
        }
        void onCheckableChanged(bool checkable) {
            handler->checkableChanged(checkable);
        }
        void onEnabledChanged(bool enabled) {
            handler->enabledChanged(enabled);
        }
        void onHovered() {
            handler->hovered();
        }
        void onToggled(bool checked_) {
            handler->toggled(checked_);
        }
        void onTriggered(bool checked_) {
            handler->triggered(checked_);
        }
        void onVisibleChanged() {
            handler->visibleChanged();
        }
    };

    void Handle_setText(HandleRef _this, std::string text) {
        THIS->setText(text.c_str());
    }

    void Handle_setSeparator(HandleRef _this, bool state) {
        THIS->setSeparator(state);
    }

    void Handle_setEnabled(HandleRef _this, bool state) {
        THIS->setEnabled(state);
    }

    void Handle_setIcon(HandleRef _this, Icon::HandleRef icon) {
        THIS->setIcon(icon->icon);
    }

    void Handle_setIconText(HandleRef _this, std::string text) {
        THIS->setIconText(text.c_str());
    }

    void Handle_setShortcut(HandleRef _this, KeySequence::HandleRef seq) {
        THIS->setShortcut(seq->seq);
    }

    void Handle_setStatusTip(HandleRef _this, std::string tip) {
        THIS->setStatusTip(tip.c_str());
    }

    void Handle_setToolTip(HandleRef _this, std::string tip) {
        THIS->setToolTip(tip.c_str());
    }

    void Handle_setSignalMask(HandleRef _this, uint32_t mask) {
        THIS->setSignalMask(mask);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create(std::shared_ptr<SignalHandler> handler) {
        return (HandleRef) new ActionWithHandler(std::move(handler));
    }
}

#include "Action.moc"
