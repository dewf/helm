#include "generated/ListView.h"

#include <QListView>
#include <utility>
#include "util/SignalStuff.h"
#include "util/convert.h"

#define THIS ((ListViewWithHandler*)_this)

namespace ListView
{
    class ListViewWithHandler : public QListView {
        Q_OBJECT
    private:
        std::shared_ptr<SignalHandler> handler;
        uint32_t lastMask = 0;
        std::vector<SignalMapItem<SignalMask>> signalMap = {
            { SignalMask::CustomContextMenuRequested, SIGNAL(customContextMenuRequested(QPoint)), SLOT(onCustomContextMenuRequested(QPoint)) },
            { SignalMask::Activated, SIGNAL(activated(QModelIndex)), SLOT(onActivated(QModelIndex)) },
            { SignalMask::Clicked, SIGNAL(clicked(QModelIndex)), SLOT(onClicked(QModelIndex)) },
            { SignalMask::DoubleClicked, SIGNAL(doubleClicked(QModelIndex)), SLOT(onDoubleClicked(QModelIndex)) },
            { SignalMask::Entered, SIGNAL(entered(QModelIndex)), SLOT(onEntered(QModelIndex)) },
            { SignalMask::IconSizeChanged, SIGNAL(iconSizeChanged(QSize)), SLOT(onIconSizeChanged(QSize)) },
            { SignalMask::Pressed, SIGNAL(pressed(QModelIndex)), SLOT(onPressed(QModelIndex)) },
            { SignalMask::ViewportEntered, SIGNAL(viewportEntered()), SLOT(onViewportEntered) },
            { SignalMask::IndexesMoved, SIGNAL(indexesMoved(QModelIndexList)), SLOT(onIndexesMoved(QModelIndexList)) },
        };
    public:
        explicit ListViewWithHandler(std::shared_ptr<SignalHandler> handler) : handler(std::move(handler)) {}
        void setSignalMask(uint32_t newMask) {
            if (newMask != lastMask) {
                processChanges(lastMask, newMask, signalMap, this);
                lastMask = newMask;
            }
        }
    public slots:
        void onCustomContextMenuRequested(const QPoint& pos) {
            handler->customContextMenuRequested(toPoint(pos));
        };
        void onActivated(const QModelIndex& index) {
            handler->activated((ModelIndex::HandleRef)&index);
        }
        void onClicked(const QModelIndex& index) {
            handler->clicked((ModelIndex::HandleRef)&index);
        }
        void onDoubleClicked(const QModelIndex& index) {
            handler->doubleClicked((ModelIndex::HandleRef)&index);
        }
        void onEntered(QModelIndex& index) {
            handler->entered((ModelIndex::HandleRef)&index);
        }
        void onIconSizeChanged(const QSize& size) {
            handler->iconSizeChanged(toSize(size));
        }
        void onPressed(const QModelIndex& index) {
            handler->pressed((ModelIndex::HandleRef)&index);
        }
        void onViewportEntered() {
            handler->viewportEntered();
        }
        void onIndexesMoved(const QModelIndexList &indexes) {
            std::vector<ModelIndex::HandleRef> indexes2;
            for (auto &index : indexes) {
                // will this work? seems sketchy
                indexes2.push_back((ModelIndex::HandleRef)&index);
            }
            handler->indexesMoved(indexes2);
        }
    };

    void Handle_setMovement(HandleRef _this, Movement movement) {
        THIS->setMovement((QListView::Movement)movement);
    }

    void Handle_setFlow(HandleRef _this, Flow flow) {
        THIS->setFlow((QListView::Flow)flow);
    }

    void Handle_setResizeMode(HandleRef _this, ResizeMode mode) {
        THIS->setResizeMode((QListView::ResizeMode)mode);
    }

    void Handle_setLayoutMode(HandleRef _this, LayoutMode mode) {
        THIS->setLayoutMode((QListView::LayoutMode)mode);
    }

    void Handle_setViewMode(HandleRef _this, ViewMode mode) {
        THIS->setViewMode((QListView::ViewMode)mode);
    }

    void Handle_setSignalMask(HandleRef _this, uint32_t mask) {
        THIS->setSignalMask(mask);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create(std::shared_ptr<SignalHandler> handler) {
        return (HandleRef) new ListViewWithHandler(std::move(handler));
    }
}

#include "ListView.moc"