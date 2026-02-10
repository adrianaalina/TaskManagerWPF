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
    private TaskViewModel _viewModel;
    public MainWindow()
    {
        InitializeComponent();
        statusComboBox.ItemsSource = Enum.GetValues(typeof(StatusTask));
        categorieComboBox.ItemsSource = Enum.GetValues(typeof(CategoriiTask));
        prioritateComboBox.ItemsSource = Enum.GetValues(typeof(PrioritateTask));
       
    }
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        _viewModel = new TaskViewModel();
        this.DataContext = _viewModel;
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
                _viewModel.StergeTask(taskSelectat.Id);
            }
        }
    
    
    private void ActualizeazaTask_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (taskuri.SelectedItem is TaskModel selectedTask)
            {
                selectedTask.Titlu = txtTitlu.Text;
                selectedTask.Descriere = txtDescriere.Text;

                
                if (statusComboBox.SelectedItem is StatusTask status)
                    selectedTask.Status = status;
                else
                    throw new Exception("Statusul selectat nu este valid.");

                // Validare și extragere Categorie
                if (categorieComboBox.SelectedItem is CategoriiTask categorie)
                    selectedTask.Categorie = categorie;
                else
                    throw new Exception("Categoria selectată nu este validă.");

                if (prioritateComboBox.SelectedItem is PrioritateTask prioritate)
                    selectedTask.Prioritate = prioritate;
                else
                {
                    MessageBox.Show("Prioritatea selectată nu este validă.");
                    return;
                }

                if (!int.TryParse(txtOra.Text, out int ora) || ora < 0 || ora > 23)
                {
                    MessageBox.Show("Introduceți o oră validă (0-23).");
                    return;
                }

                if (!int.TryParse(txtMinut.Text, out int minut) || minut < 0 || minut > 59)
                {
                    MessageBox.Show("Introduceți minute valide (0-59).");
                    return;
                }

                if (datePickerDeadline.SelectedDate == null)
                {
                    MessageBox.Show("Selectați o dată pentru deadline.");
                    return;
                }

                DateTime deadline = datePickerDeadline.SelectedDate.Value
                    .AddHours(ora)
                    .AddMinutes(minut);

                selectedTask.Deadline = deadline;


               _viewModel.ActualizareTask(selectedTask); // actualizare în SQLite

                MessageBox.Show("Task actualizat cu succes!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
              //  taskuri.RefreshTaskuri();
                //ClearInputFields();
            }
            else
            {
                MessageBox.Show("Selectează un task din listă.", "Atenție", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Eroare la actualizare: " + ex.Message, "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
}