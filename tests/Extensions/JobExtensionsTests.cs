using UkrGuru.WebJobs.Data;

namespace WebJobsTests;

public class JobExtensionsTests
{

    [Theory]
    [InlineData("RunSqlProcAction, UkrGuru.WebJobs")]
    [InlineData("SendEmailAction, UkrGuru.WebJobs")]
    [InlineData("FillTemplateAction, UkrGuru.WebJobs")]
    [InlineData("DownloadPageAction, UkrGuru.WebJobs")]
    [InlineData("RunApiProcAction, UkrGuru.WebJobs")]
    [InlineData("ProcItemsAction, UkrGuru.WebJobs")]
    [InlineData("ParseTextAction, UkrGuru.WebJobs")]
    [InlineData("SsrsExportReportAction, UkrGuru.WebJobs")]
    public void CreateActionTest(string? actionType)
    {
        // Arrange
        var job = new Job() { ActionType = actionType };

        // Act
        var result = job.CreateAction();

        // Assert
        Assert.NotNull(result);
    }
}