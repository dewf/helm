#pragma once
#include "Application.h"

namespace Application
{

    void Handle__push(HandleRef value);
    HandleRef Handle__pop();

    void Handle_dispose__wrapper();

    void setStyle__wrapper();

    void exec__wrapper();

    void create__wrapper();

    int __register();
}
