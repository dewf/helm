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

namespace Action
{

    struct __Handle; typedef struct __Handle* HandleRef;

    void Handle_onTriggered(HandleRef _this, std::function<BoolDelegate> handler);
    void Handle_dispose(HandleRef _this);
    HandleRef create(std::string title);
}
