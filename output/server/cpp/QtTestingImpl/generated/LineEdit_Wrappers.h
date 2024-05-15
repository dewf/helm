#pragma once
#include "LineEdit.h"

namespace LineEdit
{

    void Handle__push(HandleRef value);
    HandleRef Handle__pop();

    void Handle_setText__wrapper();

    void Handle_onTextEdited__wrapper();

    void Handle_onReturnPressed__wrapper();

    void Handle_dispose__wrapper();

    void create__wrapper();

    int __register();
}
