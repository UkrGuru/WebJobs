using UkrGuru.SqlJson;
using static UkrGuru.WebJobs.GlobalTests;

namespace WebJobsTests.Actions;

public class RunApiProcActionTests
{
    public RunApiProcActionTests()
    {
        DbHelper.ConnectionString = ConnectionString;
    }

    [Fact]
    public void FirstTest()
    {
        Assert.True(true);
    }
}
