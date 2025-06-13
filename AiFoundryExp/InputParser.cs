using System.Text.Json;

namespace AiFoundryExp;

/// <summary>
/// Utility to parse the initial context provided in the input file.
/// Supports JSON or simple key-value lines.
/// </summary>
public static class InputParser
{
    public static Dictionary<string, string> ParseFile(string path)
    {
        var context = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (!File.Exists(path))
            return context;

        string text = File.ReadAllText(path).Trim();
        if (string.IsNullOrEmpty(text))
            return context;

        if (text.StartsWith("{") || text.StartsWith("[") )
        {
            try
            {
                var jsonContext = JsonSerializer.Deserialize<Dictionary<string, string>>(text);
                if (jsonContext is not null)
                {
                    foreach (var kv in jsonContext)
                    {
                        context[kv.Key] = kv.Value;
                    }
                    return context;
                }
            }
            catch (JsonException)
            {
                // Fall back to line parsing
            }
        }

        string[] lines = File.ReadAllLines(path);
        if (lines.Length == 1 && !lines[0].Contains(':') && !lines[0].Contains('='))
        {
            context["business_idea"] = lines[0].Trim();
            return context;
        }

        foreach (string line in lines)
        {
            string trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed))
                continue;

            int sep = trimmed.IndexOf(':');
            if (sep == -1)
                sep = trimmed.IndexOf('=');
            if (sep > 0)
            {
                string key = trimmed[..sep].Trim().Replace(' ', '_');
                string value = trimmed[(sep + 1)..].Trim();
                if (!string.IsNullOrEmpty(key))
                {
                    context[key] = value;
                }
            }
        }

        if (context.Count == 0 && !string.IsNullOrEmpty(text))
        {
            context["business_idea"] = text;
        }

        return context;
    }
}
