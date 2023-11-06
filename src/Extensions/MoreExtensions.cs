// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using UkrGuru.SqlJson.Extensions;

namespace UkrGuru.WebJobs.Data;

/// <summary>
/// Provides extension methods for the <see cref="More"/> class.
/// </summary>
public static class MoreExtensions
{
    /// <summary>
    /// Creates a new <see cref="More"/> object that contains only the items whose keys start with the specified prefix.
    /// </summary>
    /// <param name="more">The <see cref="More"/> object to filter.</param>
    /// <param name="next_prefix">The prefix to filter by.</param>
    /// <returns>A new <see cref="More"/> object that contains only the items whose keys start with the specified prefix.</returns>
    public static More CreateNextMore(this More more, string next_prefix)
    {
        var next_more = new More();
        foreach (var m in more.Where(item => item.Key.StartsWith(next_prefix)))
            next_more.Add(m.Key[next_prefix.Length..], m.Value);
        
        return next_more;
    }
}