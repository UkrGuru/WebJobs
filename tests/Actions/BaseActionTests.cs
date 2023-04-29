using UkrGuru.WebJobs.Actions;
using UkrGuru.WebJobs.Data;
using Xunit;

namespace WebJobsTests.Actions;

public class BaseActionTests
{
    public BaseActionTests()
    {
    }

    [Theory]
    [InlineData(null, null, null, null)]

    [InlineData(null, null, """{"test":"jjj"}""", "jjj")]
    [InlineData(null, """{"test":"rrr"}""", """{"test":"jjj"}""", "jjj")]
    [InlineData("""{"test":"aaa"}""", null, """{"test":"jjj"}""", "jjj")]
    [InlineData("""{"test":"aaa"}""", """{"test":"rrr"}""", """{"test":"jjj"}""", "jjj")]

    [InlineData(null, """{"test":"rrr"}""", null, "rrr")]
    [InlineData("""{"test":"aaa"}""", """{"test":"rrr"}""", null, "rrr")]

    [InlineData("""{"test":"aaa"}""", null, null, "aaa")]
    public void InitTest(string? actionMore, string? ruleMore, string jobMore, string? expected)
    {
        Job job = new() { ActionMore = actionMore, RuleMore = ruleMore, JobMore = jobMore };

        var action = new BaseAction();

        action.Init(job);

        Assert.Equal(expected, action.More.GetValue("test"));
    }
}