#include "generated/Widget.h"

#include <QObject>
#include <QWidget>
#include <QLayout>

#include <QPainter>
#include <QPaintEvent>

#include <QMimeData>
#include <QDrag>

#include "util/convert.h"

#define THIS ((QWidget*)_this)

namespace Widget
{
    const int32_t WIDGET_SIZE_MAX = QWIDGETSIZE_MAX;

    void Handle_setParent(HandleRef _this, HandleRef parent) {
        THIS->setParent((QWidget*)parent);
    }

    HandleRef Handle_getWindow(HandleRef _this) {
        return (HandleRef)THIS->window();
    }

    void Handle_setEnabled(HandleRef _this, bool state) {
        THIS->setEnabled(state);
    }

    void Handle_setMaximumWidth(HandleRef _this, int32_t maxWidth) {
        THIS->setMaximumWidth(maxWidth);
    }

    void Handle_setMaximumHeight(HandleRef _this, int32_t maxHeight) {
        THIS->setMaximumHeight(maxHeight);
    }

    Rect Handle_getRect(HandleRef _this) {
        auto x = THIS->rect();
        return toRect(x);
    }

    Size Handle_getSize(HandleRef _this) {
        return toSize(THIS->size());
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

    void Handle_onWindowTitleChanged(HandleRef _this, std::function<StringDelegate> func) {
        QObject::connect(
            THIS,
            &QWidget::windowTitleChanged,
            THIS,
            [func](const QString& title) {
                func(title.toStdString());
            });
    }

    void Handle_onCustomContextMenuRequested(HandleRef _this, std::function<PointDelegate> func) {
        QObject::connect(
                THIS,
                &QWidget::customContextMenuRequested,
                THIS,
                [func](const QPoint& point) {
                    func(toPoint(point));
                });
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create() {
        return (HandleRef)new QWidget();
    }

    // =========================================================================

#define EVENTTHIS ((QEvent*)_this)

    void Event_accept(EventRef _this) {
        EVENTTHIS->accept();
    }

    void Event_ignore(EventRef _this) {
        EVENTTHIS->ignore();
    }

    void Event_dispose(EventRef _this) {
        // not owned, do nothing
    }

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

    void MimeData_dispose(MimeDataRef _this) {
        // if it was created on a drop, we're not responsible for it
        // if we created it for a drag, we're also not responsible for releasing it
    }

    MimeDataRef createMimeData() {
        return (MimeDataRef) new QMimeData();
    }

#define DRAGTHIS ((QDrag*)_this)

    inline QFlags<Qt::DropAction> toQDropActions(const std::set<DropAction>& supportedActions) {
        QFlags<Qt::DropAction> result;
        if (supportedActions.contains(DropAction::Copy)) {
            result.setFlag(Qt::CopyAction);
        }
        if (supportedActions.contains(DropAction::Move)) {
            result.setFlag(Qt::MoveAction);
        }
        if (supportedActions.contains(DropAction::Link)) {
            result.setFlag(Qt::LinkAction);
        }
        if (supportedActions.contains(DropAction::TargetMoveAction)) {
            result.setFlag(Qt::TargetMoveAction);
        }
        return result;
    }

    void Drag_setMimeData(DragRef _this, MimeDataRef data) {
        DRAGTHIS->setMimeData((QMimeData*)data);
    }

    DropAction Drag_exec(DragRef _this, std::set<DropAction> supportedActions, DropAction defaultAction) {
        auto qDefault = (Qt::DropAction)defaultAction;
        QFlags<Qt::DropAction> qSupported = toQDropActions(supportedActions);
        return (DropAction) DRAGTHIS->exec(qSupported, qDefault);
    }

    void Drag_dispose(DragRef _this) {
        // we're not responsible for deleting these (if they are exec'ed)
    }

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

    std::set<DropAction> DragMoveEvent_possibleActions(DragMoveEventRef _this) {
        std::set<DropAction> result;
        auto possible = DRAGMOVETHIS->possibleActions();
        if (possible.testFlag(Qt::CopyAction)) {
            result.emplace(DropAction::Copy);
        }
        if (possible.testFlag(Qt::MoveAction)) {
            result.emplace(DropAction::Move);
        }
        if (possible.testFlag(Qt::LinkAction)) {
            result.emplace(DropAction::Link);
        }
        if (possible.testFlag(Qt::TargetMoveAction)) {
            result.emplace(DropAction::TargetMoveAction);
        }
        return result;
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

    void DragMoveEvent_dispose(DragMoveEventRef _this) {
        // not owned
    }

    // subclass stuff ==========================================================

    std::set<Modifier> fromQtModifiers (Qt::KeyboardModifiers modifiers) {
        std::set<Modifier> ret;
        if (modifiers.testFlag(Qt::ShiftModifier)) {
            ret.emplace(Modifier::Shift);
        }
        if (modifiers.testFlag(Qt::ControlModifier)) {
            ret.emplace(Modifier::Control);
        }
        if (modifiers.testFlag(Qt::AltModifier)) {
            ret.emplace(Modifier::Alt);
        }
        if (modifiers.testFlag(Qt::MetaModifier)) {
            ret.emplace(Modifier::Meta);
        }
        return ret;
    }

    MouseButton fromQtButton(Qt::MouseButton button) {
        switch (button) {
            case Qt::NoButton:
                return MouseButton::None;
            case Qt::LeftButton:
                return MouseButton::Left;
            case Qt::RightButton:
                return MouseButton::Right;
            case Qt::MiddleButton:
                return MouseButton::Middle;
            default:
                return MouseButton::Other;
        }
    }

    std::set<MouseButton> fromQtButtons(Qt::MouseButtons buttons) {
        std::set<MouseButton> ret;
        if (buttons.testFlag(Qt::LeftButton)) {
            ret.emplace(MouseButton::Left);
        }
        if (buttons.testFlag(Qt::RightButton)) {
            ret.emplace(MouseButton::Right);
        }
        if (buttons.testFlag(Qt::MiddleButton)) {
            ret.emplace(MouseButton::Middle);
        }
        return ret;
    }

    class WidgetSubclass : public QWidget {
    private:
        std::shared_ptr<MethodDelegate> methodDelegate;
        uint32_t methodMask;
    public:
        WidgetSubclass(std::shared_ptr<MethodDelegate> &methodDelegate, uint32_t methodMask) :
            methodDelegate(methodDelegate),
            methodMask(methodMask) {}
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
                auto button = fromQtButton(event->button());
                auto modifiers = fromQtModifiers(event->modifiers());
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
                auto buttons = fromQtButtons(event->buttons());
                auto modifiers = fromQtModifiers(event->modifiers());
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
                auto button = fromQtButton(event->button());
                auto modifiers = fromQtModifiers(event->modifiers());
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

        void dragEnterEvent(QDragEnterEvent *event) override {
            if (methodMask & MethodMask::DropEvents) {
                auto pos = toPoint(event->position().toPoint());
                auto modifiers = fromQtModifiers(event->modifiers());
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
                auto modifiers = fromQtModifiers(event->modifiers());
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
                auto modifiers = fromQtModifiers(event->modifiers());
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

    HandleRef createSubclassed(std::shared_ptr<MethodDelegate> methodDelegate, uint32_t methodMask) {
        return (HandleRef) new WidgetSubclass(methodDelegate, methodMask);
    }
}
