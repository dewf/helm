#include "generated/TabWidget.h"

#include <QObject>
#include <QTabWidget>
#include <utility>

#include "util/SignalStuff.h"

#define THIS ((TabWidgetWithHandler*)_this)
#define WIDGET(w) ((QWidget*)w)

namespace TabWidget
{
    class TabWidgetWithHandler : public QTabWidget {
        Q_OBJECT
    private:
        std::shared_ptr<SignalHandler> handler;
        uint32_t lastMask = 0;
        std::vector<SignalMapItem<SignalMask>> signalMap = {
                { SignalMask::CurrentChanged, SIGNAL(currentChanged(int)), SLOT(onCurrentChanged(int)) },
                { SignalMask::TabBarClicked, SIGNAL(tabBarClicked(int)), SLOT(onTabBarClicked(int)) },
                { SignalMask::TabBarDoubleClicked, SIGNAL(tabBarDoubleClicked(int)), SLOT(onTabBarDoubleClicked(int)) },
                { SignalMask::TabCloseRequested, SIGNAL(tabCloseRequested(int)), SLOT(onTabCloseRequested(int)) }
        };
    public:
        explicit TabWidgetWithHandler(std::shared_ptr<SignalHandler> handler) : handler(std::move(handler)) {}
        void setSignalMask(uint32_t newMask) {
            if (newMask != lastMask) {
                processChanges(lastMask, newMask, signalMap, this);
                lastMask = newMask;
            }
        }
    public slots:
        void onCurrentChanged(int index) {
            handler->currentChanged(index);
        }
        void onTabBarClicked(int index) {
            handler->tabBarClicked(index);
        }
        void onTabBarDoubleClicked(int index) {
            handler->tabBarDoubleClicked(index);
        }
        void onTabCloseRequested(int index) {
            handler->tabCloseRequested(index);
        }
    };

    void Handle_addTab(HandleRef _this, Widget::HandleRef page, std::string label) {
        THIS->addTab(WIDGET(page), QString::fromStdString(label));
    }

    void Handle_insertTab(HandleRef _this, int32_t index, Widget::HandleRef page, std::string label) {
        THIS->insertTab(index, WIDGET(page), QString::fromStdString(label));
    }

    Widget::HandleRef Handle_widgetAt(HandleRef _this, int32_t index) {
        return (Widget::HandleRef)THIS->widget(index);
    }

    void Handle_clear(HandleRef _this) {
        THIS->clear();
    }

    void Handle_removeTab(HandleRef _this, int32_t index) {
        THIS->removeTab(index);
    }

    void Handle_setSignalMask(HandleRef _this, uint32_t mask) {
        THIS->setSignalMask(mask);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create(std::shared_ptr<SignalHandler> handler) {
        return (HandleRef) new TabWidgetWithHandler(std::move(handler));
    }
}

#include "TabWidget.moc"
