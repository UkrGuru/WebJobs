using UkrGuru.WebJobs.Data;
using Xunit;

namespace WebJobsTests;

public class JobExtensionsTests
{
    [Fact]
    public void CreateActionTest()
    {
        // Arrange
        var job = new Job();

        // Act
        var result = job.CreateAction();

        // Assert
        Assert.NotNull(result);
    }
}