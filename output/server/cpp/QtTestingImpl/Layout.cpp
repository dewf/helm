#include "generated/Layout.h"

#include <QLayout>

namespace Layout
{
    void Handle_dispose(HandleRef _this) {
        delete (QLayout*)_this;
    }
}
