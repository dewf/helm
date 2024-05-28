#include "generated/LineEdit.h"

#include <QObject>
#include <QLineEdit>
#include <utility>

#define THIS ((LineEdit2*)_this)

namespace LineEdit
{
    class LineEdit2 : public QLineEdit {
    private:
        std::function<VoidDelegate> onLostFocusHandler = nullptr;
    public:
        explicit LineEdit2(QWidget *parent = nullptr) : QLineEdit(parent) {}
        void onLostFocus(std::function<VoidDelegate> handler) {
            onLostFocusHandler = std::move(handler);
        }
    protected:
        void focusOutEvent(QFocusEvent *event) override {
            QLineEdit::focusOutEvent(event);
            if (onLostFocusHandler) {
                onLostFocusHandler();
            }
        }
    };

    void Handle_setText(HandleRef _this, std::string str) {
        THIS->setText(str.c_str());
    }

    void Handle_onTextEdited(HandleRef _this, std::function<StringDelegate> handler) {
        QObject::connect(
            THIS,
            &LineEdit2::textEdited,
            THIS,
            [handler](const QString& str) {
                handler(str.toStdString());
            });
    }

    void Handle_onReturnPressed(HandleRef _this, std::function<VoidDelegate> handler) {
        QObject::connect(
            THIS,
            &LineEdit2::returnPressed,
            THIS,
            handler);
    }

    void Handle_onLostFocus(HandleRef _this, std::function<VoidDelegate> handler) {
        THIS->onLostFocus(std::move(handler));
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create() {
        return (HandleRef)new LineEdit2();
    }
}
