#include "../support/NativeImplServer.h"
#include "MenuBar_wrappers.h"
#include "MenuBar.h"

#include "Widget_wrappers.h"
using namespace ::Widget;

#include "Menu_wrappers.h"
using namespace ::Menu;

namespace MenuBar
{
    void Handle__push(HandleRef value) {
        ni_pushPtr(value);
    }

    HandleRef Handle__pop() {
        return (HandleRef)ni_popPtr();
    }

    void Handle_addMenu__wrapper() {
        auto _this = Handle__pop();
        auto menu = Menu::Handle__pop();
        Handle_addMenu(_this, menu);
    }

    void Handle_dispose__wrapper() {
        auto _this = Handle__pop();
        Handle_dispose(_this);
    }

    void create__wrapper() {
        Handle__push(create());
    }

    int __register() {
        auto m = ni_registerModule("MenuBar");
        ni_registerModuleMethod(m, "create", &create__wrapper);
        ni_registerModuleMethod(m, "Handle_addMenu", &Handle_addMenu__wrapper);
        ni_registerModuleMethod(m, "Handle_dispose", &Handle_dispose__wrapper);
        return 0; // = OK
    }
}
