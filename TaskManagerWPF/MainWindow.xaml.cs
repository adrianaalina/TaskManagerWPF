using System.Text;
using System.Windows;
using System.Data;
using System.IO;
using System.Data.Common;
using System.Data.SQLite;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaskManagerWPF.Models;
using TaskManagerWPF.ViewModels;
using TaskManagerWPF.Data;
using System.Collections.ObjectModel;


namespace TaskManagerWPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new TaskViewModel();
    }

    private void StergeTask_Click(object sender, RoutedEventArgs e)
        {
            if (taskuri.SelectedItem is not TaskModel taskSelectat)
            {
                MessageBox.Show("Selecteaza un task pentru a-l sterge.");
                return;
            }

            var rezultat = MessageBox.Show(
                $"Sigur vrei sa stergi task-ul \"{taskSelectat.Titlu}\"?",
                "Confirmare",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (rezultat == MessageBoxResult.Yes)
            {
                ((TaskViewModel)DataContext).StergeTask(taskSelectat.Id);
            }
        }
    
    
  
}