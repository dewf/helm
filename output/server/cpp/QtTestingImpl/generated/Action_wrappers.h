#pragma once
#include "Action.h"

namespace Action
{

    void Handle__push(HandleRef value);
    HandleRef Handle__pop();

    void Handle_onTriggered__wrapper();

    void Handle_dispose__wrapper();

    void create__wrapper();

    int __register();
}
