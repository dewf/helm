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

#include "Signal.h"
using namespace ::Signal;
#include "Widget.h"
using namespace ::Widget;

namespace TabWidget
{

    struct __Handle; typedef struct __Handle* HandleRef; // extends Widget::HandleRef

    void Handle_addTab(HandleRef _this, Widget::HandleRef page, std::string label);
    void Handle_insertTab(HandleRef _this, int32_t index, Widget::HandleRef page, std::string label);
    void Handle_onCurrentChanged(HandleRef _this, std::function<IntDelegate> handler);
    void Handle_dispose(HandleRef _this);
    HandleRef create();
}
