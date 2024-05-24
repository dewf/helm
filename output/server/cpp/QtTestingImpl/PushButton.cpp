#include "generated/PushButton.h"

#include <QObject>
#include <QPushButton>

#define THIS ((QPushButton*)_this)

namespace PushButton
{
    void Handle_setText(HandleRef _this, std::string label) {
        THIS->setText(label.c_str());
    }

    void Handle_onClicked(HandleRef _this, std::function<VoidDelegate> handler) {
        QObject::connect(
            THIS,
            &QPushButton::clicked,
            THIS,
            handler);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create() {
        return (HandleRef)new QPushButton();
    }

    HandleRef create(std::string label) {
        return (HandleRef)new QPushButton(label.c_str());
    }
}
