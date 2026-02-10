using System.Configuration;
using System.Data;
using System.Windows;
using TaskManagerWPF;


namespace TaskManagerWPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

           
            DatabaseInitializer.InitializeDatabase();

          
            MainWindow window = new MainWindow();
            window.Show();
        }
    }
}
