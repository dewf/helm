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

    void Handle_dispose(HandleRef _this) {
        delete (QLayout*)_this;
    }
}
