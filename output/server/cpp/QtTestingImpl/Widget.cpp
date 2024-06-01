#include "generated/Widget.h"

#include <QObject>
#include <QWidget>
#include <QLayout>

#include <QPainter>
#include <QPaintEvent>

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

    void Handle_resize(HandleRef _this, int32_t width, int32_t height) {
        THIS->resize(width, height);
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

    void Handle_setUpdatesEnabled(HandleRef _this, bool enabled) {
        THIS->setUpdatesEnabled(enabled);
    }

    void Handle_setMouseTracking(HandleRef _this, bool enabled) {
        THIS->setMouseTracking(enabled);
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

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create() {
        return (HandleRef)new QWidget();
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
    };

    HandleRef createSubclassed(std::shared_ptr<MethodDelegate> methodDelegate, uint32_t methodMask) {
        return (HandleRef) new WidgetSubclass(methodDelegate, methodMask);
    }
}
