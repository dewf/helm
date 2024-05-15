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

namespace PlainTextEdit
{

    struct __Handle; typedef struct __Handle* HandleRef; // extends Widget::HandleRef

    void Handle_setText(HandleRef _this, std::string text);
    void Handle_onTextChanged(HandleRef _this, std::function<StringDelegate> handler);
    void Handle_dispose(HandleRef _this);
    HandleRef create();
}
