using Spectre.Console;
using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.IO;
using SnackToSixPack.Handlers;
using SnackToSixPack.Classes;

public class Authentication
{
    private static bool emailSent = false;
    public static void TwoFactorAuth(int? existingCode = null)
    {
        AnsiConsole.Clear();
        Random oneSingleUseCode = new Random();
        int code = existingCode ?? oneSingleUseCode.Next(100000, 999999);

        if (!emailSent)
        {
            Authentication auth = new Authentication();
            // converts int to string but does not save as a string variable
            bool sent = auth.SendEmail("Snack To Six Pack: Your authentication code", "Your authentication code is: " + code);

            if (!sent)
            {
                AnsiConsole.MarkupLine("\n[bold red]Failed to send authentication email.[/]");
                // KOLLAAAAAAAAAA DETTA----------------------------
                AnsiConsole.MarkupLine("[bold yellow]Press enter to return to login or 'Q' to quit.[/]");
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Q)
                {
                    Environment.Exit(0);
                }
                AuthForms.ShowLogInForm();
                // exit the method to avoid continuing the authentication process   
                return;
            }
            emailSent = true;
        }
        string inputCode = "";

        AnsiConsole.MarkupLine("\n[bold cyan]Please enter the 6-digit authentication code: [/]");
        int cursorPosition = 5;
        // <= 6 becasue we want to allow 6 digits and show the entered digits
        while (inputCode.Length <= 6)
        {
            // draw boxes for each digit 
            AnsiConsole.Cursor.SetPosition(0, cursorPosition);

            var display = "";
            for (int i = 0; i < 6; i++)
            {
                // draw boxes for each digit
                // if the digit is entered, draw a filled(with number) box, else draw an empty box
                if (i < inputCode.Length)
                {
                    display += "[green] [[ " + inputCode[i] + " ]] [/]";
                }
                else
                {
                    display += "[grey] [[_]] [/]";
                }
            }
            AnsiConsole.Write(new string(' ', Console.WindowWidth));
            AnsiConsole.Cursor.SetPosition(0, cursorPosition);
            AnsiConsole.Markup(display);

            // break the loop if 6 digits are entered
            if (inputCode.Length == 6)
            {
                break;
            }

            // read the next digit without enter, it's here we capture each key press
            var key = Console.ReadKey(true);
            // if the key is a digit, add it to the code
            if (char.IsDigit(key.KeyChar))
            {
                // adds every digit to the end of the code
                // thats why Substring is used in backspace
                inputCode += key.KeyChar;
            }
            else if (key.Key == ConsoleKey.Backspace && inputCode.Length > 0)
            {
                // finish all in the parentheses first and work outward then back to the beginning
                inputCode = inputCode.Substring(0, inputCode.Length - 1);
            }
        }

        AnsiConsole.WriteLine("\n");
        bool success = false;
        while (!success)
        {
            // check if the code is correct
            if (inputCode == code.ToString())
            {
                AnsiConsole.Status()
                    .Start("Redirecting...", ctx =>
                    {
                        // Simulate some work, 3 seconds
                        System.Threading.Thread.Sleep(2000);
                    });
                AnsiConsole.MarkupLine("\n[bold green]Authentication successful![/]");

                success = true;
            }
            else
            {
                AnsiConsole.MarkupLine("\n[bold red]Authentication failed![/]");
                AnsiConsole.MarkupLine("[bold yellow]Press enter to try again.[/]");
                Console.ReadLine();
                TwoFactorAuth(code);
                break;
            }
        }
    }

    public bool SendEmail(string subject, string body)
    {
        // Load configuration from appsettings.json
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            // sätt optional: false om filen måste finnas
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
        
        var smtpUsername = APIHandler.readAPIUN();
        var smtpPassword = APIHandler.readAPIPW();

        using var mail = new MailMessage();
        mail.From = new MailAddress(smtpUsername);
        // if current user exists and has an email, use it
        // "is string email" if the email is not null, assign it to the variable email
        if (Session.CurrentUser?.Email is string email)
        {
            mail.To.Add(email); 
        }
        else
        {
            // if no current user or no email, exit the method
            Console.WriteLine("No email address available for current user.");
            return false;
        }
        mail.Subject = subject;
        mail.Body = body;

        using var smtp = new SmtpClient(config["Smtp:Host"], int.Parse(config["Smtp:Port"]!));
        smtp.UseDefaultCredentials = false;
        smtp.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
        smtp.EnableSsl = true; // for TLS on port 587

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

    public void ForgotPassword(User user)
    {
        var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("[cyan blue]Forgot password?[/]")
            .AddChoices("Try again", "Forgot Password", "[yellow]Exit[/]")
            );

        switch (choice)
        {
            case "Try again":
                return;
            
            case "Forgot Password":
                if (user.Email == null)
                {
                    AnsiConsole.MarkupLine("[red]No email found for this user.[/]");
                    return;
                }
                
                bool sent = SendEmail("Snack To Six Pack: Your password", "Your password is: " + user.Password);

                if (sent)
                { 
                    AnsiConsole.MarkupLine("[green]Your password has been sent to your email[]/");
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Failed to send email.[/]");
                }
                return;
            
            case "[yellow]Exit[/]":
                Environment.Exit(0);
                break;
        }
    }
}