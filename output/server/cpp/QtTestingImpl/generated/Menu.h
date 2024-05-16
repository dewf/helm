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
#include "Action.h"
using namespace ::Action;

namespace Menu
{

    struct __Handle; typedef struct __Handle* HandleRef; // extends Widget::HandleRef

    void Handle_addAction(HandleRef _this, Action::HandleRef action);
    void Handle_dispose(HandleRef _this);
    HandleRef create(std::string title);
}
