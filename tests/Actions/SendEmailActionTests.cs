using UkrGuru.SqlJson;
using static UkrGuru.WebJobs.GlobalTests;

namespace WebJobsTests.Actions;

public class SendEmailActionTests
{
    public SendEmailActionTests()
    {
        DbHelper.ConnectionString = ConnectionString;
    }

    [Fact]
    public void FirstTest()
    {
        Assert.True(true);
    }
}
