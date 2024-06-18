#pragma once

#include <QKeySequence>
#include <utility>

namespace KeySequence {
    struct __Handle {
        QKeySequence seq;
        explicit __Handle(QKeySequence seq) : seq(std::move(seq)) {}
    };
}
