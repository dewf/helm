#include "generated/AbstractItemView.h"

#include <QAbstractItemView>

#define THIS ((QAbstractItemView*)_this)

namespace AbstractItemView
{
    void Handle_setModel(HandleRef _this, AbstractItemModel::HandleRef model) {
        THIS->setModel((QAbstractItemModel*)model);
    }
}
