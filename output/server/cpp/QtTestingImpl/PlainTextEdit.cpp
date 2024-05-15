#include "generated/PlainTextEdit.h"

#include <QObject>
#include <QPlainTextEdit>

namespace PlainTextEdit
{
    void Handle_setText(HandleRef _this, std::string text) {
        ((QPlainTextEdit*)_this)->setPlainText(text.c_str());
    }

    void Handle_onTextChanged(HandleRef _this, std::function<StringDelegate> handler) {
        auto widget = (QPlainTextEdit*)_this;
        QObject::connect(
            widget,
            &QPlainTextEdit::textChanged,
            widget,
            [=]() {
                auto str = widget->toPlainText();
                handler(str.toStdString());
            });
    }

    void Handle_dispose(HandleRef _this) {
        delete (QPlainTextEdit*)_this;
    }

    HandleRef create() {
        return (HandleRef)new QPlainTextEdit();
    }
}
