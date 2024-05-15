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

namespace ComboBox
{

    struct __Handle; typedef struct __Handle* HandleRef; // extends Widget::HandleRef

    void Handle_todo(HandleRef _this);
    void Handle_dispose(HandleRef _this);
    HandleRef create();
}
