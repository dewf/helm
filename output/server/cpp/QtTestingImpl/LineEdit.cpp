#include "generated/LineEdit.h"

#include <QObject>
#include <QLineEdit>

#define THIS ((QLineEdit*)_this)

namespace LineEdit
{
    void Handle_setText(HandleRef _this, std::string str) {
        THIS->setText(str.c_str());
    }

    void Handle_onTextEdited(HandleRef _this, std::function<StringDelegate> handler) {
        QObject::connect(
            THIS,
            &QLineEdit::textEdited,
            THIS,
            [handler](const QString& str) {
                handler(str.toStdString());
            });
    }

    void Handle_onReturnPressed(HandleRef _this, std::function<VoidDelegate> handler) {
        QObject::connect(
            THIS,
            &QLineEdit::returnPressed,
            THIS,
            handler);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create() {
        return (HandleRef)new QLineEdit();
    }
}
