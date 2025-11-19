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

        Console.WriteLine("Beskriv vad som �r m�let med din tr�ning:");

        string userInput = Console.ReadLine();

        string AiPrompt = $@"
           Skapa ett tr�ningsschema i JSON-format med f�ljande egenskaper: (Jag vill även att du tar hänsyn till användarens kroppsliga profil: {Session.CurrentUser.Profile})
           - ""PlanName"": (Namn p� tr�ningsschemat)
           - ""Goal"": (tydlig och inspirerande beskrivning av tr�ningschemat p� svenska, baserat p� informationen ifr�n anv�ndaren : {userInput})
           - ""StartDate"": (Dagens datum om inget annat specificerats i {userInput})            
           - ""EndDate"": (Datum d� m�let b�r vara n�tt)
           - ""Workouts"": (En lista med tr�ningsdagar som passar baserat p� {userInput}. Varje dag skall ha en titel som med ett eller tv� ord sammanfattar dagens tr�ningsplan)
               [
                   - ""DayOfWeek"": (Vilken dag i veckan tr�ningen avser)
                   - ""Title"": (Kort sammanfattning med 1-2 ord som namn p� dagens tr�ning)
                   - ""Exercises"": (En lista �ver �vningar som skall utf�ras p� den specifika dagen)
                           [
                               - ""Name"": (Namn p� �vningen)
                               - ""Sets"": (Antal set om det �r applicerbart)
                               - ""Reps"": (repetitioner om det �r applicerbart)
                               - ""Weight"": (F�rslagen vikt i kg om det �r applicerbart)
                               - ""RestTime"": (Vilotid i sekunder mellan set om det �r applicerbart) 
                           ]
               ]

           Svara ENDAST med giltig, minimerad JSON och ingen annan text.";

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

        var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content); // =========== L�gg till en loading-spinnr h�r
        string responseString = await response.Content.ReadAsStringAsync();

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
        try
        {
            JSONFileHanldler<WorkoutPlan>.Save($"Data/Users/{Session.CurrentUser.Id}/workoutplans.json", AiReply);
            WPUI.ShowWPUI(AiReply);

        } catch (FileNotFoundException ex) {
            File.AppendAllText("log.json", $"[{DateTime.Now}] ERROR: Failed to load users: {ex.Message}{Environment.NewLine}");
            AnsiConsole.MarkupLine("[red] Could not found workout plan .[/]");
            AnsiConsole.MarkupLine("Press any key to return to the main menu...");
            Console.ReadKey(true);
            return;
        }





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
