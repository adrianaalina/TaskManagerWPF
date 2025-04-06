using System.Data.SQLite;
using System.IO;

namespace TaskManagerWPF;

public class DatabaseInitializer
{
    public static void CreeazaBazaDacaNuExista()
    {
        string dbFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Data");
        if (!Directory.Exists(dbFolder))
        {
            Directory.CreateDirectory(dbFolder);
        }
        string dbPath = Path.Combine(dbFolder, "TaskManager.db");
        if (!File.Exists(dbPath))
        {
            SQLiteConnection.CreateFile(dbPath);
            using var connection = new SQLiteConnection($"Data Source={dbPath}");
            connection.Open();
            string creareTabel = @"
              CREATE TABLE IF NOT EXISTS Taskuri(
                   Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Titlu TEXT NOT NULL,
                    Descriere TEXT,
                    Deadline TEXT,
                    Status TEXT
             );";
            using var command = new SQLiteCommand(creareTabel, connection);
            command.ExecuteNonQuery();
        }
    }
}