#include "generated/PaintResources.h"

#include "PaintResourcesInternal.h"

#include "util/convert.h"

namespace PaintResources
{
    // color ========================
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
    void Gradient_setColorAt(GradientRef _this, double location, ColorRef color) {
        _this->qGrad.setColorAt(location, color->qColor);
    }

    void Gradient_dispose(GradientRef _this) {
        delete _this;
    }

    // radial gradient =============
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
    namespace Brush {
        BrushRef create(Style style) {
            return new __Brush {
                QBrush((Qt::BrushStyle)style)
            };
        }

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

    void Pen_setWidth(PenRef _this, int32_t width) {
        _this->qPen.setWidth(width);
    }

    void Pen_setWidth(PenRef _this, double width) {
        _this->qPen.setWidthF(width);
    }

    void Pen_dispose(PenRef _this) {
        delete _this;
    }

    // font =====================================
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

    // painter path =========================================
    namespace PainterPath {
        PainterPathRef create() {
            return new __PainterPath();
        }
    }

    void PainterPath_moveTo(PainterPathRef _this, PointF p) {
        _this->qPath.moveTo(toQPointF(p));
    }

    void PainterPath_moveTo(PainterPathRef _this, double x, double y) {
        _this->qPath.moveTo(x, y);
    }

    void PainterPath_lineto(PainterPathRef _this, PointF p) {
        _this->qPath.lineTo(toQPointF(p));
    }

    void PainterPath_lineTo(PainterPathRef _this, double x, double y) {
        _this->qPath.lineTo(x, y);
    }

    void PainterPath_cubicTo(PainterPathRef _this, PointF c1, PointF c2, PointF endPoint) {
        _this->qPath.cubicTo(toQPointF(c1), toQPointF(c2), toQPointF(endPoint));
    }

    void PainterPath_cubicTo(PainterPathRef _this, double c1X, double c1Y, double c2X, double c2Y, double endPointX, double endPointY) {
        _this->qPath.cubicTo(c1X, c1Y, c2X, c2Y, endPointX, endPointY);
    }

    void PainterPath_dispose(PainterPathRef _this) {
        delete _this;
    }

    // painter path stroker ==================================
    namespace PainterPathStroker {
        PainterPathStrokerRef create() {
            return new __PainterPathStroker();
        }
    }

    void PainterPathStroker_setWidth(PainterPathStrokerRef _this, double width) {
        _this->qStroker.setWidth(width);
    }

    void PainterPathStroker_setJoinStyle(PainterPathStrokerRef _this, Pen::JoinStyle style) {
        _this->qStroker.setJoinStyle(Pen::toQtJoinStyle(style));
    }

    void PainterPathStroker_setCapStyle(PainterPathStrokerRef _this, Pen::CapStyle style) {
        _this->qStroker.setCapStyle(Pen::toQtCapStyle(style));
    }

    void PainterPathStroker_setDashPattern(PainterPathStrokerRef _this, Pen::Style style) {
        _this->qStroker.setDashPattern(Pen::toQtStyle(style));
    }

    void PainterPathStroker_setDashPattern(PainterPathStrokerRef _this, std::vector<double> dashPattern) {
        QList<qreal> qPattern;
        for (auto &x : dashPattern) {
            qPattern.push_back(x);
        }
        _this->qStroker.setDashPattern(qPattern);
    }

    PainterPathRef PainterPathStroker_createStroke(PainterPathStrokerRef _this, PainterPathRef path) {
        return new __PainterPath { _this->qStroker.createStroke(path->qPath) };
    }

    void PainterPathStroker_dispose(PainterPathStrokerRef _this) {
        delete _this;
    }
}
