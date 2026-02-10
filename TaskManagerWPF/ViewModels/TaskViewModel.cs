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


public class TaskViewModel:BaseViewModel
{ 
    public ICommand AddCommand { get; }
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
    private TaskModel _selectedTask;
    public TaskModel SelectedTask
    {
        get => _selectedTask;
        set
        {
            _selectedTask = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Ora));
            OnPropertyChanged(nameof(Minut));
            OnPropertyChanged(nameof(DeadlineDate));
        }
    }
    
    //selectare deadline
    public int Ora
    {
        get => TaskNou?.Deadline.Hour ?? 0;
        set
        {
            if (TaskNou == null) return;

            TaskNou.Deadline = new DateTime(
                TaskNou.Deadline.Year,
                TaskNou.Deadline.Month,
                TaskNou.Deadline.Day,
                value,
                TaskNou.Deadline.Minute,
                0);

            OnPropertyChanged();
        }
    }

    public int Minut
    {
        get => TaskNou?.Deadline.Minute ?? 0;
        set
        {
            if (TaskNou == null) return;

            TaskNou.Deadline = new DateTime(
                TaskNou.Deadline.Year,
                TaskNou.Deadline.Month,
                TaskNou.Deadline.Day,
                TaskNou.Deadline.Hour,
                value,
                0);

            OnPropertyChanged();
        } 
    }
    public DateTime? DeadlineDate
    {
        get => SelectedTask?.Deadline;
        set
        {
            if (SelectedTask == null || value == null) return;
            SelectedTask.Deadline = new DateTime(
                value.Value.Year,
                value.Value.Month,
                value.Value.Day,
                SelectedTask.Deadline.Hour,
                SelectedTask.Deadline.Minute,
                0);

            OnPropertyChanged();
            OnPropertyChanged(nameof(Ora));
            OnPropertyChanged(nameof(Minut));
        }
    }

    //task in lucru
    private TaskModel _taskNou = new TaskModel()
    {
        Deadline = DateTime.Now
    };

    public TaskModel TaskNou
    {
        get => _taskNou;
        set
        {
            _taskNou = value;
            OnPropertyChanged();
        }
    }

    //constructor
    public TaskViewModel()
    {
        IncarcaTaskuri();
        AddCommand = new RelayCommand(_ => AdaugaTaskDinUI());
    }
    
    
    //Adaugare
    public void AdaugaTask(TaskModel task)
    {
        using var connection = DatabaseHelper.GetConnection(); 
        string insertQuery =
            "INSERT INTO Taskuri (Titlu, Descriere, Deadline, Categorie, Status, Prioritate) VALUES (@titlu, @descriere, @deadline,@categorie ,@status, @prioritate)";

        using (var command = new SQLiteCommand(insertQuery, connection))
        {
            command.Parameters.AddWithValue("@titlu", task.Titlu);
            command.Parameters.AddWithValue("@descriere", task.Descriere);
            command.Parameters.AddWithValue("@deadline", task.Deadline.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@categorie", task.Categorie.ToString());
            command.Parameters.AddWithValue("@status", task.Status.ToString());
            command.Parameters.AddWithValue("@prioritate", task.Prioritate.ToString());
            command.ExecuteNonQuery();
        }
        Console.WriteLine("Task adăugat!");
    }
    private void AdaugaTaskDinUI()
    {
      AdaugaTask(TaskNou);
      IncarcaTaskuri();
      TaskNou = new TaskModel()
      {
          Deadline = DateTime.Now
      };
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
                            categorie = CategoriiTask.Nespecificat ;
                        StatusTask status; 
                        if (!Enum.TryParse(reader.GetString(5), out status))
                            status = StatusTask.Neinceput;
                        PrioritateTask prioritate; 
                        if (!Enum.TryParse(reader.GetString(6), out prioritate))
                            prioritate = PrioritateTask.Normal;
                        TaskModel task = new TaskModel{
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
    
    //Actualizare Taskuri
    public void ActualizareTask(TaskModel task)
    {
        using var connection = DatabaseHelper.GetConnection();

        string query = @"UPDATE Taskuri 
                     SET Titlu = @titlu, 
                         Descriere = @descriere, 
                         Deadline = @deadline, 
                         Categorie = @categorie,
                         Status = @status,
                         Prioritate = @prioritate
                     WHERE Id = @id";

        using var command = new SQLiteCommand(query, connection);

        command.Parameters.AddWithValue("@titlu", task.Titlu);
        command.Parameters.AddWithValue("@descriere", task.Descriere);
        command.Parameters.AddWithValue("@deadline", task.Deadline.ToString("yyyy-MM-dd HH:mm:ss"));
        command.Parameters.AddWithValue("@categorie", task.Categorie.ToString());
        command.Parameters.AddWithValue("@status", task.Status.ToString());
        command.Parameters.AddWithValue("@prioritate", task.Prioritate.ToString());
        command.Parameters.AddWithValue("@id", task.Id);

        int rows = command.ExecuteNonQuery();

        if (rows == 0)
            MessageBox.Show("UPDATE nu a modificat niciun rand! ID-ul nu a fost gasit.");

        IncarcaTaskuri();
    }
}

    