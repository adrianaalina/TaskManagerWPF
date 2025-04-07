using System;
using System.Data.SQLite;
using System.IO;

namespace TaskManagerWPF.Data
{
    public static class DataBaseHelper
    {
        public static string ObțineCaleaBazei()
        {
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            return Path.Combine(folder, "TaskManager.db");
        }

        
    }
}