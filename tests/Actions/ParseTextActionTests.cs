using System.Text.Json;
using System.Threading.Tasks;
using UkrGuru.SqlJson;
using UkrGuru.SqlJson.Extensions;
using UkrGuru.WebJobs.Actions;
using UkrGuru.WebJobs.Data;
using WebJobsTests.Extensions;
using static UkrGuru.WebJobs.GlobalTests;

namespace WebJobsTests.Actions;

public class ParseTextActionTests
{
    public ParseTextActionTests()
    {
        DbHelper.ConnectionString = ConnectionString;
    }

    [Fact]
    public async Task CanParseText()
    {
        Job job = new() { ActionType = nameof(ParseTextAction) };
        job.JobMore = JsonSerializer.Serialize(new { text = ParseTextExtensionsTests.Text, goals = ParseTextExtensionsTests.Goals, result_name = "next_data" });

        var action = job.CreateAction();

        Assert.True(await action.ExecuteAsync());

        Assert.Equal(ParseTextExtensionsTests.Result, ((More)action.More).GetValue("next_data"));
    }
}
