#include "generated/Icon.h"

#include <QIcon>
#include "IconInternal.h"

namespace Icon
{
    void Handle_dispose(HandleRef _this) {
        delete _this;
    }

    // no opaque methods yet, if ever
    // currently we only need this for a widget signal (WindowIconChanged), might not for anything else

    // F# API will be designed around Deferred version
}
