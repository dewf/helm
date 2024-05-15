#include "../support/NativeImplServer.h"
#include "Widget_wrappers.h"
#include "Widget.h"

#include "Common_wrappers.h"
using namespace ::Common;

#include "Signal_wrappers.h"
using namespace ::Signal;

#include "Layout_wrappers.h"
using namespace ::Layout;

namespace Widget
{
    void Handle__push(HandleRef value) {
        ni_pushPtr(value);
    }

    HandleRef Handle__pop() {
        return (HandleRef)ni_popPtr();
    }

    void Handle_setMaximumWidth__wrapper() {
        auto _this = Handle__pop();
        auto maxWidth = ni_popInt32();
        Handle_setMaximumWidth(_this, maxWidth);
    }

    void Handle_setMaximumHeight__wrapper() {
        auto _this = Handle__pop();
        auto maxHeight = ni_popInt32();
        Handle_setMaximumHeight(_this, maxHeight);
    }

    void Handle_getRect__wrapper() {
        auto _this = Handle__pop();
        Rect__push(Handle_getRect(_this), true);
    }

    void Handle_resize__wrapper() {
        auto _this = Handle__pop();
        auto width = ni_popInt32();
        auto height = ni_popInt32();
        Handle_resize(_this, width, height);
    }

    void Handle_show__wrapper() {
        auto _this = Handle__pop();
        Handle_show(_this);
    }

    void Handle_setWindowTitle__wrapper() {
        auto _this = Handle__pop();
        auto title = popStringInternal();
        Handle_setWindowTitle(_this, title);
    }

    void Handle_setLayout__wrapper() {
        auto _this = Handle__pop();
        auto layout = Layout::Handle__pop();
        Handle_setLayout(_this, layout);
    }

    void Handle_onWindowTitleChanged__wrapper() {
        auto _this = Handle__pop();
        auto func = StringDelegate__pop();
        Handle_onWindowTitleChanged(_this, func);
    }

    void Handle_dispose__wrapper() {
        auto _this = Handle__pop();
        Handle_dispose(_this);
    }

    void create__wrapper() {
        Handle__push(create());
    }

    void __constantsFunc() {
        ni_pushInt32(WIDGET_SIZE_MAX);
    }

    int __register() {
        auto m = ni_registerModule("Widget");
        ni_registerModuleConstants(m, &__constantsFunc);
        ni_registerModuleMethod(m, "create", &create__wrapper);
        ni_registerModuleMethod(m, "Handle_setMaximumWidth", &Handle_setMaximumWidth__wrapper);
        ni_registerModuleMethod(m, "Handle_setMaximumHeight", &Handle_setMaximumHeight__wrapper);
        ni_registerModuleMethod(m, "Handle_getRect", &Handle_getRect__wrapper);
        ni_registerModuleMethod(m, "Handle_resize", &Handle_resize__wrapper);
        ni_registerModuleMethod(m, "Handle_show", &Handle_show__wrapper);
        ni_registerModuleMethod(m, "Handle_setWindowTitle", &Handle_setWindowTitle__wrapper);
        ni_registerModuleMethod(m, "Handle_setLayout", &Handle_setLayout__wrapper);
        ni_registerModuleMethod(m, "Handle_onWindowTitleChanged", &Handle_onWindowTitleChanged__wrapper);
        ni_registerModuleMethod(m, "Handle_dispose", &Handle_dispose__wrapper);
        return 0; // = OK
    }
}
