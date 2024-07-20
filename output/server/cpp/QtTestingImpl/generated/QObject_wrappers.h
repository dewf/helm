#pragma once
#include "QObject.h"

namespace QObject
{

    void Handle__push(HandleRef value);
    HandleRef Handle__pop();

    void Handle_onDestroyed__wrapper();

    void Handle_dispose__wrapper();

    int __register();
}
