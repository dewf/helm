#include "../support/NativeImplServer.h"
#include "MainWindow_wrappers.h"
#include "MainWindow.h"

#include "Widget_wrappers.h"
using namespace ::Widget;

#include "Layout_wrappers.h"
using namespace ::Layout;

namespace MainWindow
{
    void Kind__push(Kind value) {
        ni_pushInt32((int32_t)value);
    }

    Kind Kind__pop() {
        auto tag = ni_popInt32();
        return (Kind)tag;
    }
    void Handle__push(HandleRef value) {
        ni_pushPtr(value);
    }

    HandleRef Handle__pop() {
        return (HandleRef)ni_popPtr();
    }

    void Handle_setCentralWidget__wrapper() {
        auto _this = Handle__pop();
        auto widget = Widget::Handle__pop();
        Handle_setCentralWidget(_this, widget);
    }

    void Handle_dispose__wrapper() {
        auto _this = Handle__pop();
        Handle_dispose(_this);
    }

    void create__wrapper() {
        auto parent = Widget::Handle__pop();
        auto kind = Kind__pop();
        Handle__push(create(parent, kind));
    }

    int __register() {
        auto m = ni_registerModule("MainWindow");
        ni_registerModuleMethod(m, "create", &create__wrapper);
        ni_registerModuleMethod(m, "Handle_setCentralWidget", &Handle_setCentralWidget__wrapper);
        ni_registerModuleMethod(m, "Handle_dispose", &Handle_dispose__wrapper);
        return 0; // = OK
    }
}
