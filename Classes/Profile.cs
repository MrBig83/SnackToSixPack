using Microsoft.VisualBasic;
using Spectre.Console;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SnackToSixPack.Classes
{

    public class Profile
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public double Weight { get; set; } 
        public double Height { get; set; } 
        public double Waist { get; set; }
        public double Chest { get; set; }
        public double Hips { get; set; }
        public double Arm { get; set; }
        public double Thigh { get; set; }
        public FitnessLevel FitnessLevel { get; set; } 
        public Gender Gender { get; set; }


        public Profile(){}

        public Profile(int age, double weight, double height, FitnessLevel fitnessLevel, double waist, double chest, double hips, double arm, double thigh)
        {
            Age = age;
            Weight = weight;
            Height = height;
            FitnessLevel = fitnessLevel;
            Waist = waist;
            Chest = chest;
            Hips = hips;
            Arm = arm;
            Thigh = thigh;
        }
    }
    public enum FitnessLevel
        {
            Beginner,
            Intermediate,
            Advanced
        }

    public enum Gender
    {
        Woman,
        Man,
        PreferNotToSay
    }


    //Implement userprofile in the prompt to OpenAI to get a more accurate workout plan
    public static class AIService
    {
        public static async Task<string> AskAI(string userInput)
        {
            string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("API key not found");
                return null;
            }

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            // Prompt till AI
            string AiPrompt = $@"
            Skapa ett träningsschema i JSON-format med följande egenskaper:
            - ""PlanName""
            - ""Goal"": baserat på detta: {userInput}
            - ""StartDate""
            - ""EndDate""
            - ""Workouts"":
            [
                - ""DayOfWeek""
                - ""Title""
                - ""Exercises"":
            [
                - ""Name""
                - ""Sets""
                - ""Reps""
                - ""Weight""
                - ""RestTime""
            ]
            ]
                Svara ENDAST med minimerad JSON.
                ";

            var requestBody = new
            {
                model = "gpt-4.1",
                messages = new[]
                {
                    new { role = "user", content = AiPrompt }
                },
                temperature = 0.7
            };

            string json = JsonSerializer.Serialize(requestBody);

            var response = await client.PostAsync(
                "https://api.openai.com/v1/chat/completions",
                new StringContent(json, Encoding.UTF8, "application/json")
            );

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("API ERROR: " + response.StatusCode);
                return null;
            }

            string responseJson = await response.Content.ReadAsStringAsync();

            using JsonDocument doc = JsonDocument.Parse(responseJson);
            string aiReply = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return aiReply;
        }
    }
}
