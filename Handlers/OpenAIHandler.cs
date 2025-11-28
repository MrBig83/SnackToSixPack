using SnackToSixPack.Classes;
using SnackToSixPack.Handlers;
using Spectre.Console;
using System.Text;
using System.Text.Json;

public class OpenAIHandler
{
    private static readonly HttpClient client = new HttpClient();

    public static async Task AskAI()
    {
        string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("API key not found");
            return;
        }

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

        Console.WriteLine("Describe the goal of your training:");

        string userInput = Console.ReadLine();
        string todaysDate = DateTime.Now.ToShortDateString();
        string userProfile = JsonSerializer.Serialize(Session.CurrentUser.Profile, new JsonSerializerOptions());

        string AiPrompt = $@"
        Create a workout schedule in JSON format with the following structure.
        Also take the user's physical profile into account: {userProfile}

        - ""PlanName"": (A suitable name for the workout plan)
        - ""Goal"": (A clear and inspiring description of the workout plan in English, based on the user's input: {userInput})
        - ""StartDate"": ({todaysDate} unless the user specifies something else in {userInput})
        - ""EndDate"": (The date when the goal should ideally be reached)
        - ""Workouts"": (A list of workout days based on {userInput}. Each day should have a short title summarizing the training focus in one or two words)
            [
                - ""DayOfWeek"": (The day of the week the workout applies to, in English, e.g. ""Monday"")
                - ""Title"": (A brief 1–2 word summary of the day's workout)
                - ""Exercises"": (A list of exercises to be performed on that day)
                    [
                        - ""Name"": (Name of the exercise)
                        - ""Sets"": (Number of sets if applicable, otherwise 0)
                        - ""Reps"": (Number of repetitions if applicable, otherwise 0)
                        - ""Weight"": (Suggested weight in kg if applicable, otherwise 0)
                        - ""RestTime"": (Rest time in seconds between sets if applicable, otherwise 0)
                    ]
            ]

        Respond ONLY with valid, minified JSON and nothing else.";
        
        var requestBody = new
        {
            model = "gpt-4.1-mini",
            messages = new[]
           {
               new { role = "system", content = "You are a helpful personal trainer," },
               new { role = "user", content = AiPrompt }
               }
        };

        string json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        string responseString = "";

        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .StartAsync("Communicating with OpenAI...", async ctx =>
            {

                var response = await client.PostAsync(
                    "https://api.openai.com/v1/chat/completions",
                    content);

                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
            });

        using JsonDocument doc = JsonDocument.Parse(responseString);
        string reply = doc.RootElement
                           .GetProperty("choices")[0]
                           .GetProperty("message")
                           .GetProperty("content")
                           .GetString();

        string cleanedJson = reply
        .Replace("```json", "")
        .Replace("```", "")
        .Trim();

        WorkoutPlan AiReply = JsonSerializer.Deserialize<WorkoutPlan>(cleanedJson);
        
        if (AiReply == null)
        {
            AnsiConsole.MarkupLine("[red]Failed to generate workout plan from AI output.[/]");
            return;
        }
        
        JSONFileHanldler.Save($"Data/Users/{Session.CurrentUser.Id}/workoutplans.json", AiReply);

         WPUI.ShowWPUI(AiReply);
         Console.ReadKey(true);

         ////======== Spara en Sample-workout till en JSON-fil =======
         //string json2Store = cleanedJson;

         // Spara det till en fil
         //File.WriteAllText("sample_workoutplans.json", json2Store);

         /*string localJson = File.ReadAllText("sample_workoutplans.json");
         var AiReply = JsonSerializer.Deserialize<WorkoutPlan>(localJson);
         JSONFileHanldler<WorkoutPlan>.Save($"Data/Users/{Session.CurrentUser.Id}/workoutplans.json", AiReply);
         //JSONFileHanldler<List<User>>.Save("Data/Users.json", users);
         //JSONHelper.SaveWP(AiReply);
         WPUI.ShowWPUI(AiReply);*/

    }
}
