using System.Configuration;
using System.Data;
using System.Windows;

using TaskManagerWPF;

public partial class App : Application
{
    public App()
    {
        DatabaseInitializer.InitializeDatabase();
    }
}