#include "generated/BoxLayout.h"

#include <QBoxLayout>

namespace BoxLayout
{
    void Handle_addSpacing(HandleRef _this, int32_t size) {
        ((QBoxLayout*)_this)->addSpacing(size);
    }

    void Handle_addStretch(HandleRef _this, int32_t stretch) {
        ((QBoxLayout*)_this)->addStretch(stretch);
    }

    void Handle_addWidget(HandleRef _this, Widget::HandleRef widget) {
        ((QBoxLayout*)_this)->addWidget((QWidget*)widget);
    }

    void Handle_addWidget(HandleRef _this, Widget::HandleRef widget, int32_t stretch) {
        ((QBoxLayout*)_this)->addWidget((QWidget*)widget, stretch);
    }

    void Handle_dispose(HandleRef _this) {
        delete (QBoxLayout*)_this;
    }

    HandleRef create(Direction dir) {
        auto qDir = (QBoxLayout::Direction)dir;
        return (HandleRef)new QBoxLayout(qDir);
    }
}
