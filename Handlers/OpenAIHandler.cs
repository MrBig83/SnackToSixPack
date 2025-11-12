using OpenAI;
using OpenAI.Chat;
using Spectre.Console;

public class openAIHandler
{
    public static async Task OpenAIHandler()
    {
        string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("API key not found");
            return;
        }


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
                    await AskAI(apiKey);
                    break;

                case "Quit":
                    running = false;
                    break;
            }
        }
    }

    private static async Task AskAI(string apiKey)
    {
        throw new NotImplementedException();
    }
}
