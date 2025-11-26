using SnackToSixPack.Classes;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SnackToSixPack.Handlers
{
    public class MenuHandler
    {
        public static void ShowTitle()
        {
            // TITEL SCREEN
            AnsiConsole.Clear();
            AnsiConsole.Write(
            new FigletText("SnackToSixPack")
                    .Centered()
                    .Color(Color.BlueViolet));
        }

        public static async Task ShowMainMenu()
        {
            bool exit = false;
            while (!exit)
            {
                ShowTitle();

                var menu = new SelectionPrompt<string>()
                    .Title("[BlueViolet] Welcome to SnackToSixPack! Please choose an option:[/]")
                    .WrapAround(true)
                    .AddChoices("Login", "Register", "Quit");

                string choice = AnsiConsole.Prompt(menu);

                switch (choice)
                {
                    case "Login":
                        AuthForms.ShowLogInForm();

                        // Visa UserMenu bara om login lyckades
                        if (Session.CurrentUser != null)
                            await ShowUserMenu();
                        break;

                    case "Register":
                            await RegistrationHandler.Run();
                            if (Session.CurrentUser != null)
                            await ShowUserMenu();
                        break;

                    case "Quit":
                        exit = true;
                        break;
                }
            }
        }
        
        public static bool skipPause = false;

        public static async Task ShowUserMenu()
        {
            while (Session.CurrentUser != null)
            {
                AnsiConsole.Clear();
                ShowTitle();
                var menu = new SelectionPrompt<string>()
                    .Title("[cyan1] User Menu - Please choose an option:[/]")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                        "Show Profile",
                        "Update Profile",
                        "Schedule Options",
                        "Create Workout Plan",
                        "Log Out"
                    });
                string choice = AnsiConsole.Prompt(menu);
                switch (choice)
                {
                    case "Show Profile":
                        skipPause = true;
                        ProfileHandler.ShowProfile(Session.CurrentUser.Profile);
                        break;
                    case "Update Profile":
                        ProfileHandler profileHandler = new ProfileHandler();
                        profileHandler.UpdateProfile(Session.CurrentUser.Profile);
                        
                        break;
                    case "Schedule Options":
                        skipPause = true;
                        ScheduleHandler();
                        break;
                    case "Create Workout Plan":
                        await AIMenu();
                        break;
                    case "Log Out":
                        Session.CurrentUserLogout();
                        break;
                }
                if (!skipPause && Session.CurrentUser != null)
                {
                    AnsiConsole.WriteLine();
                    AnsiConsole.Markup("[grey]Press any key to return to the User Menu...[/]");
                    Console.ReadKey(true);
                }
                skipPause = false; // återställ inför nästa loop, gäller för det aktuella valet inte för alla kommande.
            }
        }
        
        public static async Task AIMenu()
        {
            bool running = true;

            while (running)
            {
                ShowTitle();
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Choose an option:")
                        .AddChoices("Generate a training schedule", "Quit"));

                switch (choice)
                {
                    case "Generate a training schedule":
                        await OpenAIHandler.AskAI();
                        break;

                    case "Quit":
                        running = false;
                        skipPause = true;
                        break;
                }
            }
        }

        public static void ScheduleHandler()
        {
            var planPath = Path.Combine($"Data/Users/{Session.CurrentUser.Id}", "workoutplans.json");
            WorkoutPlan plan;
            
            plan = JSONFileHanldler.Load<WorkoutPlan>(planPath);

            if (plan == null)
            {
                AnsiConsole.MarkupLine("[red]No workout plan generated yet.[/]");
                AnsiConsole.MarkupLine("[grey]Press ENTER to continue...[/]");
                Console.ReadKey(true);
                return;
            }
            
            bool running = true;
            bool hasDeleteExercise = false;

            while (running)
            {
                AnsiConsole.Clear();
                ShowTitle();
                var menu = new SelectionPrompt<string>()
                    .Title("[cyan]Schedule Menu - Choose an option:[/]")
                    .AddChoices(new[]
                    {
                        "Show Schedule",
                        "Update Exercise",
                        "Remove Exercise"
                    });
                if (hasDeleteExercise == true)
                {
                    menu.AddChoice("Restore Deleted Exercise");
                }

                menu.AddChoices("Add Exercise", "[yellow]Back[/]");

                string choice = AnsiConsole.Prompt(menu);
                
                switch (choice)
                {
                    case "Show Schedule":
                        var plans = JSONFileHanldler.Load<WorkoutPlan>(
                        Path.Combine($"Data/Users/{Session.CurrentUser.Id}", "workoutplans.json"));
                        WPUI.ShowWPUI(plans);
                        AnsiConsole.MarkupLine("[grey]Press ENTER to continue...[/]");
                        Console.ReadKey(true);
                        
                        if (plans  == null)
                        {
                            AnsiConsole.MarkupLine("[red]No workout plan generated yet.2[/]");
                            AnsiConsole.MarkupLine("[grey]Press ENTER to continue...[/]");
                            Console.ReadKey(true);
                            return;
                        }
                        break;
                    
                    case "Update Exercise":
                        WPUI.UpdateExercise(plan);
                        break;

                    case "Remove Exercise":
                        WPUI.RemoveExercise(plan);
                        // now 
                        hasDeleteExercise = true;
                        break;
                    
                    case "Restore Deleted Exercise":
                        WPUI.UndoLastDelete(plan);
                            // efter Undo:
                        hasDeleteExercise = WPUI.undoRemoveStack.Count > 0;
                        break;

                    case "Add Exercise":
                       // WPUI.AddExercise(plan); inte klar 
                        break;

                    case "[yellow]Back[/]":
                        running = false;
                        break;
                    
                }
            }
        }
    }
}


