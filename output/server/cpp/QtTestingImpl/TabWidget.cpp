#include "generated/TabWidget.h"

#include <QObject>
#include <QTabWidget>

#define THIS ((QTabWidget*)_this)
#define WIDGET(w) ((QWidget*)w)

namespace TabWidget
{
    void Handle_addTab(HandleRef _this, Widget::HandleRef page, std::string label) {
        THIS->addTab(WIDGET(page), label.c_str());
    }

    void Handle_insertTab(HandleRef _this, int32_t index, Widget::HandleRef page, std::string label) {
        THIS->insertTab(index, WIDGET(page), label.c_str());
    }

    void Handle_onCurrentChanged(HandleRef _this, std::function<IntDelegate> handler) {
        QObject::connect(
            THIS,
            &QTabWidget::currentChanged,
            THIS,
            handler);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create() {
        return (HandleRef)new QTabWidget();
    }
}
