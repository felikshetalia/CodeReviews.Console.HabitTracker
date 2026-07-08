# Habit Logger App

Console based CRUD application to track all types of habits.
Developed using C#/.NET and SQLite.

# Given Requirements:

- [x] Users need to be able to input the date of the occurrence of the habit.
- [x] The application should store and retrieve data from a real database.
- [x] When the application starts, it should create a sqlite database, if one isn’t present.
- [x] It should also create a table in the database, where the habit will be logged.
- [x] You need to be able to insert, delete, update and view your logged habits.
- [x] You should handle all possible errors so that the application never crashes
- [x] The application should only be terminated when the user inserts 0.
- [x] You can only interact with the database using ADO.NET. You can’t use mappers such as Entity Framework or Dapper.
- [x] Follow the DRY (Don't Repeat Yourself) principle as much as you can.
- [x] This pretty README file :D

# Features

- SQLite database connection
  - The program uses a SQLite db connection to store and read information.
  - If no database exists, or the correct table does not exist they will be created on program start.

- A console based UI where users can navigate by key presses
  - ![image](https://raw.githubusercontent.com/felikshetalia/CodeReviews.Console.HabitTracker/refs/heads/master/screenshots/mainmenu.png)

- CRUD DB functions
  - From the main menu, users can Create, Read, Update, and Delete habits.
  - Users can also Create, Read, Update, and Delete habit entries linked to a specific habit.
  - Users can record habit entries for any date they choose, entered in dd-MM-yyyy format.
  - Each habit entry stores a date, quantity, and optional notes.
  - Dates and quantities are validated before being saved to the database.
  - Duplicate habit names are not allowed.

# What I've learned from this project

- Using SQLite as the database for a .NET console application.

- Connecting a C# console app to a local SQLite database file and create the database tables from code if they do not already exist.

- Using ADO.NET with SQLite to execute `SELECT`, `INSERT`, `UPDATE`, and `DELETE` commands.

- Validating console input before saving it to the database, including IDs, quantities, dates, habit names, and units.

- Parsing user-entered dates with `DateTime.TryParseExact()` and store them in a consistent `dd-MM-yyyy` format.

- Practicing DRY principle

# Areas to Improve

- Not hesitating to split the code into as much chunks as I can if it helps.

- Making error handling more consistent across the application, especially for database errors and invalid user actions.

- Improving the console UI by showing habit names alongside habit entries instead of only displaying habit IDs (JOIN tables).

- Maybe applying the DRY principle to methods that run queries too.

- Continue improving the overall project structure by spreading more logic across classes as the application grows.
