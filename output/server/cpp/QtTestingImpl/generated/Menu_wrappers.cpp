#include "../support/NativeImplServer.h"
#include "Menu_wrappers.h"
#include "Menu.h"

#include "Widget_wrappers.h"
using namespace ::Widget;

#include "Signal_wrappers.h"
using namespace ::Signal;

#include "Action_wrappers.h"
using namespace ::Action;

namespace Menu
{
    void Handle__push(HandleRef value) {
        ni_pushPtr(value);
    }

    HandleRef Handle__pop() {
        return (HandleRef)ni_popPtr();
    }

    void Handle_addAction__wrapper() {
        auto _this = Handle__pop();
        auto action = Action::Handle__pop();
        Handle_addAction(_this, action);
    }

    void Handle_dispose__wrapper() {
        auto _this = Handle__pop();
        Handle_dispose(_this);
    }

    void create__wrapper() {
        auto title = popStringInternal();
        Handle__push(create(title));
    }

    int __register() {
        auto m = ni_registerModule("Menu");
        ni_registerModuleMethod(m, "create", &create__wrapper);
        ni_registerModuleMethod(m, "Handle_addAction", &Handle_addAction__wrapper);
        ni_registerModuleMethod(m, "Handle_dispose", &Handle_dispose__wrapper);
        return 0; // = OK
    }
}
