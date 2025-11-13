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
        AnsiConsole.Clear();
        var menu = new SelectionPrompt<string>()
            .Title("[bold green]Welcome to SnackToSixPack! Please choose an option:[/]")
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

        public static async Task ShowUserMenu()
        //User menu , Show profile, Edit profile, Show schedule, Create Workout plan, LogOut
        public static async void ShowUserMenu()
        {
            while (Session.CurrentUser != null)
            {
                AnsiConsole.Clear();
                var menu = new SelectionPrompt<string>()
                    .Title("[bold green]User Menu - Please choose an option:[/]")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                        "Show Profile",
                        "Update Profile",
                        "Show Schedule",
                        "Create Workout Plan",
                        "Log Out"
                    });
                string choice = AnsiConsole.Prompt(menu);
                switch (choice)
                {
                    case "Show Profile":
                        ProfileHandler.ShowProfile(Session.CurrentUser.Profile);
                        break;
                    case "Update Profile":
                        ProfileHandler profileHandler = new ProfileHandler();
                        profileHandler.UpdateProfile(Session.CurrentUser.Profile);
                        break;
                    case "Show Schedule":
                        JSONHelper.ReadWP();
                        break;
                    case "Create Workout Plan":
                        await AIMenu();
                        WPUI.ShowWPUI(JSONHelper.ReadWP());
                        break;
                    case "Create Workout Plan":
                        await openAIHandler.OpenAIHandler();
                        break;
                    case "Log Out":
                        Session.CurrentUserLogout();
                        break;
                }
                if (Session.CurrentUser != null)
                {
                    AnsiConsole.WriteLine();
                    AnsiConsole.Markup("[grey]Press any key to return to the user menu...[/]");
                    Console.ReadKey(true);
                }
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
                    case "Ask AI":
                        await OpenAIHandler.AskAI();
                        break;

                    case "Quit":
                        running = false;
                        break;
                }
            }
        }
    }
}

