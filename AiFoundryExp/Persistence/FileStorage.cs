using System.Text.Json;

namespace AiFoundryExp;

public static class FileStorage
{
    public static EngineState LoadState(string path)
    {
        if (!File.Exists(path))
        {
            return new EngineState();
        }

        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<EngineState>(json) ?? new EngineState();
    }

    public static void SaveState(string path, EngineState state)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        string json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }
}
