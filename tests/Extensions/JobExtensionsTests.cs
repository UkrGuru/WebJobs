using System;
using System.Collections.Generic;
using System.Linq;
using UkrGuru.WebJobs.Data;

namespace WebJobsTests.Extensions;

public class JobExtensionsTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("UnknownAction")]
    [InlineData("UkrGuru.WebJobs.Actions.UnknownAction")]
    [InlineData("UkrGuru.WebJobs.Actions.UnknownAction, UkrGuru.WebJobs")]
    [InlineData("BaseAction, UkrGuru.WebJobs")]
    [InlineData("DownloadPageAction, UkrGuru.WebJobs")]
    [InlineData("FillTemplateAction, UkrGuru.WebJobs")]
    [InlineData("ParseTextAction, UkrGuru.WebJobs")]
    [InlineData("ProcItemsAction, UkrGuru.WebJobs")]
    [InlineData("RunApiProcAction, UkrGuru.WebJobs")]
    [InlineData("RunSqlProcAction, UkrGuru.WebJobs")]
    [InlineData("SendEmailAction, UkrGuru.WebJobs")]
    [InlineData("SendHttpRequestAction, UkrGuru.WebJobs")]
    [InlineData("SsrsExportReportAction, UkrGuru.WebJobs")]
    public void CanCreateAction(string? actionType)
    {
        var job = new Job() { ActionType = actionType };

        if (actionType == null || actionType.Contains("UnknownAction"))
        {
            Assert.Throws<ArgumentNullException>(() => job.CreateAction());
        }
        else
        {
            var action = job.CreateAction();

            Assert.NotNull(action);
            Assert.Equal(0, action.JobId);
            Assert.NotNull(action.More);
        }
    }

    public static IEnumerable<object[]> GetTestJobs(int numTests)
    {
        var allData = new List<object[]>
        {
            new object[] { new Job() { JobId = 1, ActionType = "BaseAction, UkrGuru.WebJobs",
                ActionMore = null, RuleMore = null, JobMore = """{"test":"jjj"}""" }, "jjj" },
            new object[] { new Job() { JobId = 1, ActionType = "BaseAction, UkrGuru.WebJobs",
                ActionMore = null, RuleMore = """{"test":"rrr"}""", JobMore = """{"test":"jjj"}""" }, "jjj" },
            new object[] { new Job() { JobId = 1, ActionType = "BaseAction, UkrGuru.WebJobs",
                ActionMore = """{"test":"aaa"}""", RuleMore = null, JobMore = """{"test":"jjj"}""" }, "jjj" },
            new object[] { new Job() { JobId = 1, ActionType = "BaseAction, UkrGuru.WebJobs",
                ActionMore = """{"test":"aaa"}""", RuleMore = """{"test":"rrr"}""", JobMore = """{"test":"jjj"}""" }, "jjj" },

            new object[] { new Job() { JobId = 1, ActionType = "BaseAction, UkrGuru.WebJobs",
                ActionMore = null, RuleMore = """{"test":"rrr"}""", JobMore = null }, "rrr" },
            new object[] { new Job() { JobId = 1, ActionType = "BaseAction, UkrGuru.WebJobs",
                ActionMore = """{"test":"aaa"}""", RuleMore = """{"test":"rrr"}""", JobMore = null }, "rrr" },

            new object[] { new Job() { JobId = 1, ActionType = "BaseAction, UkrGuru.WebJobs",
                ActionMore = """{"test":"aaa"}""", RuleMore = null, JobMore = null }, "aaa" }
        };

        return allData.Take(numTests);
    }

    [Theory]
    [MemberData(nameof(GetTestJobs), parameters: 7)]
    public void CanCreateActionInit(Job job, string? expected)
    {
        var action = job.CreateAction();

        Assert.NotNull(action);
        Assert.Equal(job.JobId, action.JobId);
        Assert.Equal(expected, action.More.GetValue("test"));
    }
}