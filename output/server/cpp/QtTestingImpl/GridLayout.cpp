#include "generated/GridLayout.h"

#include <QGridLayout>
#include "util/convert.h"

#define THIS ((QGridLayout*)_this)

namespace GridLayout
{
    void Handle_addWidget(HandleRef _this, Widget::HandleRef widget, int32_t row, int32_t col) {
        THIS->addWidget((QWidget*)widget, row, col);
    }

    void Handle_addWidget(HandleRef _this, Widget::HandleRef widget, int32_t row, int32_t col, Alignment align) {
        THIS->addWidget((QWidget*)widget, row, col, toQtAlign(align));
    }

    void Handle_addWidget(HandleRef _this, Widget::HandleRef widget, int32_t row, int32_t col, int32_t rowSpan, int32_t colSpan) {
        THIS->addWidget((QWidget*)widget, row, col, rowSpan, colSpan);
    }

    void Handle_addWidget(HandleRef _this, Widget::HandleRef widget, int32_t row, int32_t col, int32_t rowSpan, int32_t colSpan, Alignment align) {
        THIS->addWidget((QWidget*)widget, row, col, rowSpan, colSpan, toQtAlign(align));
    }

    void Handle_addLayout(HandleRef _this, Layout::HandleRef layout, int32_t row, int32_t col) {
        THIS->addLayout((QLayout*)layout, row, col);
    }

    void Handle_addLayout(HandleRef _this, Layout::HandleRef layout, int32_t row, int32_t col, Alignment align) {
        THIS->addLayout((QLayout*)layout, row, col, toQtAlign(align));
    }

    void Handle_addLayout(HandleRef _this, Layout::HandleRef layout, int32_t row, int32_t col, int32_t rowSpan, int32_t colSpan) {
        THIS->addLayout((QLayout*)layout, row, col, rowSpan, colSpan);
    }

    void Handle_addLayout(HandleRef _this, Layout::HandleRef layout, int32_t row, int32_t col, int32_t rowSpan, int32_t colSpan, Alignment align) {
        THIS->addLayout((QLayout*)layout, row, col, rowSpan, colSpan, toQtAlign(align));
    }

    void Handle_setRowMinimumHeight(HandleRef _this, int32_t row, int32_t minHeight) {
        THIS->setRowMinimumHeight(row, minHeight);
    }

    void Handle_setRowStretch(HandleRef _this, int32_t row, int32_t stretch) {
        THIS->setRowStretch(row, stretch);
    }

    void Handle_setColumnMinimumWidth(HandleRef _this, int32_t column, int32_t minWidth) {
        THIS->setColumnMinimumWidth(column, minWidth);
    }

    void Handle_setColumnStretch(HandleRef _this, int32_t column, int32_t stretch) {
        THIS->setColumnStretch(column, stretch);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create() {
        return (HandleRef) new QGridLayout();
    }
}
