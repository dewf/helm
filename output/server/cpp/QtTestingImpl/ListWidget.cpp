#include "generated/ListWidget.h"

#include <QObject>
#include <QListWidget>
#include <QListWidgetItem>

#define THIS ((QListWidget*)_this)

namespace ListWidget
{
    void Handle_setItems(HandleRef _this, std::vector<std::string> items) {
        for (auto i = items.begin(); i != items.end(); i++) {
            new QListWidgetItem(i->c_str(), THIS);
        }
    }

    void Handle_setSelectionMode(HandleRef _this, SelectionMode mode) {
        // should be a 1:1 mapping
        // but how can we always keep these in sync across Qt versions, without having to resort to explicit switch() statements to verify individual values are the same?
        THIS->setSelectionMode((QAbstractItemView::SelectionMode)mode);
    }

    void Handle_onCurrentRowChanged(HandleRef _this, std::function<IntDelegate> handler) {
        QObject::connect(
            THIS,
            &QListWidget::currentRowChanged,
            THIS,
            handler);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create() {
        return (HandleRef)new QListWidget();
    }
}
