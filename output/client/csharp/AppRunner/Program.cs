using Org.Whatever.QtTesting;

namespace AppRunner;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        Library.Init();
        
        using (var app = Application.Create(args))
        {
            app.SetStyle("Fusion");
            
            using (var window = Widget.Create())
            {
                window.SetWindowTitle("Haloooo");
                window.Resize(800, 600);

                var layout = BoxLayout.Create(BoxLayout.Direction.TopToBottom);

                var list = ListWidget.Create();
                list.SetSelectionMode(ListWidget.SelectionMode.Extended);
                list.SetItems(Enumerable.Range(0, 100).Select(i => $"Item {i}").ToArray());
                layout.AddWidget(list);

                var combo1 = ComboBox.Create();
                combo1.SetItems([ "one", "two", "three", "four", "five", "six"]);
                layout.AddWidget(combo1);

                var edit1 = PlainTextEdit.Create();
                layout.AddWidget(edit1);
                
                var button1 = PushButton.Create($"Wahoo!! {Widget.WIDGET_SIZE_MAX}");
                button1.SetMaximumHeight(300);
                layout.AddWidget(button1, 1);
                
                var button2 = PushButton.Create("#2 !!!");
                button2.SetMaximumHeight(300);
                layout.AddWidget(button2, 1);
                
                // layout.AddStretch(1);
                
                window.SetLayout(layout);
                
                window.Show();
                
                // runloop, before window destroyed plz
                app.Exec();
            }
        }
        
        Library.DumpTables(); // check to make sure we're not leaking anything
        Library.Shutdown();
    }
}
