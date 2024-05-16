#include "../support/NativeImplServer.h"
#include "TabWidget_wrappers.h"
#include "TabWidget.h"

#include "Signal_wrappers.h"
using namespace ::Signal;

#include "Widget_wrappers.h"
using namespace ::Widget;

namespace TabWidget
{
    void Handle__push(HandleRef value) {
        ni_pushPtr(value);
    }

    HandleRef Handle__pop() {
        return (HandleRef)ni_popPtr();
    }

    void Handle_addTab__wrapper() {
        auto _this = Handle__pop();
        auto page = Widget::Handle__pop();
        auto label = popStringInternal();
        Handle_addTab(_this, page, label);
    }

    void Handle_insertTab__wrapper() {
        auto _this = Handle__pop();
        auto index = ni_popInt32();
        auto page = Widget::Handle__pop();
        auto label = popStringInternal();
        Handle_insertTab(_this, index, page, label);
    }

    void Handle_onCurrentChanged__wrapper() {
        auto _this = Handle__pop();
        auto handler = IntDelegate__pop();
        Handle_onCurrentChanged(_this, handler);
    }

    void Handle_dispose__wrapper() {
        auto _this = Handle__pop();
        Handle_dispose(_this);
    }

    void create__wrapper() {
        Handle__push(create());
    }

    int __register() {
        auto m = ni_registerModule("TabWidget");
        ni_registerModuleMethod(m, "create", &create__wrapper);
        ni_registerModuleMethod(m, "Handle_addTab", &Handle_addTab__wrapper);
        ni_registerModuleMethod(m, "Handle_insertTab", &Handle_insertTab__wrapper);
        ni_registerModuleMethod(m, "Handle_onCurrentChanged", &Handle_onCurrentChanged__wrapper);
        ni_registerModuleMethod(m, "Handle_dispose", &Handle_dispose__wrapper);
        return 0; // = OK
    }
}
