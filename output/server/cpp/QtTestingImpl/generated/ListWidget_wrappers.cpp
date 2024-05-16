#include "../support/NativeImplServer.h"
#include "ListWidget_wrappers.h"
#include "ListWidget.h"

#include "Widget_wrappers.h"
using namespace ::Widget;

#include "Signal_wrappers.h"
using namespace ::Signal;

namespace ListWidget
{
    // built-in array type: std::vector<std::string>
    void SelectionMode__push(SelectionMode value) {
        ni_pushInt32((int32_t)value);
    }

    SelectionMode SelectionMode__pop() {
        auto tag = ni_popInt32();
        return (SelectionMode)tag;
    }
    void Handle__push(HandleRef value) {
        ni_pushPtr(value);
    }

    HandleRef Handle__pop() {
        return (HandleRef)ni_popPtr();
    }

    void Handle_setItems__wrapper() {
        auto _this = Handle__pop();
        auto items = popStringArrayInternal();
        Handle_setItems(_this, items);
    }

    void Handle_setSelectionMode__wrapper() {
        auto _this = Handle__pop();
        auto mode = SelectionMode__pop();
        Handle_setSelectionMode(_this, mode);
    }

    void Handle_onCurrentRowChanged__wrapper() {
        auto _this = Handle__pop();
        auto handler = IntDelegate__pop();
        Handle_onCurrentRowChanged(_this, handler);
    }

    void Handle_dispose__wrapper() {
        auto _this = Handle__pop();
        Handle_dispose(_this);
    }

    void create__wrapper() {
        Handle__push(create());
    }

    int __register() {
        auto m = ni_registerModule("ListWidget");
        ni_registerModuleMethod(m, "create", &create__wrapper);
        ni_registerModuleMethod(m, "Handle_setItems", &Handle_setItems__wrapper);
        ni_registerModuleMethod(m, "Handle_setSelectionMode", &Handle_setSelectionMode__wrapper);
        ni_registerModuleMethod(m, "Handle_onCurrentRowChanged", &Handle_onCurrentRowChanged__wrapper);
        ni_registerModuleMethod(m, "Handle_dispose", &Handle_dispose__wrapper);
        return 0; // = OK
    }
}
