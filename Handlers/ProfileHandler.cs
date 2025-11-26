using System.Threading.Tasks;
using SnackToSixPack.Classes;
using SnackToSixPack.Handlers;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace SnackToSixPack.Classes
{
    public class ProfileHandler
    {

        //Generic
        private double ReadDoubleInput(string label)
        {
            while (true)
            {
                AnsiConsole.Markup("[bold]" + label + "[/]");

                try
                {
                    return double.Parse(Console.ReadLine());
                }
                catch
                {
                    AnsiConsole.MarkupLine("[yellow]Wrong input, try again[/]");
                }
            }
        }

        private FitnessLevel ReadFitnessLevel()
        {
            AnsiConsole.WriteLine();
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold]Select fitness level:[/]")
                    .AddChoices("Beginner", "Intermediate", "Advanced"));

            AnsiConsole.WriteLine("Level: " + choice);
            switch (choice)
            {
                case "Beginner":
                    return FitnessLevel.Beginner;

                case "Intermediate":
                    return FitnessLevel.Intermediate;

                case "Advanced":
                    return FitnessLevel.Advanced;

                default:
                    return FitnessLevel.Beginner;
            }
        }

        private Gender ChooseGender()
        {
            AnsiConsole.WriteLine();
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold]Select gender:[/]")
                    .AddChoices("Woman", "Man", "PreferNotToSay")
            );

            AnsiConsole.WriteLine("Gender: " + choice);
            switch (choice)
            {
                case "Woman":
                    return Gender.Woman;
                case "Man":
                    return Gender.Man;
                case "PreferNotToSay":
                    return Gender.PreferNotToSay;
            }

            // No default case needed because the user must select one of the provided options,
            // but the compiler does not know that, so we handle the "impossible" case below.
            throw new Exception("Unexpected gender selecion.");
        }

        public Profile CreateProfile()
        {
            Profile profile = new Profile();
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[BlueViolet]Create your profile[/]");
            AnsiConsole.MarkupLine("---------------------------");
            AnsiConsole.Markup("[bold] Full name: [/]");
            profile.Name = Console.ReadLine();

            bool inputNumber = false;
            profile.Age = 0;

            while (!inputNumber)
            {
                AnsiConsole.Markup("[bold] Age: [/]");
                string input = Console.ReadLine();

                if (int.TryParse(input, out int newAge))
                {
                    profile.Age = newAge;
                    inputNumber = true;
                }
                else
                {
                    AnsiConsole.MarkupLine("[yellow]Wrong input, try again[/]");
                }
            }

            profile.Height = ReadDoubleInput(" Height (cm): ");
            profile.Weight = ReadDoubleInput(" Weight (kg): ");
            profile.Waist = ReadDoubleInput(" Waist (cm): ");
            profile.Chest = ReadDoubleInput(" Chest (cm): ");
            profile.Hips = ReadDoubleInput(" Hips (cm): ");
            profile.Arm = ReadDoubleInput(" Arm (cm): ");
            profile.Thigh = ReadDoubleInput(" Thigh (cm): ");
            profile.FitnessLevel = ReadFitnessLevel();
            profile.Gender = ChooseGender();
            AnsiConsole.MarkupLine("---------------------------");
            AnsiConsole.MarkupLine("[bold purple]Profile created.[/]");
            System.Threading.Thread.Sleep(3000);


            Session.CurrentUser.Profile = profile;
            // profile can never be null here
            JSONFileHanldler.Save($"Data/Users/{Session.CurrentUser.Id}/profile.json", profile);
            return profile;
        }

        public static async Task ShowProfile(Profile profile)
        {
            AnsiConsole.Clear();

            if (profile == null)
            {
                AnsiConsole.MarkupLine("[red]Profile does not exist.[/]");
                return;
            }

            AnsiConsole.Write(
                new FigletText($"{profile.Name}'s Profile")
                    .Centered()
                    .Color(Color.Purple));


            AnsiConsole.WriteLine();
            AnsiConsole.Write(
                new Markup("[bold yellow]Personal Info[/]")
                    .Centered());

            var personalTable = new Table()
                .Border(TableBorder.MinimalHeavyHead)
                .Centered();

            personalTable.AddColumn(new TableColumn("Field").Centered());
            personalTable.AddColumn(new TableColumn("Value").Centered());

            personalTable.AddRow("Name", profile.Name);
            personalTable.AddRow("Age", profile.Age.ToString());

            AnsiConsole.Write(personalTable);
            AnsiConsole.WriteLine();

            AnsiConsole.Write(
                new Markup("[bold yellow]Body Data[/]")
                    .Centered());

            var bodyTable = new Table()
                .Border(TableBorder.MinimalHeavyHead)
                .Centered();

            bodyTable.AddColumn(new TableColumn("Field").Centered());
            bodyTable.AddColumn(new TableColumn("Value").Centered());

            bodyTable.AddRow("Height (cm)", profile.Height.ToString());
            bodyTable.AddRow("Weight (kg)", profile.Weight.ToString());

            AnsiConsole.Write(bodyTable);
            AnsiConsole.WriteLine();

            AnsiConsole.Write(
                new Markup("[bold yellow]Measurements[/]")
                    .Centered());

            var measurementsTable = new Table()
                .Border(TableBorder.MinimalHeavyHead)
                .Centered();

            measurementsTable.AddColumn(new TableColumn("Field").Centered());
            measurementsTable.AddColumn(new TableColumn("Value").Centered());

            measurementsTable.AddRow("Waist (cm)", profile.Waist.ToString());
            measurementsTable.AddRow("Chest (cm)", profile.Chest.ToString());
            measurementsTable.AddRow("Hips (cm)", profile.Hips.ToString());
            measurementsTable.AddRow("Arm (cm)", profile.Arm.ToString());
            measurementsTable.AddRow("Thigh (cm)", profile.Thigh.ToString());

            AnsiConsole.Write(measurementsTable);
            AnsiConsole.WriteLine();

            AnsiConsole.Write(
                new Markup("[bold yellow]Fitness[/]")
                    .Centered());

            var fitnessTable = new Table()
                .Border(TableBorder.MinimalHeavyHead)
                .Centered();

            fitnessTable.AddColumn(new TableColumn("Field").Centered());
            fitnessTable.AddColumn(new TableColumn("Value").Centered());

            fitnessTable.AddRow("Level", profile.FitnessLevel.ToString());
            fitnessTable.AddRow("Gender", profile.Gender.ToString());

            AnsiConsole.Write(fitnessTable);

            var personalMenu = new SelectionPrompt<string>()
                .PageSize(10)
                .AddChoices("Exit", "[red]Delete account[/]");

            string choice = AnsiConsole.Prompt(personalMenu);

            switch (choice)
            {
                case "[red]Delete account[/]":
                    await RegistrationHandler.DeleteCurrentUser();
                    return;

                case "Exit":
                    break;
            }
        }

        public void UpdateProfile(Profile profile)
        {
            bool editing = true;

            // en temp kopia 
            var tempProfile = new Profile
            {
                Name = profile.Name,
                Age = profile.Age,
                Weight = profile.Weight,
                Height = profile.Height,
                FitnessLevel = profile.FitnessLevel,
                Waist = profile.Waist,
                Chest = profile.Chest,
                Hips = profile.Hips,
                Arm = profile.Arm,
                Thigh = profile.Thigh
            };

            while (editing)
            {
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine("[bold purple]Update Your Profile[/]");
                AnsiConsole.MarkupLine("[grey]Select a field to update:[/]");
                AnsiConsole.WriteLine();

                // Visa nuvarande (temp) värden
                AnsiConsole.MarkupLine($"Name: [blue]{tempProfile.Name}[/]");
                AnsiConsole.MarkupLine($"Age: [blue]{tempProfile.Age}[/]");
                AnsiConsole.MarkupLine($"Height: [blue]{tempProfile.Height} cm[/]");
                AnsiConsole.MarkupLine($"Weight: [blue]{tempProfile.Weight} kg[/]");
                AnsiConsole.MarkupLine($"Waist: [blue]{tempProfile.Waist} cm[/]");
                AnsiConsole.MarkupLine($"Chest: [blue]{tempProfile.Chest} cm[/]");
                AnsiConsole.MarkupLine($"Hips: [blue]{tempProfile.Hips} cm[/]");
                AnsiConsole.MarkupLine($"Arm: [blue]{tempProfile.Arm} cm[/]");
                AnsiConsole.MarkupLine($"Thigh: [blue]{tempProfile.Thigh} cm[/]");
                AnsiConsole.MarkupLine($"Fitness Level: [blue]{tempProfile.FitnessLevel}[/]");
                AnsiConsole.WriteLine();

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold]What do you want to update?[/]")
                        .AddChoices(
                            "Name", "Age", "Height", "Weight",
                            "Waist", "Chest", "Hips", "Arm", "Thigh",
                            "Fitness Level",
                            "[green]Done[/]",
                            "[red]Exit[/]"
                        )
                );

                switch (choice)
                {
                    case "Name":
                        AnsiConsole.Markup("[bold]New Name: [/]");
                        tempProfile.Name = Console.ReadLine();
                        break;

                    case "Age":
                        while (true)
                        {
                            AnsiConsole.Markup("[bold]New Age: [/]");
                            string input = Console.ReadLine();
                            if (int.TryParse(input, out int age))
                            {
                                tempProfile.Age = age;
                                break;
                            }

                            AnsiConsole.MarkupLine("[yellow]Invalid number, try again.[/]");
                        }
                        break;

                    case "Height":
                        tempProfile.Height = ReadDoubleInput("New Height (cm): ");
                        break;

                    case "Weight":
                        tempProfile.Weight = ReadDoubleInput("New Weight (kg): ");
                        break;

                    case "Waist":
                        tempProfile.Waist = ReadDoubleInput("New Waist (cm): ");
                        break;

                    case "Chest":
                        tempProfile.Chest = ReadDoubleInput("New Chest (cm): ");
                        break;

                    case "Hips":
                        tempProfile.Hips = ReadDoubleInput("New Hips (cm): ");
                        break;

                    case "Arm":
                        tempProfile.Arm = ReadDoubleInput("New Arm (cm): ");
                        break;

                    case "Thigh":
                        tempProfile.Thigh = ReadDoubleInput("New Thigh (cm): ");
                        break;

                    case "Fitness Level":
                        tempProfile.FitnessLevel = ReadFitnessLevel();
                        break;

                    case "[green]Done[/]":
                        // Kopiera alla värden från temp → original
                        profile.Name = tempProfile.Name;
                        profile.Age = tempProfile.Age;
                        profile.Weight = tempProfile.Weight;
                        profile.Height = tempProfile.Height;
                        profile.FitnessLevel = tempProfile.FitnessLevel;
                        profile.Waist = tempProfile.Waist;
                        profile.Chest = tempProfile.Chest;
                        profile.Hips = tempProfile.Hips;
                        profile.Arm = tempProfile.Arm;
                        profile.Thigh = tempProfile.Thigh;
                        
                        Session.CurrentUser.Profile = profile;
                        JSONFileHanldler.Save($"Data/Users/{Session.CurrentUser.Id}/profile.json", profile);

                        if (profile == null)
                        {
                            AnsiConsole.MarkupLine("[red]Failed to update profile.[/]");
                            Console.ReadKey(true);
                            return;
                        }
                        
                        AnsiConsole.MarkupLine("\n[bold green]Profile updated successfully![/]");
                        Console.ReadKey(true);
                        MenuHandler.skipPause = true;
                        return;

                    case "[red]Exit[/]":
                        AnsiConsole.MarkupLine("[yellow]No changes saved.[/]");
                        Console.ReadKey(true);
                        MenuHandler.skipPause = true;
                        return;
                }
            }
        }
    }
}