#pragma once

#include <QIcon>
#include <utility>

namespace Icon {
    struct __Handle {
        QIcon icon;
        explicit __Handle(QIcon icon) : icon(std::move(icon)) {}
    };
}
