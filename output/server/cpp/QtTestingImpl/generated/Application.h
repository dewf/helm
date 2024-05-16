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

    void Handle_dispose(HandleRef _this);
    void setStyle(std::string name);
    int32_t exec();
    HandleRef create(std::vector<std::string> args);
}
