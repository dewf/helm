#include "generated/Object.h"

#include <QObject>

#define THIS ((QObject*)_this)

namespace Object
{
    void Handle_dumpObjectTree(HandleRef _this) {
        THIS->dumpObjectTree();
    }

    void Handle_dispose(HandleRef _this) {
    }
}
