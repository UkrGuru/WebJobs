// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using UkrGuru.SqlJson;
using Action = UkrGuru.WebJobs.Data.Action;
using Rule = UkrGuru.WebJobs.Data.Rule;
using static UkrGuru.WebJobs.Actions.RunApiProcAction;
using static UkrGuru.WebJobs.Actions.SendEmailAction;
using static UkrGuru.WebJobs.Actions.SsrsExportReportAction;
using UkrGuru.WebJobs.Data;
using UkrGuru.Extensions;

namespace UkrGuru.WebJobs;

public class DbTests
{
    public DbTests() { int i = 0; while (!GlobalTests.DbOk && i++ < 100) { Thread.Sleep(100); } }

    [Fact]
    public async Task WJbActionsTests()
    {
        var actions_get_tsql = """
            SELECT ActionId, ActionName, ActionType FROM WJbActions WHERE ActionId = @Data
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
            """;

        Action? action = await DbHelper.ExecAsync<Action?>(actions_get_tsql, 1);
        Assert.NotNull(action);
        Assert.Equal("RunSqlProc", action.ActionName);
        Assert.Equal("RunSqlProcAction, UkrGuru.WebJobs", action.ActionType);

        action = await DbHelper.ExecAsync<Action?>(actions_get_tsql, 2);
        Assert.NotNull(action);
        Assert.Equal("SendEmail", action.ActionName);
        Assert.Equal("SendEmailAction, UkrGuru.WebJobs", action.ActionType);

        action = await DbHelper.ExecAsync<Action?>(actions_get_tsql, 3);
        Assert.NotNull(action);
        Assert.Equal("FillTemplate", action.ActionName);
        Assert.Equal("FillTemplateAction, UkrGuru.WebJobs", action.ActionType);

        action = await DbHelper.ExecAsync<Action?>(actions_get_tsql, 4);
        Assert.NotNull(action);
        Assert.Equal("DownloadPage", action.ActionName);
        Assert.Equal("DownloadPageAction, UkrGuru.WebJobs", action.ActionType);

        action = await DbHelper.ExecAsync<Action?>(actions_get_tsql, 5);
        Assert.NotNull(action);
        Assert.Equal("RunApiProc", action.ActionName);
        Assert.Equal("RunApiProcAction, UkrGuru.WebJobs", action.ActionType);

        action = await DbHelper.ExecAsync<Action?>(actions_get_tsql, 6);
        Assert.NotNull(action);
        Assert.Equal("ProcItems", action.ActionName);
        Assert.Equal("ProcItemsAction, UkrGuru.WebJobs", action.ActionType);

        action = await DbHelper.ExecAsync<Action?>(actions_get_tsql, 7);
        Assert.NotNull(action);
        Assert.Equal("ParseText", action.ActionName);
        Assert.Equal("ParseTextAction, UkrGuru.WebJobs", action.ActionType);

        action = await DbHelper.ExecAsync<Action?>(actions_get_tsql, 10);
        Assert.NotNull(action);
        Assert.Equal("SSRS.ExportReport", action.ActionName);
        Assert.Equal("SsrsExportReportAction, UkrGuru.WebJobs", action.ActionType);
    }

    [Fact]
    public async Task WJbRulesTests()
    {
        var rules_get_tsql = """
            SELECT RuleId, ActionId FROM WJbRules WHERE RuleId = @Data 
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
            """;

        Rule? rule = await DbHelper.ExecAsync<Rule?>(rules_get_tsql, 1);
        Assert.NotNull(rule);
        Assert.Equal(1, rule.ActionId);

        rule = await DbHelper.ExecAsync<Rule?>(rules_get_tsql, 2);
        Assert.NotNull(rule);
        Assert.Equal(2, rule.ActionId);

        rule = await DbHelper.ExecAsync<Rule?>(rules_get_tsql, 3);
        Assert.NotNull(rule);
        Assert.Equal(3, rule.ActionId);

        rule = await DbHelper.ExecAsync<Rule?>(rules_get_tsql, 4);
        Assert.NotNull(rule);
        Assert.Equal(4, rule.ActionId);

        rule = await DbHelper.ExecAsync<Rule?>(rules_get_tsql, 5);
        Assert.NotNull(rule);
        Assert.Equal(5, rule.ActionId);

        rule = await DbHelper.ExecAsync<Rule?>(rules_get_tsql, 6);
        Assert.NotNull(rule);
        Assert.Equal(6, rule.ActionId);

        rule = await DbHelper.ExecAsync<Rule?>(rules_get_tsql, 7);
        Assert.NotNull(rule);
        Assert.Equal(7, rule.ActionId);

        rule = await DbHelper.ExecAsync<Rule?>(rules_get_tsql, 10);
        Assert.NotNull(rule);
        Assert.Equal(10, rule.ActionId);
    }

    [Fact]
    public async Task WJbJobsTests()
    {
        var jobId = await DbHelper.ExecAsync<int?>("WJbQueue_Ins",
            new { Rule = "1", RulePriority = (byte)Priorities.ASAP, RuleMore = new More { { "proc", "Clean" } } });
        Assert.NotNull(jobId);

        var job = await DbHelper.ExecAsync<Job?>("WJbQueue_Get", jobId);
        Assert.NotNull(job);
        Assert.Equal(1, job.RuleId);
        Assert.Equal(Priorities.ASAP, job.JobPriority);
        Assert.Equal(JobStatus.Queued, job.JobStatus);
        Assert.Null(job.Started);
        Assert.Null(job.Finished);

        job = await DbHelper.ExecAsync<Job?>("WJbQueue_Start1st");
        Assert.NotNull(job);
        Assert.Equal(1, job.RuleId);
        Assert.Equal(Priorities.ASAP, job.JobPriority);
        Assert.Equal(JobStatus.Running, job.JobStatus);
        Assert.NotNull(job.Started);
        Assert.Null(job.Finished);

        await DbHelper.ExecAsync<Job?>("WJbQueue_Finish", new { JobId = jobId, JobStatus = JobStatus.Cancelled });

        job = await DbHelper.ExecAsync<Job?>("""
            SELECT TOP (1) H.*, R.RuleMore, A.ActionName, A.ActionType, A.ActionMore
            FROM WJbHistory H
            INNER JOIN WJbRules R ON H.RuleId = R.RuleId
            INNER JOIN WJbActions A ON R.ActionId = A.ActionId
            WHERE H.JobId = @Data
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
            """, jobId);

        Assert.NotNull(job);
        Assert.Equal(1, job.RuleId);
        Assert.Equal(Priorities.ASAP, job.JobPriority);
        Assert.Equal(JobStatus.Cancelled, job.JobStatus);
        Assert.NotNull(job.Started);
        Assert.NotNull(job.Finished);

        await DbHelper.ExecAsync("""UPDATE WJbRules SET RuleMore = '{ "cron": "* * * * *" }' WHERE RuleId IN (1, 2, 3)""");

        var q1 = await DbHelper.ExecAsync<int?>("SELECT COUNT(*) FROM WJbQueue") ?? 0;

        await DbHelper.ExecAsync("WJbQueue_InsCron");

        await DbHelper.ExecAsync("""UPDATE WJbRules SET RuleMore = NULL WHERE RuleId IN (1, 2, 3)""");

        var q2 = await DbHelper.ExecAsync<int?>("SELECT COUNT(*) FROM WJbQueue") ?? 0;

        Assert.Equal(3, (q2 - q1));

        q1 = await DbHelper.ExecAsync<int?>("SELECT COUNT(*) FROM WJbHistory") ?? 0;

        await DbHelper.ExecAsync("""UPDATE WJbQueue SET [Started] = GETDATE(), JobStatus = 2 /* Running */""");

        await DbHelper.ExecAsync("WJbQueue_FinishAll");

        q2 = await DbHelper.ExecAsync<int?>("SELECT COUNT(*) FROM WJbHistory") ?? 0;

        Assert.Equal(3, (q2 - q1));
    }

    [Fact]
    public async Task WJbSettingsTests()
    {
        var excepted = Guid.NewGuid().ToString();

        await DbHelper.ExecAsync("""
            INSERT INTO WJbSettings 
            SELECT * FROM OPENJSON(@Data) 
            WITH ([Name] nvarchar(100), [Value] nvarchar(max)) D
            """,
            new { Name = excepted, Value = excepted });

        var actual = await DbHelper.ExecAsync<string?>("WJbSettings_Get", excepted);

        Assert.Equal(excepted, actual);

        await DbHelper.ExecAsync<ApiSettings?>("DELETE FROM WJbSettings WHERE [Name] = @Data", excepted);

        var api_settings = await DbHelper.ExecAsync<ApiSettings?>("WJbSettings_Get", "ApiSettings");
        Assert.NotNull(api_settings);
        //Assert.Equal("https://youwebsite/", api_settings.Url);
        //Assert.Equal("test", api_settings.Key);

        var smtp_settings = await DbHelper.ExecAsync<SmtpSettings?>("WJbSettings_Get", "StmpSettings");
        Assert.NotNull(smtp_settings);
        //Assert.Equal("test@test.com", smtp_settings.From);
        //Assert.Equal("smtp.test.com", smtp_settings.Host);
        //Assert.Equal(587, smtp_settings.Port);
        //Assert.True(smtp_settings.EnableSsl);
        //Assert.Equal("test@test.com", smtp_settings.UserName);
        //Assert.Equal("12345", smtp_settings.Password);

        var ssrs_settings = await DbHelper.ExecAsync<SsrsSettings?>("WJbSettings_Get", "SsrsSettings");
        Assert.NotNull(ssrs_settings);
        //Assert.Equal("https://youwebsite/ReportServer_2019/?", ssrs_settings.BaseUrl);
        //Assert.Equal("test", ssrs_settings.UserName);
        //Assert.Equal("12345", ssrs_settings.Password);
    }

    [Fact]
    public async Task WJbItemsTests()
    {
        Assert.True(true);
    }
}