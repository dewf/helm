#include "../support/NativeImplServer.h"
#include "Signal_wrappers.h"
#include "Signal.h"

namespace Signal
{
    void VoidDelegate__push(std::function<VoidDelegate> f) {
        size_t uniqueKey = 0;
        if (f) {
            VoidDelegate* ptr_fun = f.target<VoidDelegate>();
            if (ptr_fun != nullptr) {
                uniqueKey = (size_t)ptr_fun;
            }
        }
        auto wrapper = [f]() {
            f();
        };
        pushServerFuncVal(wrapper, uniqueKey);
    }

    std::function<VoidDelegate> VoidDelegate__pop() {
        auto id = ni_popClientFunc();
        auto cf = std::shared_ptr<ClientFuncVal>(new ClientFuncVal(id));
        auto wrapper = [cf]() {
            cf->remoteExec();
        };
        return wrapper;
    }
    void BoolDelegate__push(std::function<BoolDelegate> f) {
        size_t uniqueKey = 0;
        if (f) {
            BoolDelegate* ptr_fun = f.target<BoolDelegate>();
            if (ptr_fun != nullptr) {
                uniqueKey = (size_t)ptr_fun;
            }
        }
        auto wrapper = [f]() {
            auto b = ni_popBool();
            f(b);
        };
        pushServerFuncVal(wrapper, uniqueKey);
    }

    std::function<BoolDelegate> BoolDelegate__pop() {
        auto id = ni_popClientFunc();
        auto cf = std::shared_ptr<ClientFuncVal>(new ClientFuncVal(id));
        auto wrapper = [cf](bool b) {
            ni_pushBool(b);
            cf->remoteExec();
        };
        return wrapper;
    }
    void IntDelegate__push(std::function<IntDelegate> f) {
        size_t uniqueKey = 0;
        if (f) {
            IntDelegate* ptr_fun = f.target<IntDelegate>();
            if (ptr_fun != nullptr) {
                uniqueKey = (size_t)ptr_fun;
            }
        }
        auto wrapper = [f]() {
            auto i = ni_popInt32();
            f(i);
        };
        pushServerFuncVal(wrapper, uniqueKey);
    }

    std::function<IntDelegate> IntDelegate__pop() {
        auto id = ni_popClientFunc();
        auto cf = std::shared_ptr<ClientFuncVal>(new ClientFuncVal(id));
        auto wrapper = [cf](int32_t i) {
            ni_pushInt32(i);
            cf->remoteExec();
        };
        return wrapper;
    }
    void StringDelegate__push(std::function<StringDelegate> f) {
        size_t uniqueKey = 0;
        if (f) {
            StringDelegate* ptr_fun = f.target<StringDelegate>();
            if (ptr_fun != nullptr) {
                uniqueKey = (size_t)ptr_fun;
            }
        }
        auto wrapper = [f]() {
            auto s = popStringInternal();
            f(s);
        };
        pushServerFuncVal(wrapper, uniqueKey);
    }

    std::function<StringDelegate> StringDelegate__pop() {
        auto id = ni_popClientFunc();
        auto cf = std::shared_ptr<ClientFuncVal>(new ClientFuncVal(id));
        auto wrapper = [cf](std::string s) {
            pushStringInternal(s);
            cf->remoteExec();
        };
        return wrapper;
    }

    int __register() {
        auto m = ni_registerModule("Signal");
        return 0; // = OK
    }
}
