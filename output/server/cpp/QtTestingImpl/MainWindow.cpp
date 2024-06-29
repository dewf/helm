#include "generated/MainWindow.h"

#include <QMainWindow>
#include <utility>

#include "util/SignalStuff.h"
#include "util/convert.h"

#include "IconInternal.h"

#define THIS ((MainWindowWithHandler*)_this)

namespace MainWindow
{
    class MainWindowWithHandler : public QMainWindow {
        Q_OBJECT
    private:
        std::shared_ptr<SignalHandler> handler;
        SignalMask lastMask = 0;
        std::vector<SignalMapItem<SignalMaskFlags>> signalMap = {
            { SignalMaskFlags::CustomContextMenuRequested, SIGNAL(customContextMenuRequested(QPoint)), SLOT(onCustomContextMenuRequested(QPoint)) },
            { SignalMaskFlags::WindowIconChanged, SIGNAL(windowIconChanged(QIcon)), SLOT(onWindowIconChanged(QIcon)) },
            { SignalMaskFlags::WindowTitleChanged, SIGNAL(windowTitleChanged(QString)), SLOT(onWindowTitleChanged(QString)) },
            { SignalMaskFlags::IconSizeChanged, SIGNAL(iconSizeChanged(QSize)), SLOT(onIconSizeChanged(QSize)) },
            { SignalMaskFlags::TabifiedDockWidgetActivated, SIGNAL(tabifiedDockWidgetActivated(QDockWidget)), SLOT(onTabifiedDockWidgetActivated(QDockWidget)) },
            { SignalMaskFlags::ToolButtonStyleChanged, SIGNAL(toolButtonStyleChanged(Qt::ToolButtonStyle)), SLOT(onToolButtonStyleChanged(Qt::ToolButtonStyle)) },
            { SignalMaskFlags::Closed, SIGNAL(windowClosed()), SLOT(onWindowClosed()) },
        };
    public:
        explicit MainWindowWithHandler(std::shared_ptr<SignalHandler> handler) : handler(std::move(handler)) {}
        void setSignalMask(SignalMask newMask) {
            if (newMask != lastMask) {
                processChanges(lastMask, newMask, signalMap, this);
                lastMask = newMask;
            }
        }
    signals:
        void windowClosed();
    protected:
        // something we're implementing ourselves since it's not a stock signal
        void closeEvent(QCloseEvent *event) override {
            QWidget::closeEvent(event);
            emit windowClosed();
        }
    public slots:
        // QWidget:
        void onCustomContextMenuRequested(const QPoint& pos) {
            handler->customContextMenuRequested(toPoint(pos));
        };
        void onWindowIconChanged(const QIcon& icon) {
            handler->windowIconChanged((Icon::HandleRef)&icon);
        }
        void onWindowTitleChanged(const QString& title) {
            handler->windowTitleChanged(title.toStdString());
        }
        // QMainWindow:
        void onIconSizeChanged(const QSize& size) {
            handler->iconSizeChanged(toSize(size));
        }
        void onTabifiedDockWidgetActivated(QDockWidget* dockWidget) {
            handler->tabifiedDockWidgetActivated((DockWidget::HandleRef)dockWidget);
        }
        void onToolButtonStyleChanged(Qt::ToolButtonStyle style) {
            handler->toolButtonStyleChanged((Enums::ToolButtonStyle)style);
        }
        // custom:
        void onWindowClosed() {
            handler->closed();
        }
    };

    void Handle_setCentralWidget(HandleRef _this, Widget::HandleRef widget) {
        THIS->setCentralWidget((QWidget*)widget);
    }

    void Handle_setMenuBar(HandleRef _this, MenuBar::HandleRef menubar) {
        THIS->setMenuBar((QMenuBar*)menubar);
    }

    void Handle_setStatusBar(HandleRef _this, StatusBar::HandleRef statusbar) {
        THIS->setStatusBar((QStatusBar*)statusbar);
    }

    void Handle_addToolBar(HandleRef _this, ToolBar::HandleRef toolbar) {
        THIS->addToolBar((QToolBar*)toolbar);
    }

    void Handle_setSignalMask(HandleRef _this, SignalMask mask) {
        THIS->setSignalMask(mask);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create(std::shared_ptr<SignalHandler> handler) {
        return (HandleRef) new MainWindowWithHandler(std::move(handler));
    }
}

#include "MainWindow.moc"
