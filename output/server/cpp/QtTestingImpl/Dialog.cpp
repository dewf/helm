#include "generated/Dialog.h"

#include <QObject>
#include <QDialog>

#include "util/SignalStuff.h"

#define THIS ((DialogWithHandler*)_this)

namespace Dialog
{
    class DialogWithHandler : public QDialog {
        Q_OBJECT
    private:
        std::shared_ptr<SignalHandler> handler;
        SignalMask lastMask = 0;
        std::vector<SignalMapItem<SignalMaskFlags>> signalMap = {
                { SignalMaskFlags::Accepted, SIGNAL(accepted()), SLOT(onAccepted()) },
                { SignalMaskFlags::Finished, SIGNAL(finished(int)), SLOT(onFinished(int)) },
                { SignalMaskFlags::Rejected, SIGNAL(rejected()), SLOT(onRejected()) },
        };
    public:
        DialogWithHandler(QWidget *parent, const std::shared_ptr<SignalHandler> &handler)
            : handler(handler), QDialog(parent)
        {
            // nothing yet
        }

        void setSignalMask(SignalMask newMask) {
            if (newMask != lastMask) {
                processChanges(lastMask, newMask, signalMap, this);
                lastMask = newMask;
            }
        }
    public slots:
        void onAccepted() {
            handler->accepted();
        }
        void onFinished(int result) {
            handler->finished(result);
        }
        void onRejected() {
            handler->rejected();
        }
    };

    void Handle_setParentDialogFlags(HandleRef _this, Widget::HandleRef parent) {
        THIS->setParent((QWidget*)parent, Qt::Dialog);
    }

    void Handle_accept(HandleRef _this) {
        THIS->accept();
    }

    void Handle_reject(HandleRef _this) {
        THIS->reject();
    }

    int Handle_exec(HandleRef _this) {
        return THIS->exec();
    }

    void Handle_setSignalMask(HandleRef _this, SignalMask mask) {
        THIS->setSignalMask(mask);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create(Widget::HandleRef parent, std::shared_ptr<SignalHandler> handler) {
        return (HandleRef) new DialogWithHandler((QWidget*)parent, handler);
    }
}

#include "Dialog.moc"
