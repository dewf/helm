#include "../support/NativeImplServer.h"
#include "Common_wrappers.h"
#include "Common.h"

namespace Common
{
    void Rect__push(Rect value, bool isReturn) {
        ni_pushInt32(value.height);
        ni_pushInt32(value.width);
        ni_pushInt32(value.y);
        ni_pushInt32(value.x);
    }

    Rect Rect__pop() {
        auto x = ni_popInt32();
        auto y = ni_popInt32();
        auto width = ni_popInt32();
        auto height = ni_popInt32();
        return Rect { x, y, width, height };
    }
    void Point__push(Point value, bool isReturn) {
        ni_pushInt32(value.y);
        ni_pushInt32(value.x);
    }

    Point Point__pop() {
        auto x = ni_popInt32();
        auto y = ni_popInt32();
        return Point { x, y };
    }

    int __register() {
        auto m = ni_registerModule("Common");
        return 0; // = OK
    }
}
