#include "generated/ModelIndex.h"

#include <QModelIndex>
#include "ModelIndexInternal.h"

#define THIS ((QModelIndex*)_this)

namespace ModelIndex
{
    bool Handle_isValid(HandleRef _this) {
        return THIS->isValid();
    }

    int32_t Handle_row(HandleRef _this) {
        return THIS->row();
    }

    int32_t Handle_column(HandleRef _this) {
        return THIS->column();
    }

    class FromDeferred : public ModelIndex::Deferred::Visitor {
    private:
        QModelIndex &modelIndex;
    public:
        explicit FromDeferred(QModelIndex &modelIndex)
                : modelIndex(modelIndex) {}

        void onEmpty(const Deferred::Empty *empty) override {
            modelIndex = QModelIndex();
        }

        void onFromHandle(const Deferred::FromHandle *fromHandle) override {
            modelIndex = *((QModelIndex*)fromHandle->handle);
        }
    };

    QModelIndex fromDeferred(const std::shared_ptr<ModelIndex::Deferred::Base>& deferred) {
        QModelIndex ret;
        FromDeferred visitor(ret);
        deferred->accept(&visitor);
        return ret;
    }
}
