using System.Text;
using System.Windows;
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

namespace TaskManagerWPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private TaskViewModel _viewModel;
    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new TaskViewModel();
        DataContext = _viewModel;
    }

    private void StergeTask_Click(object sender, RoutedEventArgs e)
    {
        if(taskuri.SelectedItem == null)
        {
            MessageBox.Show("Selecteaza un task pentru a-l sterge", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        var taskSelectat= (TaskModel) taskuri.SelectedItem;

        var rezultat = MessageBox.Show($"Sigur vrei sa stergi task-ul \"{taskSelectat.Titlul}\"?", "Confirmare",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (rezultat == MessageBoxResult.Yes)
        {
            _viewModel.Taskuri.Remove(taskSelectat);
        }
    }

    private void AdaugaTask_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtTitlu.Text))
        {
            MessageBox.Show("Introduceti un titlu pentru task", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (!int.TryParse(txtOra.Text, out int ora) || ora < 0 || ora > 23)
        {
            MessageBox.Show("Introduceți o oră validă (0-23).", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (!int.TryParse(txtMinut.Text, out int minut) || minut < 0 || minut > 59)
        {
            MessageBox.Show("Introduceți minute valide (0-59).", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (datePickerDeadline.SelectedDate == null)
        {
            MessageBox.Show("Selectați o dată pentru deadline.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        DateTime deadline = datePickerDeadline.SelectedDate.Value.AddHours(ora).AddMinutes(minut);
        
            var nouTask = new TaskModel
            {
                Id = _viewModel.Taskuri.Count + 1,
                Titlul = txtTitlu.Text,
                Descriere = txtDescriere.Text,
                Deadline = deadline,
                Finalizat = false
            };
            _viewModel.Taskuri.Add(nouTask);
        
        txtTitlu.Clear();
        txtDescriere.Clear();
        txtOra.Clear();
        txtMinut.Clear();
        datePickerDeadline.SelectedDate = null;
    }

    
}