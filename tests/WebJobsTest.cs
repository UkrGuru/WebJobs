using Xunit;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Text.Json;
using WebJobsTests.Extensions;
using UkrGuru.Extensions;

namespace System.Reflection.Tests;

public class WebJobsTest
{
    private readonly bool dbOK = false;

    public WebJobsTest()
    {
        var dbName = "WebJobsTest2";

        var connectionString = $"Server=(localdb)\\mssqllocaldb;Database={dbName};Trusted_Connection=True";

        DbHelper.ConnectionString = connectionString.Replace(dbName, "master");

        DbHelper.ExecCommand($"IF DB_ID('{dbName}') IS NULL CREATE DATABASE {dbName};");

        DbHelper.ConnectionString = connectionString;

        if (dbOK) return;

        var assembly = Assembly.GetAssembly(typeof(UkrGuru.WebJobs.Actions.BaseAction));
        ArgumentNullException.ThrowIfNull(assembly);

        dbOK = assembly.InitDb();
    }

    [Fact]
    public void InitDbTest()
    {
        Assert.True(dbOK);
    }

    [Fact]
    public async Task WJbSettingsTests()
    {
        await DbHelper.ExecProcAsync("WJbSettings_Set", new { Name = "Name1", Value = "Value1" });

        Assert.Equal("Value1", DbHelper.FromProc<string?>("WJbSettings_Get", "Name1"));

        await DbHelper.ExecProcAsync("WJbSettings_Set", new { Name = "Name1", Value = "Value11" });

        Assert.Equal("Value11", DbHelper.FromProc<string?>("WJbSettings_Get", "Name1"));
    }

    [Fact]
    public async Task BaseActionTest()
    {
        Job job = new() { ActionType = "BaseAction, UkrGuru.WebJobs" };

        bool result = false;

        var action = job.CreateAction();

        if (action != null)
        {
            result = await action.ExecuteAsync();

            await action.NextAsync(result);
        }

        Assert.True(result);
    }

    [Fact]
    public async Task RunSqlProcDataTest()
    {
        await DbHelper.ExecCommandAsync("CREATE OR ALTER PROCEDURE [dbo].[WJb_DataTest] (@Data varchar(100)) AS SELECT @Data");

        Job job = new() { ActionType = "RunSqlProcAction, UkrGuru.WebJobs" };
        job.JobMore = @"{ ""proc"": ""DataTest"", ""data"": ""DATA"", ""result_name"": ""proc_result"" }";

        var proc_result = null as string;

        var action = job.CreateAction();

        if (action != null)
        {
            await action.ExecuteAsync();

            proc_result = ((More)action.More).GetValue("proc_result");
        }

        Assert.Equal("DATA", proc_result);
    }

    [Fact]
    public async Task RunSqlProcNullTest()
    {
        await DbHelper.ExecCommandAsync("CREATE OR ALTER PROCEDURE [dbo].[WJb_NullTest] AS SELECT 'OK'");

        Job job = new() { ActionType = "RunSqlProcAction, UkrGuru.WebJobs" };
        job.JobMore = JsonSerializer.Serialize(new { proc = "NullTest", data = null as string, result_name = "proc_result" });

        var proc_result = null as string;

        var action = job.CreateAction();

        if (action != null)
        {
            await action.ExecuteAsync();

            proc_result = ((More)action.More).GetValue("proc_result");
        }

        Assert.Equal("OK", proc_result);
    }

    //[Fact]
    //public void SendEmailActionTest()
    //{
    //    Assert.True(true);
    //}

    [Fact]
    public async Task FillTemplateActionTest()
    {
        Job job = new() { ActionType = "FillTemplateAction, UkrGuru.WebJobs" };
        job.ActionMore = @"{ ""tname_pattern"": ""[A-Z]{1,}[_]{1,}[A-Z]{1,}[_]{0,}[A-Z]{0,}"" }";
        job.JobMore = @"{ ""template_subject"": ""Hello DEAR_NAME!"", ""tvalue_DEAR_NAME"": ""Alex"" }";

        var subject = null as string;

        var action = job.CreateAction();

        if (action != null)
        {
            await action.ExecuteAsync();

            subject = ((More)action.More).GetValue("next_subject");
        }

        Assert.Equal("Hello Alex!", subject);
    }

    [Fact]
    public async Task DownloadPageActionTest()
    {
        Job job = new() { ActionType = "DownloadPageAction, UkrGuru.WebJobs" };
        job.JobMore = @"{ ""url"": ""https://ukrguru.com/"", ""result_name"": ""next_body"" }";

        bool result = false; 

        var action = job.CreateAction();

        if (action != null)
        {
            result = await action.ExecuteAsync();
        }

        Assert.True(result);

        var guid = null as string;

        if (action != null)
        {
            guid = ((More)action.More).GetValue("next_body");
        }

        var file = DbHelper.FromProc<WJbFile>("WJbFiles_Get", guid);

        if (file?.FileContent != null)
        {
            await file.DecompressAsync();

            var body = Text.Encoding.UTF8.GetString(file.FileContent);

            Assert.Contains("Oleksandr Viktor (UkrGuru)", body);
        }
    }

    [Fact]
    public async Task ParseTextActionTest()
    {
        Job job = new() { ActionType = "ParseTextAction, UkrGuru.WebJobs" };
        job.JobMore = JsonSerializer.Serialize(new { text = ParseTextExtensionsTests.Text, goals = JsonSerializer.Serialize(ParseTextExtensionsTests.Goals)});
    
        var result = null as string;

        var action = job.CreateAction();

        if (action != null)
        {
            await action.ExecuteAsync();

            result = ((More)action.More).GetValue("result");
        }

        Assert.Equal(ParseTextExtensionsTests.Result, result);
    }
}
