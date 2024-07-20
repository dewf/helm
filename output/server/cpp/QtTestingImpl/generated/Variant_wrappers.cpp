#include "../support/NativeImplServer.h"
#include "Variant_wrappers.h"
#include "Variant.h"

#include "Icon_wrappers.h"
using namespace ::Icon;

#include "PaintResources_wrappers.h"
using namespace ::PaintResources;

namespace Variant
{
    void Handle__push(HandleRef value) {
        ni_pushPtr(value);
    }

    HandleRef Handle__pop() {
        return (HandleRef)ni_popPtr();
    }

    void Handle_isValid__wrapper() {
        auto _this = Handle__pop();
        ni_pushBool(Handle_isValid(_this));
    }

    void Handle_toString2__wrapper() {
        auto _this = Handle__pop();
        pushStringInternal(Handle_toString2(_this));
    }

    void Handle_toInt__wrapper() {
        auto _this = Handle__pop();
        ni_pushInt32(Handle_toInt(_this));
    }

    void Handle_dispose__wrapper() {
        auto _this = Handle__pop();
        Handle_dispose(_this);
    }
    void OwnedHandle__push(OwnedHandleRef value) {
        ni_pushPtr(value);
    }

    OwnedHandleRef OwnedHandle__pop() {
        return (OwnedHandleRef)ni_popPtr();
    }

    void OwnedHandle_dispose__wrapper() {
        auto _this = OwnedHandle__pop();
        OwnedHandle_dispose(_this);
    }

    class Deferred_PushVisitor : public Deferred::Visitor {
    private:
        bool isReturn;
    public:
        Deferred_PushVisitor(bool isReturn) : isReturn(isReturn) {}
        void onEmpty(const Deferred::Empty* empty) override {
            // kind:
            ni_pushInt32(0);
        }
        void onFromString(const Deferred::FromString* fromString) override {
            pushStringInternal(fromString->value);
            // kind:
            ni_pushInt32(1);
        }
        void onFromInt(const Deferred::FromInt* fromInt) override {
            ni_pushInt32(fromInt->value);
            // kind:
            ni_pushInt32(2);
        }
        void onFromIcon(const Deferred::FromIcon* fromIcon) override {
            Icon::Deferred__push(fromIcon->value, isReturn);
            // kind:
            ni_pushInt32(3);
        }
        void onFromColor(const Deferred::FromColor* fromColor) override {
            Color::Deferred__push(fromColor->value, isReturn);
            // kind:
            ni_pushInt32(4);
        }
    };

    void Deferred__push(std::shared_ptr<Deferred::Base> value, bool isReturn) {
        Deferred_PushVisitor v(isReturn);
        value->accept((Deferred::Visitor*)&v);
    }

    std::shared_ptr<Deferred::Base> Deferred__pop() {
        Deferred::Base* __ret = nullptr;
        switch (ni_popInt32()) {
        case 0: {
            __ret = new Deferred::Empty();
            break;
        }
        case 1: {
            auto value = popStringInternal();
            __ret = new Deferred::FromString(value);
            break;
        }
        case 2: {
            auto value = ni_popInt32();
            __ret = new Deferred::FromInt(value);
            break;
        }
        case 3: {
            auto value = Icon::Deferred__pop();
            __ret = new Deferred::FromIcon(value);
            break;
        }
        case 4: {
            auto value = Color::Deferred__pop();
            __ret = new Deferred::FromColor(value);
            break;
        }
        default:
            printf("C++ Deferred__pop() - unknown kind! returning null\n");
        }
        return std::shared_ptr<Deferred::Base>(__ret);
    }

    int __register() {
        auto m = ni_registerModule("Variant");
        ni_registerModuleMethod(m, "Handle_isValid", &Handle_isValid__wrapper);
        ni_registerModuleMethod(m, "Handle_toString2", &Handle_toString2__wrapper);
        ni_registerModuleMethod(m, "Handle_toInt", &Handle_toInt__wrapper);
        ni_registerModuleMethod(m, "Handle_dispose", &Handle_dispose__wrapper);
        ni_registerModuleMethod(m, "OwnedHandle_dispose", &OwnedHandle_dispose__wrapper);
        return 0; // = OK
    }
}
