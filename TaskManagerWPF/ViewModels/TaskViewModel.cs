using System.Collections.ObjectModel;
using System.IO;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Windows.Media;
using TaskManagerWPF.Models;
using TaskManagerWPF.Data;
namespace TaskManagerWPF.ViewModels;


public class TaskViewModel
{
    public ObservableCollection<TaskModel> Taskuri { get; set; } = new ObservableCollection<TaskModel>();
    public TaskViewModel()
    {
       // Taskuri.Add(new TaskModel { Titlul = "Test Task", Descriere = "Descriere test",  Deadline = DateTime.Now.AddDays(1), Status = StatusTask.InLucru});
    }
    
    public void AdaugaTask(TaskModel task)
    {
        string connectionString = $"Data Source={DataBaseHelper.ObțineCaleaBazei()}";

        using var connection = new SQLiteConnection(connectionString);
        connection.Open();


        
        string insertQuery = "INSERT INTO Taskuri (Titlu, Descriere, Deadline, Status) VALUES (@titlu, @descriere, @deadline, @status)";

        using (var command = new SQLiteCommand(insertQuery, connection))
        {
            command.Parameters.AddWithValue("@titlu", task.Titlul);
            command.Parameters.AddWithValue("@descriere", task.Descriere);
            command.Parameters.AddWithValue("@deadline", task.Deadline.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@status", task.Status.ToString());
            command.ExecuteNonQuery();
        }

        Console.WriteLine("Task adăugat!");
    }

    public SQLiteConnection ConnectToDatabase()
    {
        string dbFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Data");
        string dbPath = Path.Combine(dbFolder, "TaskManager.db");
        
        string connectionString = $"Data Source={dbPath};Version=3;UTF8=True";
        
        SQLiteConnection connection = new SQLiteConnection(connectionString);
        connection.Open();
        return connection;
    }
    public List<TaskModel> ObtinereTaskuri()
    {
        List<TaskModel> taskuri = new List<TaskModel>();
        using (var connection = ConnectToDatabase())
        {
            string selectQuery = "SELECT * FROM Taskuri";
            using (var command = new SQLiteCommand(selectQuery, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    StatusTask status = Enum.Parse<StatusTask>(reader.GetString(4), true);
                    TaskModel task = new TaskModel(

                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.GetDateTime(3),
                        status
                    );
                    taskuri.Add(task);
                }
            }
            
        }

        return taskuri;
    }
   
    }
    