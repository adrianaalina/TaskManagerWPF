using System;
using System.Data.SQLite;
using System.IO;
using System.Data.SQLite;

namespace TaskManagerWPF.Data
{
    public static class DataBaseHelper
    {
        public static SQLiteConnection ConnectToDatabase()
        {
            string dbFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Data");
            string dbPath = Path.Combine(dbFolder, "TaskManager.db");
        
            string connectionString = $"Data Source={dbPath};Version=3;UTF8=True";
        
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            connection.Open();
            return connection;
        }
        
    }
}