#include "generated/Widget.h"

#include <QObject>
#include <QWidget>
#include "util/convert.h"

#define THIS ((QWidget*)_this)

namespace Widget
{
    const int32_t WIDGET_SIZE_MAX = QWIDGETSIZE_MAX;

    void Handle_setMaximumWidth(HandleRef _this, int32_t maxWidth) {
        THIS->setMaximumWidth(maxWidth);
    }

    void Handle_setMaximumHeight(HandleRef _this, int32_t maxHeight) {
        THIS->setMaximumHeight(maxHeight);
    }

    Rect Handle_getRect(HandleRef _this) {
        auto x = THIS->rect();
        return qRectToRect(x);
    }

    void Handle_resize(HandleRef _this, int32_t width, int32_t height) {
        THIS->resize(width, height);
    }

    void Handle_show(HandleRef _this) {
        THIS->show();
    }

    void Handle_setWindowTitle(HandleRef _this, std::string title) {
        THIS->setWindowTitle(title.c_str());
    }

    void Handle_setLayout(HandleRef _this, Layout::HandleRef layout) {
        THIS->setLayout((QLayout*)layout);
    }

    void Handle_onWindowTitleChanged(HandleRef _this, std::function<StringDelegate> func) {
        QObject::connect(
            THIS,
            &QWidget::windowTitleChanged,
            THIS,
            [func](const QString& title) {
                func(title.toStdString());
            });
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create() {
        return (HandleRef)new QWidget();
    }
}
