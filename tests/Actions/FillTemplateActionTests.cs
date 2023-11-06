using System.Threading.Tasks;
using UkrGuru.SqlJson;
using UkrGuru.SqlJson.Extensions;
using UkrGuru.WebJobs.Actions;
using UkrGuru.WebJobs.Data;
using static UkrGuru.WebJobs.GlobalTests;

namespace WebJobsTests.Actions;

public class FillTemplateActionTests
{
    public FillTemplateActionTests()
    {
        DbHelper.ConnectionString = ConnectionString;
    }

    [Fact]
    public async Task CanFillTemplate()
    {
        Job job = new() { ActionType = nameof(FillTemplateAction) };
        job.ActionMore = """{ "tname_pattern": "[A-Z]{1,}[_]{1,}[A-Z]{1,}[_]{0,}[A-Z]{0,}" }""";
        job.JobMore = """{ "template_subject": "Hello DEAR_NAME!", "tvalue_DEAR_NAME": "Alex" }""";

        var action = job.CreateAction();

        Assert.True(await action.ExecuteAsync());

        Assert.Equal("Hello Alex!", ((More)action.More).GetValue("next_subject"));
    }
}