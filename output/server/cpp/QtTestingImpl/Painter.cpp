#include "generated/Painter.h"

#include <QColor>
#include <QGradient>
#include <QRadialGradient>
#include <QBrush>
#include <QPen>
#include <QFont>
#include <QPainter>

#include "util/convert.h"

#define THIS ((QPainter*)_this)

namespace Painter
{
    // color ========================
    struct __Color {
        QColor qColor;
    };

    namespace Color {
        QColor fromConstant(Constant name) {
            switch (name) {
                case Constant::Black:
                    return QColorConstants::Black;
                case Constant::White:
                    return QColorConstants::White;
                case Constant::DarkGray:
                    return QColorConstants::DarkGray;
                case Constant::Gray:
                    return QColorConstants::Gray;
                case Constant::LightGray:
                    return QColorConstants::LightGray;
                case Constant::Red:
                    return QColorConstants::Red;
                case Constant::Green:
                    return QColorConstants::Green;
                case Constant::Blue:
                    return QColorConstants::Blue;
                case Constant::Cyan:
                    return QColorConstants::Cyan;
                case Constant::Magenta:
                    return QColorConstants::Magenta;
                case Constant::Yellow:
                    return QColorConstants::Yellow;
                case Constant::DarkRed:
                    return QColorConstants::DarkRed;
                case Constant::DarkGreen:
                    return QColorConstants::DarkGreen;
                case Constant::DarkBlue:
                    return QColorConstants::DarkBlue;
                case Constant::DarkCyan:
                    return QColorConstants::DarkCyan;
                case Constant::DarkMagenta:
                    return QColorConstants::DarkMagenta;
                case Constant::DarkYellow:
                    return QColorConstants::DarkYellow;
                case Constant::Transparent:
                    return QColorConstants::Transparent;
                default:
                    printf("Painter.cpp Color::fromConstant - unhandled value\n");
            }
            return QColorConstants::Black;
        }

        ColorRef create(Constant name) {
            return new __Color {
                fromConstant(name)
            };
        }

        ColorRef create(int32_t r, int32_t g, int32_t b) {
            return new __Color {
                QColor::fromRgb(r, g, b)
            };
        }

        ColorRef create(int32_t r, int32_t g, int32_t b, int32_t a) {
            return new __Color {
                QColor::fromRgb(r, g, b, a)
            };
        }

        ColorRef create(float r, float g, float b) {
            return new __Color {
                QColor::fromRgbF(r, g, b)
            };
        }

        ColorRef create(float r, float g, float b, float a) {
            return new __Color {
                QColor::fromRgbF(r, g, b, a)
            };
        }
    }

    void Color_dispose(ColorRef _this) {
        delete _this;
    }

    // gradient ====================
    struct __Gradient {
        QGradient qGrad;
    };

    void Gradient_setColorAt(GradientRef _this, double location, ColorRef color) {
        _this->qGrad.setColorAt(location, color->qColor);
    }

    void Gradient_dispose(GradientRef _this) {
        delete _this;
    }

    // radial gradient =============
    struct __RadialGradient {
        QRadialGradient qGrad;
    };

    namespace RadialGradient {
        RadialGradientRef create(double cx, double cy, double centerRadius, double fx, double fy, double focalRadius) {
            return new __RadialGradient {
                QRadialGradient(cx, cy, centerRadius, fx, fy, focalRadius)
            };
        }
    }

    void RadialGradient_dispose(RadialGradientRef _this) {
        delete _this;
    }

    // brush ======================
    struct __Brush {
        QBrush qBrush;
    };

    namespace Brush {
        BrushRef create(ColorRef color) {
            return new __Brush {
                QBrush(color->qColor)
            };
        }

        BrushRef create(GradientRef gradient) {
            return new __Brush {
                QBrush(gradient->qGrad)
            };
        }
    }

    void Brush_dispose(BrushRef _this) {
        delete _this;
    }

    // pen =========================
    struct __Pen {
        QPen qPen;
    };

    namespace Pen {
        PenRef create() {
            return new __Pen { QPen() };
        }

        inline Qt::PenStyle toQtStyle(Style style) {
            return (Qt::PenStyle)style;
        }

        inline Qt::PenCapStyle toQtCapStyle(CapStyle capStyle) {
            return (Qt::PenCapStyle)capStyle;
        }

        inline Qt::PenJoinStyle toQtJoinStyle(JoinStyle joinStyle) {
            return (Qt::PenJoinStyle)joinStyle;
        }

        PenRef create(Style style) {
            return new __Pen {
                QPen(toQtStyle(style))
            };
        }

        PenRef create(ColorRef color) {
            return new __Pen {
                QPen(color->qColor)
            };
        }

        PenRef create(BrushRef brush, double width, Style style, CapStyle cap, JoinStyle join) {
            return new __Pen {
                QPen(brush->qBrush, width, toQtStyle(style), toQtCapStyle(cap), toQtJoinStyle(join))
            };
        }
    }

    void Pen_setBrush(PenRef _this, BrushRef brush) {
        _this->qPen.setBrush(brush->qBrush);
    }

    void Pen_dispose(PenRef _this) {
        delete _this;
    }

    // font =====================================

    struct __Font {
        QFont qFont;
    };

    namespace Font {
        FontRef create(std::string family, int32_t pointSize) {
            return new __Font {
                QFont(family.c_str(), pointSize)
            };
        }

        inline QFont::Weight toQtWeight(Weight weight) {
            // simple cast should be OK for now
            return (QFont::Weight)weight;
        }

        FontRef create(std::string family, int32_t pointSize, Weight weight) {
            return new __Font {
                QFont( family.c_str(), pointSize, toQtWeight(weight))
            };
        }

        FontRef create(std::string family, int32_t pointSize, Weight weight, bool italic) {
            return new __Font {
                QFont( family.c_str(), pointSize, toQtWeight(weight), italic)
            };
        }
    }

    void Font_dispose(FontRef _this) {
        delete _this;
    }

    // QPainter ========================================

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

    void Handle_dispose(HandleRef _this) {
        // presently these will never be created (therefore owned) by the client side
        printf("QPainter handle dispose called - why?\n");
    }
}
