#include "generated/Dialog.h"

#include <QObject>
#include <QDialog>

#define THIS ((QDialog*)_this)

namespace Dialog
{
    void Handle_accept(HandleRef _this) {
        THIS->accept();
    }

    void Handle_reject(HandleRef _this) {
        THIS->reject();
    }

    void Handle_exec(HandleRef _this) {
        THIS->exec();
    }

    void Handle_onAccepted(HandleRef _this, std::function<VoidDelegate> handler) {
        QObject::connect(
                THIS,
                &QDialog::accepted,
                THIS,
                handler);
    }

    void Handle_onRejected(HandleRef _this, std::function<VoidDelegate> handler) {
        QObject::connect(
                THIS,
                &QDialog::rejected,
                THIS,
                handler);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create() {
        return (HandleRef)new QDialog();
    }

    HandleRef create(Widget::HandleRef parent) {
        return (HandleRef)new QDialog((QWidget*)parent);
    }
}
