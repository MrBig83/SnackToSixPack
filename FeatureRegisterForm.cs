// Dessa "using"-rader berättar för C# vilka delar vi använder
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Spectre.Console; // För färg och snygga texter

namespace SnackToSixPack.Handlers
{
    internal class FeatureRegisterForm
    {
        public void Run()
        {
            // Läs in användare från JSON (eller skapa en tom lista)
            List<User> users = LoadUsers();

            // === Titel ===
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

                bool usernameExists = users.Any(u =>
                    u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

                if (usernameExists)
                {
                    AnsiConsole.MarkupLine("[red]That username already exists. Choose another![/]");
                    continue;
                }

                break;
            }

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

            // ---------- PASSWORD ----------
            string password;
            while (true)
            {
                AnsiConsole.Markup("[green]Enter password (min 4 characters):[/] ");
                password = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(password) || password.Length < 4)
                {
                    AnsiConsole.MarkupLine("[red] Password must be at least 4 characters long.[/]");
                    continue;
                }

                break;
            }

            // ---------- CREATE USER ----------
            var newUser = new User
            {
                Username = username,
                Email = email,
                Password = password
            };

            users.Add(newUser);
            SaveUsers(users);

            AnsiConsole.MarkupLine("\n[bold green] Registration successful![/]");
            AnsiConsole.MarkupLine($"[yellow]Welcome, {username}![/]");
        }

        // =======================
        // Save users to JSON file
        // =======================
        private void SaveUsers(List<User> users)
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
        private List<User> LoadUsers()
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

    // Enkel klass för användaren
    public class User
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}