#pragma once
#include "Layout.h"

namespace Layout
{

    void Handle__push(HandleRef value);
    HandleRef Handle__pop();

    void Handle_removeAll__wrapper();

    void Handle_dispose__wrapper();

    int __register();
}
