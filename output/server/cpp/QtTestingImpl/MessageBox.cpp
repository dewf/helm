#include "generated/MessageBox.h"

#include <QMessageBox>

#define THIS ((QMessageBox*)_this)

namespace MessageBox
{
    void Handle_setText(HandleRef _this, std::string text) {
        THIS->setText(QString::fromStdString(text));
    }

    void Handle_setInformativeText(HandleRef _this, std::string text) {
        THIS->setInformativeText(QString::fromStdString(text));
    }

    void Handle_setStandardButtons(HandleRef _this, uint32_t buttons) {
        THIS->setStandardButtons((QMessageBox::StandardButtons)buttons);
    }

    void Handle_setDefaultButton(HandleRef _this, uint32_t button) {
        THIS->setDefaultButton((QMessageBox::StandardButton)button);
    }

    void Handle_setIcon(HandleRef _this, Icon icon) {
        THIS->setIcon((QMessageBox::Icon)icon);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create() {
        return (HandleRef) new QMessageBox();
    }
}
