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

image here

- CRUD DB functions
  - From the main menu, users can Create, Read, Update, and Delete habits.
  - Users can also Create, Read, Update, and Delete habit entries linked to a specific habit.
  - Users can record habit entries for any date they choose, entered in dd-MM-yyyy format.
  - Each habit entry stores a date, quantity, and optional notes.
  - Dates and quantities are validated before being saved to the database.
  - Duplicate habit names are not allowed.
