using System.Text.Json;

namespace SnackToSixPack.Classes;

public class Logger
{
    private static string logfile = "log.json";

    public static void LogError(string message)
    {
        // gör det mer struktuerat
        var logObject = new
        {
            Timestamp = DateTime.Now,
            Level = "Error",
            Message = message
        };

        string json = JsonSerializer.Serialize(logObject) + ",\n";
        File.AppendAllText(logfile, json);
    }
}