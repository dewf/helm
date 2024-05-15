#pragma once
#include "BoxLayout.h"

namespace BoxLayout
{

    void Alignment__push(uint32_t value);
    uint32_t Alignment__pop();

    void Direction__push(Direction value);
    Direction Direction__pop();

    void Handle__push(HandleRef value);
    HandleRef Handle__pop();

    void Handle_addSpacing__wrapper();

    void Handle_addStretch__wrapper();

    void Handle_addWidget__wrapper();

    void Handle_addWidget_overload1__wrapper();

    void Handle_dispose__wrapper();

    void create__wrapper();

    int __register();
}
