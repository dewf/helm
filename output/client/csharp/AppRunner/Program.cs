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
            // Application.Exec();
            
//             QObject::connect(button, &QPushButton::clicked, button, [](bool checked){
//                 QFileDialog dialog;
// //        dialog.setOptions(QFileDialog::DontUseNativeDialog);
//                 dialog.setFileMode(QFileDialog::ExistingFile);
//                 dialog.setNameFilter("Images (*.png *.xpm *.jpg)");
//                 dialog.setViewMode(QFileDialog::Detail);
//                 if (dialog.exec()) {
//                     printf("it was good!\n");
//                 }
//             });

            // using (var window = MainWindow.Create())
            // {
            //     window.SetWindowTitle("Haloooo");
            //     // window.Resize(640, 480);
            //
            //     // var area = ScrollArea.Create();
            //     
            //     var button = PushButton.Create(new Handler());
            //     button.SetSignalMask(PushButton.SignalMask.Pressed | PushButton.SignalMask.Released | PushButton.SignalMask.Clicked);
            //     button.SetText("Click ME");
            //
            //     var button2 = PushButton.Create(new Handler2(button));
            //     button2.SetSignalMask(PushButton.SignalMask.Clicked);
            //     button2.SetText("click to disable #1");
            //
            //     var vbox = BoxLayout.Create();
            //     vbox.SetDirection(BoxLayout.Direction.TopToBottom);
            //     vbox.AddWidget(button);
            //     vbox.AddWidget(button2);
            //
            //     var w = Widget.Create();
            //     w.SetLayout(vbox);
            //     
            //     window.SetCentralWidget(w);
            //     window.Show();
            //     
            //     // runloop, before window destroyed plz
            //     Application.Exec();
            // }
        }
        
        Library.DumpTables(); // check to make sure we're not leaking anything
        Library.Shutdown();
    }
}
