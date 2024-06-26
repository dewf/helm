#include "generated/AbstractProxyModel.h"

#include <QAbstractProxyModel>

#define THIS ((QAbstractProxyModel*)_this)

namespace AbstractProxyModel
{
    void Handle_setSourceModel(HandleRef _this, AbstractItemModel::HandleRef sourceModel) {
        THIS->setSourceModel((QAbstractItemModel*)sourceModel);
    }
}
