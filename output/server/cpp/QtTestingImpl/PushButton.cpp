#include "generated/PushButton.h"

#include <QObject>
#include <QPushButton>

namespace PushButton
{
    void Handle_onClicked(HandleRef _this, std::function<VoidDelegate> handler) {
        QObject::connect(
            (QPushButton*)_this,
            &QPushButton::clicked,
            (QPushButton*)_this,
            handler);
    }

    void Handle_dispose(HandleRef _this) {
        delete (QPushButton*)_this;
    }

    HandleRef create(std::string label) {
        return (HandleRef)new QPushButton(label.c_str());
    }
}
