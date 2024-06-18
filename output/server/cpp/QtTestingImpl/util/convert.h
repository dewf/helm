#pragma once

#include <Qt>
#include <QRect>
#include <QKeySequence>

#include "../generated/Common.h"
using namespace Common;
#include "../generated/Enums.h"
using namespace Enums;

#define STRUCT_CAST(b,a) *((b*)&a)

inline QSize toQSize(const Size& sz) {
    return {sz.width, sz.height };
}

inline Size toSize(const QSize& sz) {
    return { sz.width(), sz.height() };
}

inline Point toPoint(const QPoint& qPoint) {
    return { qPoint.x(), qPoint.y() };
}

inline PointF toPointF(const QPointF& qPointF) {
    return { qPointF.x(), qPointF.y() };
}

inline QPoint toQPoint(const Point& p) {
    return QPoint(p.x, p.y);
}

inline QPointF toQPointF(const PointF& p) {
    return QPointF(p.x, p.y);
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

inline std::set<Modifier> fromQtModifiers (Qt::KeyboardModifiers modifiers) {
    std::set<Modifier> ret;
    if (modifiers.testFlag(Qt::ShiftModifier)) {
        ret.emplace(Modifier::Shift);
    }
    if (modifiers.testFlag(Qt::ControlModifier)) {
        ret.emplace(Modifier::Control);
    }
    if (modifiers.testFlag(Qt::AltModifier)) {
        ret.emplace(Modifier::Alt);
    }
    if (modifiers.testFlag(Qt::MetaModifier)) {
        ret.emplace(Modifier::Meta);
    }
    return ret;
}

inline Qt::KeyboardModifiers toQtModifiers (const std::set<Modifier>& modifiers) {
    Qt::KeyboardModifiers ret;
    if (modifiers.contains(Modifier::Shift)) {
        ret.setFlag(Qt::ShiftModifier);
    }
    if (modifiers.contains(Modifier::Control)) {
        ret.setFlag(Qt::ControlModifier);
    }
    if (modifiers.contains(Modifier::Alt)) {
        ret.setFlag(Qt::AltModifier);
    }
    if (modifiers.contains(Modifier::Meta)) {
        ret.setFlag(Qt::MetaModifier);
    }
    return ret;
}

inline MouseButton fromQtButton(Qt::MouseButton button) {
    switch (button) {
        case Qt::NoButton:
            return MouseButton::None;
        case Qt::LeftButton:
            return MouseButton::Left;
        case Qt::RightButton:
            return MouseButton::Right;
        case Qt::MiddleButton:
            return MouseButton::Middle;
        default:
            return MouseButton::Other;
    }
}

inline std::set<MouseButton> fromQtButtons(Qt::MouseButtons buttons) {
    std::set<MouseButton> ret;
    if (buttons.testFlag(Qt::LeftButton)) {
        ret.emplace(MouseButton::Left);
    }
    if (buttons.testFlag(Qt::RightButton)) {
        ret.emplace(MouseButton::Right);
    }
    if (buttons.testFlag(Qt::MiddleButton)) {
        ret.emplace(MouseButton::Middle);
    }
    return ret;
}
