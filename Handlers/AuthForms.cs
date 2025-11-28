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
                MenuHandler.ShowTitle();
                var heading = new Panel("[lime] Log in[/]")
                    .Border(BoxBorder.Double)
                    .BorderStyle(new Style(Color.White))
                    .Padding(10, 0);

                AnsiConsole.Write(heading);
                AnsiConsole.WriteLine();
                var users = JSONFileHanldler.Load<List<User>>("Data/Users.json");

                if (users == null)
                {
                    AnsiConsole.MarkupLine("[red]Could not load users. Please register a new user first.[/]");
                    Console.ReadKey(true);
                    return;
                }

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

                AnsiConsole.Status()
                        .Start("Verifying credentials...", ctx =>
                        {
                            // Simulate some work, 2 seconds
                            System.Threading.Thread.Sleep(2000);
                        });

                var userByUsername = users.FirstOrDefault(u =>
                        u.UserName.Equals(usernameInput, StringComparison.OrdinalIgnoreCase));

                if (userByUsername == null)
                {
                    AnsiConsole.MarkupLine("[red]Invalid username.[/]");
                    AnsiConsole.MarkupLine("[grey]Press Enter to try again.[/]");
                    Console.ReadKey(true);
                    continue;
                }
                // användarnamn finns, kolla nu lösenordet
                var user = users.FirstOrDefault(u =>
                        u.UserName.Equals(usernameInput, StringComparison.OrdinalIgnoreCase) &&
                        u.Password.Equals(passwordInput));
                    
                if (user == null)
                {
                    AnsiConsole.MarkupLine("[red]Incorrect password.[/]");
    
                    Authentication auth = new Authentication();
                    bool goBackToMain = auth.ForgotPassword(users);

                    if (goBackToMain)
                    {
                        return; // → Gå till första sidan
                    }

                    continue; // → Gå till login igen
                }
                    
                AnsiConsole.Clear();
                AnsiConsole.WriteLine("Processing login...");
                AnsiConsole.Status()
                        .Start("Proceeding to authentication...", ctx =>
                        {
                            // Simulate some work, 2 seconds
                            System.Threading.Thread.Sleep(2000);
                        });
                AnsiConsole.Clear();
                Session.SetCurrentUser(user);
                //Authentication.TwoFactorAuth();
                Profile userProfile;

                userProfile = JSONFileHanldler.Load<Profile>(Path.Combine($"Data/Users/{Session.CurrentUser.Id}", "profile.json"));
                    
                if (userProfile == null)    
                {
                    AnsiConsole.MarkupLine("[red]Could not load user profile![/]");
                    Console.ReadKey(true);
                    return;
                }

                Session.CurrentUser.Profile = userProfile;
              
                running = false;
            }
        }
    }
}