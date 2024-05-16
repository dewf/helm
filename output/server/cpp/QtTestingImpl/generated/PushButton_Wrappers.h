#pragma once
#include "PushButton.h"

namespace PushButton
{

    void Handle__push(HandleRef value);
    HandleRef Handle__pop();

    void Handle_setText__wrapper();

    void Handle_onClicked__wrapper();

    void Handle_dispose__wrapper();

    void create__wrapper();

    int __register();
}
