#pragma once
#include "MainWindow.h"

namespace MainWindow
{

    void Kind__push(Kind value);
    Kind Kind__pop();

    void Handle__push(HandleRef value);
    HandleRef Handle__pop();

    void Handle_setCentralWidget__wrapper();

    void Handle_dispose__wrapper();

    void create__wrapper();

    int __register();
}
