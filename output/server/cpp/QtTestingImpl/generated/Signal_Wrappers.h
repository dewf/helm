#pragma once
#include "Signal.h"

namespace Signal
{
    void VoidDelegate__push(std::function<VoidDelegate> f);
    std::function<VoidDelegate> VoidDelegate__pop();
    void StringDelegate__push(std::function<StringDelegate> f);
    std::function<StringDelegate> StringDelegate__pop();

    int __register();
}
