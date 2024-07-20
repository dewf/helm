#include "../support/NativeImplServer.h"
#include "Font_wrappers.h"
#include "Font.h"

namespace Font
{
    void Weight__push(Weight value) {
        ni_pushInt32((int32_t)value);
    }

    Weight Weight__pop() {
        auto tag = ni_popInt32();
        return (Weight)tag;
    }
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
        void onTodo(const Deferred::Todo* todo) override {
            // kind:
            ni_pushInt32(0);
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
            __ret = new Deferred::Todo();
            break;
        }
        default:
            printf("C++ Deferred__pop() - unknown kind! returning null\n");
        }
        return std::shared_ptr<Deferred::Base>(__ret);
    }

    int __register() {
        auto m = ni_registerModule("Font");
        ni_registerModuleMethod(m, "Handle_dispose", &Handle_dispose__wrapper);
        ni_registerModuleMethod(m, "OwnedHandle_dispose", &OwnedHandle_dispose__wrapper);
        return 0; // = OK
    }
}
