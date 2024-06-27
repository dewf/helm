#include "generated/AbstractProxyModel.h"

#include <QAbstractProxyModel>
#include <QModelIndex>

#define THIS ((QAbstractProxyModel*)_this)

namespace AbstractProxyModel
{
    void Handle_setSourceModel(HandleRef _this, AbstractItemModel::HandleRef sourceModel) {
        THIS->setSourceModel((QAbstractItemModel*)sourceModel);
    }

    OwnedHandleRef Handle_mapToSource(HandleRef _this, ModelIndex::HandleRef proxyIndex) {
        auto proxyValue = *((QModelIndex*)proxyIndex);
        auto retValue = THIS->mapToSource(proxyValue);
        // most model indexes are pointers to stack-allocated stuff on the C++ side, but this one we are responsible for!
        return (OwnedHandleRef) new QModelIndex(retValue);
    }
}
