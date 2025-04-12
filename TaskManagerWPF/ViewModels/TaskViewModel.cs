using System.Collections.ObjectModel;
using System.Data;
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
    public ObservableCollection<TaskModel> TaskuriC { get; set; } = new ObservableCollection<TaskModel>();


    public TaskViewModel()
    {
        IncarcaTaskuri(); 
    }
    
    public void AdaugaTask(TaskModel task)
    {
        using var connection = DataBaseHelper.ConnectToDatabase(); // direct metoda care returnează SQLiteConnection valid
        string insertQuery =
            "INSERT INTO Taskuri (Titlu, Descriere, Deadline, Categorie, Status) VALUES (@titlu, @descriere, @deadline,@categorie ,@status)";

        using (var command = new SQLiteCommand(insertQuery, connection))
        {
            command.Parameters.AddWithValue("@titlu", task.Titlul);
            command.Parameters.AddWithValue("@descriere", task.Descriere);
            command.Parameters.AddWithValue("@deadline", task.Deadline.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@categorie", task.Categorie.ToString());
            command.Parameters.AddWithValue("@status", task.Status.ToString());
            command.ExecuteNonQuery();
        }
        Console.WriteLine("Task adăugat!");
    }


    public List<TaskModel> ObtinereTaskuri()
    {
        List<TaskModel> taskuri = new List<TaskModel>();
        using (var connection = DataBaseHelper.ConnectToDatabase())
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
                        string categorieText = reader.GetString(4);
                        string statusText = reader.GetString(5);

                        CategoriiTask categorie = Enum.TryParse(categorieText, true, out CategoriiTask cat) ? cat : CategoriiTask.Nespecificat;
                        StatusTask status = Enum.TryParse(statusText, true, out StatusTask st) ? st : StatusTask.Neinceput;
                        DateTime data = DateTime.Parse(reader.GetString(3));
                        
                        TaskModel task = new TaskModel(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            data,
                            categorie,
                            status
                        );
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

    public void IncarcaTaskuri()
    {
        TaskuriC.Clear(); 
        var listaDinBD = ObtinereTaskuri();
        foreach (var task in listaDinBD)
        {
            TaskuriC.Add(task); // adaugă fiecare task în ObservableCollection
        }
    }
    public void ActualizareTask(TaskModel task)
    {
        using (var connection = DataBaseHelper.ConnectToDatabase())
        {
                connection.Open();
                string query = @"UPDATE Taskuri 
                             SET Titlu = @Titlu, 
                                 Descriere = @Descriere, 
                                 Deadline = @Deadline, 
                                 Categorie= @Categorie,
                                 Status = @Status
                             WHERE Id = @Id";
                
              
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Titlu", task.Titlul);
                    command.Parameters.AddWithValue("@Descriere", task.Descriere);
                    command.Parameters.AddWithValue("@Deadline", task.Deadline.ToString());
                    command.Parameters.AddWithValue("@Categorie", task.Categorie.ToString());
                    command.Parameters.AddWithValue("@Status", task.Status.ToString());
                    command.Parameters.AddWithValue("@Id", task.Id);
                    command.ExecuteNonQuery();
                }
        }
        
    }
}

    