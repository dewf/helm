#pragma once
#include "Widget.h"

namespace Widget
{

    void Handle__push(HandleRef value);
    HandleRef Handle__pop();

    void Handle_getRect__wrapper();

    void Handle_resize__wrapper();

    void Handle_show__wrapper();

    void Handle_setWindowTitle__wrapper();

    void Handle_setLayout__wrapper();

    void Handle_onWindowTitleChanged__wrapper();

    void Handle_dispose__wrapper();

    void create__wrapper();

    int __register();
}
