using SnackToSixPack.Classes;
using SnackToSixPack.Handlers;
using Spectre.Console;
using System.Reflection;
using System.Runtime.InteropServices.Marshalling;

namespace SnackToSixPack.Classes
{
    public class AuthForms
    {
        public static void ShowLogInForm()
        {
            bool running = true;
            while (running)
            {
            AnsiConsole.Clear();

            var heading = new Panel("[green] Log in[/]")
                .Border(BoxBorder.Double)
                .BorderStyle(new Style(Color.White))
                .Padding(10, 0);

            AnsiConsole.Write(heading);
            AnsiConsole.WriteLine();

            var users = JSONHelper.LoadUsers();

            string usernameInput = AnsiConsole.Ask<string>("[bold]Username:[/][grey][/]");

            var passwordPrompt = new TextPrompt<string>("[bold]Password:[/][grey][/]")
                .PromptStyle("green")
                .Secret();

            string passwordInput = AnsiConsole.Prompt(passwordPrompt);

            AnsiConsole.WriteLine();

            var exitPrompt = new SelectionPrompt<string>();
            exitPrompt.AddChoice("Log in");
            exitPrompt.AddChoice("[red]Exit[/]");

            var exitChoice = AnsiConsole.Prompt<string>(exitPrompt);

            if (exitChoice == "[red]Exit[/]")
            {
                return; 
            }

                var user = users.FirstOrDefault(u => u.UserName.Equals(usernameInput, StringComparison.OrdinalIgnoreCase)
                                                     && u.Password.Equals(passwordInput));

                AnsiConsole.Status()
                    .Start("Verifying credentials...", ctx =>
                    {
                        // Simulate some work, 3 seconds
                        System.Threading.Thread.Sleep(3000);
                    });

                if (user == null)
                {
                    AnsiConsole.Clear();
                    AnsiConsole.MarkupLine("[red]Invalid username or password. Press enter to try again.[/]");
                    bool pressedEnter = false;
                    while (!pressedEnter)
                    {
                        var key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Enter)
                        {
                            pressedEnter = true;
                        }
                    }
                    // if invalid, loop again
                    continue;
                }

                AnsiConsole.Clear();
                AnsiConsole.WriteLine("Processing login...");
                AnsiConsole.Status()
                    .Start("Proceeding to authentication...", ctx =>
                    {
                        // Simulate some work, 3 seconds
                        System.Threading.Thread.Sleep(3000);
                    });
                AnsiConsole.Clear();
                Session.SetCurrentUser(user);
                //Authentication.TwoFactorAuth();
                Session.CurrentUser.Profile = JSONHelper.LoadProfile();
                AnsiConsole.WriteLine();

                AnsiConsole.Write(
                    new FigletText("Welcome " + usernameInput!)
                        .Centered()
                        .Color(Color.Purple));

                AnsiConsole.Write(new Markup
                                    ("[bold yellow]Glad to see you back![/]").Centered());

                running = false;
                return;
            }
        }
    }
}