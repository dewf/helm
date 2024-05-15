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

#include "Common.h"
using namespace ::Common;
#include "Signal.h"
using namespace ::Signal;
#include "Layout.h"
using namespace ::Layout;

namespace Widget
{

    struct __Handle; typedef struct __Handle* HandleRef;
    extern const int32_t WIDGET_SIZE_MAX;

    void Handle_setMaximumWidth(HandleRef _this, int32_t maxWidth);
    void Handle_setMaximumHeight(HandleRef _this, int32_t maxHeight);
    Rect Handle_getRect(HandleRef _this);
    void Handle_resize(HandleRef _this, int32_t width, int32_t height);
    void Handle_show(HandleRef _this);
    void Handle_setWindowTitle(HandleRef _this, std::string title);
    void Handle_setLayout(HandleRef _this, Layout::HandleRef layout);
    void Handle_onWindowTitleChanged(HandleRef _this, std::function<StringDelegate> func);
    void Handle_dispose(HandleRef _this);
    HandleRef create();
}
