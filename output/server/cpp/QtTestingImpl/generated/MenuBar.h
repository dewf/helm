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
#include "Menu.h"
using namespace ::Menu;

namespace MenuBar
{

    struct __Handle; typedef struct __Handle* HandleRef; // extends Widget::HandleRef

    void Handle_addMenu(HandleRef _this, Menu::HandleRef menu);
    void Handle_dispose(HandleRef _this);
    HandleRef create();
}
