#include "generated/Variant.h"

#include "VariantInternal.h"
#include "PaintResourcesInternal.h"

namespace Variant
{
    class FromDeferred : public Deferred::Visitor {
    private:
        QVariant &variant;
    public:
        explicit FromDeferred(QVariant &variant) : variant(variant) {}

        void onEmpty(const Deferred::Empty *empty) override {
            variant = QVariant();
        }

        void onFromString(const Deferred::FromString *fromString) override {
            variant = fromString->value.c_str();
        }

        void onFromInt(const Deferred::FromInt *fromInt) override {
            variant = fromInt->value;
        }

        void onFromIcon(const Deferred::FromIcon *fromIcon) override {
            variant = Icon::fromDeferred(fromIcon->value);
        }

        void onFromColor(const Deferred::FromColor *fromColor) override {
            variant = Color::fromDeferred(fromColor->value);
        }
    };

    QVariant fromDeferred(const std::shared_ptr<Deferred::Base>& deferred) {
        QVariant ret;
        FromDeferred visitor(ret);
        deferred->accept(&visitor);
        return ret;
    }
}
