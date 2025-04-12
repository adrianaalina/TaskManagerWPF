using System.Data.SQLite;
using System.IO;

namespace TaskManagerWPF;

public static class DatabaseInitializer
{
    private static readonly string dbPath = Path.Combine(
        Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName,
        "Data",
        "taskmanager.db"
        );
    public static void InitializeDatabase()
    {
        if (!File.Exists(dbPath))
        {
            SQLiteConnection.CreateFile(dbPath);
        }

        using var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;");
        connection.Open();

        string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS Taskuri (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Titlu TEXT NOT NULL,
                Descriere TEXT,
                Deadline TEXT,
                Prioritate TEXT,
                Categorie TEXT,
                Status INTEGER DEFAULT 0
            );";

        using var command = new SQLiteCommand(createTableQuery, connection);
        command.ExecuteNonQuery();
    }
}
