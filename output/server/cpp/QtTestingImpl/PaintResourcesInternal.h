#pragma once

#include <QColor>
#include <QGradient>
#include <QRadialGradient>
#include <QBrush>
#include <QPen>
#include <QFont>

namespace PaintResources
{
    struct __Color {
        QColor qColor;
    };

    struct __Gradient {
        QGradient qGrad;
    };

    struct __RadialGradient {
        QRadialGradient qGrad;
    };

    struct __Brush {
        QBrush qBrush;
    };

    struct __Pen {
        QPen qPen;
    };

    struct __Font {
        QFont qFont;
    };
}
