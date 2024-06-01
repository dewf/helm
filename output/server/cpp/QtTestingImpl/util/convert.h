#pragma once

#include <Qt>
#include <QRect>

#include "../generated/Common.h"

#define STRUCT_CAST(b,a) *((b*)&a)

using namespace Common;

inline Qt::Alignment toQtAlign(Alignment align) {
    // temporary until we have better flags definition parsing in NativeImpl
    switch (align) {
        case Alignment::Left:
            return Qt::AlignLeft;
        case Alignment::Leading:
            return Qt::AlignLeading;
        case Alignment::Right:
            return Qt::AlignRight;
        case Alignment::Trailing:
            return Qt::AlignTrailing;
        case Alignment::HCenter:
            return Qt::AlignHCenter;
        case Alignment::Justify:
            return Qt::AlignJustify;
        case Alignment::Absolute:
            return Qt::AlignAbsolute;
        case Alignment::Top:
            return Qt::AlignTop;
        case Alignment::Bottom:
            return Qt::AlignBottom;
        case Alignment::VCenter:
            return Qt::AlignVCenter;
        case Alignment::Baseline:
            return Qt::AlignBaseline;
        case Alignment::Center:
            return Qt::AlignCenter;
        default:
            printf("toQtAlign() - unhandled case!!\n");
    }
    return Qt::AlignLeft;
}

inline Point toPoint(const QPoint& qPoint) {
    return { qPoint.x(), qPoint.y() };
}

inline Rect toRect(const QRect& qRect) {
    return { qRect.left(), qRect.top(), qRect.width(), qRect.height() };
}

inline QRect toQRect(const Rect& r) {
    return {r.x, r.y, r.width, r.height};
}

inline QRectF toQRectF(const RectF& r) {
    return { r.x, r.y, r.width, r.height };
}
