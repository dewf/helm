#include "generated/MainWindow.h"

#include <QMainWindow>

#define THIS ((MainWindow2*)_this)

namespace MainWindow
{
    class MainWindow2 : public QMainWindow {
    private:
        std::function<VoidDelegate> closeEventHandler;
    public:
        MainWindow2() : QMainWindow() {}
        void setCloseHandler(const std::function<VoidDelegate>& handler) {
            closeEventHandler = handler;
        }
    protected:
        void closeEvent(QCloseEvent *event) override {
            QWidget::closeEvent(event);
            if (closeEventHandler) {
                closeEventHandler();
            }
        }
    };

    void Handle_setCentralWidget(HandleRef _this, Widget::HandleRef widget) {
        THIS->setCentralWidget((QWidget*)widget);
    }

    void Handle_setMenuBar(HandleRef _this, MenuBar::HandleRef menubar) {
        THIS->setMenuBar((QMenuBar*)menubar);
    }

    void Handle_onClosed(HandleRef _this, std::function<VoidDelegate> handler) {
        THIS->setCloseHandler(handler);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create() {
        return (HandleRef)new MainWindow2();
    }
}
