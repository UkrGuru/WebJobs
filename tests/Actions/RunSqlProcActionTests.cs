using System.Threading.Tasks;
using UkrGuru.SqlJson;
using UkrGuru.SqlJson.Extensions;
using UkrGuru.WebJobs.Data;
using static UkrGuru.WebJobs.GlobalTests;

namespace WebJobsTests.Actions;

public class RunSqlProcActionTests
{
    public RunSqlProcActionTests()
    {
        DbHelper.ConnectionString = ConnectionString;
    }

    [Fact]
    public async Task CanRunSqlProcNull()
    {
        await DbHelper.ExecAsync("CREATE OR ALTER PROCEDURE WJb_NullTest AS SELECT 'OK'");

        var job = new Job() { ActionType = "RunSqlProcAction, UkrGuru.WebJobs" };
        job.JobMore = """{ "proc": "NullTest", "result_name": "proc_result" }""";

        var action = job.CreateAction();

        Assert.True(await action.ExecuteAsync());

        Assert.Equal("OK", ((More)action.More).GetValue("proc_result"));
    }

    [Fact]
    public async Task CanRunSqlProcData()
    {
        await DbHelper.ExecAsync("CREATE OR ALTER PROCEDURE WJb_DataTest (@Data varchar(100)) AS SELECT @Data");

        var job = new Job() { ActionType = "RunSqlProcAction, UkrGuru.WebJobs" };
        job.JobMore = """{ "proc": "DataTest", "data": "DATA", "result_name": "proc_result" }""";

        var action = job.CreateAction();

        Assert.True(await action.ExecuteAsync());

        Assert.Equal("DATA", ((More)action.More).GetValue("proc_result"));
    }
}
