#pragma once
#include "Variant.h"

namespace Variant
{

    void Handle__push(HandleRef value);
    HandleRef Handle__pop();

    void Handle_isValid__wrapper();

    void Handle_toString2__wrapper();

    void Handle_toInt__wrapper();

    void Handle_dispose__wrapper();

    void OwnedHandle__push(OwnedHandleRef value);
    OwnedHandleRef OwnedHandle__pop();

    void OwnedHandle_dispose__wrapper();
    void Deferred__push(std::shared_ptr<Deferred::Base> value, bool isReturn);
    std::shared_ptr<Deferred::Base> Deferred__pop();

    int __register();
}
