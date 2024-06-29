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

    void Handle_dispose(HandleRef _this) {
        printf("QModelIndex Handle_dispose - you should never see this, and it needs to be removed (via @nodispose) Handle isn't owned (vs. OwnedHandle)\n");
    }

    void OwnedHandle_dispose(OwnedHandleRef _this) {
        delete THIS;
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

        void onFromOwned(const Deferred::FromOwned *fromOwned) override {
            modelIndex = *((QModelIndex*)fromOwned->owned);
        }
    };

    QModelIndex fromDeferred(const std::shared_ptr<ModelIndex::Deferred::Base>& deferred) {
        QModelIndex ret;
        FromDeferred visitor(ret);
        deferred->accept(&visitor);
        return ret;
    }
}
