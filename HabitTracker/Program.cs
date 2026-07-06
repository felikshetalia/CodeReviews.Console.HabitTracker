using System;
using System.Runtime.InteropServices;
using Microsoft.Data.Sqlite;
namespace CodeReviews_Console_HabitTracker
{
    class Program
    {

        public static DBManager db = new();
        static void Main(string[] args)
        {
            db.InitializeDB();

            string? userInput;
            bool appRunning = true;

            while (appRunning)
            {
                DisplayMenu();
                userInput = Console.ReadLine();

                switch (userInput)
                {
                    case "0":
                        Console.WriteLine("Closing application...");
                        appRunning = false;
                        break;

                    case "1":
                        DisplayHabitsTable();
                        break;

                    case "2":
                        AddNewHabit();
                        break;

                    case "3":
                        Console.WriteLine("Update Habit selected.");
                        break;

                    case "4":
                        Console.WriteLine("Delete Habit and All Its Entries selected.");
                        break;

                    case "5":
                        DisplayHabitEntriesTable();
                        break;

                    case "6":
                        Console.WriteLine("View Entries for a Specific Habit selected.");
                        break;

                    case "7":
                        Console.WriteLine("Insert Habit Entry selected.");
                        break;

                    case "8":
                        Console.WriteLine("Update Habit Entry selected.");
                        break;

                    case "9":
                        Console.WriteLine("Delete Habit Entry selected.");
                        break;

                    default:
                        Console.WriteLine("Invalid input. Please type a number from 0 to 9.");
                        break;
                }
            }

        }

        static void DisplayMenu()
        {
            Console.WriteLine("\n\nMAIN MENU");
            Console.WriteLine("\nWhat would you like to do?");

            Console.WriteLine("\nType 0 to Close Application.");

            Console.WriteLine("\n--- Habits ---");
            Console.WriteLine("Type 1 to View All Habits.");
            Console.WriteLine("Type 2 to Create New Habit.");
            Console.WriteLine("Type 3 to Update Habit.");
            Console.WriteLine("Type 4 to Delete Habit and All Its Entries.");

            Console.WriteLine("\n--- Habit Entries ---");
            Console.WriteLine("Type 5 to View All Habit Entries.");
            Console.WriteLine("Type 6 to View Entries for a Specific Habit.");
            Console.WriteLine("Type 7 to Insert Habit Entry.");
            Console.WriteLine("Type 8 to Update Habit Entry.");
            Console.WriteLine("Type 9 to Delete Habit Entry.");

            Console.WriteLine("------------------------------------------\n");
        }
        static void DisplayHabitsTable()
        {
            var habitList = db.GetHabits();

            if (habitList.Count == 0)
            {
                Console.WriteLine("\nNo habits found.");
                return;
            }

            Console.WriteLine("\nHABITS");
            Console.WriteLine("------------------------------------------------------------");

            Console.WriteLine(
                "ID".PadRight(5) +
                "Name".PadRight(25) +
                "Unit".PadRight(15) +
                "Created At".PadRight(12)
            );

            Console.WriteLine("------------------------------------------------------------");

            foreach (var habit in habitList)
            {
                Console.WriteLine(
                    habit.ID.ToString().PadRight(5) +
                    habit.Name.PadRight(25) +
                    habit.Unit.PadRight(15) +
                    habit.CreatedAt.PadRight(12)
                );
            }

            Console.WriteLine("------------------------------------------------------------");

        }
        static void DisplayHabitEntriesTable()
        {
            var habitEntriesList = db.GetHabitEntries();

            if (habitEntriesList.Count == 0)
            {
                Console.WriteLine("\nNo habits found.");
                return;
            }

            Console.WriteLine("\nHABIT ENTRIES");
            Console.WriteLine("--------------------------------------------------------------------------------");

            Console.WriteLine(
                "ID".PadRight(5) +
                "Habit ID".PadRight(10) +
                "Date".PadRight(15) +
                "Quantity".PadRight(12) +
                "Notes".PadRight(30)
            );

            Console.WriteLine("--------------------------------------------------------------------------------");

            foreach (var entry in habitEntriesList)
            {
                Console.WriteLine(
                    entry.ID.ToString().PadRight(5) +
                    entry.HabitID.ToString().PadRight(10) +
                    entry.Date.ToString("yyyy-MM-dd").PadRight(15) +
                    entry.Quantity.ToString().PadRight(12) +
                    (entry.Notes ?? "").PadRight(30)
                );
            }

            Console.WriteLine("--------------------------------------------------------------------------------");

        }
        static void AddNewHabit()
        {
            string? name;
            string? unit;

            System.Console.WriteLine("Which habit do you want to start?");
            name = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(name) || name.Length < 2)
            {
                System.Console.WriteLine("Please type at least 2 charachers");
                name = Console.ReadLine();
            }

            System.Console.WriteLine("In which unit will you be tracking?");
            unit = Console.ReadLine();

            while (unit == null)
            {
                System.Console.WriteLine("Unit cannot be null");
                unit = Console.ReadLine();
            }

            db.InsertHabit(name, unit);
        }
    }
}