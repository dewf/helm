#pragma once
#include "MenuBar.h"

namespace MenuBar
{

    void Handle__push(HandleRef value);
    HandleRef Handle__pop();

    void Handle_addMenu__wrapper();

    void Handle_dispose__wrapper();

    void create__wrapper();

    int __register();
}
