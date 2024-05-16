#pragma once
#include "TabWidget.h"

namespace TabWidget
{

    void Handle__push(HandleRef value);
    HandleRef Handle__pop();

    void Handle_addTab__wrapper();

    void Handle_insertTab__wrapper();

    void Handle_onCurrentChanged__wrapper();

    void Handle_dispose__wrapper();

    void create__wrapper();

    int __register();
}
