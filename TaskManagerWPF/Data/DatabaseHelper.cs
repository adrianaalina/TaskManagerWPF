using System.Data.SQLite;
using System.IO;

namespace TaskManagerWPF.Data;

public static class DatabaseHelper
{
    private static readonly string folder =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

    private static readonly string dbPath =
        Path.Combine(folder, "taskmanager.db");

    private static readonly string connectionString =
        $"Data Source={dbPath};Version=3;";

    public static SQLiteConnection GetConnection()
    {
        var connection = new SQLiteConnection(connectionString);
        connection.Open();
        return connection;
    }
}