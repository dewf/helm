#include "generated/Widget.h"

#include <QObject>
#include <QWidget>
#include "util/convert.h"

namespace Widget
{
    Rect Handle_getRect(HandleRef _this) {
        auto x = ((QWidget*)_this)->rect();
        return qRectToRect(x);
    }

    void Handle_resize(HandleRef _this, int32_t width, int32_t height) {
        ((QWidget*)_this)->resize(width, height);
    }

    void Handle_show(HandleRef _this) {
        ((QWidget*)_this)->show();
    }

    void Handle_setWindowTitle(HandleRef _this, std::string title) {
        ((QWidget*)_this)->setWindowTitle(title.c_str());
    }

    void Handle_setLayout(HandleRef _this, Layout::HandleRef layout) {
        ((QWidget*)_this)->setLayout((QLayout*)layout);
    }

    void Handle_onWindowTitleChanged(HandleRef _this, std::function<StringDelegate> func) {
        QObject::connect(
            (QWidget*)_this,
            &QWidget::windowTitleChanged,
            (QWidget*)_this,
            [func](const QString& title) {
                func(title.toStdString());
            });
    }

    void Handle_dispose(HandleRef _this) {
        delete (QWidget*)_this;
    }

    HandleRef create() {
        return (HandleRef)new QWidget();
    }
}
