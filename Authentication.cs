using Spectre.Console;
using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.IO;
using SnackToSixPack.Handlers;

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
            auth.SendEmail(code.ToString());
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

    public void SendEmail(string code)
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
        mail.To.Add("mjohansson176@gmail.com");
        mail.Subject = "Your Authentication Code";
        mail.Body = "Your authentication code is: " + code;

        using var smtp = new SmtpClient(config["Smtp:Host"], int.Parse(config["Smtp:Port"]!));
        smtp.UseDefaultCredentials = false;
        smtp.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
        smtp.EnableSsl = true; // for TLS on port 587

        try
        {
            smtp.Send(mail);
            Console.WriteLine("Email sent successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send email: {ex.Message}");
        }
    }
}