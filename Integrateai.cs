using OpenAI;
using OpenAI.Chat;
using Spectre.Console;

public class openAIHandler
{
    public static async Task OpenAIHander()
    {
        var client = new OpenAIClient("API_KEY");


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
                    await AskAI(client);
                    break;

                case "Quit":
                    running = false;
                    break;
            }
        }
    }

    private static async Task AskAI(OpenAIClient client)
    {
        throw new NotImplementedException();
    }
}
