using SnackToSixPack.Classes;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SnackToSixPack.Handlers
{
    public class WPUI
    {
        public static void ShowWPUI(WorkoutPlan plan)
        {

            //Huvudtabell
            var header = new Table()
            {
                Border = TableBorder.Rounded,
                Expand = true
            };

            header.Title = new TableTitle($"[bold yellow]{plan.PlanName}[/]");
            header.AddColumn(new TableColumn("[bold]Info[/]"));

            header.AddRow($"[bold]Mål:[/]\n{plan.Goal}");
            header.AddRow($"[bold]Period:[/] {plan.StartDate.ToShortDateString()} till {plan.EndDate.ToShortDateString()}");
            header.AddEmptyRow();

            AnsiConsole.Write(header);

            //En tabell per träningsdag
            foreach (var day in plan.Workouts)
            {
                var dayTable = new Table()
                {
                    Border = TableBorder.Rounded,
                    Expand = true
                };
                
                dayTable.Title = new TableTitle($"[yellow]{day.DayOfWeek} – {day.Title}[/]");

                dayTable.AddColumn(new TableColumn("[bold]Övning[/]").Centered().Width(40));
                dayTable.AddColumn(new TableColumn("[bold]Set[/]").Centered());
                dayTable.AddColumn(new TableColumn("[bold]Reps[/]").Centered());
                dayTable.AddColumn(new TableColumn("[bold]Vikt[/]").Centered());
                dayTable.AddColumn(new TableColumn("[bold]Vila (sek)[/]").Centered());

                foreach (var ex in day.Exercises)
                {
                    string setsText = ex.Sets > 0 ? $"{ex.Sets}" : "-";
                    string repsText = ex.Reps > 0 ? $"{ex.Reps}" : "-";
                    string weightText = ex.Weight > 0 ? $"{ex.Weight} kg" : "-";

                    dayTable.AddRow(
                        ex.Name,
                        setsText,
                        repsText,
                        weightText,
                        ex.RestTime.ToString()
                    );
                }

                AnsiConsole.Write(dayTable);
                AnsiConsole.WriteLine();
            }
        }

        public static void UpdateExercise(WorkoutPlan exercise)
        {
            // Create a list with days
            var dayNames = exercise.Workouts
                .Select(d => d.DayOfWeek)
                .ToList();

            // Prompt
            var updateWorkoutplanDay = new SelectionPrompt<string>()
                .Title("Choose which day to edit")
                .PageSize(10)
                .AddChoices(dayNames);

            string dayChoice = AnsiConsole.Prompt(updateWorkoutplanDay);

            var selectedDay = exercise.Workouts
                .First(d => d.DayOfWeek == dayChoice);

            if (selectedDay.Exercises.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]There are no exercises on this day.[/]");
                AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
                Console.ReadKey(true);
                return;
            }
            // Create a list with exercises
            var exerciseNames = selectedDay.Exercises
                .Select(n => n.Name)
                .ToList();

            //Prompt
            var updateExercise = new SelectionPrompt<string>()
                .Title("Which exercise would you like to update?")
                .PageSize(10)
                .AddChoices(exerciseNames);
            
            string workoutChoice = AnsiConsole.Prompt(updateExercise);

            var selectedExercise = selectedDay.Exercises
                .First(e => e.Name == workoutChoice);
            
            // temporary copy of the exercise
            var tempExercise = new Exercise
            {
                Name = selectedExercise.Name,
                Sets = selectedExercise.Sets,
                Reps = selectedExercise.Reps,
                Weight = selectedExercise.Weight,
                RestTime = selectedExercise.RestTime
            };

            AnsiConsole.MarkupLine("[bold yellow]You selected: [/]" + selectedExercise.Name);
            AnsiConsole.MarkupLine($"Name: [blue]{selectedExercise.Name}[/]");
            AnsiConsole.MarkupLine($"Sets: [blue]{selectedExercise.Sets}[/]");
            AnsiConsole.MarkupLine($"Reps: [blue]{selectedExercise.Reps}[/]");
            AnsiConsole.MarkupLine(selectedExercise.Weight != null ? $"Weight: [blue]{selectedExercise.Weight} kg[/]": "Weight: -");
            AnsiConsole.MarkupLine(selectedExercise.RestTime != null ? $"Resttime: [blue]{selectedExercise.RestTime} sek[/]": "Resttime: -");

            bool updateWorkoutplan = true;

            while (updateWorkoutplan)
            {
                AnsiConsole.WriteLine();
                var SelectExerciseUpdateOption = new SelectionPrompt<string>()
                    .Title("Which part of the exercise would you like to update?")
                    .PageSize(10)
                    .AddChoices("Sets", "Reps", "Weight", "Rest time", "[yellow]Undo Last Change[/]", "[green]Done[/]", "[red]Exit[/]");

                string updateOption = AnsiConsole.Prompt(SelectExerciseUpdateOption);

                switch (updateOption)
                {
                    case "Sets":
                    tempExercise.Sets = PromptForInt("[bold]New Sets: [/]");
                    break;
                    case "Reps":
                    tempExercise.Reps = PromptForInt("[bold]New Reps: [/]");
                    break;
                    case "Weight":            
                    while (true)
                    {
                        AnsiConsole.Markup("[bold]New Weight (kg): [/]");
                        string input = Console.ReadLine();

                        if (double.TryParse(input, out double weight))
                        {
                            tempExercise.Weight = weight;
                            break;
                        }

                        AnsiConsole.MarkupLine("[red]Invalid number, please try again.[/]");
                    }
                    break;
                    case "Rest time":
                    tempExercise.RestTime = PromptForInt("[bold]New Rest time: [/]");
                    break;
                    case "[yellow]Undo Last Change[/]":
                    if (undoStack.Count > 0)
                    {
                        // "Pop" tar tillbaka den gamla versionen
                        var previous = undoStack.Pop();

                        tempExercise.Name = previous.Name;
                        tempExercise.Sets = previous.Sets;
                        tempExercise.Reps = previous.Reps;
                        tempExercise.Weight = previous.Weight;
                        tempExercise.RestTime = previous.RestTime;

                        AnsiConsole.MarkupLine("[green]Reverted to previous version![/]");
                        AnsiConsole.MarkupLine("[grey]Press ENTER to continue.[/]");
                        Console.ReadKey(true);
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[yellow]No changes to undo.[/]");
                    }
                    break; 
                    
                    case "[green]Done[/]":
                        // Spara originalvärdena i undo-stacken
                        undoStack.Push(new Exercise
                        {
                            Name = selectedExercise.Name,
                            Sets = selectedExercise.Sets,
                            Reps = selectedExercise.Reps,
                            Weight = selectedExercise.Weight,
                            RestTime = selectedExercise.RestTime
                        });

                        // Applicera ändringarna
                        selectedExercise.Sets = tempExercise.Sets;
                        selectedExercise.Reps = tempExercise.Reps;
                        selectedExercise.Weight = tempExercise.Weight;
                        selectedExercise.RestTime = tempExercise.RestTime;

                        JSONFileHanldler<WorkoutPlan>.Save(Path.Combine($"Data/Users/{Session.CurrentUser.Id}", "workoutplans.json"),exercise);

                        AnsiConsole.MarkupLine("[green]Exercise updated![/]");
                        AnsiConsole.MarkupLine("[grey]Press ENTER to continue.[/]");
                        Console.ReadKey(true);
                        return;
                    
                    case "[red]Exit[/]":
                    AnsiConsole.Clear();
                    // if exit, no change
                    AnsiConsole.MarkupLine("[yellow]No changes saved.[/]");
                    AnsiConsole.MarkupLine("[grey]Press ENTER to continue.[/]");
                    Console.ReadKey(true);
                    return;
                }
            }
        }
        public static Stack<Exercise> undoStack = new Stack<Exercise>();

/*public static Exercise CloneExercise(Exercise ex)
{
    return new Exercise
    {
        Name = ex.Name,
        Sets = ex.Sets,
        Reps = ex.Reps,
        Weight = ex.Weight,
        RestTime = ex.RestTime
    };
}

public static void UpdateExercise(WorkoutPlan plan)
{
    // Pick day
    var dayNames = plan.Workouts.Select(d => d.DayOfWeek).ToList();
    var dayPrompt = new SelectionPrompt<string>()
        .Title("Choose which day to edit")
        .AddChoices(dayNames);

    string dayChoice = AnsiConsole.Prompt(dayPrompt);
    var selectedDay = plan.Workouts.First(d => d.DayOfWeek == dayChoice);

    if (selectedDay.Exercises.Count == 0)
    {
        AnsiConsole.MarkupLine("[yellow]There are no exercises on this day.[/]");
        Console.ReadKey();
        return;
    }

    // Pick exercise
    var exerciseNames = selectedDay.Exercises.Select(e => e.Name).ToList();
    var exercisePrompt = new SelectionPrompt<string>()
        .Title("Which exercise would you like to update?")
        .AddChoices(exerciseNames);

    string exerciseChoice = AnsiConsole.Prompt(exercisePrompt);
    var selectedExercise = selectedDay.Exercises.First(e => e.Name == exerciseChoice);

    // Save ORIGINAL before anything is changed
    undoStack.Push(CloneExercise(selectedExercise));

    // Create temp copy for editing
    var temp = CloneExercise(selectedExercise);

    // Display current
    AnsiConsole.MarkupLine($"[yellow]Editing: {selectedExercise.Name}[/]");
    AnsiConsole.MarkupLine($"Sets: [blue]{selectedExercise.Sets}[/]");
    AnsiConsole.MarkupLine($"Reps: [blue]{selectedExercise.Reps}[/]");
    AnsiConsole.MarkupLine(selectedExercise.Weight != null
        ? $"Weight: [blue]{selectedExercise.Weight} kg[/]"
        : "Weight: -");
    AnsiConsole.MarkupLine($"Rest time: [blue]{selectedExercise.RestTime}[/] sec");

    bool editing = true;

    while (editing)
    {
        var option = new SelectionPrompt<string>()
            .Title("What do you want to update?")
            .AddChoices("Sets", "Reps", "Weight", "Rest time", "[yellow]Undo Last Change[/]", "[green]Done[/]", "[red]Exit[/]");

        string choice = AnsiConsole.Prompt(option);

        switch (choice)
        {
            case "Sets":
                temp.Sets = PromptForInt("New Sets:");
                break;

            case "Reps":
                temp.Reps = PromptForInt("New Reps:");
                break;

            case "Weight":
                while (true)
                {
                    AnsiConsole.Markup("New Weight (kg) or '-' for none: ");
                    string w = Console.ReadLine();

                    if (w == "-")
                    {
                        temp.Weight = null;
                        break;
                    }

                    if (double.TryParse(w, out double weight))
                    {
                        temp.Weight = weight;
                        break;
                    }

                    AnsiConsole.MarkupLine("[red]Invalid input.[/]");
                }
                break;

            case "Rest time":
                temp.RestTime = PromptForInt("New Rest time (sec):");
                break;

            case "[yellow]Undo Last Change[/]":
                if (undoStack.Count > 0)
                {
                    var previous = undoStack.Pop();

                    selectedExercise.Name = previous.Name;
                    selectedExercise.Sets = previous.Sets;
                    selectedExercise.Reps = previous.Reps;
                    selectedExercise.Weight = previous.Weight;
                    selectedExercise.RestTime = previous.RestTime;

                    JSONFileHanldler<WorkoutPlan>.Save(
                        Path.Combine($"Data/Users/{Session.CurrentUser.Id}", "workoutplans.json"), plan);

                    AnsiConsole.MarkupLine("[green]Reverted to previous version![/]");
                    Console.ReadKey();
                    return;
                }
                else
                {
                    AnsiConsole.MarkupLine("[yellow]No changes to undo.[/]");
                }
                break;

            case "[green]Done[/]":
                // Apply changes
                selectedExercise.Name = temp.Name;
                selectedExercise.Sets = temp.Sets;
                selectedExercise.Reps = temp.Reps;
                selectedExercise.Weight = temp.Weight;
                selectedExercise.RestTime = temp.RestTime;

                JSONFileHanldler<WorkoutPlan>.Save(
                    Path.Combine($"Data/Users/{Session.CurrentUser.Id}", "workoutplans.json"), plan);

                AnsiConsole.MarkupLine("[green]Exercise updated![/]");
                Console.ReadKey();
                return;

            case "[red]Exit[/]":
                AnsiConsole.MarkupLine("[yellow]No changes saved.[/]");
                Console.ReadKey();
                return;
        }
    }
}*/


        private static int PromptForInt(string message)
        {
            while (true)
            {
                 AnsiConsole.Markup($"[bold]{message}[/] ");
                string input = Console.ReadLine();

                if (int.TryParse(input, out int value))
                {
                    return value;
                }

                AnsiConsole.MarkupLine("[red]Invalid number, please try again.[/]");
            }
        }

        public static void RemoveExercise(WorkoutPlan exercise)
        {
            var dayNames = exercise.Workouts
                .Select(d => d.DayOfWeek)
                .ToList();

            var updateWorkoutplanDay = new SelectionPrompt<string>()
                .Title("Choose which day to edit")
                .PageSize(10)
                .AddChoices(dayNames);

            string dayChoice = AnsiConsole.Prompt(updateWorkoutplanDay);

            var selectedDay = exercise.Workouts
                .First(d => d.DayOfWeek == dayChoice);
            
            if (selectedDay.Exercises.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]There are no exercises on this day.[/]");
                AnsiConsole.MarkupLine("[grey]Press ENTER to continue.[/]");
                Console.ReadKey(true);
                return;
            }

            var exerciseNames = selectedDay.Exercises
                .Select(n => n.Name)
                .ToList();

            var updateExercise = new SelectionPrompt<string>()
                .Title("Which exercise would you like to delete?")
                .PageSize(10)
                .AddChoices(exerciseNames);
            
            string workoutChoice = AnsiConsole.Prompt(updateExercise);

            var selectedExercise = selectedDay.Exercises
                .First(e => e.Name == workoutChoice);
            
                // Confirmation menu
            var confirmDelete = new SelectionPrompt<string>()
                .Title($"Are you sure you want to delete [red]{selectedExercise.Name}[/]?")
                .AddChoices("[green]Yes, delete[/]", "[yellow]No, cancel[/]");

            string confirmChoice = AnsiConsole.Prompt(confirmDelete);

            switch (confirmChoice)
            {
                case "[green]Yes, delete[/]":
                    // pushar till undo stack innan den raderas
                    undoRemoveStack.Push((selectedDay.DayOfWeek, selectedExercise));
                    //raderar övningen
                    selectedDay.Exercises.Remove(selectedExercise);

                    // Save changes
                    JSONFileHanldler<WorkoutPlan>.Save(
                        Path.Combine($"Data/Users/{Session.CurrentUser.Id}", "workoutplans.json"),
                        exercise
                    );

                    AnsiConsole.MarkupLine($"[green]Exercise '{selectedExercise.Name}' deleted successfully![/]");
                    AnsiConsole.MarkupLine("[grey]Press ENTER to continue.[/]");
                    Console.ReadKey(true);
                    break;

                case "[yellow]No, cancel[/]":
                    AnsiConsole.MarkupLine("[yellow]Deletion cancelled.[/]");
                    AnsiConsole.MarkupLine("[grey]Press ENTER to continue.[/]");
                    Console.ReadKey(true);
                    break;
            }
        }
        // Remove metoden behvöer veta vilken dag övningen tillhör
        public static Stack<(string Day, Exercise Exercise)> undoRemoveStack  = new Stack<(string, Exercise)>();
      
        public static void UndoLastDelete(WorkoutPlan plan)
        {
            if (undoRemoveStack.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No deleted exercise to restore.[/]");
                AnsiConsole.MarkupLine("[grey]Press ENTER to continue.[/]");
                Console.ReadKey(true);
                return;
            }

            // 2. Titta på vad som ska återskapas (poppa inte ännu)
            var (dayName, deletedExercise) = undoRemoveStack.Peek();

            var confirmMenu = new SelectionPrompt<string>()
                .Title($"[yellow]Restore deleted exercise: {deletedExercise.Name} (Day: {dayName})?[/]")
                .AddChoices("[green]Yes, restore[/]", "[yellow]No, cancel[/]");

            string confirm = AnsiConsole.Prompt(confirmMenu);

            switch (confirm)
            {
                case "[green]Yes, restore[/]":
                    // 4. Pop från stacken nu när vi vet att användaren vill
                    undoRemoveStack.Pop();

                    // 5. Hitta rätt dag
                    var dayToRestore = plan.Workouts
                        .First(d => d.DayOfWeek == dayName);

                    // 6. Lägg tillbaka övningen
                    dayToRestore.Exercises.Add(deletedExercise);

                    // 7. Spara
                    JSONFileHanldler<WorkoutPlan>.Save(
                        Path.Combine($"Data/Users/{Session.CurrentUser.Id}", "workoutplans.json"),
                        plan
                    );

                    AnsiConsole.MarkupLine($"[green]Restored exercise: {deletedExercise.Name}[/]");
                    AnsiConsole.MarkupLine("[grey]Press ENTER to continue.[/]");
                    Console.ReadKey(true);
                    break;

                case "[yellow]No, cancel[/]":
                    AnsiConsole.MarkupLine("[yellow]Restore cancelled.[/]");
                    AnsiConsole.MarkupLine("[grey]Press ENTER to continue.[/]");
                    Console.ReadKey(true);
                    break;
            }
        }

    }
}

