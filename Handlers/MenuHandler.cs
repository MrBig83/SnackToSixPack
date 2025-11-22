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

public static async Task ShowMainMenu()
{
    bool exit = false;
    while (!exit)
    {
                // TITEL SCREEN
                AnsiConsole.Clear();
                AnsiConsole.Write(
                new FigletText("SnackToSixPack")
                        .Centered()
                        .Color(Color.BlueViolet));

        var menu = new SelectionPrompt<string>()
            .Title("[BlueViolet] Welcome to SnackToSixPack! Please choose an option:[/]")
            .PageSize(10)
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

        //public static async Task ShowUserMenu();
        //User menu , Show profile, Edit profile, Show schedule, Create Workout plan, LogOut
        public static async Task ShowUserMenu()
        {
            bool skipPause = false;

            while (Session.CurrentUser != null)
            {
                AnsiConsole.Clear();
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
                    case "Update Schedule":
                    try
                    {
                        var workoutPlan = JSONFileHanldler<WorkoutPlan>.Load<WorkoutPlan>(
                        Path.Combine($"Data/Users/{Session.CurrentUser.Id}", "workoutplans.json"));
                        WPUI.UpdateSchedule(workoutPlan);
                    }
                    catch (FileNotFoundException ex)
                    {
                        File.AppendAllText("log.json", $"[{DateTime.Now}] ERROR: Failed to load users: {ex.Message}{Environment.NewLine}");
                        AnsiConsole.MarkupLine("[red]No workout plan generated yet.[/]");
                    }
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
                        break;
                }
            }
        }

        public static void ScheduleHandler()
        {
            var planPath = Path.Combine($"Data/Users/{Session.CurrentUser.Id}", "workoutplans.json");
            WorkoutPlan plan;

            try
            {
                plan = JSONFileHanldler<WorkoutPlan>.Load<WorkoutPlan>(planPath);
            }
            catch
            {
                AnsiConsole.MarkupLine("[red]No workout plan generated yet.[/]");
                return;
            }

            bool running = true;

            while (running)
            {
                AnsiConsole.Clear();

                var menu = new SelectionPrompt<string>()
                    .Title("[cyan]Schedule Menu - Choose an option:[/]")
                    .AddChoices(new[]
                    {
                        "Show Schedule",
                        "Update Exercise",
                        "Remove Exercise",
                        "Add Exercise",
                        "Back"
                    });

                string choice = AnsiConsole.Prompt(menu);

                switch (choice)
                {
                    case "Show Schedule":
                                            try
                    {
                        var plans = JSONFileHanldler<WorkoutPlan>.Load<WorkoutPlan>(
                        Path.Combine($"Data/Users/{Session.CurrentUser.Id}", "workoutplans.json"));

                        WPUI.ShowWPUI(plans);
                        AnsiConsole.MarkupLine("[grey]Press ENTER to continue[/]");
                        Console.ReadLine();

                    }
                    catch (FileNotFoundException ex)
                    {
                        File.AppendAllText("log.json", $"[{DateTime.Now}] ERROR: Failed to load users: {ex.Message}{Environment.NewLine}");
                        AnsiConsole.MarkupLine("[red]No workout plan genereated yet.[/]");
                        AnsiConsole.MarkupLine("Please generate a workout plan from User Menu.");
                    }
                        break;

                    case "Update Exercise":
                        WPUI.UpdateSchedule(plan);
                        break;

                    case "Remove Exercise":
                        WPUI.RemoveExercise(plan);
                        break;

                    case "Add Exercise":
                       // WPUI.AddExercise(plan);
                        break;

                    case "Back":
                        running = false;
                        break;
                }
            }
        }
    }
}


