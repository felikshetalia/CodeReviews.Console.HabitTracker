using System;
using Microsoft.Data.Sqlite;

namespace CodeReviews_Console_HabitTracker;

internal class DBManager
{
    private const string _dbFilename = "habit-tracker.db";
    public string connectionString { get; } = new SqliteConnectionStringBuilder
    {
        DataSource = _dbFilename,
        ForeignKeys = true
    }.ToString();

    public void InitializeDB()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            // Step 1: open the connection
            connection.Open();

            // Step 2: build the command
            var tableCommand = connection.CreateCommand();

            tableCommand.CommandText = @"CREATE TABLE IF NOT EXISTS Habits (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL UNIQUE,
                        Unit TEXT NOT NULL,
                        CreatedAt TEXT NOT NULL
                        )";

            // Step 3: execute
            tableCommand.ExecuteNonQuery();

            // Repeat for HabitEntries
            tableCommand = connection.CreateCommand();

            tableCommand.CommandText = @"CREATE TABLE IF NOT EXISTS HabitEntries (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        HabitId INTEGER NOT NULL,
                        Date TEXT NOT NULL,
                        Quantity INTEGER NOT NULL CHECK (Quantity > 0),
                        Notes TEXT,

                        FOREIGN KEY (HabitId) 
                            REFERENCES Habits(Id)
                            ON DELETE CASCADE
                        )";

            // Step 3: execute
            tableCommand.ExecuteNonQuery();

            connection.Close();
        }
    }
}