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

#include "Widget.h"
using namespace ::Widget;
#include "Layout.h"
using namespace ::Layout;

namespace BoxLayout
{

    struct __Handle; typedef struct __Handle* HandleRef; // extends Layout::HandleRef

    enum Alignment {
        AlignLeft = 0x1,
        AlignRight = 0x2,
        AlignHCenter = 0x4,
        AlignJustify = 0x8,
        AlignTop = 0x20,
        AlignBottom = 0x40,
        AlignVCenter = 0x80,
        AlignBaseline = 0x100,
        AlignCenter = 0x84
    };

    enum class Direction {
        LeftToRight,
        RightToLeft,
        TopToBottom,
        BottomToTop
    };

    void Handle_setDirection(HandleRef _this, Direction dir);
    void Handle_setSpacing(HandleRef _this, int32_t spacing);
    void Handle_addSpacing(HandleRef _this, int32_t size);
    void Handle_addStretch(HandleRef _this, int32_t stretch);
    void Handle_addWidget(HandleRef _this, Widget::HandleRef widget);
    void Handle_addWidget(HandleRef _this, Widget::HandleRef widget, int32_t stretch);
    void Handle_dispose(HandleRef _this);
    HandleRef create(Direction dir);
}
