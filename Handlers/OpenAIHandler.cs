using SnackToSixPack.Classes;
using SnackToSixPack.Handlers;
using Spectre.Console;
using System.Text;
using System.Text.Json;

public class openAIHandler
{
    private static readonly HttpClient client = new HttpClient();
    public static async Task OpenAIHandler()
    {
        bool running = true;

        while (running)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choose an option:")
                    .AddChoices("Ask AI", "Quit"));

            switch (choice)
            {
                case "Ask AI":
                    await AskAI();
                    break;

                case "Quit":
                    running = false;
                    break;
            }
        }
    }

    private static async Task AskAI()
    {
        //string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        //if (string.IsNullOrEmpty(apiKey))
        //{
        //    Console.WriteLine("API key not found");
        //    return;
        //}

        //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

        //Console.WriteLine("Beskriv vad som är målet med din träning:");
        //string userInput = Console.ReadLine();

        //string AiPrompt = $@"
        //    Skapa ett träningsschema i JSON-format med följande egenskaper:
        //    - ""PlanName"": (Namn på träningsschemat)
        //    - ""Goal"": (tydlig och inspirerande beskrivning av träningschemat på svenska, baserat på informationen ifrån användaren : {userInput})
        //    - ""StartDate"": (Dagens datum om inget annat specificerats i {userInput})            
        //    - ""EndDate"": (Datum då målet bör vara nått)
        //    - ""Workouts"": (En lista med träningsdagar som passar baserat på {userInput}. Varje dag skall ha en titel som med ett eller två ord sammanfattar dagens träningsplan)
        //        [
        //            - ""DayOfWeek"": (Vilken dag i veckan träningen avser)
        //            - ""Title"": (Kort sammanfattning med 1-2 ord som namn på dagens träning)
        //            - ""Exercises"": (En lista över övningar som skall utföras på den specifika dagen)
        //                    [
        //                        - ""Name"": (Namn på övningen)
        //                        - ""Sets"": (Antal set om det är applicerbart)
        //                        - ""Reps"": (repetitioner om det är applicerbart)
        //                        - ""Weight"": (Förslagen vikt i kg om det är applicerbart)
        //                        - ""RestTime"": (Vilotid i sekunder mellan set om det är applicerbart) 
        //                    ]
        //        ]

        //    Svara ENDAST med giltig, minimerad JSON och ingen annan text.";

        //var requestBody = new
        //{
        //    model = "gpt-4.1-mini",
        //    messages = new[]
        //    {
        //        new { role = "system", content = "You are a helpful personal trainer," },
        //        new { role = "user", content = AiPrompt }
        //        }
        //};

        //string json = JsonSerializer.Serialize(requestBody);
        //var content = new StringContent(json, Encoding.UTF8, "application/json");

        //var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content); // =========== Lägg till en loading-spinnr här
        //string responseString = await response.Content.ReadAsStringAsync();

        //using JsonDocument doc = JsonDocument.Parse(responseString);
        //string reply = doc.RootElement
        //                    .GetProperty("choices")[0]
        //                    .GetProperty("message")
        //                    .GetProperty("content")
        //                    .GetString();

        //string cleanedJson = reply
        //.Replace("```json", "")
        //.Replace("```", "")
        //.Trim();

        //WorkoutPlan AiReply = JsonSerializer.Deserialize<WorkoutPlan>(cleanedJson);
        //WPUI.ShowWPUI(AiReply);

        // ======== FOR DEMO UN-COMMENT THE CODE ABOVE AND COMMENT THE CODE BELOW ========
        // =============== FOR DEV PURPOSES ONLY ================================
        //string json2Store = cleanedJson;
        //// Spara det till en fil
        //File.WriteAllText("sample_workoutplan.json", json2Store);

        string localJson = File.ReadAllText("sample_workoutplan.json");
        WorkoutPlan aiReply = JsonSerializer.Deserialize<WorkoutPlan>(localJson);
        WPUI.ShowWPUI(aiReply);
    }
}
