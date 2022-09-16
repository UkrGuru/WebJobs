using System;
using System.Text.Json;
using Xunit;

namespace UkrGuru.WebJobs.Data.Tests;

public class MoreExtensionsTests
{
    [Fact]
    public void AddNewTest()
    {
        More more = new();
        more.AddNew(null);
        more.AddNew("");
        more.AddNew(" ");
        more.AddNew(JsonSerializer.Serialize(new {type = "Rule", data = "", data1 = null as string, enabled = true }));

        var files = new[] { "file1.txt", "file2.txt" };
        more.AddNew(JsonSerializer.Serialize(new { type = "Action", timeout = 60, amount = 123.45, files }));

        Assert.Equal("Rule", more.GetValue("type"));
        Assert.Empty(more.GetValue("data")!);
        Assert.Null(more.GetValue("data1"));
        Assert.Null(more.GetValue("data2"));
        Assert.Equal(true, more.GetValue("enabled", false));
        Assert.Equal(60, more.GetValue("timeout", 0));
        Assert.Equal(123.45, more.GetValue("amount", 0.0));

        var files1 = more.GetValue("files", (object[]?)null);
        Assert.Equal(files[0], Convert.ToString(files1?[0]));
        Assert.Equal(files[1], Convert.ToString(files1?[1]));
    }
}