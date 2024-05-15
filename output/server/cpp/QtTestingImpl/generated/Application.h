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

namespace Application
{

    struct __Handle; typedef struct __Handle* HandleRef;

    int32_t Handle_exec(HandleRef _this);
    void Handle_setStyle(HandleRef _this, std::string name);
    void Handle_dispose(HandleRef _this);
    HandleRef create(std::vector<std::string> args);
}
