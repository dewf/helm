#include "generated/ListWidget.h"

#include <QObject>
#include <QListView>
#include <QListWidget>

#define THIS ((QListWidget*)_this)

namespace ListWidget
{
    std::vector<int32_t> Handle_selectedIndices(HandleRef _this) {
        std::vector<int32_t> result;
        for (auto item : THIS->selectedItems()) {
            // seems inefficient but AFAIK no other easy way (other than maybe setting data per item? hmm)
            result.push_back(THIS->row(item));
        }
        return result;
    }

    void Handle_setItems(HandleRef _this, std::vector<std::string> items) {
        THIS->clear(); // sufficient?
        for (auto &str : items) {
            new QListWidgetItem(str.c_str(), THIS);
        }
    }

    void Handle_setSelectionMode(HandleRef _this, SelectionMode mode) {
        // should be a 1:1 mapping
        // but how can we always keep these in sync across Qt versions, without having to resort to explicit switch() statements to verify individual values are the same?
        THIS->setSelectionMode((QAbstractItemView::SelectionMode)mode);
    }

    void Handle_setCurrentRow(HandleRef _this, int32_t index) {
        THIS->setCurrentRow(index);
    }

    void Handle_scrollToRow(HandleRef _this, int32_t index, ScrollHint hint) {
        auto item = THIS->item(index);
        if (item != nullptr) {
            THIS->scrollToItem(item, (QAbstractItemView::ScrollHint)hint);
        }
    }

    void Handle_onCurrentRowChanged(HandleRef _this, std::function<IntDelegate> handler) {
        QObject::connect(
            THIS,
            &QListWidget::currentRowChanged,
            THIS,
            handler);
    }

    void Handle_onItemSelectionChanged(HandleRef _this, std::function<VoidDelegate> handler) {
        QObject::connect(
            THIS,
            &QListWidget::itemSelectionChanged,
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
