using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using TaskManagerWPF.Models;
using TaskManagerWPF.Services;
using TaskManagerWPF.Data;
using TaskManagerWPF.ViewModels.Base;
using System.ComponentModel;
namespace TaskManagerWPF.ViewModels;


public class TaskViewModel : BaseViewModel
{
    public ICommand AddCommand { get; private set; }
    public ICommand DeleteCommand { get; private set; }

    public Array Statusuri => Enum.GetValues(typeof(StatusTask));
    public Array Categorii => Enum.GetValues(typeof(CategoriiTask));
    public Array Prioritati => Enum.GetValues(typeof(PrioritateTask));

    private ObservableCollection<TaskModel> _taskuriC = new();

    private readonly IDialogService _dialogService;


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
            CommandManager.InvalidateRequerySuggested();
        }
    }

    private TaskModel _currentTask = new TaskModel();

    public TaskModel CurrentTask
    {
        get => _currentTask;
        set
        {
            if (_currentTask != null)
                _currentTask.PropertyChanged -= CurrentTask_PropertyChanged;

            _currentTask = value;

            if (_currentTask != null)
                _currentTask.PropertyChanged += CurrentTask_PropertyChanged;

            OnPropertyChanged();
            CommandManager.InvalidateRequerySuggested();
        }
    }
    private void CurrentTask_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        CommandManager.InvalidateRequerySuggested();
    }



    //selectare deadline
    public int Ora
    {
        get => CurrentTask?.Deadline?.Hour ?? 0;
        set
        {
            if (CurrentTask?.Deadline == null) return;

            var d = CurrentTask.Deadline.Value;

            CurrentTask.Deadline = new DateTime(
                d.Year,
                d.Month,
                d.Day,
                value,
                d.Minute,
                0);

            OnPropertyChanged(nameof(Ora));
        }
    }

    public int Minut
    {
        get => CurrentTask?.Deadline?.Minute ?? 0;
        set
        {
            if (CurrentTask?.Deadline == null) return;

            var d = CurrentTask.Deadline.Value;

            CurrentTask.Deadline = new DateTime(
                d.Year,
                d.Month,
                d.Day,
                d.Hour,
                value,
                0);

            OnPropertyChanged(nameof(Minut));
        }
    }
    

    //constructor
    public TaskViewModel()
    {
        _dialogService = new DialogService();
        
        CurrentTask = new TaskModel(); 
        IncarcaTaskuri();

        AddCommand = new RelayCommand(_ => SaveTask(), _ => IsTaskValid());
        DeleteCommand = new RelayCommand(_ => DeleteSelected(), _ => SelectedTask != null);

        CommandManager.InvalidateRequerySuggested(); 
    }

    //Stergere
    private void DeleteSelected()
    {
        if(SelectedTask==null) return;
        
        bool rezultat = _dialogService.ShowConfirmation(
            $"Sigur vrei sa stergi task-ul \"{SelectedTask.Titlu}\"?",
            "Confirmare");

        if (!rezultat) return;
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
            cmd.Parameters.AddWithValue("@Deadline", CurrentTask.Deadline.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@Categorie", (int)CurrentTask.Categorie);
            cmd.Parameters.AddWithValue("@Status",  (int)CurrentTask.Status);
            cmd.Parameters.AddWithValue("@Prioritate", (int)CurrentTask.Prioritate);

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
            cmd.Parameters.AddWithValue("@Deadline", CurrentTask.Deadline.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@Categorie",(int)CurrentTask.Categorie);
            cmd.Parameters.AddWithValue("@Status", (int)CurrentTask.Status);
            cmd.Parameters.AddWithValue("@Prioritate", (int)CurrentTask.Prioritate);
            cmd.Parameters.AddWithValue("@Id", CurrentTask.Id);

            cmd.ExecuteNonQuery();
        }

        IncarcaTaskuri();

        // RESET FORMULAR
        CurrentTask = new TaskModel { Deadline = DateTime.Now };
        OnPropertyChanged(nameof(CurrentTask));
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
                        DateTime? deadline = null;

                        var rawDeadline = reader.GetValue(3);

                        if (rawDeadline != DBNull.Value)
                        {
                            if (rawDeadline is string s)
                                deadline = DateTime.Parse(s);
                            else
                                deadline = Convert.ToDateTime(rawDeadline);
                        }
                        CategoriiTask categorie = (CategoriiTask)reader.GetInt32(4);
                        StatusTask status = (StatusTask)reader.GetInt32(5);
                        PrioritateTask prioritate = (PrioritateTask)reader.GetInt32(6);
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


    //Validare
    private bool IsTaskValid()
    {
        if (CurrentTask == null)
            return false;

        if (!string.IsNullOrEmpty(CurrentTask[nameof(CurrentTask.Titlu)]))
            return false;

        if (!string.IsNullOrEmpty(CurrentTask[nameof(CurrentTask.Deadline)]))
            return false;

        if (!string.IsNullOrEmpty(CurrentTask[nameof(CurrentTask.Descriere)]))
            return false;

        return true;
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
   

    