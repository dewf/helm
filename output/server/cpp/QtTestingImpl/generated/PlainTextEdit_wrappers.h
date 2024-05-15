#pragma once
#include "PlainTextEdit.h"

namespace PlainTextEdit
{

    void Handle__push(HandleRef value);
    HandleRef Handle__pop();

    void Handle_setText__wrapper();

    void Handle_onTextChanged__wrapper();

    void Handle_dispose__wrapper();

    void create__wrapper();

    int __register();
}
