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

    bool Handle_hasAcceptableInput(HandleRef _this) {
        return THIS->hasAcceptableInput();
    }

    void Handle_setAlignment(HandleRef _this, uint32_t align) {
        auto intValue = (int)align;
        THIS->setAlignment((Qt::Alignment)intValue);
    }

    void Handle_setClearButtonEnabled(HandleRef _this, bool enabled) {
        THIS->setClearButtonEnabled(enabled);
    }

    void Handle_setCursorMoveStyle(HandleRef _this, CursorMoveStyle style) {
        THIS->setCursorMoveStyle((Qt::CursorMoveStyle)style);
    }

    void Handle_setCursorPosition(HandleRef _this, int32_t pos) {
        THIS->setCursorPosition(pos);
    }

    std::string Handle_displayText(HandleRef _this) {
        return THIS->displayText().toStdString();
    }

    void Handle_setDragEnabled(HandleRef _this, bool enabled) {
        THIS->setDragEnabled(enabled);
    }

    void Handle_setEchoMode(HandleRef _this, EchoMode mode) {
        THIS->setEchoMode((QLineEdit::EchoMode)mode);
    }

    void Handle_setFrame(HandleRef _this, bool enabled) {
        THIS->setFrame(enabled);
    }

    bool Handle_hasSelectedText(HandleRef _this) {
        return THIS->hasSelectedText();
    }

    void Handle_setInputMask(HandleRef _this, std::string mask) {
        return THIS->setInputMask(QString::fromStdString(mask));
    }

    void Handle_setMaxLength(HandleRef _this, int32_t length) {
        THIS->setMaxLength(length);
    }

    bool Handle_isModified(HandleRef _this) {
        return THIS->isModified();
    }

    void Handle_setModified(HandleRef _this, bool modified) {
        THIS->setModified(modified);
    }

    void Handle_setPlaceholderText(HandleRef _this, std::string text) {
        THIS->setPlaceholderText(QString::fromStdString(text));
    }

    void Handle_setReadOnly(HandleRef _this, bool value) {
        THIS->setReadOnly(value);
    }

    bool Handle_isRedoAvailable(HandleRef _this) {
        return THIS->isRedoAvailable();
    }

    std::string Handle_selectedText(HandleRef _this) {
        return THIS->selectedText().toStdString();
    }

    void Handle_setText(HandleRef _this, std::string text) {
        THIS->setText(QString::fromStdString(text));
    }

    bool Handle_isUndoAvailable(HandleRef _this) {
        return THIS->isUndoAvailable();
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