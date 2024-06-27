#include "generated/AbstractItemModel.h"

#include <QAbstractItemModel>
#include <QModelIndex>

#include "ModelIndexInternal.h"

#define THIS ((QAbstractItemModel*)_this)

namespace AbstractItemModel
{
    OwnedHandleRef Handle_index(HandleRef _this, int32_t row, int32_t column) {
        auto value = THIS->index(row, column);
        return (ModelIndex::OwnedHandleRef) new QModelIndex(value);
    }

    OwnedHandleRef Handle_index(HandleRef _this, int32_t row, int32_t column, std::shared_ptr<Deferred::Base> parent) {
        auto value = THIS->index(row, column, ModelIndex::fromDeferred(parent));
        return (ModelIndex::OwnedHandleRef) new QModelIndex(value);
    }
}
