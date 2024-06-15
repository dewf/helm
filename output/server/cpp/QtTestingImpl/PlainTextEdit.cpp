#include "generated/PlainTextEdit.h"

#include <QObject>
#include <QPlainTextEdit>

#include "util/SignalStuff.h"
#include "util/convert.h"

#define THIS ((PlainTextEditWithHandler*)_this)

namespace PlainTextEdit
{
    class PlainTextEditWithHandler : public QPlainTextEdit {
        Q_OBJECT
    private:
        std::shared_ptr<SignalHandler> handler;
        uint32_t lastMask = 0;
        std::vector<SignalMapItem<SignalMask>> signalMap = {
            { SignalMask::BlockCountChanged, SIGNAL(blockCountChanged(int)), SLOT(onBlockCountChanged(int)) },
            { SignalMask::CopyAvailable, SIGNAL(copyAvailable(bool)), SLOT(onCopyAvailable(bool)) },
            { SignalMask::CursorPositionChanged, SIGNAL(cursorPositionChanged()), SLOT(onCursorPositionChanged()) },
            { SignalMask::ModificationChanged, SIGNAL(modificationChanged(bool)), SLOT(onModificationChanged(bool)) },
            { SignalMask::RedoAvailable, SIGNAL(redoAvailable(bool)), SLOT(onRedoAvailable(bool)) },
            { SignalMask::SelectionChanged, SIGNAL(selectionChanged()), SLOT(onSelectionChanged()) },
            { SignalMask::TextChanged, SIGNAL(textChanged()), SLOT(onTextChanged()) },
            { SignalMask::UndoAvailable, SIGNAL(undoAvailable(bool)), SLOT(onUndoAvailable(bool)) },
            { SignalMask::UpdateRequest, SIGNAL(updateRequest(QRect,int)), SLOT(onUpdateRequest(QRect,int)) },
        };
    public:
        explicit PlainTextEditWithHandler(std::shared_ptr<SignalHandler> handler) : handler(std::move(handler)) {}
        void setSignalMask(uint32_t newMask) {
            if (newMask != lastMask) {
                processChanges(lastMask, newMask, signalMap, this);
                lastMask = newMask;
            }
        }
    public slots:
        void onBlockCountChanged(int newBlockCount) {
            handler->blockCountChanged(newBlockCount);
        }
        void onCopyAvailable(bool yes) {
            handler->copyAvailable(yes);
        }
        void onCursorPositionChanged() {
            handler->cursorPositionChanged();
        }
        void onModificationChanged(bool changed) {
            handler->modificationChanged(changed);
        }
        void onRedoAvailable(bool available) {
            handler->redoAvailable(available);
        }
        void onSelectionChanged() {
            handler->selectionChanged();
        }
        void onTextChanged() {
            handler->textChanged();
        }
        void onUndoAvailable(bool available) {
            handler->undoAvailable(available);
        }
        void onUpdateRequest(const QRect &rect, int dy) {
            handler->updateRequest(toRect(rect), dy);
        }
    };

    std::string Handle_toPlainText(HandleRef _this) {
        return THIS->toPlainText().toStdString();
    }

    void Handle_setPlainText(HandleRef _this, std::string text) {
        THIS->setPlainText(text.c_str());
    }

    void Handle_setSignalMask(HandleRef _this, uint32_t mask) {
        THIS->setSignalMask(mask);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create(std::shared_ptr<SignalHandler> handler) {
        return (HandleRef) new PlainTextEditWithHandler(std::move(handler));
    }
}

#include "PlainTextEdit.moc"
