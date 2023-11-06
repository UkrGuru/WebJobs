using System;
using System.Threading.Tasks;
using UkrGuru.SqlJson;
using UkrGuru.SqlJson.Extensions;
using UkrGuru.WebJobs.Data;
using static UkrGuru.WebJobs.GlobalTests;

namespace WebJobsTests.Extensions;

public class WJbDbHelperTests
{
    public WJbDbHelperTests()
    {
        DbHelper.ConnectionString = ConnectionString;
    }

    [Fact]
    public async Task WJbQueue_FullCycle_Test()
    {
        var jobId = await WJbDbHelper.WJbQueue_InsAsync("1", new More { { "proc", "Clean" } }, Priority.ASAP);
        Assert.NotNull(jobId);

        var jobQ = await WJbDbHelper.WJbQueue_Start1stAsync();
        Assert.NotNull(jobQ);
        Assert.Equal(jobId, jobQ.JobId);
        Assert.Equal(Priority.ASAP, jobQ.JobPriority);
        Assert.Equal(1, jobQ.RuleId);
        Assert.NotNull(jobQ.Started);
        Assert.Null(jobQ.Finished);
        Assert.Equal("""{"proc":"Clean"}""", jobQ.JobMore);
        Assert.Equal(JobStatus.Running, jobQ.JobStatus);

        await WJbDbHelper.WJbQueue_FinishAsync(jobId, true);

        var jobH = await DbHelper.ReadAsync<Job?>("""
            SELECT TOP (1) H.*, R.RuleMore, A.ActionName, A.ActionType, A.ActionMore
            FROM WJbHistory H
            INNER JOIN WJbRules R ON H.RuleId = R.RuleId
            INNER JOIN WJbActions A ON R.ActionId = A.ActionId
            WHERE H.JobId = @Data
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
            """, jobId);

        Assert.NotNull(jobH);
        Assert.Equal(jobId, jobH.JobId);
        Assert.Equal(Priority.ASAP, jobH.JobPriority);
        Assert.Equal(1, jobH.RuleId);
        Assert.NotNull(jobH.Started);
        Assert.NotNull(jobH.Finished);
        Assert.Equal("""{"proc":"Clean"}""", jobH.JobMore);
        Assert.Equal(JobStatus.Completed, jobH.JobStatus);

        await DbHelper.DeleteAsync("DELETE WJbHistory WHERE JobId = @Data", jobId);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task WJbQueue_FinishAsync_Tests(bool exec_result)
    {
        var jobId = await DbHelper.CreateAsync<int?>("INSERT INTO WJbQueue (RuleId, Started) \r\nVALUES (1, GETDATE()); \r\nSELECT SCOPE_IDENTITY();");
        Assert.NotNull(jobId);

        await WJbDbHelper.WJbQueue_FinishAsync(jobId, exec_result);

        var job = await WJbDbHelper.WJbQueue_GetAsync(jobId);
        Assert.Null(job);

        Assert.NotNull(await DbHelper.ReadAsync<DateTime?>("SElECT TOP 1 Finished FROM WJbHistory WHERE JobId = @Data", jobId));

        var expectedJobStatus = exec_result ? JobStatus.Completed : JobStatus.Failed; 
        Assert.Equal(expectedJobStatus, await DbHelper.ReadAsync<JobStatus?>("SElECT TOP 1 JobStatus FROM WJbHistory WHERE JobId = @Data", jobId));

        await DbHelper.DeleteAsync("DELETE WJbHistory WHERE JobId = @Data", jobId);
    }

    //[Fact]
    //public async Task WJbQueue_GetAsync_Test()
    //{
    //    var jobId = await DbHelper.CreateAsync<int?>("INSERT INTO WJbQueue (RuleId, Started) \r\nVALUES (1, GETDATE()); \r\nSELECT SCOPE_IDENTITY();");
    //    Assert.NotNull(jobId);

    //    var job = await WJbDbHelper.WJbQueue_GetAsync(jobId);
    //    Assert.NotNull(job);
    //    Assert.Equal(1, job.RuleId);

    //    await DbHelper.DeleteAsync("DELETE WJbQueue WHERE JobId = @Data", jobId);
    //}

    //[Fact]
    //public async Task WJbQueue_InsAsync_Test()
    //{
    //    var jobId = await WJbDbHelper.WJbQueue_InsAsync(rule: "1", priority: Priority.ASAP, more: new More { { "proc", "Clean" } });
    //    Assert.NotNull(jobId);

    //    var job = await WJbDbHelper.WJbQueue_GetAsync(jobId);
    //    Assert.NotNull(job);
    //    Assert.Equal(1, job.RuleId);
    //    Assert.Equal(Priority.ASAP, job.JobPriority);
    //    Assert.Equal(JobStatus.Queued, job.JobStatus);
    //    Assert.Null(job.Started);
    //    Assert.Null(job.Finished);
    //    Assert.Equal("""{"proc":"Clean"}""", job.JobMore);
        
    //    await DbHelper.DeleteAsync("DELETE WJbQueue WHERE JobId = @Data", jobId);
    //}

    //[Fact]
    //public async Task WJbQueue_InsCronAsync_Test()
    //{
    //    await DbHelper.ExecAsync("""UPDATE WJbRules SET RuleMore = '{ "cron": "* * * * *" }' WHERE RuleId IN (2, 3)""");

    //    var q1 = await DbHelper.ExecAsync<int?>("SELECT COUNT(*) FROM WJbQueue") ?? 0;

    //    await DbHelper.ExecAsync("WJbQueue_InsCron");

    //    await DbHelper.ExecAsync("""UPDATE WJbRules SET RuleMore = NULL WHERE RuleId IN (2, 3)""");

    //    var q2 = await DbHelper.ExecAsync<int?>("SELECT COUNT(*) FROM WJbQueue") ?? 0;

    //    Assert.Equal(2, (q2 - q1));

    //    q1 = await DbHelper.ExecAsync<int?>("SELECT COUNT(*) FROM WJbHistory") ?? 0;

    //    await DbHelper.ExecAsync("""UPDATE WJbQueue SET [Started] = GETDATE(), JobStatus = 2 /* Running */""");

    //    await DbHelper.ExecAsync("WJbQueue_FinishAll");

    //    q2 = await DbHelper.ExecAsync<int?>("SELECT COUNT(*) FROM WJbHistory") ?? 0;

    //    Assert.Equal(3, (q2 - q1));
    //}

    [Theory]
    [InlineData(null, null)]
    [InlineData("ApiSettings", "{\r\n  \"url\": \"https://youwebsite/\",\r\n  \"key\": \"test\"\r\n}")]
    [InlineData("HttpSettings", "{\r\n  \"url\": \"https://youwebsite/\",\r\n  \"headers\": null\r\n}")]
    [InlineData("StmpSettings", "{\r\n  \"from\": \"test@test.com\",\r\n  \"host\": \"smtp.test.com\",\r\n  \"port\": 587,\r\n  \"enableSsl\": true,\r\n  \"userName\": \"test@test.com\",\r\n  \"password\": \"12345\"\r\n}")]
    [InlineData("SsrsSettings", "{\r\n  \"baseUrl\": \"https://youwebsite/ReportServer_2019/?\",\r\n  \"userName\": \"test\",\r\n  \"password\": \"12345\"\r\n}")]
    public async Task WJbSettings_GetAsync_Tests(string? name, string? expected)
        => Assert.Equal(expected, await WJbDbHelper.WJbSettings_GetAsync<string?>(name));
}