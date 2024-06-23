#include "generated/Icon.h"

#include <QIcon>
#include "IconInternal.h"

namespace Icon
{
    void Handle_dispose(HandleRef _this) {
        delete _this;
    }

    // no opaque methods yet, if ever
    // currently we only need this for a widget signal (WindowIconChanged), might not for anything else

    // F# API will be designed around Deferred version

    class FromDeferred : public Deferred::Visitor {
    private:
        QIcon &icon;
    public:
        explicit FromDeferred(QIcon &icon) : icon(icon) {}

        void onFromThemeIcon(const Deferred::FromThemeIcon *fromThemeIcon) override {
            icon = QIcon::fromTheme((QIcon::ThemeIcon)fromThemeIcon->themeIcon);
        }

        void onFromFilename(const Deferred::FromFilename *fromFilename) override {
            icon = QIcon(fromFilename->filename.c_str());
        }
    };

    QIcon fromDeferred(const std::shared_ptr<Deferred::Base>& deferred) {
        QIcon icon;
        FromDeferred visitor(icon);
        deferred->accept(&visitor);
        return icon;
    }
}
