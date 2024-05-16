#include "../support/NativeImplServer.h"
#include "MainWindow_wrappers.h"
#include "MainWindow.h"

#include "Widget_wrappers.h"
using namespace ::Widget;

#include "Layout_wrappers.h"
using namespace ::Layout;

#include "MenuBar_wrappers.h"
using namespace ::MenuBar;

namespace MainWindow
{
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

    void Handle_setMenuBar__wrapper() {
        auto _this = Handle__pop();
        auto menubar = MenuBar::Handle__pop();
        Handle_setMenuBar(_this, menubar);
    }

    void Handle_dispose__wrapper() {
        auto _this = Handle__pop();
        Handle_dispose(_this);
    }

    void create__wrapper() {
        Handle__push(create());
    }

    int __register() {
        auto m = ni_registerModule("MainWindow");
        ni_registerModuleMethod(m, "create", &create__wrapper);
        ni_registerModuleMethod(m, "Handle_setCentralWidget", &Handle_setCentralWidget__wrapper);
        ni_registerModuleMethod(m, "Handle_setMenuBar", &Handle_setMenuBar__wrapper);
        ni_registerModuleMethod(m, "Handle_dispose", &Handle_dispose__wrapper);
        return 0; // = OK
    }
}
