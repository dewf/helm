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

namespace PushButton
{

    struct __Handle; typedef struct __Handle* HandleRef; // extends Widget::HandleRef

    void Handle_setText(HandleRef _this, std::string label);
    void Handle_onClicked(HandleRef _this, std::function<VoidDelegate> handler);
    void Handle_dispose(HandleRef _this);
    HandleRef create(std::string label);
}
