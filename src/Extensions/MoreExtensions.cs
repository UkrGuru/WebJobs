// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text.Json;

namespace UkrGuru.WebJobs.Data;

public static class MoreExtensions
{
    public static void AddNew(this More more, string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return;

        var items = JsonSerializer.Deserialize<More>(json);

        if (items == null) return;

        foreach (var item in items.Where(item => !more.ContainsKey(item.Key)))
            more.Add(item.Key, item.Value);
    }

    public static string? GetValue(this More more, string name) => more.TryGetValue(name, out var value) && value != null ? Convert.ToString(value) : null;

    public static bool? GetValue(this More more, string name, bool? defaultValue) => bool.TryParse(more.GetValue(name), out bool value) ? value : defaultValue;

    public static int? GetValue(this More more, string name, int? defaultValue) => int.TryParse(more.GetValue(name), out int value) ? value : defaultValue;

    public static double? GetValue(this More more, string name, double? defaultValue) => double.TryParse(more.GetValue(name), out double value) ? value : defaultValue;

    public static DateTime? GetValue(this More more, string name, DateTime? defaultValue) => DateTime.TryParse(more.GetValue(name), out DateTime value) ? value : defaultValue;

    public static object[]? GetValue(this More more, string name, object[]? defaultValue)
    {
        try
        {
            var value = more.GetValue(name);
            ArgumentNullException.ThrowIfNull(value);

            return JsonSerializer.Deserialize<object[]?>(value);
        }
        catch
        {
            return defaultValue;
        }
    }
}