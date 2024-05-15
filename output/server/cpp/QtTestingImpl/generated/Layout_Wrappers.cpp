#include "../support/NativeImplServer.h"
#include "Layout_wrappers.h"
#include "Layout.h"

namespace Layout
{
    void Handle__push(HandleRef value) {
        ni_pushPtr(value);
    }

    HandleRef Handle__pop() {
        return (HandleRef)ni_popPtr();
    }

    void Handle_dispose__wrapper() {
        auto _this = Handle__pop();
        Handle_dispose(_this);
    }

    int __register() {
        auto m = ni_registerModule("Layout");
        ni_registerModuleMethod(m, "Handle_dispose", &Handle_dispose__wrapper);
        return 0; // = OK
    }
}
