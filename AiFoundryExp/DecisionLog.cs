using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace AiFoundryExp;

/// <summary>
/// Represents a record of decisions made by the orchestration layer.
/// </summary>
public class DecisionLog
{
    private readonly List<DecisionLogEntry> _entries = new();

    /// <summary>
    /// List of logged decisions.
    /// </summary>
    public IReadOnlyList<DecisionLogEntry> Entries => _entries;

    /// <summary>
    /// Add a decision entry with context and rationale.
    /// </summary>
    public void AddEntry(string context, string decision, string rationale)
    {
        _entries.Add(new DecisionLogEntry
        {
            Timestamp = DateTime.UtcNow,
            Context = context,
            Decision = decision,
            Rationale = rationale
        });
    }

    /// <summary>
    /// Persist the log to a JSON file.
    /// </summary>
    public void Save(string path)
    {
        string json = JsonSerializer.Serialize(_entries, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }
}

/// <summary>
/// Details about a single decision.
/// </summary>
public class DecisionLogEntry
{
    public DateTime Timestamp { get; init; }

    public string Context { get; init; } = string.Empty;

    public string Decision { get; init; } = string.Empty;

    public string Rationale { get; init; } = string.Empty;
}
