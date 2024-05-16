#pragma once
#include "ListWidget.h"

namespace ListWidget
{

    void SelectionMode__push(SelectionMode value);
    SelectionMode SelectionMode__pop();

    void Handle__push(HandleRef value);
    HandleRef Handle__pop();

    void Handle_setItems__wrapper();

    void Handle_setSelectionMode__wrapper();

    void Handle_onCurrentRowChanged__wrapper();

    void Handle_dispose__wrapper();

    void create__wrapper();

    int __register();
}
