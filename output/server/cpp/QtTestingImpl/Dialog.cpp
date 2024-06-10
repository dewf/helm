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
        uint32_t lastMask = 0;
        std::vector<SignalMapItem<SignalMask>> signalMap = {
                { SignalMask::Accepted, SIGNAL(accepted()), SLOT(onAccepted()) },
                { SignalMask::Finished, SIGNAL(finished(int)), SLOT(onFinished(int)) },
                { SignalMask::Rejected, SIGNAL(rejected()), SLOT(onRejected()) },
        };
    public:
        explicit DialogWithHandler(const std::shared_ptr<SignalHandler> &handler) : handler(handler) {}
        void setSignalMask(uint32_t newMask) {
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

    void Handle_setSignalMask(HandleRef _this, uint32_t mask) {
        THIS->setSignalMask(mask);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create(std::shared_ptr<SignalHandler> handler) {
        return (HandleRef) new DialogWithHandler(handler);
    }
}

#include "Dialog.moc"
