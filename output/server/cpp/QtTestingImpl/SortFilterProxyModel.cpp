#include "generated/SortFilterProxyModel.h"

#include <QSortFilterProxyModel>
#include "RegularExpressionInternal.h"

#define THIS ((QSortFilterProxyModel*)_this)

namespace SortFilterProxyModel
{
    void Handle_setFilterRegularExpression(HandleRef _this, std::shared_ptr<Deferred::Base> regex) {
        THIS->setFilterRegularExpression(RegularExpression::fromDeferred(regex));
    }

    void Handle_setFilterKeyColumn(HandleRef _this, int32_t column) {
        THIS->setFilterKeyColumn(column);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create() {
        return (HandleRef) new QSortFilterProxyModel();
    }
}
