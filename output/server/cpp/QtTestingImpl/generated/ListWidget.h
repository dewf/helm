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
#include "Signal.h"
using namespace ::Signal;

namespace ListWidget
{

    struct __Handle; typedef struct __Handle* HandleRef; // extends Widget::HandleRef

    enum class SelectionMode {
        None,
        Single,
        Multi,
        Extended,
        Contiguous
    };

    void Handle_setItems(HandleRef _this, std::vector<std::string> items);
    void Handle_setSelectionMode(HandleRef _this, SelectionMode mode);
    void Handle_onCurrentRowChanged(HandleRef _this, std::function<IntDelegate> handler);
    void Handle_dispose(HandleRef _this);
    HandleRef create();
}
