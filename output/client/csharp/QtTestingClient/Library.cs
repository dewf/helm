using Org.Whatever.QtTesting.Support;

namespace Org.Whatever.QtTesting;

public static class Library
{
    public static void Init()
    {
        if (NativeImplClient.Init() != 0)
        {
            Console.WriteLine("NativeImplClient.Init failed");
            return;
        }
        // registrations, static module inits
        Application.__Init();
        Common.__Init();
        Signal.__Init();
        Layout.__Init();
        Widget.__Init();
        BoxLayout.__Init();
        ComboBox.__Init();
        LineEdit.__Init();
        ListWidget.__Init();
        MainWindow.__Init();
        PlainTextEdit.__Init();
        PushButton.__Init();
    }

    public static void Shutdown()
    {
        // module static shutdowns (if any, might be empty)
        PushButton.__Shutdown();
        PlainTextEdit.__Shutdown();
        MainWindow.__Shutdown();
        ListWidget.__Shutdown();
        LineEdit.__Shutdown();
        ComboBox.__Shutdown();
        BoxLayout.__Shutdown();
        Widget.__Shutdown();
        Layout.__Shutdown();
        Signal.__Shutdown();
        Common.__Shutdown();
        Application.__Shutdown();
        // bye
        NativeImplClient.Shutdown();
    }

    public static void DumpTables()
    {
        NativeImplClient.DumpTables();
    }
}
