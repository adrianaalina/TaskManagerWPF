using System;
using System.IO;
using System.Data.SQLite;

namespace TaskManagerWPF.Data
{
    public static class DataBaseHelper
    {
        public static SQLiteConnection GetConnection()
      
        {
            return new SQLiteConnection(DatabaseInitializer.ConnectionString);
        }
    }
}