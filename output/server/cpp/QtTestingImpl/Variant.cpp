#include "generated/Variant.h"

#include "VariantInternal.h"

namespace Variant
{
    class FromDeferred : public Deferred::Visitor {
    private:
        QVariant &variant;
    public:
        explicit FromDeferred(QVariant &variant) : variant(variant) {}

        void onFromString(const Deferred::FromString *fromString) override {
            variant = fromString->value.c_str();
        }

        void onFromInt(const Deferred::FromInt *fromInt) override {
            variant = fromInt->value;
        }

        void onFromIcon(const Deferred::FromIcon *fromIcon) override {
            variant = Icon::fromDeferred(fromIcon->value);
        }
    };

    QVariant fromDeferred(const std::shared_ptr<Deferred::Base>& deferred) {
        QVariant ret;
        FromDeferred visitor(ret);
        deferred->accept(&visitor);
        return ret;
    }
}
