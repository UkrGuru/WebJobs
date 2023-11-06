using System.Threading.Tasks;
using UkrGuru.SqlJson.Extensions;
using UkrGuru.SqlJson;
using static UkrGuru.WebJobs.GlobalTests;

using UkrGuru.WebJobs.Data;
using UkrGuru.WebJobs.Actions;

namespace WebJobsTests.Actions;

public class DownloadPageActionTests
{
    public DownloadPageActionTests()
    {
        DbHelper.ConnectionString = ConnectionString;
    }

    [Fact]
    public async Task CanDownloadPage()
    {
        var job = new Job() { ActionType = nameof(DownloadPageAction) };
        job.JobMore = """{"url": "https://www.ukrguru.com/", "result_name": "next_body"}""";

        var action = job.CreateAction();

        Assert.True(await action.ExecuteAsync());

        //var guid = ((More)action.More).GetValue("next_body");

        //var file = await DbHelper.ReadAsync<DbFile?>("WJbFiles_Get", guid);

        //if (file?.FileContent != null)
        //{
        //    await file.DecompressAsync();

        //    var body = System.Text.Encoding.UTF8.GetString(file.FileContent);

        //    Assert.Contains("Oleksandr Viktor (UkrGuru)", body);
        //}
    }
}