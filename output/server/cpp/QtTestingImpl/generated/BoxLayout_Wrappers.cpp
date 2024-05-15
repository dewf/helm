#include "../support/NativeImplServer.h"
#include "BoxLayout_wrappers.h"
#include "BoxLayout.h"

#include "Widget_wrappers.h"
using namespace ::Widget;

#include "Layout_wrappers.h"
using namespace ::Layout;

namespace BoxLayout
{
    void Alignment__push(uint32_t value) {
        ni_pushUInt32(value);
    }

    uint32_t Alignment__pop() {
        return ni_popUInt32();
    }
    void Direction__push(Direction value) {
        ni_pushInt32((int32_t)value);
    }

    Direction Direction__pop() {
        auto tag = ni_popInt32();
        return (Direction)tag;
    }
    void Handle__push(HandleRef value) {
        ni_pushPtr(value);
    }

    HandleRef Handle__pop() {
        return (HandleRef)ni_popPtr();
    }

    void Handle_addSpacing__wrapper() {
        auto _this = Handle__pop();
        auto size = ni_popInt32();
        Handle_addSpacing(_this, size);
    }

    void Handle_addStretch__wrapper() {
        auto _this = Handle__pop();
        auto stretch = ni_popInt32();
        Handle_addStretch(_this, stretch);
    }

    void Handle_addWidget__wrapper() {
        auto _this = Handle__pop();
        auto widget = Widget::Handle__pop();
        Handle_addWidget(_this, widget);
    }

    void Handle_addWidget_overload1__wrapper() {
        auto _this = Handle__pop();
        auto widget = Widget::Handle__pop();
        auto stretch = ni_popInt32();
        Handle_addWidget(_this, widget, stretch);
    }

    void Handle_dispose__wrapper() {
        auto _this = Handle__pop();
        Handle_dispose(_this);
    }

    void create__wrapper() {
        auto dir = Direction__pop();
        Handle__push(create(dir));
    }

    int __register() {
        auto m = ni_registerModule("BoxLayout");
        ni_registerModuleMethod(m, "create", &create__wrapper);
        ni_registerModuleMethod(m, "Handle_addSpacing", &Handle_addSpacing__wrapper);
        ni_registerModuleMethod(m, "Handle_addStretch", &Handle_addStretch__wrapper);
        ni_registerModuleMethod(m, "Handle_addWidget", &Handle_addWidget__wrapper);
        ni_registerModuleMethod(m, "Handle_addWidget_overload1", &Handle_addWidget_overload1__wrapper);
        ni_registerModuleMethod(m, "Handle_dispose", &Handle_dispose__wrapper);
        return 0; // = OK
    }
}
