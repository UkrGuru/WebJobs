using UkrGuru.SqlJson;
using static UkrGuru.WebJobs.GlobalTests;

namespace WebJobsTests.Actions;

public class SendHttpRequestActionTests
{
    public SendHttpRequestActionTests()
    {
        DbHelper.ConnectionString = ConnectionString;
    }

    [Fact]
    public void FirstTest()
    {
        Assert.True(true);
    }
}
