#pragma once
#include "Application.h"

namespace Application
{

    void Handle__push(HandleRef value);
    HandleRef Handle__pop();

    void Handle_exec__wrapper();

    void Handle_dispose__wrapper();

    void create__wrapper();

    int __register();
}
