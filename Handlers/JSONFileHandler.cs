using System.Text.Json;

public static class JSONFileHanldler<T>
{
    public static T Load<T>(string filePath)
    {
        if (!File.Exists(filePath))
            return default!;

        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<T>(json);
    }

    public static void Save<T>(string filePath, T data)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        string json = JsonSerializer.Serialize(data, options);
        File.WriteAllText(filePath, json);
    }

}