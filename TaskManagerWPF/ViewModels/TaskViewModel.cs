using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using TaskManagerWPF.Models;
using TaskManagerWPF.Data;
using TaskManagerWPF.ViewModels.Base;

namespace TaskManagerWPF.ViewModels;


public class TaskViewModel : BaseViewModel
{
    public ICommand AddCommand { get; private set; }
    public ICommand DeleteCommand { get; private set; }

    public Array Statusuri => Enum.GetValues(typeof(StatusTask));
    public Array Categorii => Enum.GetValues(typeof(CategoriiTask));
    public Array Prioritati => Enum.GetValues(typeof(PrioritateTask));

    private ObservableCollection<TaskModel> _taskuriC = new();

    public ObservableCollection<TaskModel> TaskuriC
    {
        get => _taskuriC;
        set
        {
            _taskuriC = value;
            OnPropertyChanged();
        }
    }

    //task curent pentru editare sau adaugare
    private TaskModel _selectedTask;

    public TaskModel SelectedTask
    {
        get => _selectedTask;
        set
        {
            _selectedTask = value;
            OnPropertyChanged();
           
           
            if (_selectedTask != null)
            {
                CurrentTask = new TaskModel
                {
                    Id = _selectedTask.Id,
                    Titlu = _selectedTask.Titlu,
                    Descriere = _selectedTask.Descriere,
                    Deadline = _selectedTask.Deadline,
                    Categorie = _selectedTask.Categorie,
                    Status = _selectedTask.Status,
                    Prioritate = _selectedTask.Prioritate
                };
            } 
            OnPropertyChanged(nameof(Ora));
            OnPropertyChanged(nameof(Minut));
        }
    }

    private TaskModel _currentTask = new TaskModel();

    public TaskModel CurrentTask
    {
        get => _currentTask;
        set
        {
            _currentTask = value;
            OnPropertyChanged();
        }
    }


    //selectare deadline
    public int Ora
    {
        get => CurrentTask?.Deadline.Hour ?? 0;
        set
        {
            if (CurrentTask == null) return;

            CurrentTask.Deadline = new DateTime(
                CurrentTask.Deadline.Year,
                CurrentTask.Deadline.Month,
                CurrentTask.Deadline.Day,
                value,
                CurrentTask.Deadline.Minute,
                0);

            OnPropertyChanged(nameof(Ora));
        }
    }

    public int Minut
    {
        get => CurrentTask?.Deadline.Minute ?? 0;
        set
        {
            if (CurrentTask == null) return;

            CurrentTask.Deadline = new DateTime(
                CurrentTask.Deadline.Year,
                CurrentTask.Deadline.Month,
                CurrentTask.Deadline.Day,
                CurrentTask.Deadline.Hour,
                value,
                0);

            OnPropertyChanged(nameof(Minut));
        }
    }
    

    //constructor
    public TaskViewModel()
    {
        IncarcaTaskuri();
        AddCommand = new RelayCommand(_ => SaveTask());
        DeleteCommand= new RelayCommand (_=>DeleteSelected(),_=>SelectedTask != null);
    }

    //Stergere
    private void DeleteSelected()
    {
        if(SelectedTask==null) return;
        
        var rezultat=MessageBox.Show( 
            $"Sigur vrei sa stergi task-ul \"{SelectedTask.Titlu}\"?",
        "Confirmare",
        MessageBoxButton.YesNo,
        MessageBoxImage.Question);

        if(rezultat !=MessageBoxResult.Yes) return;
        using var connection=DatabaseHelper.GetConnection();
        string query = "DELETE FROM Taskuri WHERE Id=@id";
        using var command = new SQLiteCommand(query, connection);
        command.Parameters.AddWithValue("@id", SelectedTask.Id);
        command.ExecuteNonQuery();

        IncarcaTaskuri();
        CurrentTask = new TaskModel { Deadline = DateTime.Now };

        OnPropertyChanged(nameof(Ora));
        OnPropertyChanged(nameof(Minut));
    }
    

    
    //Adaugare si actualizare
    public void SaveTask()
    {
        using var connection = DatabaseHelper.GetConnection();

        if (CurrentTask.Id == 0)
        {
            // INSERT
            string insert = @"INSERT INTO Taskuri
        (Titlu, Descriere, Deadline, Categorie, Status, Prioritate)
        VALUES (@Titlu,@Descriere,@Deadline,@Categorie,@Status,@Prioritate)";

            using var cmd = new SQLiteCommand(insert, connection);

            cmd.Parameters.AddWithValue("@Titlu", CurrentTask.Titlu);
            cmd.Parameters.AddWithValue("@Descriere", CurrentTask.Descriere);
            cmd.Parameters.AddWithValue("@Deadline", CurrentTask.Deadline.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@Categorie", CurrentTask.Categorie.ToString());
            cmd.Parameters.AddWithValue("@Status", CurrentTask.Status.ToString());
            cmd.Parameters.AddWithValue("@Prioritate", CurrentTask.Prioritate.ToString());

            cmd.ExecuteNonQuery();
        }
        else
        {
            // UPDATE
            string update = @"UPDATE Taskuri SET
        Titlu=@Titlu,
        Descriere=@Descriere,
        Deadline=@Deadline,
        Categorie=@Categorie,
        Status=@Status,
        Prioritate=@Prioritate
        WHERE Id=@Id";

            using var cmd = new SQLiteCommand(update, connection);

            cmd.Parameters.AddWithValue("@Titlu", CurrentTask.Titlu);
            cmd.Parameters.AddWithValue("@Descriere", CurrentTask.Descriere);
            cmd.Parameters.AddWithValue("@Deadline", CurrentTask.Deadline.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@Categorie", CurrentTask.Categorie.ToString());
            cmd.Parameters.AddWithValue("@Status", CurrentTask.Status.ToString());
            cmd.Parameters.AddWithValue("@Prioritate", CurrentTask.Prioritate.ToString());
            cmd.Parameters.AddWithValue("@Id", CurrentTask.Id);

            cmd.ExecuteNonQuery();
        }

        IncarcaTaskuri();

        // RESET FORMULAR
        CurrentTask = new TaskModel { Deadline = DateTime.Now };
        OnPropertyChanged(nameof(Ora));
        OnPropertyChanged(nameof(Minut));

    }


    //Obtinere Taskuri
    public List<TaskModel> ObtinereTaskuri()
    {
        List<TaskModel> taskuri = new List<TaskModel>();
        using (var connection = DatabaseHelper.GetConnection())
        {
            string selectQuery = "SELECT * FROM Taskuri";
            using (var command = new SQLiteCommand(selectQuery, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    try
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.WriteLine($"Coloana {i}: {reader.GetName(i)}");
                        }

                        int id = reader.GetInt32(0);
                        string titlu = reader.GetString(1);
                        string descriere = reader.IsDBNull(2) ? "" : reader.GetString(2);
                        DateTime deadline = DateTime.Parse(reader.GetString(3));

                        CategoriiTask categorie;
                        if (!Enum.TryParse(reader.GetString(4), out categorie))
                            categorie = CategoriiTask.Nespecificat;
                        StatusTask status;
                        if (!Enum.TryParse(reader.GetString(5), out status))
                            status = StatusTask.Neinceput;
                        PrioritateTask prioritate;
                        if (!Enum.TryParse(reader.GetString(6), out prioritate))
                            prioritate = PrioritateTask.Normal;
                        TaskModel task = new TaskModel
                        {
                            Titlu = titlu,
                            Descriere = descriere,
                            Deadline = deadline,
                            Categorie = categorie,
                            Status = status,
                            Prioritate = prioritate
                        };
                        task.Id = id;
                        taskuri.Add(task);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Eroare la citirea taskului: {ex.Message}");
                    }
                }
            }
        }

        return taskuri;
    }


    //Sterge Task
    public void StergeTask(int id)
    {
        using var connection = DatabaseHelper.GetConnection();

        string query = "DELETE FROM Taskuri WHERE Id=@id";
        using var command = new SQLiteCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();

        IncarcaTaskuri();
    }


    public void IncarcaTaskuri()
    {
        TaskuriC.Clear();
        var listaDinBD = ObtinereTaskuri();
        foreach (var task in listaDinBD)
        {
            TaskuriC.Add(task);
        }
    }

}
   

    