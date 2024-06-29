#include "generated/ComboBox.h"

#include <QObject>
#include <QComboBox>
#include <QStringList>
#include <utility>

#include "util/SignalStuff.h"

#define THIS ((ComboBoxWithHandler*)_this)

namespace ComboBox
{
    class ComboBoxWithHandler : public QComboBox {
        Q_OBJECT
    private:
        std::shared_ptr<SignalHandler> handler;
        SignalMask lastMask = 0;
        std::vector<SignalMapItem<SignalMaskFlags>> signalMap = {
            { SignalMaskFlags::Activated, SIGNAL(activated(int)), SLOT(onActivated(int)) },
            { SignalMaskFlags::CurrentIndexChanged, SIGNAL(currentIndexChanged(int)), SLOT(onCurrentIndexChanged(int)) },
            { SignalMaskFlags::CurrentTextChanged, SIGNAL(currentTextChanged(QString)), SLOT(onCurrentTextChanged(QString)) },
            { SignalMaskFlags::EditTextChanged, SIGNAL(editTextChanged(QString)), SLOT(onEditTextChanged(QString)) },
            { SignalMaskFlags::Highlighted, SIGNAL(highlighted(int)), SLOT(onHighlighted(int)) },
            { SignalMaskFlags::TextActivated, SIGNAL(textActivated(QString)), SLOT(onTextActivated(QString)) },
            { SignalMaskFlags::TextHighlighted, SIGNAL(textHighlighted(QString)), SLOT(onTextHighlighted(QString)) },
        };
    public:
        explicit ComboBoxWithHandler(std::shared_ptr<SignalHandler> handler) : handler(std::move(handler)) {}
        void setSignalMask(SignalMask newMask) {
            if (newMask != lastMask) {
                processChanges(lastMask, newMask, signalMap, this);
                lastMask = newMask;
            }
        }
    public slots:
        void onActivated(int index) {
            handler->activated(index);
        }
        void onCurrentIndexChanged(int index) {
            handler->currentIndexChanged(index);
        }
        void onCurrentTextChanged(const QString& text) {
            handler->currentTextChanged(text.toStdString());
        }
        void onEditTextChanged(const QString& text) {
            handler->editTextChanged(text.toStdString());
        }
        void onHighlighted(int index) {
            handler->highlighted(index);
        }
        void onTextActivated(const QString& text) {
            handler->textActivated(text.toStdString());
        }
        void onTextHighlighted(const QString& text) {
            handler->textHighlighted(text.toStdString());
        }
    };

    void Handle_clear(HandleRef _this) {
        THIS->clear();
    }

    void Handle_setItems(HandleRef _this, std::vector<std::string> items) {
        QStringList items2;
        for (auto & str : items) {
            items2.push_back(QString::fromStdString(str));
        }
//        std::transform(items.begin(), items.end(), std::back_inserter(items2), std::mem_fn(&std::string::c_str));
        THIS->addItems(items2);
    }

    void Handle_setCurrentIndex(HandleRef _this, int32_t index) {
        THIS->setCurrentIndex(index);
    }

    void Handle_setSignalMask(HandleRef _this, SignalMask mask) {
        THIS->setSignalMask(mask);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create(std::shared_ptr<SignalHandler> handler) {
        return (HandleRef) new ComboBoxWithHandler(std::move(handler));
    }
}

#include "ComboBox.moc"
