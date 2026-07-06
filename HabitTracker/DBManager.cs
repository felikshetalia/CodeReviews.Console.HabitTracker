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

    public List<Habit> GetHabits()
    {
        List<Habit> habitsList = new();

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT * FROM Habits";

                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        habitsList.Add(new Habit
                        {
                            ID = (Int64)reader["Id"],
                            Name = (string)reader["Name"],
                            Unit = (string)reader["Unit"],
                            CreatedAt = (string)reader["CreatedAt"]
                        });
                    }
                }
            }
            connection.Close();
        }

        return habitsList;
    }

    public void InsertHabit(string _name, string _unit)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var cmd = connection.CreateCommand())
            {

                cmd.CommandText = @"
                    INSERT INTO Habits (Name, Unit, CreatedAt)
                    VALUES ($name, $unit, $createdAt);
                ";

                cmd.Parameters.AddWithValue("$name", _name);
                cmd.Parameters.AddWithValue("$unit", _unit);
                cmd.Parameters.AddWithValue("$createdAt", DateTime.Now.ToString("dd-MM-yyyy"));
                cmd.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    public List<HabitEntry> GetHabitEntries()
    {
        List<HabitEntry> habitEntriesList = new();

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT * FROM HabitEntries";

                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        habitEntriesList.Add(new HabitEntry
                        {
                            ID = (int)reader["Id"],
                            HabitID = (int)reader["HabitId"],
                            Date = (DateTime)reader["Date"],
                            Quantity = (int)reader["Quantity"],
                            Notes = (string)reader["Notes"]
                        });
                    }
                }
            }
            connection.Close();
        }

        return habitEntriesList;
    }
}