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
        DatabaseInitializer.InitializeDatabase();
        _viewModel = new TaskViewModel();
        this.DataContext = _viewModel;
        
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
            _viewModel.TaskuriC.Remove(taskSelectat);
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
        
        if (txtStatus.Text == null)
        {
            MessageBox.Show("Selectati un status", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        string categorietxt = txtCategorie.Text;
        if (!Enum.TryParse<CategoriiTask>(categorietxt, true, out CategoriiTask categorie))
        {
            string valoriAcceptate = string.Join(", ", Enum.GetNames(typeof(CategoriiTask)));
            MessageBox.Show($"Status invalid. Folosește: {valoriAcceptate}.");
            return;
        }
        
        string statusText = txtStatus.Text;
        if (!Enum.TryParse<StatusTask>(statusText, true, out StatusTask status))
        {
            string valoriAcceptate = string.Join(", ", Enum.GetNames(typeof(StatusTask)));
            MessageBox.Show($"Status invalid. Folosește: {valoriAcceptate}.");
            return;
        }
            var nouTask = new TaskModel(
            
                 txtTitlu.Text,
                txtDescriere.Text,
                 deadline,
                 categorie,
                 status
            );
            _viewModel.AdaugaTask(nouTask);
        
        txtTitlu.Clear();
        txtDescriere.Clear();
        txtOra.Clear();
        txtMinut.Clear();
        txtCategorie.Clear();
        txtStatus.Clear();
        datePickerDeadline.SelectedDate = null;
    }

    private void ActualizeazaTask_Click(object sender, RoutedEventArgs e)
    {
        if (taskuri.SelectedItem == null)
        {
            MessageBox.Show("Selectează un task pentru a-l modifica.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!int.TryParse(txtOra.Text, out int ora) || ora < 0 || ora > 23 ||
            !int.TryParse(txtMinut.Text, out int minut) || minut < 0 || minut > 59)
        {
            MessageBox.Show("Introduceți o oră și minute valide.");
            return;
        }

        if (!Enum.TryParse<StatusTask>(txtStatus.Text, true, out StatusTask status))
        {
            MessageBox.Show("Status invalid.");
            return;
        }

        DateTime deadline = datePickerDeadline.SelectedDate.Value.AddHours(ora).AddMinutes(minut);

        var taskSelectat = (TaskModel)taskuri.SelectedItem;

        taskSelectat.Titlul = txtTitlu.Text;
        taskSelectat.Descriere = txtDescriere.Text;
        taskSelectat.Deadline = deadline;
        taskSelectat.Status = status;

        _viewModel.ActualizareTask(taskSelectat);
        taskuri.Items.Refresh();

        MessageBox.Show("Task actualizat cu succes!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    
    
}