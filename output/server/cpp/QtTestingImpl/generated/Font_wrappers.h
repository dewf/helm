#pragma once
#include "Font.h"

namespace Font
{

    void Weight__push(Weight value);
    Weight Weight__pop();

    void Handle__push(HandleRef value);
    HandleRef Handle__pop();

    void Handle_dispose__wrapper();

    void OwnedHandle__push(OwnedHandleRef value);
    OwnedHandleRef OwnedHandle__pop();

    void OwnedHandle_dispose__wrapper();
    void Deferred__push(std::shared_ptr<Deferred::Base> value, bool isReturn);
    std::shared_ptr<Deferred::Base> Deferred__pop();

    int __register();
}
