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
            using (var window = Widget.Create())
            {
                window.SetWindowTitle("Haloooo");
                window.Resize(800, 600);

                var layout = BoxLayout.Create(BoxLayout.Direction.TopToBottom);

                var edit1 = LineEdit.Create();
                edit1.OnReturnPressed(() =>
                {
                    Console.WriteLine("return pressed!!!");
                    edit1.SetText("");
                });
                layout.AddWidget(edit1);
                
                var button1 = PushButton.Create("Wahoo!!");
                layout.AddWidget(button1);
                
                var button2 = PushButton.Create("#2 !!!");
                layout.AddWidget(button2);
                
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
