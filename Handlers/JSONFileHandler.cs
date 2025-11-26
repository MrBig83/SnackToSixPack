using System.Text.Json;

namespace SnackToSixPack.Classes;
public static class JSONFileHanldler
{
    public static T Load<T>(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");
            
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(json);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error loading file at {filePath}: {ex.Message}");
            return default;
        }
    }

    public static void Save<T>(string filePath, T data)
    {
        try
        {
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions()
            {
                WriteIndented = true
            });
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error saving file at {filePath}: {ex.Message}");
        }
        
    }

}