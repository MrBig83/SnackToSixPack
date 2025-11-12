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

                // Display the title
                // [green] makes the text green
                var heading = new Panel("[green] Log in[/]")
                    .Border(BoxBorder.Double)
                    // makes the border color white
                    .BorderStyle(new Style(Color.White))
                    .Padding(10, 0);

                AnsiConsole.Write(heading);
                AnsiConsole.WriteLine();

                var users = JSONHelper.LoadUsers();

                string usernameInput = AnsiConsole.Ask<string>("[bold]Username:[/][grey][/]");

                var passwordPrompt = new TextPrompt<string>("[bold]Password:[/][grey][/] ")
                    .PromptStyle("green")
                    .Secret();


                string passwordInput = AnsiConsole.Prompt(passwordPrompt);

                //var user = users.FirstOrDefault(u => u.UserName.Equals(usernameInput, StringComparison.OrdinalIgnoreCase)
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
                Authentication.TwoFactorAuth();

                AnsiConsole.WriteLine();
                AnsiConsole.Markup("[bold]Welcome[/]" + ",[green] " + usernameInput + "[/]!");
                MenuHandler.ShowUserMenu();
                //running = false;
            }
        }

        internal static void ShowRegisterForm()
        {
            throw new NotImplementedException();
        }
    }
}