#include "generated/ComboBox.h"

#include <QObject>
#include <QComboBox>
#include <QStringList>

#define THIS ((QComboBox*)_this)

namespace ComboBox
{
    void Handle_clear(HandleRef _this) {
        THIS->clear();
    }

    void Handle_setItems(HandleRef _this, std::vector<std::string> items) {
        QStringList items2;
        for (auto & str : items) {
            items2.push_back(str.c_str());
        }
//        std::transform(items.begin(), items.end(), std::back_inserter(items2), std::mem_fn(&std::string::c_str));
        THIS->addItems(items2);
    }

    void Handle_setCurrentIndex(HandleRef _this, int32_t index) {
        THIS->setCurrentIndex(index);
    }

    void Handle_onCurrentIndexChanged(HandleRef _this, std::function<IntDelegate> handler) {
        QObject::connect(
            THIS,
            &QComboBox::currentIndexChanged,
            THIS,
            handler);
    }

    void Handle_onCurrentTextChanged(HandleRef _this, std::function<StringDelegate> handler) {
        QObject::connect(
            THIS,
            &QComboBox::currentTextChanged,
            THIS,
            [handler](const QString& text) {
                handler(text.toStdString());
            });
    }

    void Handle_dispose(HandleRef _this) {
        delete (QComboBox*)_this;
    }

    HandleRef create() {
        return (HandleRef)new QComboBox();
    }
}
