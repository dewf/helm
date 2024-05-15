#include "generated/Widgets.h"

#include <QApplication>
#include <QWidget>
#include <QPaintEvent>
#include <QMouseEvent>
#include <QPainter>

namespace Widgets
{
    namespace Application {
        ApplicationRef create(std::vector<std::string> args) {
            auto argc = (int)args.size();
            auto argv = new char*[argc];
            for (int i = 0; i< argc; i++) {
                argv[i] = const_cast<char*>(args[i].c_str());
            }
            auto ret = new QApplication(argc, argv);
            delete[] argv;
            return (ApplicationRef)ret;
        }
    }

    int32_t Application_exec(ApplicationRef _this) {
        return ((QApplication*)_this)->exec();
    }

    void Application_dispose(ApplicationRef _this) {
        delete (QApplication*)_this;
    }

    struct __Color {
        QColor qColor;
    };

    namespace Color {
        static __Color black { QColorConstants::Black };
        static __Color white { QColorConstants::White };
        static __Color red { QColorConstants::Red };

        ColorRef getConstant(Constant name) {
            switch (name) {
            case Constant::Black:
                return &black;
            case Constant::White:
                return &white;
            case Constant::Red:
                return &red;
            }
            return (ColorRef)nullptr;
        }
    }

    void Color_dispose(ColorRef _this) {
        // not owned by client
    }

    namespace Brush {
        BrushRef create(ColorRef color) {
            return (BrushRef)new QBrush(color->qColor);
        }
    }

    void Brush_dispose(BrushRef _this) {
        delete (QBrush*)_this;
    }

    static Rect qRectToRect(const QRect& qRect) {
        return { qRect.left(), qRect.top(), qRect.width(), qRect.height() };
    }

    static QRect rectToQRect(Rect r) {
        return QRect(r.x, r.y, r.width, r.height);
    }

    void Painter_fillRect(PainterRef _this, Rect r, BrushRef brush) {
        auto b2 = *((QBrush*)brush);
        ((QPainter*)_this)->fillRect(rectToQRect(r), b2);
    }

    void Painter_dispose(PainterRef _this) {
        // not owned by client
    }

    namespace Widget {
        WidgetRef create() {
            return (WidgetRef)new QWidget();
        }

        class WidgetSubclass : public QWidget {
        private:
            std::shared_ptr<MethodDelegate> methodDelegate;
            uint32_t methodMask;
        public:
            WidgetSubclass(std::function<CreateFunc> createFunc, uint32_t methodMask) : methodMask(methodMask) {
                // create the method delegate by injecting the 'this' pointer
                methodDelegate = createFunc((WidgetRef)this);
            }
        protected:
            void paintEvent(QPaintEvent *event) override {
                if (methodMask & MethodMask::PaintEvent) {
                    QPainter painter(this);
                    auto rect = qRectToRect(event->rect());
                    methodDelegate->paintEvent((PainterRef)&painter, rect);
                } else {
                    QWidget::paintEvent(event);
                }
            }
            void mousePressEvent(QMouseEvent *event) override {
                if (methodMask & MethodMask::MousePressEvent) {
                    MouseEvent ev;
                    ev.pos.x = event->pos().x();
                    ev.pos.y = event->pos().y();
                    methodDelegate->mousePressEvent(ev);
                } else {
                    QWidget::mousePressEvent(event);
                }
            }
        };

        WidgetRef createSubclassed(std::function<CreateFunc> createFunc, uint32_t methodMask) {
            return (WidgetRef)new WidgetSubclass(createFunc, methodMask);
        }
    }

    Rect Widget_getRect(WidgetRef _this)
    {
        return qRectToRect(((QWidget*)_this)->rect());
    }

    void Widget_resize(WidgetRef _this, int32_t width, int32_t height)
    {
        ((QWidget*)_this)->resize(width, height);
    }

    void Widget_show(WidgetRef _this)
    {
        ((QWidget*)_this)->show();
    }

    void Widget_setWindowTitle(WidgetRef _this, std::string title)
    {
        ((QWidget*)_this)->setWindowTitle(title.c_str());
    }

    void Widget_onWindowTitleChanged(WidgetRef _this, std::function<Widget::StringSignalDelegate> func)
    {
        auto thisWidget = ((QWidget*)_this);
        QObject::connect(thisWidget, &QWidget::windowTitleChanged, thisWidget, [func](const QString& title) {
            func(title.toStdString());
        });
    }

    void Widget_dispose(WidgetRef _this)
    {
        delete (QWidget*)_this;
    }
}
