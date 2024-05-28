#include "generated/Layout.h"

#include <QLayout>

#define THIS ((QLayout*)_this)

namespace Layout
{
    void Handle_removeAll(HandleRef _this) {
        while (true) {
            auto child = THIS->takeAt(0);
            if (child != nullptr) {
                // don't touch child->widget
                // hmm we're really going to have to think about ownership, removing things from parents, etc
                delete child; // delete the layout item, not the child widget
            } else {
                break;
            }
        }
    }

    void Handle_setSpacing(HandleRef _this, int32_t spacing) {
        THIS->setSpacing(spacing);
    }

    void Handle_setContentsMargins(HandleRef _this, int32_t left, int32_t top, int32_t right, int32_t bottom) {
        THIS->setContentsMargins(left, top, right, bottom);
    }

    void Handle_dispose(HandleRef _this) {
        delete (QLayout*)_this;
    }
}
