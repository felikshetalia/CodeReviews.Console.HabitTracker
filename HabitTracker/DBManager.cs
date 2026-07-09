using System;
using Microsoft.Data.Sqlite;

namespace CodeReviews_Console_HabitTracker;

public class DBManager
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
                            ID = (long)reader["Id"],
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

    public Habit? GetHabitByName(string _name)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var cmd = connection.CreateCommand())
            {

                cmd.CommandText = @"
                    SELECT Id, Name, Unit, CreatedAt
                    FROM Habits
                    WHERE Name = $name COLLATE NOCASE
                    LIMIT 1
                ";
                cmd.Parameters.AddWithValue("$name", _name);

                var reader = cmd.ExecuteReader();

                if (!reader.Read())
                    return null;

                return new Habit
                {
                    ID = (long)reader["Id"],
                    Name = (string)reader["Name"],
                    Unit = (string)reader["Unit"],
                    CreatedAt = (string)reader["CreatedAt"]
                };

            }
        }
    }

    public Habit? GetHabitById(long _habitId)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var cmd = connection.CreateCommand())
            {

                cmd.CommandText = @"
                    SELECT Id, Name, Unit, CreatedAt
                    FROM Habits
                    WHERE Id = $id
                    LIMIT 1
                ";
                cmd.Parameters.AddWithValue("$id", _habitId);

                var reader = cmd.ExecuteReader();

                if (!reader.Read())
                    return null;

                return new Habit
                {
                    ID = (long)reader["Id"],
                    Name = (string)reader["Name"],
                    Unit = (string)reader["Unit"],
                    CreatedAt = (string)reader["CreatedAt"]
                };

            }
        }
    }

    public void DeleteHabitById(long _habitId)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"
                    DELETE FROM Habits
                    WHERE Id = $id;
                ";

                cmd.Parameters.AddWithValue("$id", _habitId);

                cmd.ExecuteNonQuery();
            }
            connection.Close();
        }
    }
    public List<HabitEntry> GetHabitEntries(long? _habitId = null)
    {
        List<HabitEntry> habitEntriesList = new();

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var cmd = connection.CreateCommand())
            {
                if (_habitId == null)
                    cmd.CommandText = @"SELECT * FROM HabitEntries";
                else
                {
                    cmd.CommandText = @"SELECT * FROM HabitEntries 
                                        WHERE HabitId = $habitId";
                    cmd.Parameters.AddWithValue("$habitId", _habitId.Value);
                }

                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        habitEntriesList.Add(new HabitEntry
                        {
                            ID = (long)reader["Id"],
                            HabitID = (long)reader["HabitId"],
                            Date = (string)reader["Date"],
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            Notes = reader.IsDBNull(4) ? null : reader.GetString(4)
                        });
                    }
                }
            }
            connection.Close();
        }

        return habitEntriesList;
    }

    public void InsertHabitEntry(long _habitEntryId, string _date, int _quantity, string? notes)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var cmd = connection.CreateCommand())
            {

                cmd.CommandText = @"
                    INSERT INTO HabitEntries (HabitId, Date, Quantity, Notes)
                    VALUES ($habitId, $date, $quantity, $notes);
                ";

                cmd.Parameters.AddWithValue("$habitId", _habitEntryId);
                cmd.Parameters.AddWithValue("$date", _date);
                cmd.Parameters.AddWithValue("$quantity", _quantity);
                cmd.Parameters.AddWithValue("$notes", string.IsNullOrWhiteSpace(notes) ? DBNull.Value : notes);

                cmd.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    public void DeleteHabitEntryById(long _habitId)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"
                    DELETE FROM HabitEntries
                    WHERE Id = $id;
                ";

                cmd.Parameters.AddWithValue("$id", _habitId);

                cmd.ExecuteNonQuery();
            }
            connection.Close();
        }
    }



    public void UpdateHabit(long _habitId, string _name, string _unit)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"
                    UPDATE Habits
                    SET Name = $name,
                        Unit = $unit
                    WHERE Id = $id;
                ";

                cmd.Parameters.AddWithValue("$id", _habitId);
                cmd.Parameters.AddWithValue("$name", _name);
                cmd.Parameters.AddWithValue("$unit", _unit);

                cmd.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    public void UpdateHabitEntry(long _habitEntryId, string _date, int _quantity, string? _notes)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"
                    UPDATE HabitEntries
                    SET Date = $date,
                        Quantity = $quantity,
                        Notes = $notes
                    WHERE Id = $id;
                ";

                cmd.Parameters.AddWithValue("$id", _habitEntryId);
                cmd.Parameters.AddWithValue("$date", _date);
                cmd.Parameters.AddWithValue("$quantity", _quantity);
                cmd.Parameters.AddWithValue("$notes", string.IsNullOrWhiteSpace(_notes) ? DBNull.Value : _notes);

                cmd.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    public HabitEntry? GetHabitEntryById(long _habitEntryId)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var cmd = connection.CreateCommand())
            {

                cmd.CommandText = @"
                    SELECT Id, HabitId, Date, Quantity, Notes
                    FROM HabitEntries
                    WHERE Id = $id
                    LIMIT 1
                ";
                cmd.Parameters.AddWithValue("$id", _habitEntryId);

                var reader = cmd.ExecuteReader();

                if (!reader.Read())
                    return null;

                return new HabitEntry
                {
                    ID = (long)reader["Id"],
                    HabitID = (long)reader["HabitId"],
                    Date = (string)reader["Date"],
                    Quantity = Convert.ToInt32(reader["Quantity"]),
                    Notes = reader.IsDBNull(4) ? null : reader.GetString(4)
                };

            }
        }
    }
}