using System.Data.SQLite;
using System.IO;

namespace TaskManagerWPF;

public static class DatabaseInitializer
{
    private static readonly string folder = Path.Combine(Directory.GetCurrentDirectory(), "Data");
    private static readonly string dbPath = Path.Combine(folder, "taskmanager.db");
    private static readonly string connectionString = $"Data Source={dbPath};Version=3;";

    public static string ConnectionString => connectionString;
    public static void InitializeDatabase()
    {
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        if (!File.Exists(dbPath))
            SQLiteConnection.CreateFile(dbPath);

        using var connection = new SQLiteConnection(connectionString);
        connection.Open();

        string createTable = @"
                CREATE TABLE IF NOT EXISTS Taskuri (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Titlu TEXT NOT NULL,
                    Descriere TEXT,
                    Deadline TEXT NOT NULL,
                    Categorie TEXT,
                    Status TEXT,
                    Prioritate INTEGER NOT NULL
                );";

        using var command = new SQLiteCommand(createTable, connection);
        command.ExecuteNonQuery();
    }
}
