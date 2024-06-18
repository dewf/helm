#include "generated/BoxLayout.h"

#include <QBoxLayout>

#include "util/convert.h"

#define THIS ((QBoxLayout*)_this)

namespace BoxLayout
{
    void Handle_setDirection(HandleRef _this, Direction dir) {
        auto qDir = (QBoxLayout::Direction)dir;
        THIS->setDirection(qDir);
    }

    void Handle_addSpacing(HandleRef _this, int32_t size) {
        THIS->addSpacing(size);
    }

    void Handle_addStretch(HandleRef _this, int32_t stretch) {
        THIS->addStretch(stretch);
    }

    void Handle_addWidget(HandleRef _this, Widget::HandleRef widget) {
        THIS->addWidget((QWidget*)widget);
    }

    void Handle_addWidget(HandleRef _this, Widget::HandleRef widget, int32_t stretch) {
        THIS->addWidget((QWidget*)widget, stretch);
    }

    void Handle_addWidget(HandleRef _this, Widget::HandleRef widget, int32_t stretch, uint32_t align) {
        THIS->addWidget((QWidget*)widget, stretch, (Qt::AlignmentFlag)align);
    }

    void Handle_addLayout(HandleRef _this, Layout::HandleRef layout) {
        THIS->addLayout((QLayout*)layout);
    }

    void Handle_addLayout(HandleRef _this, Layout::HandleRef layout, int32_t stretch) {
        THIS->addLayout((QLayout*)layout, stretch);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create(Direction dir) {
        auto qDir = (QBoxLayout::Direction)dir;
        return (HandleRef) new QBoxLayout(qDir);
    }
}
