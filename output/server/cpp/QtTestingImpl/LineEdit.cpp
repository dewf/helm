#include "generated/LineEdit.h"

#include <QObject>
#include <QLineEdit>

namespace LineEdit
{
    void Handle_setText(HandleRef _this, std::string str) {
        ((QLineEdit*)_this)->setText(str.c_str());
    }

    void Handle_onTextEdited(HandleRef _this, std::function<StringDelegate> handler) {
        QObject::connect(
            (QLineEdit*)_this,
            &QLineEdit::textEdited,
            (QLineEdit*)_this,
            [handler](const QString& str) {
                handler(str.toStdString());
            });
    }

    void Handle_onReturnPressed(HandleRef _this, std::function<VoidDelegate> handler) {
        QObject::connect(
            (QLineEdit*)_this,
            &QLineEdit::returnPressed,
            (QLineEdit*)_this,
            handler);
    }

    void Handle_dispose(HandleRef _this) {
        delete (QLineEdit*)_this;
    }

    HandleRef create() {
        return (HandleRef)new QLineEdit();
    }
}
