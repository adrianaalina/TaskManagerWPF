using System.Collections.ObjectModel;
using System.IO;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Windows.Media;
using TaskManagerWPF.Models;
namespace TaskManagerWPF.ViewModels;


public class TaskViewModel
{
    public ObservableCollection<TaskModel> Taskuri { get; set; } = new ObservableCollection<TaskModel>();
    public TaskViewModel()
    {
        Taskuri.Add(new TaskModel { Titlul = "Test Task", Descriere = "Descriere test",  Deadline = DateTime.Now.AddDays(1), Status = StatusTask.InLucru});
    }
    
    public void AdaugaTask(TaskModel task)
    {
        string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "taskmanager.db");
        string connectionString = $"Data Source={dbPath}";

        using var connection = new SQLiteConnection(connectionString);
        connection.Open();

        string insertQuery = "INSERT INTO Taskuri (Titlu, Descriere, Deadline, Status) VALUES (@titlu, @descriere, @deadline, @status)";
    
        using var command = new SQLiteCommand(insertQuery, connection);
        command.Parameters.AddWithValue("@titlu", task.Titlul);
        command.Parameters.AddWithValue("@descriere", task.Descriere);
        command.Parameters.AddWithValue("@deadline", task.Deadline.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("@status", task.Status.ToString());
        command.ExecuteNonQuery();
        Console.WriteLine("Task adăugat!");
    }
   
    }
    