#include "generated/Action.h"

#include <QObject>
#include <QAction>

#include "util/SignalStuff.h"
#include "util/convert.h"
#include "IconInternal.h"

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
        ActionWithHandler(QObject *parent, const std::shared_ptr<SignalHandler> &handler) : QAction(parent), handler(handler) {}
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

    void Handle_setEnabled(HandleRef _this, bool state) {
        THIS->setEnabled(state);
    }

    void Handle_setText(HandleRef _this, std::string text) {
        THIS->setText(text.c_str());
    }

    void Handle_setSeparator(HandleRef _this, bool state) {
        THIS->setSeparator(state);
    }

    void Handle_setCheckable(HandleRef _this, bool state) {
        THIS->setCheckable(state);
    }

    void Handle_setChecked(HandleRef _this, bool state) {
        THIS->setChecked(state);
    }

    class SetIconVisitor : public Icon::Deferred::Visitor {
    private:
        HandleRef _this;
    public:
        explicit SetIconVisitor(HandleRef actionThis) : _this(actionThis) {}

        void onFromThemeIcon(const Icon::Deferred::FromThemeIcon *fromThemeIcon) override {
            auto icon = QIcon::fromTheme((QIcon::ThemeIcon)fromThemeIcon->themeIcon);
            THIS->setIcon(icon);
        }

        void onFromFilename(const Icon::Deferred::FromFilename *fromFilename) override {
            QIcon icon(fromFilename->filename.c_str());
            THIS->setIcon(icon);
        }
    };

    void Handle_setIcon(HandleRef _this, std::shared_ptr<Icon::Deferred::Base> icon) {
        SetIconVisitor visitor(_this);
        icon->accept(&visitor);
    }

    void Handle_setIconText(HandleRef _this, std::string text) {
        THIS->setIconText(text.c_str());
    }

    // visitor (pattern match) for Handle_setShortcut
    class SetShortcutVisitor : public KeySequence::Deferred::Visitor {
    private:
        HandleRef _this;
    public:
        explicit SetShortcutVisitor(HandleRef actionThis) : _this(actionThis) {}

        void onFromString(const KeySequence::Deferred::FromString *fromString) override {
            QKeySequence seq(fromString->s.c_str());
            THIS->setShortcut(seq);
        }

        void onFromStandard(const KeySequence::Deferred::FromStandard *fromStandard) override {
            QKeySequence seq((QKeySequence::StandardKey)fromStandard->key);
            THIS->setShortcut(seq);
        }

        void onFromKey(const KeySequence::Deferred::FromKey *fromKey) override {
            // could have also used a QKeyCombination instead of 'key | mods'
            auto key = (Qt::Key)fromKey->key;
            auto mods = (Qt::Modifiers)fromKey->modifiers;
            QKeySequence seq(key | mods);
            THIS->setShortcut(seq);
        }
    };

    void Handle_setShortcut(HandleRef _this, std::shared_ptr<KeySequence::Deferred::Base> seq) {
        SetShortcutVisitor visitor(_this);
        seq->accept(&visitor);
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

    HandleRef create(Object::HandleRef owner, std::shared_ptr<SignalHandler> handler) {
        return (HandleRef) new ActionWithHandler((QObject*)owner, handler);
    }
}

#include "Action.moc"
