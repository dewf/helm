#include "generated/KeySequence.h"

#include "util/convert.h"
#include "KeySequenceInternal.h"

namespace KeySequence
{
    void Handle_dispose(HandleRef _this) {
        delete _this;
    }

    HandleRef create(std::string seq) {
        return new __Handle {
            QKeySequence(seq.c_str())
        };
    }

    HandleRef create(StandardKey key) {
        return new __Handle {
            QKeySequence((QKeySequence::StandardKey)key)
        };
    }

    HandleRef create(Key key, std::set<Modifier> modifiers) {
        return new __Handle {
            QKeySequence(
                QKeyCombination(toQtModifiers(modifiers), (Qt::Key)key)
            )
        };
    }
}
