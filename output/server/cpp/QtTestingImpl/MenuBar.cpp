#include "generated/MenuBar.h"

#include <QObject>
#include <QMenuBar>

#include "util/SignalStuff.h"

#define THIS ((MenuBarWithHandler*)_this)

namespace MenuBar
{
    class MenuBarWithHandler : public QMenuBar {
        Q_OBJECT
    private:
        std::shared_ptr<SignalHandler> handler;
        uint32_t lastMask = 0;
        std::vector<SignalMapItem<SignalMask>> signalMap = {
            { SignalMask::Hovered, SIGNAL(hovered(QAction*)), SLOT(onHovered(QAction*)) },
            { SignalMask::Triggered, SIGNAL(triggered(QAction*)), SLOT(onTriggered(QAction*))}
        };
    public:
        explicit MenuBarWithHandler(std::shared_ptr<SignalHandler> handler) : handler(std::move(handler)) {}
        void setSignalMask(uint32_t newMask) {
            if (newMask != lastMask) {
                processChanges(lastMask, newMask, signalMap, this);
                lastMask = newMask;
            }
        }
    public slots:
        void onHovered(QAction* action) {
            handler->hovered((Action::HandleRef)action);
        }
        void onTriggered(QAction* action) {
            handler->triggered((Action::HandleRef)action);
        }
    };

    void Handle_clear(HandleRef _this) {
        THIS->clear();
    }

    void Handle_addMenu(HandleRef _this, Menu::HandleRef menu) {
        THIS->addMenu((QMenu*)menu);
    }

    void Handle_setSignalMask(HandleRef _this, uint32_t mask) {
        THIS->setSignalMask(mask);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create(std::shared_ptr<SignalHandler> handler) {
        return (HandleRef) new MenuBarWithHandler(std::move(handler));
    }
}

#include "MenuBar.moc"
