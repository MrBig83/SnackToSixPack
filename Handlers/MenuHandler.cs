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

        //Start menu, Login, Register, Quit
        public static void ShowMainMenu()
        {
            bool exit = false;
            while (!exit)
            {
                AnsiConsole.Clear();
                var menu = new SelectionPrompt<string>()
                    .Title("[bold green]Welcome to SnackToSixPack! Please choose an option:[/]")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                        "Login",
                        "Register",
                        "Quit"
                    });
                string choice = AnsiConsole.Prompt(menu);
                switch (choice)
                {
                    case "Login":
                        AuthForms.ShowLogInForm();
                        break;
                    case "Register":
                        //AuthForms.ShowRegisterForm();
                        RegistrationHandler.Run();
                        break;
                    case "Quit":
                        exit = true;
                        break;
                }
                if (!exit)
                {
                    AnsiConsole.WriteLine();
                    AnsiConsole.Markup("[grey]Press any key to continue to the User menu...[/]");
                    Console.ReadKey(true);
                    ShowUserMenu();
                    
                }
            }
        }

        //User menu , Show profile, Edit profile, Show schedule, Create Workout plan, LogOut
        public static void ShowUserMenu()
        {
            if(Session.CurrentUser != null)
            {
                
            //}
            //bool logout = false;
            //while (!logout)
            //{
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
                        // ShowSchedule();
                        JSONHelper.ReadWP();
                        break;
                    case "Create Workout Plan":
                        // CreateWorkoutPlan();
                        break;
                    case "Log Out":
                        Session.CurrentUserLogout();
                        //logout = true;
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
    }
}

