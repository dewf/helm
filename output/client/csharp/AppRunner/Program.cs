using Org.Whatever.QtTesting;
using Action = Org.Whatever.QtTesting.Action;

namespace AppRunner;

internal static class Program
{
    private static Widget.Handle CreatePage01()
    {
        var page = Widget.Create();
        var layout = BoxLayout.Create(BoxLayout.Direction.TopToBottom);
        
        var list = ListWidget.Create();
        list.SetSelectionMode(ListWidget.SelectionMode.Extended);
        list.SetItems(Enumerable.Range(0, 100).Select(i => $"Item {i}").ToArray());
        layout.AddWidget(list);
        
        page.SetLayout(layout);
        return page;
    }

    private static Widget.Handle CreatePage02()
    {
        var page = Widget.Create();
        var layout = BoxLayout.Create(BoxLayout.Direction.TopToBottom);
        
        var combo1 = ComboBox.Create();
        combo1.SetItems([ "one", "two", "three", "four", "five", "six"]);
        layout.AddWidget(combo1);

        var edit1 = PlainTextEdit.Create();
        layout.AddWidget(edit1);
                
        var button1 = PushButton.Create($"Wahoo!! {Widget.WIDGET_SIZE_MAX}");
        button1.SetMaximumHeight(300);
        layout.AddWidget(button1, 1);
        
        page.SetLayout(layout);
        return page;
    }

    private static Widget.Handle CreatePage03()
    {
        var page = Widget.Create();
        var layout = BoxLayout.Create(BoxLayout.Direction.TopToBottom);
        
        var button2 = PushButton.Create("#2 !!!");
        button2.SetMaximumHeight(Widget.WIDGET_SIZE_MAX - 1);
        layout.AddWidget(button2);
        
        page.SetLayout(layout);
        return page;
    }
    
    [STAThread]
    private static void Main(string[] args)
    {
        Library.Init();
        
        using (var app = Application.Create(args))
        {
            Application.SetStyle("Fusion");
            
            using (var window = MainWindow.Create())
            {
                window.SetWindowTitle("Haloooo");
                window.Resize(800, 600);

                var central = Widget.Create();
                // var layout = BoxLayout.Create(BoxLayout.Direction.TopToBottom);

                var tabs = TabWidget.Create();
                tabs.AddTab(CreatePage01(), "Page 1");
                tabs.AddTab(CreatePage02(), "Page 2");
                tabs.AddTab(CreatePage03(), "Page 3");
                // layout.AddWidget(tabs);
                //
                // central.SetLayout(layout);
                window.SetCentralWidget(tabs);

                var action = Action.Create("E&xit");
                action.OnTriggered(_ =>
                {
                    Console.WriteLine("Exit triggered!");
                    Application.Quit();
                });
                
                var menu = Menu.Create("&File");
                menu.AddAction(action);
                var menuBar = MenuBar.Create();
                menuBar.AddMenu(menu);
                window.SetMenuBar(menuBar);
                
                window.Show();
                
                // runloop, before window destroyed plz
                Application.Exec();
            }
        }
        
        Library.DumpTables(); // check to make sure we're not leaking anything
        Library.Shutdown();
    }
}
