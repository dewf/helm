#include "generated/ToolBar.h"

#include <QToolBar>
#include <utility>
#include "util/SignalStuff.h"
#include "util/convert.h"

#define THIS ((ToolBarWithHandler*)_this)

namespace ToolBar
{
    class ToolBarWithHandler : public QToolBar {
        Q_OBJECT
    private:
        std::shared_ptr<SignalHandler> handler;
        SignalMask lastMask = 0;
        std::vector<SignalMapItem<SignalMaskFlags>> signalMap = {
            { SignalMaskFlags::ActionTriggered, SIGNAL(actionTriggered(QAction)), SLOT(onActionTriggered(QAction)) },
            { SignalMaskFlags::AllowedAreasChanged, SIGNAL(allowedAreasChanged(Qt::ToolBarAreas)), SLOT(onAllowedAreasChanged(Qt::ToolBarAreas)) },
            { SignalMaskFlags::IconSizeChanged, SIGNAL(iconSizeChanged(QSize)), SLOT(onIconSizeChanged(QSize)) },
            { SignalMaskFlags::MoveableChanged, SIGNAL(movableChanged(bool)), SLOT(onMovableChanged(bool)) },
            { SignalMaskFlags::OrientationChanged, SIGNAL(orientationChanged(Qt::Orientation)), SLOT(onOrientationChanged(Qt::Orientation)) },
            { SignalMaskFlags::ToolButtonStyleChanged, SIGNAL(toolButtonStyleChanged(Qt::ToolButtonStyle)), SLOT(onToolButtonStyleChanged(Qt::ToolButtonStyle)) },
            { SignalMaskFlags::TopLevelChanged, SIGNAL(topLevelChanged(bool)), SLOT(onTopLevelChanged(bool)) },
            { SignalMaskFlags::VisibilityChanged, SIGNAL(visibilityChanged(bool)), SLOT(onVisibilityChanged(bool)) },
        };
    public:
        explicit ToolBarWithHandler(std::shared_ptr<SignalHandler> handler) : handler(std::move(handler)) {}
        void setSignalMask(SignalMask newMask) {
            if (newMask != lastMask) {
                processChanges(lastMask, newMask, signalMap, this);
                lastMask = newMask;
            }
        }
    public slots:
        void onActionTriggered(QAction *action) {
            handler->actionTriggered((Action::HandleRef)action);
        };
        void onAllowedAreasChanged(Qt::ToolBarAreas allowed) {
            handler->allowedAreasChanged(allowed);
        }
        void onIconSizeChanged(const QSize& size) {
            handler->iconSizeChanged(toSize(size));
        }
        void onMovableChanged(bool movable) {
            handler->movableChanged(movable);
        }
        void onOrientationChanged(Qt::Orientation value) {
            handler->orientationChanged((Enums::Orientation)value);
        }
        void onToolButtonStyleChanged(Qt::ToolButtonStyle style) {
            handler->toolButtonStyleChanged((ToolButtonStyle)style);
        }
        void onTopLevelChanged(bool topLevel) {
            handler->topLevelChanged(topLevel);
        }
        void onVisibilityChanged(bool visible) {
            handler->visibilityChanged(visible);
        }
    };

    Action::HandleRef Handle_addSeparator(HandleRef _this) {
        return (Action::HandleRef)THIS->addSeparator();
    }

    void Handle_addWidget(HandleRef _this, Widget::HandleRef widget) {
        THIS->addWidget((QWidget*)widget);
    }

    void Handle_clear(HandleRef _this) {
        THIS->clear();
    }

    void Handle_setAllowedAreas(HandleRef _this, Enums::ToolBarAreas allowed) {
        THIS->setAllowedAreas((Qt::ToolBarAreas)allowed);
    }

    void Handle_setFloatable(HandleRef _this, bool floatable) {
        THIS->setFloatable(floatable);
    }

    void Handle_setIconSize(HandleRef _this, Size size) {
        THIS->setIconSize(toQSize(size));
    }

    void Handle_setMovable(HandleRef _this, bool value) {
        THIS->setMovable(value);
    }

    void Handle_setOrientation(HandleRef _this, Orientation value) {
        THIS->setOrientation((Qt::Orientation)value);
    }

    void Handle_setToolButtonStyle(HandleRef _this, ToolButtonStyle style) {
        THIS->setToolButtonStyle((Qt::ToolButtonStyle)style);
    }

    void Handle_setSignalMask(HandleRef _this, SignalMask mask) {
        THIS->setSignalMask(mask);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create(std::shared_ptr<SignalHandler> handler) {
        return (HandleRef) new ToolBarWithHandler(std::move(handler));
    }
}

#include "ToolBar.moc"
