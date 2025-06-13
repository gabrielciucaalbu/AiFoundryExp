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
        if (!File.Exists(path))
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        string text = File.ReadAllText(path);
        return ParseTextInternal(text, "business_idea");
    }

    /// <summary>
    /// Parse free-form text entered by a user. Supports JSON or key/value pairs.
    /// Unstructured text is returned under the key "value".
    /// </summary>
    public static Dictionary<string, string> ParseText(string text)
        => ParseTextInternal(text, "value");

    private static Dictionary<string, string> ParseTextInternal(string text, string defaultKey)
    {
        var context = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(text))
            return context;

        text = text.Trim();

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

        string[] lines = text.Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length == 1 && !lines[0].Contains(':') && !lines[0].Contains('='))
        {
            context[defaultKey] = lines[0].Trim();
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
            context[defaultKey] = text;
        }

        return context;
    }
}
