#include "../support/NativeImplServer.h"
#include "LineEdit_wrappers.h"
#include "LineEdit.h"

#include "Signal_wrappers.h"
using namespace ::Signal;

#include "Widget_wrappers.h"
using namespace ::Widget;

namespace LineEdit
{
    void Handle__push(HandleRef value) {
        ni_pushPtr(value);
    }

    HandleRef Handle__pop() {
        return (HandleRef)ni_popPtr();
    }

    void Handle_setText__wrapper() {
        auto _this = Handle__pop();
        auto str = popStringInternal();
        Handle_setText(_this, str);
    }

    void Handle_onTextEdited__wrapper() {
        auto _this = Handle__pop();
        auto handler = StringDelegate__pop();
        Handle_onTextEdited(_this, handler);
    }

    void Handle_onReturnPressed__wrapper() {
        auto _this = Handle__pop();
        auto handler = VoidDelegate__pop();
        Handle_onReturnPressed(_this, handler);
    }

    void Handle_dispose__wrapper() {
        auto _this = Handle__pop();
        Handle_dispose(_this);
    }

    void create__wrapper() {
        Handle__push(create());
    }

    int __register() {
        auto m = ni_registerModule("LineEdit");
        ni_registerModuleMethod(m, "create", &create__wrapper);
        ni_registerModuleMethod(m, "Handle_setText", &Handle_setText__wrapper);
        ni_registerModuleMethod(m, "Handle_onTextEdited", &Handle_onTextEdited__wrapper);
        ni_registerModuleMethod(m, "Handle_onReturnPressed", &Handle_onReturnPressed__wrapper);
        ni_registerModuleMethod(m, "Handle_dispose", &Handle_dispose__wrapper);
        return 0; // = OK
    }
}
