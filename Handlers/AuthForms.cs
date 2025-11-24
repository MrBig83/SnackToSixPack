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

            var heading = new Panel("[lime] Log in[/]")
                .Border(BoxBorder.Double)
                .BorderStyle(new Style(Color.White))
                .Padding(10, 0);

            AnsiConsole.Write(heading);
            AnsiConsole.WriteLine();
                List<User> users;
                try
                {
                    users = JSONFileHanldler<List<User>>.Load<List<User>>(        
                    Path.Combine($"Data", "Users.json")
                     );
                }
                catch (FileNotFoundException ex)
                {
                    File.AppendAllText("log.json", $"[{DateTime.Now}] ERROR: Failed to load users: {ex.Message}{Environment.NewLine}");
                    AnsiConsole.MarkupLine("[red]No users found. Please register a user first.[/]");
                    AnsiConsole.MarkupLine("Press any key to return to the main menu...");
                    Console.ReadKey(true);
                    return;
                }

                //var users = JSONHelper.LoadUsers();

                string usernameInput = AnsiConsole.Ask<string>("[bold]Username:[/][grey][/]");

            var passwordPrompt = new TextPrompt<string>("[bold]Password:[/][grey][/]")
                .PromptStyle("green")
                .Secret();

            string passwordInput = AnsiConsole.Prompt(passwordPrompt);

            AnsiConsole.WriteLine();

            var exitPrompt = new SelectionPrompt<string>();
                exitPrompt.AddChoice("Log in");
                exitPrompt.AddChoice("Forget password");
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
                    AnsiConsole.MarkupLine("[red]Invalid username or password.[/]");
                    AnsiConsole.MarkupLine("Press Enter to try again or type [yellow]f[/] to reset password.");

                    while (true)
                    {
                        var key = Console.ReadKey(true);

                        if (key.Key == ConsoleKey.Enter)
                            break;

                        if (char.ToLower(key.KeyChar) == 'f')   // User pressed "f"
                        {
                            Console.Clear();
                            Console.Write("Enter your email: ");
                            var email = Console.ReadLine();

                            SendPasswordResetEmail(email);

                            Console.WriteLine("A verification code has been sent to your email.");
                            Console.Write("Enter verification code: ");
                            var code = Console.ReadLine();

                            // TODO: verify code
                            // Example:
                            // if (PasswordResetStore.Codes[email] == code)

                            Console.WriteLine("Verification successful. You can now create a new password.");
                            Console.Write("New password: ");
                            var newPassword = Console.ReadLine();

                            // TODO: Save new password to user account.

                            Console.WriteLine("Password reset successfully.");
                            Console.ReadKey(true);
                            break;
                        }
                    }

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
                Profile userProfile;
                try
                {
                 userProfile = JSONFileHanldler<Profile>.Load<Profile>(
                 Path.Combine($"Data/Users/{Session.CurrentUser.Id}", "profile.json")
                 );

                } catch (FileNotFoundException ex)
                {
                    File.AppendAllText("log.json", $"[{DateTime.Now}] ERROR: Failed to load users: {ex.Message}{Environment.NewLine}");
                    AnsiConsole.MarkupLine("[red] User profile not found. Please create your profile.[/]");
                    AnsiConsole.MarkupLine("Press any key to continue...");
                    Console.ReadKey(true);
                    return;
                }


                Session.CurrentUser.Profile = userProfile;
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

        private static void SendPasswordResetEmail(string? email)
        {
            string token = Guid.NewGuid().ToString();
            string resetLink = $"https://example.com/reset-password?token={token}";

            Console.WriteLine($"Reset link sent to {email}");
            Console.WriteLine(resetLink);
        }
    }
}