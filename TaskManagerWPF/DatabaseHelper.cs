using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

public class DatabaseHelper
{
    private const string ConnectionString = "Data Source=task.db";

    public static void InitializareDatabase()
    {
        using (var connection = new SqliteConnection(ConnectionString))
        {
            connection.Open();
            string tableCommand="CREATE TABLE IF NOT EXISTS Task(Id INTEGER PRIMARY KEY AUTOINCREMENT, Titlul TEXT, Descriere TEXT , Deadline TEXT)";
            SqliteCommand command = new SqliteCommand(tableCommand, connection);
            command.ExecuteNonQuery();
        }
    }
}