using Spectre.Console;


//Program.cs
//using System.Threading.Tasks;
//class Program
//{
//    static async Task Main(string[] args)
//    {
//        await OpenAIHandler.RunAsync();
//    }
//}

//Körinstruktioner

//1.Installera paketen på bash
//dotnet add package Spectre.Console
//dotnet add package OpenAI


//2.Sätt din API key:
//setx OPENAI_API_KEY "sk-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"


//3.Kör programmet:
//skriv dotnet run på bash

public class openAIHandler
{
    private static object chatResponse;

    public static async Task OpenAIHandler()
    {
        string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            AnsiConsole.MarkupLine("[red]API key not found.[/]");
            AnsiConsole.MarkupLine("Set it with:");
            AnsiConsole.MarkupLine("[yellow]setx OPENAI_API_KEY your_api_key_here[/]");
            return;
        }


        bool running = true;

        while (running)

        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold cyan]Choose an option:[/]")
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

    private static async Task OpenAIHandler(string apiKey)
    {
        var client = new OpenAIClient(apiKey);

        var question = AnsiConsole.Ask<string>("[green]Enter your question:[/]");

        // Visa spinner medan AI tänker
        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .StartAsync("Asking AI...", static async ctx =>
            {



                string answer = chatResponse.ToString();    

                // Skriv ut svaret snyggt efter spinnern
                AnsiConsole.MarkupLine($"\n[bold yellow]AI:[/] {answer}\n");
            });
    }


}
