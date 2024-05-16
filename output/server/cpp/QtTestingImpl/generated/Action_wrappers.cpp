#include "../support/NativeImplServer.h"
#include "Action_wrappers.h"
#include "Action.h"

#include "Signal_wrappers.h"
using namespace ::Signal;

namespace Action
{
    void Handle__push(HandleRef value) {
        ni_pushPtr(value);
    }

    HandleRef Handle__pop() {
        return (HandleRef)ni_popPtr();
    }

    void Handle_onTriggered__wrapper() {
        auto _this = Handle__pop();
        auto handler = BoolDelegate__pop();
        Handle_onTriggered(_this, handler);
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
        auto m = ni_registerModule("Action");
        ni_registerModuleMethod(m, "create", &create__wrapper);
        ni_registerModuleMethod(m, "Handle_onTriggered", &Handle_onTriggered__wrapper);
        ni_registerModuleMethod(m, "Handle_dispose", &Handle_dispose__wrapper);
        return 0; // = OK
    }
}
