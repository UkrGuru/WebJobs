using System.Threading.Tasks;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Actions;
using UkrGuru.WebJobs.Data;
using static UkrGuru.WebJobs.GlobalTests;

namespace WebJobsTests.Actions;

public class BaseActionTests
{
    private const string GOOD_RULE = "next";
    private const string FAIL_RULE = "fail";

    public BaseActionTests()
    {
        DbHelper.ConnectionString = ConnectionString;
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsTrue()
    {
        var job = new Job() { ActionType = nameof(BaseAction) };
        
        var action = job.CreateAction();

        var result = await action.ExecuteAsync();

        Assert.True(result);
    }

    [Theory]
    [InlineData(true, null)]
    [InlineData(true, """{"next": null}""")]
    [InlineData(true, """{"next": ""}""")]
    [InlineData(false, null)]
    [InlineData(false, """{"fail": null}""")]
    [InlineData(false, """{"fail": ""}""")]
    public async Task NextAsync_ReturnsFalse(bool exec_result, string? more)
    {
        var job = new Job() { ActionType = nameof(BaseAction), JobMore = more };

        Assert.False(await job.CreateAction().NextAsync(exec_result));
    }

    [Theory]
    [InlineData(true, """{"next": 1, "next_proc": "Next" }""")]
    [InlineData(false, """{"fail": 1, "fail_proc": "Next" }""")]
    public async Task NextAsync_ReturnsTrue(bool exec_result, string? more)
    {
        var job = new Job() { ActionType = nameof(BaseAction), JobMore = more };

        Assert.True(await job.CreateAction().NextAsync(exec_result));
        
        await DbHelper.DeleteAsync("DELETE WJbQueue WHERE JSON_VALUE(JobMore, '$.proc') = 'Next'");
    }
}