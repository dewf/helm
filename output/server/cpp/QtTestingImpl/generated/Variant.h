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

#include "Icon.h"
using namespace ::Icon;
#include "PaintResources.h"
using namespace ::PaintResources;

namespace Variant
{

    struct __Handle; typedef struct __Handle* HandleRef;
    struct __OwnedHandle; typedef struct __OwnedHandle* OwnedHandleRef; // extends HandleRef

    namespace Deferred {
        class Base;
    }

    bool Handle_isValid(HandleRef _this);
    std::string Handle_toString2(HandleRef _this);
    int32_t Handle_toInt(HandleRef _this);
    void Handle_dispose(HandleRef _this);

    void OwnedHandle_dispose(OwnedHandleRef _this);

    namespace Deferred {
        class Empty;
        class FromString;
        class FromInt;
        class FromIcon;
        class FromColor;

        class Visitor {
        public:
            virtual void onEmpty(const Empty* empty) = 0;
            virtual void onFromString(const FromString* fromString) = 0;
            virtual void onFromInt(const FromInt* fromInt) = 0;
            virtual void onFromIcon(const FromIcon* fromIcon) = 0;
            virtual void onFromColor(const FromColor* fromColor) = 0;
        };

        class Base {
        public:
            virtual void accept(Visitor* visitor) = 0;
        };

        class Empty : public Base {
        public:
            Empty() {}
            void accept(Visitor* visitor) override {
                visitor->onEmpty(this);
            }
        };

        class FromString : public Base {
        public:
            const std::string value;
            FromString(std::string value) : value(value) {}
            void accept(Visitor* visitor) override {
                visitor->onFromString(this);
            }
        };

        class FromInt : public Base {
        public:
            const int32_t value;
            FromInt(int32_t value) : value(value) {}
            void accept(Visitor* visitor) override {
                visitor->onFromInt(this);
            }
        };

        class FromIcon : public Base {
        public:
            const std::shared_ptr<Icon::Deferred::Base> value;
            FromIcon(std::shared_ptr<Icon::Deferred::Base> value) : value(value) {}
            void accept(Visitor* visitor) override {
                visitor->onFromIcon(this);
            }
        };

        class FromColor : public Base {
        public:
            const std::shared_ptr<PaintResources::Color::Deferred::Base> value;
            FromColor(std::shared_ptr<PaintResources::Color::Deferred::Base> value) : value(value) {}
            void accept(Visitor* visitor) override {
                visitor->onFromColor(this);
            }
        };
    }
}
