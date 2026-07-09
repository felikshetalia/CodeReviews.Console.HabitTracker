using CodeReviews_Console_HabitTracker;
using Microsoft.Data.Sqlite;
using NUnit.Framework;
namespace HabitTracker.Tests;

[TestFixture]
[NonParallelizable]
public class SQL_UnitTests
{
    private DBManager _db = null!;

    private static readonly List<TestCaseData> HabitRelatedCases =
    [
        new TestCaseData("Water", "glasses"),
        new TestCaseData("Reading", "pages"),
        new TestCaseData("Running", "km")
    ];

    private static IEnumerable<TestCaseData> HabitEntryRelatedCases =
    [
        new TestCaseData("Pushups", "reps", "01-02-2025", 20, "easy session"),
        new TestCaseData("Running", "km", "02-02-2025", 5, "short run"),
        new TestCaseData("Reading", "pages", "03-02-2025", 30, "evening reading")
    ];

    private static IEnumerable<TestCaseData> HabitEntryRelatedCasesWithoutNotes =
    [
        new TestCaseData("Pushups", "reps", "01-02-2025", 20),
        new TestCaseData("Running", "km", "02-02-2025", 5),
        new TestCaseData("Reading", "pages", "03-02-2025", 30)
    ];

    private static readonly List<TestCaseData> IdRelatedCases =
    [
        new TestCaseData(0L),
        new TestCaseData(-1L),
        new TestCaseData(long.MaxValue)
    ];

    [SetUp]
    public void SetUp()
    {
        DeleteDatabaseFile();

        _db = new DBManager();
        _db.InitializeDB();
    }

    [TearDown]
    public void TearDown()
    {
        DeleteDatabaseFile();
    }
    private static void DeleteDatabaseFile()
    {
        SqliteConnection.ClearAllPools();

        var testDirectoryPath = Path.Combine(
            TestContext.CurrentContext.TestDirectory,
            "habit-tracker.db");

        var currentDirectoryPath = Path.GetFullPath("habit-tracker.db");

        TestContext.Out.WriteLine($"TestDirectory DB path: {testDirectoryPath}");
        TestContext.Out.WriteLine($"CurrentDirectory DB path: {currentDirectoryPath}");

        if (File.Exists(testDirectoryPath))
            File.Delete(testDirectoryPath);

        if (File.Exists(currentDirectoryPath))
            File.Delete(currentDirectoryPath);
    }

    [Test]
    public void InitializeDB_CreatesDatabaseFile()
    {
        DeleteDatabaseFile();

        _db = new DBManager();
        _db.InitializeDB();

        var dbPath = Path.Combine(
            TestContext.CurrentContext.TestDirectory,
            "habit-tracker.db");

        Assert.That(File.Exists(dbPath), Is.True);
    }

    [TestCaseSource(nameof(HabitRelatedCases))]
    public void InsertHabit_ThenGetHabitByName_ReturnsHabit(string name, string unit)
    {
        _db.InsertHabit(name, unit);

        var habit = _db.GetHabitByName(name);

        Assert.That(habit, Is.Not.Null);
        Assert.That(habit!.Name, Is.EqualTo(name));
        Assert.That(habit.Unit, Is.EqualTo(unit));
        Assert.That(habit.CreatedAt, Is.Not.Null.And.Not.Empty);
    }

    [TestCaseSource(nameof(HabitRelatedCases))]
    public void GetAllHabits_AfterInsert_ReturnsInsertedHabit(string name, string unit)
    {
        _db.InsertHabit(name, unit);

        var habits = _db.GetHabits();

        Assert.That(habits, Has.Count.EqualTo(1));
        Assert.That(habits[0].Name, Is.EqualTo(name));
        Assert.That(habits[0].Unit, Is.EqualTo(unit));
    }

    [TestCaseSource(nameof(HabitEntryRelatedCases))]
    public void InsertHabitEntry_ThenGetHabitEntries_ReturnsEntry(string habitName, string unit, string date, int quantity, string notes)
    {
        _db.InsertHabit(habitName, unit);

        var habit = _db.GetHabitByName(habitName);

        Assert.That(habit, Is.Not.Null);

        _db.InsertHabitEntry(
            habit!.ID,
            date,
            quantity,
            notes);

        var entries = _db.GetHabitEntries(habit.ID);

        Assert.That(entries, Has.Count.EqualTo(1));
        Assert.That(entries[0].HabitID, Is.EqualTo(habit.ID));
        Assert.That(entries[0].Date, Is.EqualTo(date));
        Assert.That(entries[0].Quantity, Is.EqualTo(quantity));
        Assert.That(entries[0].Notes, Is.EqualTo(notes));
    }

    [TestCaseSource(nameof(HabitEntryRelatedCases))]
    public void UpdateHabitEntry_ChangesStoredEntry(string habitName, string unit, string date, int quantity, string notes)
    {
        _db.InsertHabit(habitName, unit);

        var habit = _db.GetHabitByName(habitName);

        Assert.That(habit, Is.Not.Null);

        _db.InsertHabitEntry(
            habit!.ID,
            date,
            quantity,
            notes);

        var entry = _db.GetHabitEntries(habit.ID).Single();

        _db.UpdateHabitEntry(
            entry.ID,
            date,
            quantity,
            notes);

        var updatedEntry = _db.GetHabitEntryById(entry.ID);

        Assert.That(updatedEntry, Is.Not.Null);
        Assert.That(updatedEntry!.Date, Is.EqualTo(date));
        Assert.That(updatedEntry.Quantity, Is.EqualTo(quantity));
        Assert.That(updatedEntry.Notes, Is.EqualTo(notes));
    }

    [TestCaseSource(nameof(HabitRelatedCases))]
    public void DeleteHabitById_RemovesHabit(string name, string unit)
    {
        _db.InsertHabit(name, unit);

        var habit = _db.GetHabitByName(name);

        Assert.That(habit, Is.Not.Null);

        _db.DeleteHabitById(habit!.ID);

        var deletedHabit = _db.GetHabitByName(name);

        Assert.That(deletedHabit, Is.Null);
    }

    [TestCaseSource(nameof(HabitEntryRelatedCases))]
    public void DeleteHabitById_RemovesRelatedEntries(string habitName, string unit, string date, int quantity, string notes)
    {
        _db.InsertHabit(habitName, unit);

        var habit = _db.GetHabitByName(habitName);

        Assert.That(habit, Is.Not.Null);

        _db.InsertHabitEntry(
            habit!.ID,
            date,
            quantity,
            notes);

        var entriesBeforeDelete = _db.GetHabitEntries(habit.ID);

        Assert.That(entriesBeforeDelete, Has.Count.EqualTo(1));

        _db.DeleteHabitById(habit.ID);

        var entriesAfterDelete = _db.GetHabitEntries(habit.ID);

        Assert.That(entriesAfterDelete, Is.Empty);
    }

    [TestCaseSource(nameof(HabitRelatedCases))]
    public void InsertHabit_WithDuplicateName_ThrowsSqliteException(string name, string unit)
    {
        _db.InsertHabit(name, unit);

        Assert.Throws<SqliteException>(() =>
            _db.InsertHabit(name, unit));
    }

    [TestCaseSource(nameof(HabitEntryRelatedCasesWithoutNotes))]
    public void InsertHabitEntry_WithEmptyNotes_StoresNull(string habitName, string unit, string date, int quantity)
    {
        _db.InsertHabit(habitName, unit);

        var habit = _db.GetHabitByName(habitName);

        Assert.That(habit, Is.Not.Null);

        _db.InsertHabitEntry(
            habit!.ID,
            date,
            quantity,
            null);

        var entries = _db.GetHabitEntries(habit.ID);

        Assert.That(entries, Has.Count.EqualTo(1));
        Assert.That(entries[0].Notes, Is.Null);
    }

    [Test]
    public void DeleteHabitEntryById_RemovesOnlyThatEntry()
    {
        _db.InsertHabit("Reading", "pages");

        var habit = _db.GetHabitByName("Reading");

        Assert.That(habit, Is.Not.Null);

        _db.InsertHabitEntry(habit!.ID, "01-02-2025", 10, "first");
        _db.InsertHabitEntry(habit.ID, "02-02-2025", 20, "second");

        var entries = _db.GetHabitEntries(habit.ID);

        Assert.That(entries, Has.Count.EqualTo(2));

        var entryToDelete = entries.Single(e => e.Date == "01-02-2025");

        _db.DeleteHabitEntryById(entryToDelete.ID);

        var remainingEntries = _db.GetHabitEntries(habit.ID);

        Assert.That(remainingEntries, Has.Count.EqualTo(1));
        Assert.That(remainingEntries[0].Date, Is.EqualTo("02-02-2025"));
        Assert.That(remainingEntries[0].Quantity, Is.EqualTo(20));
    }

    [Test]
    public void GetHabitByName_WhenHabitDoesNotExist_ReturnsNull()
    {
        var habit = _db.GetHabitByName("Nonexistent");

        Assert.That(habit, Is.Null);
    }

    [TestCaseSource(nameof(IdRelatedCases))]
    public void GetHabitEntryById_WhenEntryDoesNotExist_ReturnsNull(long id)
    {
        var entry = _db.GetHabitEntryById(id);

        Assert.That(entry, Is.Null);
    }

    [TestCaseSource(nameof(HabitRelatedCases))]
    public void GetHabitEntries_WhenHabitHasNoEntries_ReturnsEmptyList(string name, string unit)
    {
        _db.InsertHabit(name, unit);

        var habit = _db.GetHabitByName(name);

        Assert.That(habit, Is.Not.Null);

        var entries = _db.GetHabitEntries(habit!.ID);

        Assert.That(entries, Is.Empty);
    }

    [Test]
    public void UpdateHabitEntry_WithEmptyNotes_StoresNull()
    {
        _db.InsertHabit("Reading", "pages");

        var habit = _db.GetHabitByName("Reading");

        Assert.That(habit, Is.Not.Null);

        _db.InsertHabitEntry(
            habit!.ID,
            "01-02-2025",
            10,
            "initial note");

        var entry = _db.GetHabitEntries(habit.ID).Single();

        _db.UpdateHabitEntry(
            entry.ID,
            "02-02-2025",
            20,
            "");

        var updatedEntry = _db.GetHabitEntryById(entry.ID);

        Assert.That(updatedEntry, Is.Not.Null);
        Assert.That(updatedEntry!.Date, Is.EqualTo("02-02-2025"));
        Assert.That(updatedEntry.Quantity, Is.EqualTo(20));
        Assert.That(updatedEntry.Notes, Is.Null);
    }

    [TestCaseSource(nameof(IdRelatedCases))]
    public void DeleteHabitById_WithUnknownId_DoesNotThrow(long id)
    {
        Assert.DoesNotThrow(() => _db.DeleteHabitById(id));
    }

    [TestCaseSource(nameof(IdRelatedCases))]
    public void DeleteHabitEntryById_WithUnknownId_DoesNotThrow(long id)
    {
        Assert.DoesNotThrow(() => _db.DeleteHabitEntryById(id));
    }

    [TestCaseSource(nameof(IdRelatedCases))]
    public void UpdateHabitEntry_WithUnknownId_DoesNotThrow(long id)
    {
        Assert.DoesNotThrow(() =>
            _db.UpdateHabitEntry(
                id,
                "01-02-2025",
                10,
                "note"));
    }

    [TestCaseSource(nameof(IdRelatedCases))]
    public void InsertHabitEntry_WithUnknownHabitId_ThrowsSqliteException(long id)
    {
        Assert.Throws<SqliteException>(() =>
            _db.InsertHabitEntry(
                id,
                "01-02-2025",
                10,
                "invalid habit"));
    }

    [Test]
    public void GetHabits_AfterMultipleInserts_ReturnsAllHabits()
    {
        _db.InsertHabit("Water", "glasses");
        _db.InsertHabit("Reading", "pages");
        _db.InsertHabit("Running", "km");

        var habits = _db.GetHabits();

        Assert.That(habits, Has.Count.EqualTo(3));
        Assert.That(habits.Select(h => h.Name), Does.Contain("Water"));
        Assert.That(habits.Select(h => h.Name), Does.Contain("Reading"));
        Assert.That(habits.Select(h => h.Name), Does.Contain("Running"));
    }

    [Test]
    public void GetHabitEntries_ReturnsOnlyEntriesForRequestedHabit()
    {
        _db.InsertHabit("Water", "glasses");
        _db.InsertHabit("Reading", "pages");

        var water = _db.GetHabitByName("Water");
        var reading = _db.GetHabitByName("Reading");

        Assert.That(water, Is.Not.Null);
        Assert.That(reading, Is.Not.Null);

        _db.InsertHabitEntry(water!.ID, "01-02-2025", 8, "water entry");
        _db.InsertHabitEntry(reading!.ID, "01-02-2025", 20, "reading entry");

        var waterEntries = _db.GetHabitEntries(water.ID);

        Assert.That(waterEntries, Has.Count.EqualTo(1));
        Assert.That(waterEntries[0].HabitID, Is.EqualTo(water.ID));
        Assert.That(waterEntries[0].Notes, Is.EqualTo("water entry"));
    }
}

