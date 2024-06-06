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
            foreach (var style in Application.AvailableStyles())
            {
                Console.WriteLine($"style: {style}");
            }
            // Application.SetStyle("Windows");
            
            using (var window = MainWindow.Create())
            {
                window.SetWindowTitle("Haloooo");
                window.Resize(640, 480);

                var vbox = BoxLayout.Create();
                vbox.SetDirection(BoxLayout.Direction.TopToBottom);
                
                var radio1 = RadioButton.Create();
                radio1.SetText("Cool beans 1");
                radio1.SetChecked(true);
                vbox.AddWidget(radio1);

                var radio2 = RadioButton.Create();
                radio2.SetText("Secondary Item OK");
                vbox.AddWidget(radio2);

                var button1 = PushButton.Create();
                button1.SetText("Errhghgh");
                button1.SetCheckable(true);
                vbox.AddWidget(button1);
                
                vbox.AddStretch(1);
                
                var group = GroupBox.Create();
                group.SetTitle("My Cool Group");
                group.SetLayout(vbox);

                var outerLayout = BoxLayout.Create();
                outerLayout.SetDirection(BoxLayout.Direction.TopToBottom);
                outerLayout.AddWidget(group);

                var w = Widget.Create();
                w.SetLayout(outerLayout);
                
                window.SetCentralWidget(w);
                window.Show();
                
                // runloop, before window destroyed plz
                Application.Exec();
            }
        }
        
        Library.DumpTables(); // check to make sure we're not leaking anything
        Library.Shutdown();
    }
}
