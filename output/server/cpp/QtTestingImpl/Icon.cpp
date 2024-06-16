#include "generated/Icon.h"

#include <QIcon>

#include "IconInternal.h"

namespace Icon
{
    void Handle_dispose(HandleRef _this) {
        delete _this;
    }

    HandleRef create(ThemeIcon themeIcon) {
        auto qThemeIcon = (QIcon::ThemeIcon)themeIcon;
        return new __Handle { QIcon::fromTheme(qThemeIcon) };
    }

    HandleRef create(std::string filename) {
        return new __Handle { QIcon(filename.c_str()) };
    }
}
