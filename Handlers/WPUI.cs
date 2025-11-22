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
                    string weightText = ex.Weight.HasValue ? $"{ex.Weight} kg" : "-";

                    dayTable.AddRow(
                        ex.Name,
                        ex.Sets.ToString(),
                        ex.Reps.ToString(),
                        weightText,
                        ex.RestTime.ToString()
                    );
                }

                AnsiConsole.Write(dayTable);
                AnsiConsole.WriteLine();
            }
        }

        public static void UpdateSchedule(WorkoutPlan exercise)
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
            
            // temporary cpy of the exercise
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
            AnsiConsole.MarkupLine($"Weight: [blue]{selectedExercise.Weight} kg[/]");
            AnsiConsole.MarkupLine($"Resttime: [blue]{selectedExercise.RestTime} sek[/]");
            bool updateWorkoutplan = true;

            while (updateWorkoutplan)
            {
                AnsiConsole.WriteLine();
                var SelectExerciseUpdateOption = new SelectionPrompt<string>()
                    .Title("Which part of the exercise would you like to update?")
                    .PageSize(10)
                    .AddChoices("Sets", "Reps", "Weight", "Resttime", "[green]Done[/]", "[red]Exit[/]");

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
                    case "Resttime":
                    tempExercise.Sets = PromptForInt("[bold]New Resttime: [/]");
                    break;
                    case "[green]Done[/]":
                    AnsiConsole.Clear();
                    // if user press done, thats when the change happen
                    selectedExercise.Sets = tempExercise.Sets;
                    selectedExercise.Reps = tempExercise.Reps;
                    selectedExercise.Weight = tempExercise.Weight;
                    selectedExercise.RestTime = tempExercise.RestTime;
                    JSONFileHanldler<WorkoutPlan>.Save(Path.Combine($"Data/Users/{Session.CurrentUser.Id}", "workoutplans.json"),exercise);

                    AnsiConsole.MarkupLine("[green]Exercise updated![/]");
                    updateWorkoutplan = false;
                    return;
                    case "[red]Exit[/]":
                    AnsiConsole.Clear();
                    // if exit, no change
                    AnsiConsole.MarkupLine("[yellow]No changes saved.[/]");
                    return;
                }
            }
        }

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
                    selectedDay.Exercises.Remove(selectedExercise);

                    // Save changes
                    JSONFileHanldler<WorkoutPlan>.Save(
                        Path.Combine($"Data/Users/{Session.CurrentUser.Id}", "workoutplans.json"),
                        exercise
                    );

                    AnsiConsole.MarkupLine($"[green]Exercise '{selectedExercise.Name}' deleted successfully![/]");
                    break;

                case "[yellow]No, cancel[/]":
                    AnsiConsole.MarkupLine("[yellow]Deletion cancelled.[/]");
                    break;
            }
        }
    }      
}

