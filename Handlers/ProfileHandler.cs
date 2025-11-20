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
            JSONFileHanldler<Profile>.Save($"Data/Users/{Session.CurrentUser.Id}/profile.json", profile);
            //JSONHelper.SaveProfile(profile);
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

            while (editing)
            {
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine("[bold purple]Update Your Profile[/]");
                AnsiConsole.MarkupLine("[grey]Select a field to update:[/]");

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold]What do you want to update?[/]")
                        .AddChoices(new[]
                        {
                            "Name",
                            "Age",
                            "Height",
                            "Weight",
                            "Waist",
                            "Chest",
                            "Hips",
                            "Arm",
                            "Thigh",
                            "Fitness Level",
                            "[green]Done[/]",
                            "[red]Exit[/]"
                        })
                );

                switch (choice)
                {
                    case "Name":
                        AnsiConsole.Markup("[bold]New Name: [/]");
                        profile.Name = Console.ReadLine();
                        break;

                    case "Age":
                        while (true)
                        {
                            AnsiConsole.Markup("[bold]New Age: [/]");
                            string input = Console.ReadLine();
                            if (int.TryParse(input, out int age))
                            {
                                profile.Age = age;
                                break;
                            }
                            AnsiConsole.MarkupLine("[yellow]Invalid number, try again.[/]");
                        }
                        break;

                    case "Height":
                        profile.Height = ReadDoubleInput("New Height (cm): ");
                        break;

                    case "Weight":
                        profile.Weight = ReadDoubleInput("New Weight (kg): ");
                        break;

                    case "Waist":
                        profile.Waist = ReadDoubleInput("New Waist (cm): ");
                        break;

                    case "Chest":
                        profile.Chest = ReadDoubleInput("New Chest (cm): ");
                        break;

                    case "Hips":
                        profile.Hips = ReadDoubleInput("New Hips (cm): ");
                        break;

                    case "Arm":
                        profile.Arm = ReadDoubleInput("New Arm (cm): ");
                        break;

                    case "Thigh":
                        profile.Thigh = ReadDoubleInput("New Thigh (cm): ");
                        break;

                    case "Fitness Level":
                        profile.FitnessLevel = ReadFitnessLevel();
                        break;

                    case "[green]Done[/]":
                        editing = false;
                        break;
                    case "[red]Exit[/]":
                        MenuHandler.ShowUserMenu();
                        return;
                }

                // Save back to current user
                Session.CurrentUser.Profile = profile;
                JSONFileHanldler<Profile>.Save($"Data/Users/{Session.CurrentUser.Id}/profile.json", profile);
            }
            AnsiConsole.MarkupLine("\n[bold green]Profile updated successfully![/]");
        }
    }
}
