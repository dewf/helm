#include "../support/NativeImplServer.h"
#include "ComboBox_wrappers.h"
#include "ComboBox.h"

#include "Widget_wrappers.h"
using namespace ::Widget;

namespace ComboBox
{
    void Handle__push(HandleRef value) {
        ni_pushPtr(value);
    }

    HandleRef Handle__pop() {
        return (HandleRef)ni_popPtr();
    }

    void Handle_todo__wrapper() {
        auto _this = Handle__pop();
        Handle_todo(_this);
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
        ni_registerModuleMethod(m, "Handle_todo", &Handle_todo__wrapper);
        ni_registerModuleMethod(m, "Handle_dispose", &Handle_dispose__wrapper);
        return 0; // = OK
    }
}
