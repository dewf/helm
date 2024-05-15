#pragma once
#include "ComboBox.h"

namespace ComboBox
{

    void Handle__push(HandleRef value);
    HandleRef Handle__pop();

    void Handle_setItems__wrapper();

    void Handle_onCurrentIndexChanged__wrapper();

    void Handle_onCurrentTextChanged__wrapper();

    void Handle_dispose__wrapper();

    void create__wrapper();

    int __register();
}
