using UkrGuru.SqlJson.Extensions;
using UkrGuru.WebJobs.Data;

namespace WebJobsTests.Extensions;

public class MoreExtensionsTests
{
    [Fact]
    public void CanCreateNextMore()
    {
        More more = new()
        {
            {"next_1", 1},
            {"next_2", 2},
            {"other", 3}
        };

        var next_more = more.CreateNextMore("next_");
        Assert.Equal(2, next_more.Count);
        Assert.Equal(1, next_more["1"]);
        Assert.Equal(2, next_more["2"]);
    }
}
