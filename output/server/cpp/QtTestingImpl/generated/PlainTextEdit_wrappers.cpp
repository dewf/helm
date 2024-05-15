#include "../support/NativeImplServer.h"
#include "PlainTextEdit_wrappers.h"
#include "PlainTextEdit.h"

#include "Widget_wrappers.h"
using namespace ::Widget;

#include "Signal_wrappers.h"
using namespace ::Signal;

namespace PlainTextEdit
{
    void Handle__push(HandleRef value) {
        ni_pushPtr(value);
    }

    HandleRef Handle__pop() {
        return (HandleRef)ni_popPtr();
    }

    void Handle_setText__wrapper() {
        auto _this = Handle__pop();
        auto text = popStringInternal();
        Handle_setText(_this, text);
    }

    void Handle_onTextChanged__wrapper() {
        auto _this = Handle__pop();
        auto handler = StringDelegate__pop();
        Handle_onTextChanged(_this, handler);
    }

    void Handle_dispose__wrapper() {
        auto _this = Handle__pop();
        Handle_dispose(_this);
    }

    void create__wrapper() {
        Handle__push(create());
    }

    int __register() {
        auto m = ni_registerModule("PlainTextEdit");
        ni_registerModuleMethod(m, "create", &create__wrapper);
        ni_registerModuleMethod(m, "Handle_setText", &Handle_setText__wrapper);
        ni_registerModuleMethod(m, "Handle_onTextChanged", &Handle_onTextChanged__wrapper);
        ni_registerModuleMethod(m, "Handle_dispose", &Handle_dispose__wrapper);
        return 0; // = OK
    }
}
