using Xunit;
using UkrGuru.WebJobs.Actions;

namespace WebJobsTests.Functions;

public class ParseTextTests
{
    [Theory]
    [InlineData(null, null, null, null)]
    [InlineData(null, null, "", null)]
    [InlineData(null, null, "0", null)]

    [InlineData(null, "", null, null)]
    [InlineData(null, "", "", null)]
    [InlineData(null, "", "0", null)]

    [InlineData(null, "0", null, null)]
    [InlineData(null, "0", "", null)]
    [InlineData(null, "0", "0", null)]

    [InlineData("", null, null, "")]
    [InlineData("", null, "", "")]
    [InlineData("", null, "0", null)]

    [InlineData("", "", null, "")]
    [InlineData("", "", "", "")]
    [InlineData("", "", "0", null)]

    [InlineData("", "0", null, null)]
    [InlineData("", "0", "", null)]
    [InlineData("", "0", "0", null)]

    [InlineData("0", null, null, "0")]
    [InlineData("0", null, "", "0")]
    [InlineData("0", null, "0", "")]

    [InlineData("0", "", null, "0")]
    [InlineData("0", "", "", "0")]
    [InlineData("0", "", "0", "")]

    [InlineData("0", "0", null, "")]
    [InlineData("0", "0", "", "")]
    [InlineData("0", "0", "0", null)]

    [InlineData("01", null, null, "01")]
    [InlineData("01", null, "", "01")]
    [InlineData("01", null, "0", "")]
    [InlineData("01", null, "1", "0")]

    [InlineData("01", "", null, "01")]
    [InlineData("01", "", "", "01")]
    [InlineData("01", "", "0", "")]
    [InlineData("01", "", "1", "0")]

    [InlineData("01", "0", null, "1")]
    [InlineData("01", "0", "", "1")]
    [InlineData("01", "0", "0", null)]
    [InlineData("01", "0", "1", "")]

    [InlineData("012", null, null, "012")]
    [InlineData("012", null, "", "012")]
    [InlineData("012", null, "0", "")]
    [InlineData("012", null, "1", "0")]
    [InlineData("012", null, "2", "01")]
    [InlineData("012", null, "3", null)]

    [InlineData("012", "", null, "012")]
    [InlineData("012", "", "", "012")]
    [InlineData("012", "", "0", "")]
    [InlineData("012", "", "1", "0")]
    [InlineData("012", "", "2", "01")]
    [InlineData("012", "", "3", null)]

    [InlineData("012", "0", null, "12")]
    [InlineData("012", "0", "", "12")]
    [InlineData("012", "0", "0", null)]
    [InlineData("012", "0", "1", "")]
    [InlineData("012", "0", "2", "1")]
    [InlineData("012", "0", "3", null)]

    [InlineData("012", "1", null, "2")]
    [InlineData("012", "1", "", "2")]
    [InlineData("012", "1", "0", null)]
    [InlineData("012", "1", "1", null)]
    [InlineData("012", "1", "2", "")]
    [InlineData("012", "2", "1", null)]
    [InlineData("012", "1", "3", null)]

    public void CropTest(string? text, string? start, string? end = default, string? expected = default)
    {
        Assert.Equal(expected, ParseTextAction.Crop(text, start, end));
    }
}