#pragma once
#include "Menu.h"

namespace Menu
{

    void Handle__push(HandleRef value);
    HandleRef Handle__pop();

    void Handle_addAction__wrapper();

    void Handle_dispose__wrapper();

    void create__wrapper();

    int __register();
}
