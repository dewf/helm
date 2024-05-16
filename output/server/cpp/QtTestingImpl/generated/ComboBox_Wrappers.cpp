#include "../support/NativeImplServer.h"
#include "ComboBox_wrappers.h"
#include "ComboBox.h"

#include "Widget_wrappers.h"
using namespace ::Widget;

#include "Signal_wrappers.h"
using namespace ::Signal;

namespace ComboBox
{
    // built-in array type: std::vector<std::string>
    void Handle__push(HandleRef value) {
        ni_pushPtr(value);
    }

    HandleRef Handle__pop() {
        return (HandleRef)ni_popPtr();
    }

    void Handle_clear__wrapper() {
        auto _this = Handle__pop();
        Handle_clear(_this);
    }

    void Handle_setItems__wrapper() {
        auto _this = Handle__pop();
        auto items = popStringArrayInternal();
        Handle_setItems(_this, items);
    }

    void Handle_setCurrentIndex__wrapper() {
        auto _this = Handle__pop();
        auto index = ni_popInt32();
        Handle_setCurrentIndex(_this, index);
    }

    void Handle_onCurrentIndexChanged__wrapper() {
        auto _this = Handle__pop();
        auto handler = IntDelegate__pop();
        Handle_onCurrentIndexChanged(_this, handler);
    }

    void Handle_onCurrentTextChanged__wrapper() {
        auto _this = Handle__pop();
        auto handler = StringDelegate__pop();
        Handle_onCurrentTextChanged(_this, handler);
    }

    void Handle_dispose__wrapper() {
        auto _this = Handle__pop();
        Handle_dispose(_this);
    }

    void create__wrapper() {
        Handle__push(create());
    }

    int __register() {
        auto m = ni_registerModule("ComboBox");
        ni_registerModuleMethod(m, "create", &create__wrapper);
        ni_registerModuleMethod(m, "Handle_clear", &Handle_clear__wrapper);
        ni_registerModuleMethod(m, "Handle_setItems", &Handle_setItems__wrapper);
        ni_registerModuleMethod(m, "Handle_setCurrentIndex", &Handle_setCurrentIndex__wrapper);
        ni_registerModuleMethod(m, "Handle_onCurrentIndexChanged", &Handle_onCurrentIndexChanged__wrapper);
        ni_registerModuleMethod(m, "Handle_onCurrentTextChanged", &Handle_onCurrentTextChanged__wrapper);
        ni_registerModuleMethod(m, "Handle_dispose", &Handle_dispose__wrapper);
        return 0; // = OK
    }
}
