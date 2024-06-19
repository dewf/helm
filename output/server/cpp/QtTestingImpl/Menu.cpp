#include "generated/Menu.h"

#include <QObject>
#include <QMenu>

#include "util/convert.h"
#include "util/SignalStuff.h"

#define THIS ((MenuWithHandler*)_this)

namespace Menu
{
    class MenuWithHandler : public QMenu {
        Q_OBJECT
    private:
        std::shared_ptr<SignalHandler> handler;
        uint32_t lastMask = 0;
        std::vector<SignalMapItem<SignalMask>> signalMap = {
                { SignalMask::AboutToHide, SIGNAL(aboutToHide()), SLOT(onAboutToHide()) },
                { SignalMask::AboutToShow, SIGNAL(aboutToShow()), SLOT(onAboutToShow()) },
                { SignalMask::Hovered, SIGNAL(hovered(QAction*)), SLOT(onHovered(QAction*)) },
                { SignalMask::Triggered, SIGNAL(triggered(QAction*)), SLOT(onTriggered(QAction*)) }
        };
    public:
        explicit MenuWithHandler(std::shared_ptr<SignalHandler> handler) : handler(std::move(handler)) {}
        void setSignalMask(uint32_t newMask) {
            if (newMask != lastMask) {
                processChanges(lastMask, newMask, signalMap, this);
                lastMask = newMask;
            }
        }
    public slots:
        void onAboutToHide() {
            handler->aboutToHide();
        }
        void onAboutToShow() {
            handler->aboutToShow();
        }
        void onHovered(QAction *action) {
            handler->hovered((Action::HandleRef)action);
        }
        void onTriggered(QAction *action) {
            handler->triggered((Action::HandleRef)action);
        }
    };

    void Handle_clear(HandleRef _this) {
        THIS->clear();
    }

    void Handle_setTitle(HandleRef _this, std::string title) {
        THIS->setTitle(title.c_str());
    }

    void Handle_popup(HandleRef _this, Point p) {
        THIS->popup(toQPoint(p));
    }

    void Handle_setSignalMask(HandleRef _this, uint32_t mask) {
        THIS->setSignalMask(mask);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create(std::shared_ptr<SignalHandler> handler) {
        return (HandleRef) new MenuWithHandler(std::move(handler));
    }
}

#include "Menu.moc"
