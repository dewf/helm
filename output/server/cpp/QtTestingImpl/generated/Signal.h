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

namespace Signal
{

    typedef void VoidDelegate();

    typedef void BoolDelegate(bool b);

    typedef void IntDelegate(int32_t i);

    typedef void StringDelegate(std::string s);
}
