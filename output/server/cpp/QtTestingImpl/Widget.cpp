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

    void Handle_setUpdatesEnabled(HandleRef _this, bool state) {
        THIS->setUpdatesEnabled(state);
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
            } else {
                QWidget::paintEvent(event);
            }
        }
        void mousePressEvent(QMouseEvent *event) override {
            if (methodMask & MethodMask::MousePressEvent) {
                MouseEvent ev {};
                ev.pos.x = event->pos().x();
                ev.pos.y = event->pos().y();
                methodDelegate->mousePressEvent(ev);
            } else {
                QWidget::mousePressEvent(event);
            }
        }
    };

    HandleRef createSubclassed(std::shared_ptr<MethodDelegate> methodDelegate, uint32_t methodMask) {
        return (HandleRef) new WidgetSubclass(methodDelegate, methodMask);
    }
}
