#include "../support/NativeImplServer.h"
#include "Application_wrappers.h"
#include "Application.h"

namespace Application
{
    // built-in array type: std::vector<std::string>
    void Handle__push(HandleRef value) {
        ni_pushPtr(value);
    }

    HandleRef Handle__pop() {
        return (HandleRef)ni_popPtr();
    }

    void Handle_exec__wrapper() {
        auto _this = Handle__pop();
        ni_pushInt32(Handle_exec(_this));
    }

    void Handle_dispose__wrapper() {
        auto _this = Handle__pop();
        Handle_dispose(_this);
    }

    void create__wrapper() {
        auto args = popStringArrayInternal();
        Handle__push(create(args));
    }

    int __register() {
        auto m = ni_registerModule("Application");
        ni_registerModuleMethod(m, "create", &create__wrapper);
        ni_registerModuleMethod(m, "Handle_exec", &Handle_exec__wrapper);
        ni_registerModuleMethod(m, "Handle_dispose", &Handle_dispose__wrapper);
        return 0; // = OK
    }
}
