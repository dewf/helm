#include "generated/SortFilterProxyModel.h"

#include <QSortFilterProxyModel>
#include "util/SignalStuff.h"

#include "RegularExpressionInternal.h"

#define THIS ((SortFilterProxyModelWithHandler*)_this)

namespace SortFilterProxyModel
{
    class SortFilterProxyModelWithHandler : public QSortFilterProxyModel {
        Q_OBJECT
    private:
        std::shared_ptr<SignalHandler> handler;
        uint32_t lastMask = 0;
        std::vector<SignalMapItem<SignalMask>> signalMap = {
            { SignalMask::AutoAcceptChildRowsChanged, SIGNAL(autoAcceptChildRowsChanged(bool)), SLOT(onAutoAcceptChildRowsChanged(bool)) },
        };
    public:
        explicit SortFilterProxyModelWithHandler(std::shared_ptr<SignalHandler> handler) : handler(std::move(handler)) {}
        void setSignalMask(uint32_t newMask) {
            if (newMask != lastMask) {
                processChanges(lastMask, newMask, signalMap, this);
                lastMask = newMask;
            }
        }
    public slots:
        void onAutoAcceptChildRowsChanged(bool autoAcceptChildRows) {
            handler->autoAcceptChildRowsChanged(autoAcceptChildRows);
        };
    };

    void Handle_setFilterRegularExpression(HandleRef _this, std::shared_ptr<Deferred::Base> regex) {
        THIS->setFilterRegularExpression(RegularExpression::fromDeferred(regex));
    }

    void Handle_setFilterKeyColumn(HandleRef _this, int32_t column) {
        THIS->setFilterKeyColumn(column);
    }

    void Handle_setSignalMask(HandleRef _this, uint32_t mask) {
        THIS->setSignalMask(mask);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create(std::shared_ptr<SignalHandler> handler) {
        return (HandleRef) new SortFilterProxyModelWithHandler(handler);
    }
}

#include "SortFilterProxyModel.moc"
