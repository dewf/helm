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

namespace QObject2
{

    struct __Handle; typedef struct __Handle* HandleRef;

    void Handle_onDestroyed(HandleRef _this, std::function<VoidDelegate> handler);
    void Handle_dispose(HandleRef _this);
}
