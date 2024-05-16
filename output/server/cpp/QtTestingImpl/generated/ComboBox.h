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

namespace ComboBox
{

    struct __Handle; typedef struct __Handle* HandleRef; // extends Widget::HandleRef

    void Handle_clear(HandleRef _this);
    void Handle_setItems(HandleRef _this, std::vector<std::string> items);
    void Handle_setCurrentIndex(HandleRef _this, int32_t index);
    void Handle_onCurrentIndexChanged(HandleRef _this, std::function<IntDelegate> handler);
    void Handle_onCurrentTextChanged(HandleRef _this, std::function<StringDelegate> handler);
    void Handle_dispose(HandleRef _this);
    HandleRef create();
}
