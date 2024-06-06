#include "generated/AbstractButton.h"

#include <QObject>
#include <QAbstractButton>

#define THIS ((QAbstractButton*)_this)

namespace AbstractButton
{
    void Handle_setText(HandleRef _this, std::string text) {
        THIS->setText(text.c_str());
    }

    void Handle_onClicked(HandleRef _this, std::function<BoolDelegate> handler) {
        QObject::connect(
                THIS,
                &QAbstractButton::clicked,
                THIS,
                handler);
    }

    void Handle_onPressed(HandleRef _this, std::function<VoidDelegate> handler) {
        QObject::connect(
                THIS,
                &QAbstractButton::pressed,
                THIS,
                handler);
    }

    void Handle_onReleased(HandleRef _this, std::function<VoidDelegate> handler) {
        QObject::connect(
                THIS,
                &QAbstractButton::released,
                THIS,
                handler);
    }

    void Handle_onToggled(HandleRef _this, std::function<BoolDelegate> handler) {
        QObject::connect(
                THIS,
                &QAbstractButton::toggled,
                THIS,
                handler);
    }

    void Handle_dispose(HandleRef _this) {
        // we'll always be using this through another subtype (QPushButton, QRadioButton, etc)
        // soooo kind of odd to be deleting directly
        printf("suspicious Handle_dispose of AbstractButton\n");
    }
}
