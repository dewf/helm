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

namespace LineEdit
{

    struct __Handle; typedef struct __Handle* HandleRef; // extends Widget::HandleRef

    void Handle_setText(HandleRef _this, std::string str);
    void Handle_onTextEdited(HandleRef _this, std::function<StringDelegate> handler);
    void Handle_onReturnPressed(HandleRef _this, std::function<VoidDelegate> handler);
    void Handle_dispose(HandleRef _this);
    HandleRef create();
}
