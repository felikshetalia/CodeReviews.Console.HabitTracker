using System;
using Microsoft.Data.Sqlite;
namespace CodeReviews_Console_HabitTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            DBManager db = new();
            db.InitializeDB();

            string? userInput = null;
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
                        Console.WriteLine("View All Habits selected.");
                        break;

                    case "2":
                        Console.WriteLine("Create New Habit selected.");
                        break;

                    case "3":
                        Console.WriteLine("Update Habit selected.");
                        break;

                    case "4":
                        Console.WriteLine("Delete Habit and All Its Entries selected.");
                        break;

                    case "5":
                        Console.WriteLine("View All Habit Entries selected.");
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
    }
}