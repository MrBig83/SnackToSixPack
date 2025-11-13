// Dessa "using"-rader berättar för C# vilka delar vi använder
using SnackToSixPack.Classes;
using Spectre.Console; // För färg och snygga texter
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace SnackToSixPack.Handlers
{
    internal class RegistrationHandler
    {
        public static void Run()
        {
            User user = new User();
            // Läs in användare från JSON (eller skapa en tom lista)
            List<User> users = LoadUsers();


            // === Titel ===
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold yellow]=== REGISTER NEW USER ===[/]");

            // ---------- USERNAME ----------
            string username;
            while (true)
            {
                AnsiConsole.Markup("[green]Enter username:[/] ");
                username = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(username))
                {
                    AnsiConsole.MarkupLine("[red] Username cannot be empty. Try again![/]");
                    continue;
                }

                bool usernameExists = users.Any(u => u.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));

                if (usernameExists)
                {
                    AnsiConsole.MarkupLine("[red]That username already exists. Choose another![/]");
                    continue;
                }

                break;
            }
                        // ---------- PASSWORD ----------
            string password = "";
            bool rightPassword = false;
            while (!rightPassword)
            {
                var passwordPrompt =
                new TextPrompt<string>("[green]Enter password (min 6 characters):[/]")
                    .PromptStyle("white")
                    .Secret();

                string passwordInput = AnsiConsole.Prompt(passwordPrompt);

                rightPassword = PasswordValidator.IsPassWordStrong(passwordInput);
            };

            // ---------- EMAIL ----------
            string email;
            while (true)
            {
                AnsiConsole.Markup("[green]Enter email (example: name@mail.com):[/] ");
                email = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(email) || !email.Contains("@") || !email.Contains("."))
                {
                    AnsiConsole.MarkupLine("[red] Please enter a valid email (like name@mail.com)[/]");
                    continue;
                }

                bool emailExists = users.Any(u =>
                    u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

                if (emailExists)
                {
                    AnsiConsole.MarkupLine("[red] That email is already registered![/]");
                    continue;
                }

                break;
            }

            // ---------- CREATE USER ----------

            int nextId = (users.Any() ? users.Max(u => u.Id) + 1 : 1); 

            var newUser = new User
            {
                Id = nextId,    
                UserName = username,
                Email = email,
                Password = password
            };

            if (!Directory.Exists($"Data/Users/{nextId.ToString()}"))
                Directory.CreateDirectory($"Data/Users/{nextId.ToString()}");

            users.Add(newUser);

            SaveUsers(users);

            Session.SetCurrentUser(newUser);
            ProfileHandler newProfile = new ProfileHandler();
            newProfile.CreateProfile();
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("\n[bold green] Registration successful![/]");
            AnsiConsole.MarkupLine($"[yellow]Welcome, {username}![/]");
        }

        // =======================
        // Save users to JSON file
        // =======================
        private static void SaveUsers(List<User> users)
        {
            string folder = "Data";
            string path = Path.Combine(folder, "Users.json");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string json = JsonSerializer.Serialize(users, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(path, json);
        }

        // =======================
        // Load users from JSON file (säker version)
        // =======================
        private static List<User> LoadUsers()
        {
            string folder = "Data";
            string path = Path.Combine(folder, "Users.json");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            // Om filen inte finns → tom lista
            if (!File.Exists(path))
                return new List<User>();

            string json = File.ReadAllText(path);

            // Om filen är tom → tom lista
            if (string.IsNullOrWhiteSpace(json))
                return new List<User>();

            // Försök läsa JSON
            try
            {
                return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
            }
            catch
            {
                AnsiConsole.MarkupLine("[red] Users.json is invalid. Starting with an empty list.[/]");
                return new List<User>();
            }
        }
    }


}