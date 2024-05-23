#include "generated/Painter.h"

#include <QPainter>

#include "util/convert.h"
#include "PaintResourcesInternal.h" // for struct definitions

#define THIS ((QPainter*)_this)

namespace Painter
{
    void Handle_setPen(HandleRef _this, PenRef pen) {
        THIS->setPen(pen->qPen);
    }

    void Handle_setBrush(HandleRef _this, BrushRef brush) {
        THIS->setBrush(brush->qBrush);
    }

    void Handle_setFont(HandleRef _this, FontRef font) {
        THIS->setFont(font->qFont);
    }

    void Handle_drawText(HandleRef _this, Rect rect, Alignment align, std::string text) {
        THIS->drawText(toQRect(rect), toQtAlign(align), text.c_str());
    }

    void Handle_fillRect(HandleRef _this, Rect rect, BrushRef brush) {
        THIS->fillRect(toQRect(rect), brush->qBrush);
    }

    void Handle_fillRect(HandleRef _this, Rect rect, ColorRef color) {
        THIS->fillRect(toQRect(rect), color->qColor);
    }

    void Handle_drawRect(HandleRef _this, Rect rect) {
        THIS->drawRect(toQRect(rect));
    }

    void Handle_drawRect(HandleRef _this, RectF rect) {
        THIS->drawRect(toQRectF(rect));
    }

    void Handle_drawRect(HandleRef _this, int32_t x, int32_t y, int32_t width, int32_t height) {
        THIS->drawRect(x, y, width, height);
    }

    void Handle_dispose(HandleRef _this) {
        // presently these will never be created (therefore owned) by the client side
        printf("QPainter handle dispose called - why?\n");
    }
}
