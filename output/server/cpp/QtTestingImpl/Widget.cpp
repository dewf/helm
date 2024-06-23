#include "generated/Widget.h"

#include <QObject>
#include <QWidget>
#include <QLayout>

#include <QPainter>
#include <QPaintEvent>

#include <QMimeData>
#include <QDrag>
#include <utility>

#include "util/SignalStuff.h"
#include "util/convert.h"
#include "IconInternal.h"

#define THIS ((WidgetWithHandler*)_this)

namespace Widget
{
    class WidgetWithHandler : public QWidget {
        Q_OBJECT
    private:
        std::shared_ptr<SignalHandler> handler;
        uint32_t lastMask = 0;
        std::vector<SignalMapItem<SignalMask>> signalMap = {
            { SignalMask::CustomContextMenuRequested, SIGNAL(customContextMenuRequested(QPoint)), SLOT(onCustomContextMenuRequested(QPoint)) },
            { SignalMask::WindowIconChanged, SIGNAL(windowIconChanged(QIcon)), SLOT(onWindowIconChanged(QIcon)) },
            { SignalMask::WindowTitleChanged, SIGNAL(windowTitleChanged(QString)), SLOT(onWindowTitleChanged(QString)) }
        };
    public:
        explicit WidgetWithHandler(std::shared_ptr<SignalHandler> handler) : handler(std::move(handler)) {}
        void setSignalMask(uint32_t newMask) {
            if (newMask != lastMask) {
                processChanges(lastMask, newMask, signalMap, this);
                lastMask = newMask;
            }
        }
    public slots:
        void onCustomContextMenuRequested(const QPoint& pos) {
            handler->customContextMenuRequested(toPoint(pos));
        }
        void onWindowIconChanged(const QIcon& icon) {
            handler->windowIconChanged((Icon::HandleRef)&icon);
        }
        void onWindowTitleChanged(const QString& title) {
            handler->windowTitleChanged(title.toStdString());
        }
    };

    const int32_t WIDGET_SIZE_MAX = QWIDGETSIZE_MAX;

    void Handle_addAction(HandleRef _this, Action::HandleRef action) {
        THIS->addAction((QAction*)action);
    }

    void Handle_setParent(HandleRef _this, HandleRef parent) {
        THIS->setParent((QWidget*)parent);
    }

    HandleRef Handle_getWindow(HandleRef _this) {
        return (HandleRef)THIS->window();
    }

    void Handle_setEnabled(HandleRef _this, bool state) {
        THIS->setEnabled(state);
    }

    void Handle_setMinimumWidth(HandleRef _this, int32_t minWidth) {
        THIS->setMinimumWidth(minWidth);
    }

    void Handle_setMinimumHeight(HandleRef _this, int32_t minHeight) {
        THIS->setMinimumHeight(minHeight);
    }

    void Handle_setMaximumWidth(HandleRef _this, int32_t maxWidth) {
        THIS->setMaximumWidth(maxWidth);
    }

    void Handle_setMaximumHeight(HandleRef _this, int32_t maxHeight) {
        THIS->setMaximumHeight(maxHeight);
    }

    void Handle_setSizePolicy(HandleRef _this, uint32_t hPolicy, uint32_t vPolicy) {
        THIS->setSizePolicy((QSizePolicy::Policy)hPolicy, (QSizePolicy::Policy)vPolicy);
    }

    Rect Handle_getRect(HandleRef _this) {
        auto x = THIS->rect();
        return toRect(x);
    }

    Size Handle_getSize(HandleRef _this) {
        return toSize(THIS->size());
    }

    void Handle_updateGeometry(HandleRef _this) {
        THIS->updateGeometry();
    }

    void Handle_adjustSize(HandleRef _this) {
        THIS->adjustSize();
    }

    void Handle_resize(HandleRef _this, int32_t width, int32_t height) {
        THIS->resize(width, height);
    }

    void Handle_setFixedWidth(HandleRef _this, int32_t width) {
        THIS->setFixedWidth(width);
    }

    void Handle_setFixedHeight(HandleRef _this, int32_t height) {
        THIS->setFixedHeight(height);
    }

    void Handle_setFixedSize(HandleRef _this, int32_t width, int32_t height) {
        THIS->setFixedSize(width, height);
    }

    void Handle_move(HandleRef _this, Point p) {
        THIS->move(toQPoint(p));
    }

    void Handle_move(HandleRef _this, int32_t x, int32_t y) {
        THIS->move(x, y);
    }

    void Handle_show(HandleRef _this) {
        THIS->show();
    }

    void Handle_hide(HandleRef _this) {
        THIS->hide();
    }

    void Handle_setVisible(HandleRef _this, bool state) {
        THIS->setVisible(state);
    }

    void Handle_update(HandleRef _this) {
        THIS->update();
    }

    void Handle_update(HandleRef _this, int32_t x, int32_t y, int32_t width, int32_t height) {
        THIS->update(x, y, width, height);
    }

    void Handle_update(HandleRef _this, Rect rect) {
        THIS->update(toQRect(rect));
    }

    void Handle_setWindowTitle(HandleRef _this, std::string title) {
        THIS->setWindowTitle(title.c_str());
    }

    void Handle_setWindowModality(HandleRef _this, WindowModality modality) {
        THIS->setWindowModality((Qt::WindowModality)modality);
    }

    void Handle_setLayout(HandleRef _this, Layout::HandleRef layout) {
        THIS->setLayout((QLayout*)layout);
    }

    Layout::HandleRef Handle_getLayout(HandleRef _this) {
        return (Layout::HandleRef)THIS->layout();
    }

    void Handle_setContextMenuPolicy(HandleRef _this, ContextMenuPolicy policy) {
        THIS->setContextMenuPolicy((Qt::ContextMenuPolicy)policy);
    }

    Point Handle_mapToGlobal(HandleRef _this, Point p) {
        auto p2 = THIS->mapToGlobal(toQPoint(p));
        return toPoint(p2);
    }

    void Handle_setUpdatesEnabled(HandleRef _this, bool enabled) {
        THIS->setUpdatesEnabled(enabled);
    }

    void Handle_setMouseTracking(HandleRef _this, bool enabled) {
        THIS->setMouseTracking(enabled);
    }

    void Handle_setAcceptDrops(HandleRef _this, bool enabled) {
        THIS->setAcceptDrops(enabled);
    }

    void Handle_setSignalMask(HandleRef _this, uint32_t mask) {
        THIS->setSignalMask(mask);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create(std::shared_ptr<SignalHandler> handler) {
        return (HandleRef) new WidgetWithHandler(std::move(handler));
    }

    // =========================================================================

#define EVENTTHIS ((QEvent*)_this)

    void Event_accept(EventRef _this) {
        EVENTTHIS->accept();
    }

    void Event_ignore(EventRef _this) {
        EVENTTHIS->ignore();
    }

//    void Event_dispose(EventRef _this) {
//        // not owned, do nothing
//    }

#define MIMETHIS ((QMimeData*)_this)

    std::vector<std::string> MimeData_formats(MimeDataRef _this) {
        std::vector<std::string> result;
        for (auto & fmt : MIMETHIS->formats()) {
            result.push_back(fmt.toStdString());
        }
        return result;
    }

    bool MimeData_hasFormat(MimeDataRef _this, std::string mimeType) {
        return MIMETHIS->hasFormat(mimeType.c_str());
    }

    std::string MimeData_text(MimeDataRef _this) {
        return MIMETHIS->text().toStdString();
    }

    void MimeData_setText(MimeDataRef _this, std::string text) {
        MIMETHIS->setText(text.c_str());
    }

    std::vector<std::string> MimeData_urls(MimeDataRef _this) {
        std::vector<std::string> result;
        for (auto & url : MIMETHIS->urls()) {
            result.push_back(url.toString().toStdString());
        }
        return result;
    }

    void MimeData_setUrls(MimeDataRef _this, std::vector<std::string> urls) {
        QList<QUrl> qUrls;
        for (auto & url : urls) {
            qUrls.append(QUrl(url.c_str()));
        }
        MIMETHIS->setUrls(qUrls);
    }

//    void MimeData_dispose(MimeDataRef _this) {
//        // if it was created on a drop, we're not responsible for it
//        // if we created it for a drag, we're also not responsible for releasing it
//    }

    MimeDataRef createMimeData() {
        return (MimeDataRef) new QMimeData();
    }

#define DRAGTHIS ((QDrag*)_this)

    void Drag_setMimeData(DragRef _this, MimeDataRef data) {
        DRAGTHIS->setMimeData((QMimeData*)data);
    }

    DropAction Drag_exec(DragRef _this, uint32_t supportedActions, DropAction defaultAction) {
        auto qDefault = (Qt::DropAction)defaultAction;
        auto qSupported = QFlags<Qt::DropAction>((int)supportedActions);
        return (DropAction) DRAGTHIS->exec(qSupported, qDefault);
    }

//    void Drag_dispose(DragRef _this) {
//        // we're not responsible for deleting these (if they are exec'ed)
//    }

    DragRef createDrag(HandleRef parent) {
        return (DragRef) new QDrag((QObject*)parent);
    }

#define DRAGMOVETHIS ((QDragMoveEvent*)_this)

    DropAction DragMoveEvent_proposedAction(DragMoveEventRef _this) {
        return (DropAction) DRAGMOVETHIS->proposedAction();
    }

    void DragMoveEvent_acceptProposedAction(DragMoveEventRef _this) {
        DRAGMOVETHIS->acceptProposedAction();
    }

    uint32_t DragMoveEvent_possibleActions(DragMoveEventRef _this) {
        return (uint32_t) DRAGMOVETHIS->possibleActions();
    }

    void DragMoveEvent_acceptDropAction(DragMoveEventRef _this, DropAction action) {
        auto qtAction = (Qt::DropAction)action;
        if (qtAction == DRAGMOVETHIS->proposedAction()) {
            DRAGMOVETHIS->acceptProposedAction();
        } else {
            if (DRAGMOVETHIS->possibleActions().testFlag(qtAction)) {
                DRAGMOVETHIS->setDropAction(qtAction);
                DRAGMOVETHIS->accept();
            } else {
                printf("DragMoveEvent_acceptDropAction: specified action was not in allowed set\n");
            }
        }
    }

//    void DragMoveEvent_dispose(DragMoveEventRef _this) {
//        // not owned
//    }

    // subclass stuff ==========================================================

    class WidgetSubclass : public WidgetWithHandler {
    private:
        std::shared_ptr<MethodDelegate> methodDelegate;
        uint32_t methodMask;
    public:
        WidgetSubclass(std::shared_ptr<MethodDelegate> &methodDelegate, uint32_t methodMask, std::shared_ptr<SignalHandler> handler) :
            methodDelegate(methodDelegate),
            methodMask(methodMask),
            WidgetWithHandler(std::move(handler))
            {}
    protected:
        void paintEvent(QPaintEvent *event) override {
            if (methodMask & MethodMask::PaintEvent) {
                QPainter painter(this);
                methodDelegate->paintEvent((Painter::HandleRef)&painter, toRect(event->rect()));
                // prevent it from propagating:
                // do we allow this from the method delegate somehow?
                event->accept();
            } else {
                QWidget::paintEvent(event);
            }
        }
        void mousePressEvent(QMouseEvent *event) override {
            if (methodMask & MethodMask::MousePressEvent) {
                auto pos = toPoint(event->pos());
                auto button = (MouseButton)event->button();
                auto modifiers = event->modifiers();
                methodDelegate->mousePressEvent(pos, button, modifiers);
                // prevent from propagating, see notes above
                event->accept();
            } else {
                QWidget::mousePressEvent(event);
            }
        }

        void mouseMoveEvent(QMouseEvent *event) override {
            if (methodMask & MethodMask::MouseMoveEvent) {
                auto pos = toPoint(event->pos());
                auto buttons = event->buttons();
                auto modifiers = event->modifiers();
                methodDelegate->mouseMoveEvent(pos, buttons, modifiers);
                // prevent from propagating, see notes above
                event->accept();
            } else {
                QWidget::mouseMoveEvent(event);
            }
        }

        void mouseReleaseEvent(QMouseEvent *event) override {
            if (methodMask & MethodMask::MouseReleaseEvent) {
                auto pos = toPoint(event->pos());
                auto button = (MouseButton)event->button();
                auto modifiers = event->modifiers();
                methodDelegate->mouseReleaseEvent(pos, button, modifiers);
                event->accept();
            } else {
                QWidget::mouseReleaseEvent(event);
            }
        }

        void enterEvent(QEnterEvent *event) override {
            if (methodMask & MethodMask::EnterEvent) {
                auto pos = toPoint(event->position().toPoint());
                methodDelegate->enterEvent(pos);
                event->accept();
            } else {
                QWidget::enterEvent(event);
            }
        }

        void leaveEvent(QEvent *event) override {
            if (methodMask & MethodMask::LeaveEvent) {
                methodDelegate->leaveEvent();
                event->accept();
            } else {
                QWidget::leaveEvent(event);
            }
        }

        void resizeEvent(QResizeEvent *event) override {
            if (methodMask & MethodMask::ResizeEvent) {
                methodDelegate->resizeEvent(toSize(event->oldSize()), toSize(event->size()));
                event->accept();
            } else {
                QWidget::resizeEvent(event);
            }
        }

        void dragEnterEvent(QDragEnterEvent *event) override {
            if (methodMask & MethodMask::DropEvents) {
                auto pos = toPoint(event->position().toPoint());
                auto modifiers = event->modifiers();
                auto mimeOpaque = (MimeDataRef)event->mimeData();
                auto moveEvent = (DragMoveEventRef)event;
                methodDelegate->dragMoveEvent(pos, modifiers, mimeOpaque, moveEvent, true);
                // other side needs to call acceptProposedAction if it's OK, otherwise ... .ignore?
            } else {
                QWidget::dragEnterEvent(event);
            }
        }

        void dragMoveEvent(QDragMoveEvent *event) override {
            if (methodMask & MethodMask::DropEvents) {
                auto pos = toPoint(event->position().toPoint());
                auto modifiers = event->modifiers();
                auto mimeOpaque = (MimeDataRef)event->mimeData();
                auto moveEvent = (DragMoveEventRef)event;
                methodDelegate->dragMoveEvent(pos, modifiers, mimeOpaque, moveEvent, false);
                // other side needs to call acceptProposedAction if it's OK, otherwise ... .ignore?
            } else {
                QWidget::dragMoveEvent(event);
            }
        }

        void dragLeaveEvent(QDragLeaveEvent *event) override {
            if (methodMask & MethodMask::DropEvents) {
                methodDelegate->dragLeaveEvent();
                event->accept();
            } else {
                QWidget::dragLeaveEvent(event);
            }
        }

        void dropEvent(QDropEvent *event) override {
            if (methodMask & MethodMask::DropEvents) {
                auto pos = toPoint(event->position().toPoint());
                auto modifiers = event->modifiers();
                auto mimeOpaque = (MimeDataRef)event->mimeData();
                auto action = (DropAction)event->proposedAction();
                methodDelegate->dropEvent(pos, modifiers, mimeOpaque, action);
                event->acceptProposedAction();
            } else {
                QWidget::dropEvent(event);
            }
        }

    public:
        [[nodiscard]] QSize sizeHint() const override {
            if (methodMask & MethodMask::SizeHint) {
                auto size = methodDelegate->sizeHint();
                return toQSize(size);
            } else {
                return QWidget::sizeHint();
            }
        }
    };

    HandleRef createSubclassed(std::shared_ptr<MethodDelegate> methodDelegate, uint32_t methodMask, std::shared_ptr<SignalHandler> handler) {
        return (HandleRef) new WidgetSubclass(methodDelegate, methodMask, std::move(handler));
    }
}

#include "Widget.moc"
