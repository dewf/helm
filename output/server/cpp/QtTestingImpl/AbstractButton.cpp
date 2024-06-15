#include "generated/AbstractButton.h"

#include <QObject>
#include <QAbstractButton>

#define THIS ((QAbstractButton*)_this)

namespace AbstractButton
{
    void Handle_setText(HandleRef _this, std::string text) {
        THIS->setText(text.c_str());
    }

    void Handle_setCheckable(HandleRef _this, bool checkable) {
        THIS->setCheckable(checkable);
    }

    void Handle_setChecked(HandleRef _this, bool checkState) {
        THIS->setChecked(checkState);
    }

    void Handle_dispose(HandleRef _this) {
        // we'll always be using this through another subtype (QPushButton, QRadioButton, etc)
        // soooo kind of odd to be deleting directly
        printf("suspicious Handle_dispose of AbstractButton\n");
        delete THIS;
    }
}
