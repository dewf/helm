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

namespace Common
{

    struct Rect {
        int32_t x;
        int32_t y;
        int32_t width;
        int32_t height;
    };

    struct Point {
        int32_t x;
        int32_t y;
    };
}
