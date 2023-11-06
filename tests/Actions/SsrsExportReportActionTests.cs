using UkrGuru.SqlJson;
using static UkrGuru.WebJobs.GlobalTests;

namespace WebJobsTests.Actions;

public class SsrsExportReportActionTests
{
    public SsrsExportReportActionTests()
    {
        DbHelper.ConnectionString = ConnectionString;
    }

    [Fact]
    public void FirstTest()
    {
        Assert.True(true);
    }
}
