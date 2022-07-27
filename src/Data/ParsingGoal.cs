using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UkrGuru.WebJobs.Data;

public class ParsingGoal
{
    [Key]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("parent")]
    public string? Parent { get; set; } = string.Empty;

    [JsonPropertyName("start")]
    public string? Start { get; set; }

    [JsonPropertyName("end")]
    public string? End { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }
}
