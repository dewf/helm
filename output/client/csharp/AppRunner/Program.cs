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
            using (var window = MainWindow.Create())
            {
                window.SetWindowTitle("Haloooo");
                window.Resize(640, 480);

                var area = ScrollArea.Create();
                
                var button = PushButton.Create();
                button.SetText("BIG BUTTON");
                button.SetFixedSize(1024, 1024);
                area.SetWidget(button);
                
                window.SetCentralWidget(area);
                window.Show();
                
                // runloop, before window destroyed plz
                Application.Exec();
            }
        }
        
        Library.DumpTables(); // check to make sure we're not leaking anything
        Library.Shutdown();
    }
}
