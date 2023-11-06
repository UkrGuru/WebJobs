// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading.Tasks;
using UkrGuru.SqlJson;
using Action = UkrGuru.WebJobs.Data.Action;
using Rule = UkrGuru.WebJobs.Data.Rule;
using UkrGuru.WebJobs.Data;
using UkrGuru.SqlJson.Extensions;
using static UkrGuru.WebJobs.GlobalTests;

namespace UkrGuru.WebJobs;

public class DbTests
{
    public DbTests() {
        DbHelper.ConnectionString = ConnectionString;
    }

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

        action = await DbHelper.ExecAsync<Action?>(actions_get_tsql, 15);
        Assert.NotNull(action);
        Assert.Equal("SendHttpRequest", action.ActionName);
        Assert.Equal("SendHttpRequestAction, UkrGuru.WebJobs", action.ActionType);
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

        rule = await DbHelper.ExecAsync<Rule?>(rules_get_tsql, 15);
        Assert.NotNull(rule);
        Assert.Equal(15, rule.ActionId);
    }

    [Fact]
    public async Task WJbItemsTests()
    {
        Assert.True(true);

        await Task.CompletedTask;
    }
}