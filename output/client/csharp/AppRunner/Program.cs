using Org.Whatever.QtTesting;
using Action = Org.Whatever.QtTesting.Action;

namespace AppRunner;

internal static class Program
{
    class WindowHandler : MainWindow.SignalHandler
    {
        public void Dispose()
        {
            Console.WriteLine("WindowHandler DISPOSE");
        }

        public void CustomContextMenuRequested(Common.Point pos)
        {
            throw new NotImplementedException();
        }

        public void WindowIconChanged(Icon.Handle icon)
        {
            throw new NotImplementedException();
        }

        public void WindowTitleChanged(string title)
        {
            throw new NotImplementedException();
        }

        public void IconSizeChanged(Common.Size iconSize)
        {
            throw new NotImplementedException();
        }

        public void TabifiedDockWidgetActivated(DockWidget.Handle dockWidget)
        {
            throw new NotImplementedException();
        }

        public void ToolButtonStyleChanged(Enums.ToolButtonStyle style)
        {
            throw new NotImplementedException();
        }

        public void Closed()
        {
            throw new NotImplementedException();
        }
    }

    class ToolBarHandler : ToolBar.SignalHandler
    {
        public void Dispose()
        {
            Console.WriteLine("ToolBarHandler DISPOSE");
        }
        
        public void ActionTriggered(Action.Handle action)
        {
            throw new NotImplementedException();
        }

        public void AllowedAreasChanged(Enums.ToolBarAreas allowed)
        {
            throw new NotImplementedException();
        }

        public void IconSizeChanged(Common.Size size)
        {
            throw new NotImplementedException();
        }

        public void MovableChanged(bool movable)
        {
            throw new NotImplementedException();
        }

        public void OrientationChanged(Enums.Orientation value)
        {
            throw new NotImplementedException();
        }

        public void ToolButtonStyleChanged(Enums.ToolButtonStyle style)
        {
            throw new NotImplementedException();
        }

        public void TopLevelChanged(bool topLevel)
        {
            throw new NotImplementedException();
        }

        public void VisibilityChanged(bool visible)
        {
            throw new NotImplementedException();
        }
    }

    class ActionHandler : Action.SignalHandler
    {
        public void Dispose()
        {
            Console.WriteLine("ActionHandler DISPOSE");
        }
        
        public void Changed()
        {
            throw new NotImplementedException();
        }

        public void CheckableChanged(bool checkable)
        {
            throw new NotImplementedException();
        }

        public void EnabledChanged(bool enabled)
        {
            throw new NotImplementedException();
        }

        public void Hovered()
        {
            throw new NotImplementedException();
        }

        public void Toggled(bool checked_)
        {
            throw new NotImplementedException();
        }

        public void Triggered(bool checked_)
        {
            throw new NotImplementedException();
        }

        public void VisibleChanged()
        {
            throw new NotImplementedException();
        }
    }
    
    [STAThread]
    private static void Main(string[] args)
    {
        Library.Init();
        
        using (var app = Application.Create(args))
        {
            using (var windowHandler = new WindowHandler())
            using (var toolbarHandler = new ToolBarHandler())
            using (var actionHandler = new ActionHandler())
            {
                using (var window = MainWindow.Create(windowHandler))
                {
                    window.SetWindowTitle("Haloooo");
                    window.Resize(640, 480);

                    var action01 = Action.Create(window, actionHandler);

                    var tb = ToolBar.Create(toolbarHandler);
                    tb.AddAction(action01);
                    
                    window.AddToolBar(tb);
            
                    window.Show();
                
                    // runloop, before window destroyed plz
                    Application.Exec();
                }
            }
        }
        
        Library.DumpTables(); // check to make sure we're not leaking anything
        Library.Shutdown();
    }
}
