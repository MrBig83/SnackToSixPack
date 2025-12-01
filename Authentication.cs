using Spectre.Console;
using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.IO;
using SnackToSixPack.Handlers;
using SnackToSixPack.Classes;
using System.Linq;

public class Authentication
{
    public static bool emailSent = false;

    public static void TwoFactorAuth(int? existingCode = null)
    {
        AnsiConsole.Clear();

        Random rng = new Random();
        int code = existingCode ?? rng.Next(100000, 999999);

        if (!emailSent)
        {
            Authentication auth = new Authentication();

            bool sent = auth.SendEmailToCurrentUser(
                "Snack To Six Pack: Your authentication code",
                "Your authentication code is: " + code
            );

            if (!sent)
            {
                AnsiConsole.MarkupLine("\n[bold red]Failed to send authentication email.[/]");
                Console.ReadKey(true);
                AuthForms.ShowLogInForm();
                return;
            }

            emailSent = true;
        }

        string inputCode = "";

        AnsiConsole.MarkupLine("\n[bold cyan]Please enter the 6-digit authentication code: [/]");
        int cursorPosition = 5;

        while (inputCode.Length <= 6)
        {
            AnsiConsole.Cursor.SetPosition(0, cursorPosition);

            string display = "";
            for (int i = 0; i < 6; i++)
            {
                if (i < inputCode.Length)
                    display += $"[green] [[ {inputCode[i]} ]] [/]";
                else
                    display += "[grey] [[_]] [/]";
            }

            AnsiConsole.Write(new string(' ', Console.WindowWidth));
            AnsiConsole.Cursor.SetPosition(0, cursorPosition);
            AnsiConsole.Markup(display);

            if (inputCode.Length == 6)
                break;

            var key = Console.ReadKey(true);

            if (char.IsDigit(key.KeyChar))
            {
                inputCode += key.KeyChar;
            }
            else if (key.Key == ConsoleKey.Backspace && inputCode.Length > 0)
            {
                inputCode = inputCode[..^1];
            }
        }

        AnsiConsole.WriteLine("\n");

        if (inputCode == code.ToString())
        {
            AnsiConsole.Status()
                .Start("Redirecting...", ctx => System.Threading.Thread.Sleep(2000));

            AnsiConsole.MarkupLine("\n[bold green]Authentication successful![/]");
        }
        else
        {
            AnsiConsole.MarkupLine("\n[bold red]Authentication failed![/]");
            AnsiConsole.MarkupLine("[bold yellow]Press enter to try again.[/]");
            Console.ReadLine();
            TwoFactorAuth(code);
        }
    }
    
    public bool SendEmail(string toEmail, string subject, string body)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        var smtpUsername = APIHandler.readAPIUN();
        var smtpPassword = APIHandler.readAPIPW();

        using var mail = new MailMessage();
        mail.From = new MailAddress(smtpUsername);

        if (string.IsNullOrWhiteSpace(toEmail))
        {
            Console.WriteLine("No email address provided.");
            return false;
        }

        mail.To.Add(toEmail);
        mail.Subject = subject;
        mail.Body = body;

        using var smtp = new SmtpClient(config["Smtp:Host"], int.Parse(config["Smtp:Port"]!))
        {
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(smtpUsername, smtpPassword),
            EnableSsl = true
        };

        try
        {
            smtp.Send(mail);
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to send email: {ex.Message}");
            AnsiConsole.MarkupLine("[bold red]Failed to send email.[/]");
            return false;
        }
    }
    
    public bool SendEmailToCurrentUser(string subject, string body)
    {
        if (Session.CurrentUser?.Email == null)
            return false;

        return SendEmail(Session.CurrentUser.Email, subject, body);
    }

    public bool ForgotPassword(List<User> users)
    {
        var prompt = new SelectionPrompt<string>()
            .Title("[blue]Forgot password?[/]")
            .AddChoices("Try again", "Forgot Password", "[yellow]Back[/]");

        var choice = AnsiConsole.Prompt(prompt);

        switch (choice)
        {
            case "Try again":
                return false;

            case "Forgot Password":
                AnsiConsole.Markup("Enter your email: ");
                string email = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(email))
                {
                    Console.WriteLine("Invalid email.");
                    return false;
                }

                var user = users.FirstOrDefault(u => u.Email == email);

                while (true)
                {
                    if (user == null)
                    {
                        AnsiConsole.MarkupLine("[red]No user found with that email.[/]");

                        var tryAgainEmail = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("[yellow]What would you like to do?[/]")
                                .AddChoices("Try again", "[yellow]Back[/]")
                        );

                        switch (tryAgainEmail)
                        {
                            case "Try again":
                                AnsiConsole.Markup("Enter your email: ");
                                email = Console.ReadLine()?.Trim();

                                user = users.FirstOrDefault(u =>
                                    u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

                                continue;

                            case "[yellow]Back[/]":
                                return true;
                        }
                    }

                    // skicka mejl och gå ur loopen
                    SendEmail(user.Email, "Snack To Six Pack", "Your password is " + user.Password);
                    AnsiConsole.MarkupLine("[Green]An email has been sent to your email.[/]");
                    AnsiConsole.MarkupLine("[grey]Press Enter to log in again.[/]");
                    Console.ReadKey(true);
                    break;
                }

                break;
        }
        
        return false;
    }
}
