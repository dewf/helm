#pragma once

#include <QRect>
#include "../generated/Common.h"

#define STRUCT_CAST(b,a) *((b*)&a)

using namespace Common;

inline Rect qRectToRect(const QRect& qRect) {
    return { qRect.left(), qRect.top(), qRect.width(), qRect.height() };
}

inline QRect rectToQRect(Rect r) {
    return QRect(r.x, r.y, r.width, r.height);
}
