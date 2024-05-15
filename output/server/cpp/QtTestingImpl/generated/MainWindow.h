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
#include "Layout.h"
using namespace ::Layout;

namespace MainWindow
{

    struct __Handle; typedef struct __Handle* HandleRef; // extends Widget::HandleRef

    enum class Kind {
        Widget,
        Window,
        Dialog,
        Sheet,
        Drawer,
        Popup,
        Tool,
        ToolTip,
        SplashScreen,
        SubWindow,
        ForeignWindow,
        CoverWindow
    };

    void Handle_setCentralWidget(HandleRef _this, Widget::HandleRef widget);
    void Handle_dispose(HandleRef _this);
    HandleRef create(Widget::HandleRef parent, Kind kind);
}
