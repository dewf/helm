#include "generated/LineEdit.h"

#include <QObject>
#include <QLineEdit>

#include "util/SignalStuff.h"

#define THIS ((LineEditWithHandler*)_this)

namespace LineEdit
{
    class LineEditWithHandler : public QLineEdit {
        Q_OBJECT
    private:
        std::shared_ptr<SignalHandler> handler;
        uint32_t lastMask = 0;
        std::vector<SignalMapItem<SignalMask>> signalMap = {
            { SignalMask::CursorPositionChanged, SIGNAL(cursorPositionChanged(int, int)), SLOT(onCursorPositionChanged(int,int)) },
            { SignalMask::EditingFinished, SIGNAL(editingFinished()), SLOT(onEditingFinished()) },
            { SignalMask::InputRejected, SIGNAL(inputRejected()), SLOT(onInputRejected()) },
            { SignalMask::ReturnPressed, SIGNAL(returnPressed()), SLOT(onReturnPressed()) },
            { SignalMask::SelectionChanged, SIGNAL(selectionChanged()), SLOT(onSelectionChanged()) },
            { SignalMask::TextChanged, SIGNAL(textChanged(QString)), SLOT(onTextChanged(QString)) },
            { SignalMask::TextEdited, SIGNAL(textEdited(QString)), SLOT(onTextEdited(QString)) }
        };
    public:
        explicit LineEditWithHandler(std::shared_ptr<SignalHandler> handler) : handler(std::move(handler)) {}
        void setSignalMask(uint32_t newMask) {
            if (newMask != lastMask) {
                processChanges(lastMask, newMask, signalMap, this);
                lastMask = newMask;
            }
        }
    public slots:
        void onCursorPositionChanged(int oldPos, int newPos) {
            handler->cursorPositionChanged(oldPos, newPos);
        };
        void onEditingFinished() {
            handler->editingFinished();
        }
        void onInputRejected() {
            handler->inputRejected();
        }
        void onReturnPressed() {
            handler->returnPressed();
        }
        void onSelectionChanged() {
            handler->selectionChanged();
        }
        void onTextChanged(const QString& text) {
            handler->textChanged(text.toStdString());
        }
        void onTextEdited(const QString& text) {
            handler->textEdited(text.toStdString());
        }
    };

    void Handle_setText(HandleRef _this, std::string str) {
        THIS->setText(str.c_str());
    }

    void Handle_setSignalMask(HandleRef _this, uint32_t mask) {
        THIS->setSignalMask(mask);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create(std::shared_ptr<SignalHandler> handler) {
        return (HandleRef) new LineEditWithHandler(std::move(handler));
    }
}

#include "LineEdit.moc"