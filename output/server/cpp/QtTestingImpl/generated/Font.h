#pragma once

#include "../support/NativeImplServer.h"
#include <functional>
#include <memory>
#include <string>
#include <vector>
#include <map>
#include <tuple>
#include <set>
#include <optional>
#include "../support/result.h"

namespace Font
{

    struct __Handle; typedef struct __Handle* HandleRef;
    struct __OwnedHandle; typedef struct __OwnedHandle* OwnedHandleRef; // extends HandleRef

    namespace Deferred {
        class Base;
    }

    enum class Weight {
        Thin = 100,
        ExtraLight = 200,
        Light = 300,
        Normal = 400,
        Medium = 500,
        DemiBold = 600,
        Bold = 700,
        ExtraBold = 800,
        Black = 900
    };

    void Handle_dispose(HandleRef _this);

    void OwnedHandle_dispose(OwnedHandleRef _this);

    namespace Deferred {
        class Todo;

        class Visitor {
        public:
            virtual void onTodo(const Todo* todo) = 0;
        };

        class Base {
        public:
            virtual void accept(Visitor* visitor) = 0;
        };

        class Todo : public Base {
        public:
            Todo() {}
            void accept(Visitor* visitor) override {
                visitor->onTodo(this);
            }
        };
    }
}
