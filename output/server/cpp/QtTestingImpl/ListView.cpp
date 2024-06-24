#include "generated/ListView.h"

#include <QListView>
#include <QAbstractListModel>

#define THIS ((QListView*)_this)

namespace ListView
{
    void Handle_setModel(HandleRef _this, AbstractListModel::HandleRef model) {
        THIS->setModel((QAbstractListModel*)model);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create() {
        return (HandleRef) new QListView();
    }
}
