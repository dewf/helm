#include "../support/NativeImplServer.h"
#include "PushButton_wrappers.h"
#include "PushButton.h"

#include "Widget_wrappers.h"
using namespace ::Widget;

#include "Signal_wrappers.h"
using namespace ::Signal;

namespace PushButton
{
    void Handle__push(HandleRef value) {
        ni_pushPtr(value);
    }

    HandleRef Handle__pop() {
        return (HandleRef)ni_popPtr();
    }

    void Handle_onClicked__wrapper() {
        auto _this = Handle__pop();
        auto handler = VoidDelegate__pop();
        Handle_onClicked(_this, handler);
    }

    void Handle_dispose__wrapper() {
        auto _this = Handle__pop();
        Handle_dispose(_this);
    }

    void create__wrapper() {
        auto label = popStringInternal();
        Handle__push(create(label));
    }

    int __register() {
        auto m = ni_registerModule("PushButton");
        ni_registerModuleMethod(m, "create", &create__wrapper);
        ni_registerModuleMethod(m, "Handle_onClicked", &Handle_onClicked__wrapper);
        ni_registerModuleMethod(m, "Handle_dispose", &Handle_dispose__wrapper);
        return 0; // = OK
    }
}
