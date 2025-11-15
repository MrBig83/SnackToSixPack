using Spectre.Console;
using System.Data;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SnackToSixPack.Classes
{
    // User profile model
    public class UserProfile
{
    public int Age { get; set; }
    public double WeightKg { get; set; }
    public double HeightCm { get; set; }
    public string Goal { get; set; }
    public string Experience { get; set; }
    public int WorkoutDays { get; set; }


        // Funktion som skickar användarprofilen i prompten till OpenAI
        // Konverterar användarprofilen till JSON så att AI tydligt kan läsa den
        // Skapar en prompt där profilens data används för att generera ett personligt träningsprogram
        public string CreatePrompt()
        {
            var profileJson = JsonSerializer.Serialize(this);
            string prompt = $"Create a personalized workout plan based on the following user profile: {profileJson}. The plan should include exercises, sets, reps, and rest periods tailored to the user's age, weight, height, goal, experience level, and workout frequency.";
            return prompt;
        }

        // Prompten som innehåller användarens data och begär ett personligt träningsupplägg från AI
        public string GetWorkoutPlanPrompt()
        {
            return CreatePrompt();
        }

        // Returnerar AI:ns genererade träningsprogram
        public string GetGeneratedWorkoutPlan(string aiResponse)
        {
            return aiResponse;
        }

        // Display UserProfile in console using Spectre.Console
        public void DisplayProfile()
        {
            var table = new Table();
            table.AddColumn("Attribute");
            table.AddColumn("Value");
            table.AddRow("Age", Age.ToString());
            table.AddRow("Weight (kg)", WeightKg.ToString());
            table.AddRow("Height (cm)", HeightCm.ToString());
            table.AddRow("Goal", Goal);
            table.AddRow("Experience", Experience);
            table.AddRow("Workout Days/Week", WorkoutDays.ToString());
            AnsiConsole.Render(table);
        }
    }
}
