#include "../support/NativeImplServer.h"
#include "QObject_wrappers.h"
#include "QObject.h"

#include "Signal_wrappers.h"
using namespace ::Signal;

namespace QObject
{
    void Handle__push(HandleRef value) {
        ni_pushPtr(value);
    }

    HandleRef Handle__pop() {
        return (HandleRef)ni_popPtr();
    }

    void Handle_onDestroyed__wrapper() {
        auto _this = Handle__pop();
        auto handler = VoidDelegate__pop();
        Handle_onDestroyed(_this, handler);
    }

    void Handle_dispose__wrapper() {
        auto _this = Handle__pop();
        Handle_dispose(_this);
    }

    int __register() {
        auto m = ni_registerModule("QObject");
        ni_registerModuleMethod(m, "Handle_onDestroyed", &Handle_onDestroyed__wrapper);
        ni_registerModuleMethod(m, "Handle_dispose", &Handle_dispose__wrapper);
        return 0; // = OK
    }
}
