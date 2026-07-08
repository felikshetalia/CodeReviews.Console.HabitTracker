using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
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
                        System.Console.WriteLine("Press any key to close...");
                        Console.ReadLine();
                        break;

                    case "2":
                        AddNewHabit();
                        break;

                    case "3":
                        UpdateItem("habit");
                        break;

                    case "4":
                        DeleteItem("habit");
                        break;

                    case "5":
                        DisplayHabitEntriesTable();
                        System.Console.WriteLine("Press any key to close...");
                        Console.ReadLine();
                        break;

                    case "6":
                        DisplayHabitEntriesByHabit();
                        System.Console.WriteLine("Press any key to close...");
                        Console.ReadLine();
                        break;

                    case "7":
                        AddNewHabitEntry();
                        break;

                    case "8":
                        UpdateItem("habit entry");
                        break;

                    case "9":
                        DeleteItem("habit entry");
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
        static void DisplayHabitEntriesTable(long? _habitId = null)
        {
            var habitEntriesList = db.GetHabitEntries(_habitId);

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
                    entry.Date.PadRight(15) +
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

            try
            {
                db.InsertHabit(name, unit);
                Console.WriteLine("Habit added successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Some error has occured: {e.Message} @ {e.StackTrace}");
            }
            System.Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadLine();

        }
        static void AddNewHabitEntry()
        {
            Console.WriteLine("Which habit do you want to record an entry for?");
            string? habitName = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(habitName) || habitName.Length < 2)
            {
                System.Console.WriteLine("Please type at least 2 charachers");
                habitName = Console.ReadLine();
            }

            Habit? habitSelected = db.GetHabitByName(habitName);

            if (habitSelected == null)
            {
                Console.WriteLine($"No habit found with the name '{habitName}'. Consider adding it first.");
                return;
            }

            Console.WriteLine($"Recording entry for: {habitSelected.Name} ({habitSelected.Unit})");

            Console.WriteLine($"Enter quantity in {habitSelected.Unit}:");
            string? quantityInput = Console.ReadLine();

            int quantity;

            while (!int.TryParse(quantityInput, out quantity) || quantity <= 0)
            {
                Console.WriteLine("Please enter a valid positive number.");
                quantityInput = Console.ReadLine();
            }

            Console.WriteLine("Enter notes, or press Enter to skip:");
            string? notes = Console.ReadLine();

            try
            {
                db.InsertHabitEntry(habitSelected.ID, quantity, notes);
                Console.WriteLine("Habit entry added successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Some error has occured: {e.Message} @ {e.StackTrace}");
            }
            System.Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadLine();

        }
        static void DeleteItem(string itemName)
        {
            if (itemName == null)
            {
                System.Console.WriteLine("Please provide an item name");
                return;
            }

            string? selectedId;
            long id;

            if (itemName?.ToLower() == "habit entry")
                DisplayHabitEntriesTable();
            if (itemName?.ToLower() == "habit")
                DisplayHabitsTable();

            System.Console.Write("Provide the id of the row you want to delete: ");
            selectedId = Console.ReadLine();
            while (!long.TryParse(selectedId, out id))
            {
                System.Console.WriteLine("\nInvalid argument!");
                System.Console.Write("Provide the id of the row you want to delete: ");
                selectedId = Console.ReadLine();
            }

            System.Console.WriteLine($"Are you sure you want to delete the item with ID={id}? (y/n)");
            string? response = Console.ReadLine();

            while (response == null || !Regex.IsMatch(response, "^(y|Y|n|N)"))
            {
                System.Console.WriteLine($"This is a yes/no question. What say you? (y/n)");
                response = Console.ReadLine();
            }

            if (response != null && Regex.IsMatch(response, "^(y|Y|n|N)"))
            {
                if (response.ToLower().Trim() == "y")
                {
                    if (itemName?.ToLower() == "habit entry")
                    {
                        try
                        {
                            db.DeleteHabitEntryById(id);
                            System.Console.WriteLine("Item deleted successfully");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Some error has occured: {e.Message} @ {e.StackTrace}");
                        }
                    }
                    if (itemName?.ToLower() == "habit")
                    {
                        System.Console.WriteLine($"Deleting the habit with ID={id} will delete all entries made for it. Continue? (y/n)");
                        response = Console.ReadLine();
                        while (response == null || !Regex.IsMatch(response, "^(y|Y|n|N)"))
                        {
                            System.Console.WriteLine($"This is a yes/no question. What say you? (y/n)");
                            response = Console.ReadLine();
                        }

                        if (response != null && Regex.IsMatch(response, "^(y|Y|n|N)"))
                        {
                            if (response.ToLower().Trim() == "y")
                            {
                                try
                                {
                                    db.DeleteHabitById(id);
                                    System.Console.WriteLine("Item deleted successfully");

                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine($"Some error has occured: {e.Message} @ {e.StackTrace}");
                                }
                            }
                        }
                    }
                }
                if (response?.ToLower().Trim() == "n")
                {
                    System.Console.WriteLine("Operation cancelled. Press any key to return to the main menu...");
                    Console.ReadLine();
                    return;
                }
            }
            System.Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadLine();
        }
        static void UpdateItem(string itemName)
        {
            string? enteredId;
            long id;
            if (itemName?.ToLower() == "habit")
            {
                DisplayHabitsTable();
                System.Console.WriteLine("\nEnter the ID of the habit you want to edit: ");
                enteredId = Console.ReadLine();

                while (!long.TryParse(enteredId, out id) || id <= 0)
                {
                    Console.WriteLine("Please enter a valid positive number.");
                    enteredId = Console.ReadLine();
                }
                Habit? selectedHabit = db.GetHabitById(id);

                if (selectedHabit == null)
                {
                    Console.WriteLine($"No habit found with ID={id}.");
                    Console.WriteLine("Press any key to return to the main menu...");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine($"Current name: {selectedHabit.Name}");
                Console.Write("Enter new habit name, or press Enter to keep current: ");
                string newName = Console.ReadLine()?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(newName))
                    newName = selectedHabit.Name;

                while (newName.Length < 2)
                {
                    Console.WriteLine("Please type at least 2 characters.");
                    Console.Write("Enter new habit name, or press Enter to keep current: ");
                    newName = Console.ReadLine()?.Trim() ?? "";

                    if (string.IsNullOrWhiteSpace(newName))
                        newName = selectedHabit.Name;
                }

                Console.WriteLine($"Current unit: {selectedHabit.Unit}");
                Console.Write("Enter new unit, or press Enter to keep current: ");
                string newUnit = Console.ReadLine()?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(newUnit))
                    newUnit = selectedHabit.Unit;

                try
                {
                    db.UpdateHabit(id, newName, newUnit);
                    Console.WriteLine("Habit updated successfully.");
                }
                catch (SqliteException e) when (e.SqliteErrorCode == 19)
                {
                    Console.WriteLine("Update failed. Another habit already uses that name.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Some error has occured: {e.Message} @ {e.StackTrace}");
                }

                Console.WriteLine("Press any key to return to the main menu...");
                Console.ReadLine();
                return;

            }
            if (itemName?.ToLower() == "habit entry")
            {
                DisplayHabitEntriesTable();
                System.Console.WriteLine("\nEnter the ID of the habit entry you want to edit: ");
                enteredId = Console.ReadLine();

                while (!long.TryParse(enteredId, out id) || id <= 0)
                {
                    Console.WriteLine("Please enter a valid positive number.");
                    enteredId = Console.ReadLine();
                }
                HabitEntry? selectedHabitEntry = db.GetHabitEntryById(id);

                if (selectedHabitEntry == null)
                {
                    Console.WriteLine($"No habit entry found with ID={selectedHabitEntry}.");
                    Console.WriteLine("Press any key to return to the main menu...");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine($"Current quantity: {selectedHabitEntry.Quantity}");
                Console.Write("Enter new quantity, or press Enter to keep current: ");
                string? quantityInput = Console.ReadLine();
                int newQuantity;
                if (string.IsNullOrWhiteSpace(quantityInput))
                    newQuantity = selectedHabitEntry.Quantity;
                else
                {
                    while (!int.TryParse(quantityInput, out newQuantity) || newQuantity <= 0)
                    {
                        Console.WriteLine("Please enter a valid positive number.");
                        Console.Write("Enter new quantity, or press Enter to keep current: ");
                        quantityInput = Console.ReadLine();
                    }

                }

                Console.WriteLine($"Current notes: {selectedHabitEntry.Notes ?? "(none)"}");
                Console.Write("Enter new notes, press Enter to keep current, or type CLEAR to remove notes: ");
                string? newNotes = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(newNotes))
                    newNotes = selectedHabitEntry.Notes;

                try
                {
                    db.UpdateHabitEntry(id, newQuantity, newNotes);
                    Console.WriteLine("Habit updated successfully.");
                }
                catch (SqliteException e) when (e.SqliteErrorCode == 19)
                {
                    Console.WriteLine("Update failed. Another habit already uses that name.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Some error has occured: {e.Message} @ {e.StackTrace}");
                }

                Console.WriteLine("Press any key to return to the main menu...");
                Console.ReadLine();
                return;

            }
        }
        static void DisplayHabitEntriesByHabit()
        {
            // get habit by name first
            Console.WriteLine("Which habit do you want to display its entries for?");
            string? habitName = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(habitName) || habitName.Length < 2)
            {
                System.Console.WriteLine("Please type at least 2 charachers");
                habitName = Console.ReadLine();
            }

            Habit? habitSelected = db.GetHabitByName(habitName);

            if (habitSelected == null)
            {
                Console.WriteLine($"No habit found with the name '{habitName}'. Consider adding it first.");
                return;
            }

            DisplayHabitEntriesTable(habitSelected.ID);
        }
    }
}