#pragma once
#include "Common.h"

namespace Common
{

    void Rect__push(Rect value, bool isReturn);
    Rect Rect__pop();

    void Point__push(Point value, bool isReturn);
    Point Point__pop();

    int __register();
}
