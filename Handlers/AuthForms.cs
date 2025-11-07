using Spectre.Console;
using System.Reflection;
using System.Runtime.InteropServices.Marshalling;

public class AuthForms
{
    public static void ShowLogInForm()
    {
        AnsiConsole.Clear();

        // Display the title
        // [green] makes the text green
        var heading = new Panel("[green] Log in[/]")
            .Border(BoxBorder.Double)
            // makes the border color white
            .BorderStyle(new Style(Color.White))
            .Padding(5, 0);

        AnsiConsole.Write(heading);
        AnsiConsole.WriteLine();

        string usernameInput = AnsiConsole.Ask<string>("[bold]Username:[/][grey][/]");

        // Skapa en prompt UTAN markup i frågesträngen
        var passwordPrompt = new TextPrompt<string>("[bold]Password:[/][grey][/] ")
            .PromptStyle("green")
            .Secret();


        string passwordInput = AnsiConsole.Prompt(passwordPrompt);

        AnsiConsole.WriteLine();
        AnsiConsole.Markup("[bold]Welcome[/]" + ",[green]" + usernameInput + "[/]!");
    }
}