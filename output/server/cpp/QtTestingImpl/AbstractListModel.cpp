#pragma clang diagnostic push
#pragma ide diagnostic ignored "modernize-use-nodiscard"

#include "generated/AbstractListModel.h"
#include "VariantInternal.h"
#include "ModelIndexInternal.h"

#include <QAbstractListModel>

#define THIS ((Subclassed*)_this)

namespace AbstractListModel
{
    class Subclassed : public QAbstractListModel {
    private:
        std::shared_ptr<MethodDelegate> methodDelegate;
        MethodMask methodMask;
    public:
        Subclassed(QObject *parent, const std::function<CreateFunc>& createFunc, uint32_t mask)
            : QAbstractListModel(parent)
        {
            methodDelegate = createFunc((InteriorRef)this);
            methodMask = (MethodMask)mask;
        }

        // ==== must-implement abstract methods ===========================
        QModelIndex parent(const QModelIndex &child) const override {
            auto deferred = methodDelegate->parent((ModelIndex::HandleRef)&child);
            return ModelIndex::fromDeferred(deferred);
        }

        int rowCount(const QModelIndex &parent) const override {
            return methodDelegate->rowCount((ModelIndex::HandleRef)&parent);
        }

        int columnCount(const QModelIndex &parent) const override {
            return methodDelegate->columnCount((ModelIndex::HandleRef)&parent);
        }

        QVariant data(const QModelIndex &index, int role) const override {
            auto deferred = methodDelegate->data((ModelIndex::HandleRef)&index, (ItemDataRole)role);
            return Variant::fromDeferred(deferred);
        }

        // ==== OPTIONAL methods ================================================
        QVariant headerData(int section, Qt::Orientation orientation, int role) const override {
            if (methodMask & MethodMask::HeaderData) {
                auto deferred = methodDelegate->headerData(section, (Enums::Orientation)orientation, (ItemDataRole)role);
                return Variant::fromDeferred(deferred);
            } else {
                return QAbstractItemModel::headerData(section, orientation, role);
            }
        }

        // 'interior' (friend handle) functions
        friend void Interior_beginInsertRows(InteriorRef _this, std::shared_ptr<ModelIndex::Deferred::Base> parent, int32_t first, int32_t last);
        friend void Interior_endInsertRows(InteriorRef _this);
    };

    void Interior_beginInsertRows(InteriorRef _this, std::shared_ptr<ModelIndex::Deferred::Base> parent, int32_t first, int32_t last) {
        THIS->beginInsertRows(ModelIndex::fromDeferred(parent), first, last);
    }

    void Interior_endInsertRows(InteriorRef _this) {
        THIS->endInsertRows();
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef createSubclassed(std::function<CreateFunc> func, uint32_t mask) {
        return (HandleRef) new Subclassed(nullptr, func, mask);
    }
}

#pragma clang diagnostic pop