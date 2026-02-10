using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using TaskManagerWPF.Models;
using TaskManagerWPF.Data;
using TaskManagerWPF.ViewModels.Base;

namespace TaskManagerWPF.ViewModels;


public class TaskViewModel:BaseViewModel
{
    public ObservableCollection<TaskModel> TaskuriC { get; set; } = new ObservableCollection<TaskModel>();


    public TaskViewModel()
    {
        IncarcaTaskuri(); 
    }
    
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

                        CategoriiTask categorie =Enum.Parse<CategoriiTask>(reader.GetString(4)) ;
                        StatusTask status = Enum.Parse<StatusTask>(reader.GetString(5));
                        PrioritateTask prioritate = Enum.Parse<PrioritateTask>(reader.GetString(6));

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
    public void ActualizareTask(TaskModel task)
    {
        using (var connection = DatabaseHelper.GetConnection())
        {
            if (task.Id <= 0)
            {
                MessageBox.Show("Taskul nu are ID valid. Nu se poate actualiza.");
                return;
            }
            else
            {


                connection.Open();
                string query = @"UPDATE Taskuri 
                             SET Titlu = @Titlu, 
                                 Descriere = @Descriere, 
                                 Deadline = @Deadline, 
                                 Categorie= @Categorie,
                                 Status = @Status,
                                 Prioritate = @Prioritate
                             WHERE Id = @Id";


                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@titlu", task.Titlu);
                    command.Parameters.AddWithValue("@descriere", task.Descriere);
                    command.Parameters.AddWithValue("@deadline", task.Deadline.ToString());
                    command.Parameters.AddWithValue("@categorie", (int)task.Categorie);
                    command.Parameters.AddWithValue("@status", (int)task.Status);
                    command.Parameters.AddWithValue("@prioritate", (int)task.Prioritate);
                    command.Parameters.AddWithValue("@id", task.Id);
                    command.ExecuteNonQuery();
                }
                IncarcaTaskuri();
            }
        }
        
    }
}

    