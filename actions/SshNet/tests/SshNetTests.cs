using System.Reflection;
using UkrGuru.SqlJson;
using Xunit;
using System.Text;
using UkrGuru.WebJobs.Data;
using UkrGuru.Extensions;
using UkrGuru.Extensions.Data;

namespace SshNetTests;

public class SshNetTests
{
    private readonly bool dbOK = false;

    public SshNetTests()
    {
        var dbName = "SshNetTest";

        var connectionString = $"Server=(localdb)\\mssqllocaldb;Database={dbName};Trusted_Connection=True";

        DbHelper.ConnectionString = connectionString.Replace(dbName, "master");

        DbHelper.Exec($"IF DB_ID('{dbName}') IS NULL CREATE DATABASE {dbName};");

        DbHelper.ConnectionString = connectionString;

        if (dbOK) return;

        var assembly1 = Assembly.GetAssembly(typeof(UkrGuru.WebJobs.Actions.BaseAction));
        ArgumentNullException.ThrowIfNull(assembly1);
        dbOK = assembly1.InitDb();

        var assembly2 = Assembly.GetAssembly(typeof(UkrGuru.WebJobs.Actions.SshNet.GetFilesAction));
        ArgumentNullException.ThrowIfNull(assembly2);
        dbOK &= assembly2.InitDb();

        var assembly3 = Assembly.GetAssembly(typeof(SshNetTests));
        ArgumentNullException.ThrowIfNull(assembly3);
        dbOK &= assembly3.InitDb();
    }

    [Fact]
    public void InitDbTest()
    {
        Assert.True(dbOK);
    }

    [Fact]
    public async Task SshNetTest()
    {
        var dbFile = new DbFile() { FileName = "1.txt", FileContent = Encoding.UTF8.GetBytes(new String('1', 4096)) };

        var guidFile = await dbFile.SetAsync();

        Assert.NotNull(guidFile);

        var jobId = await DbHelper.ExecAsync<int>("WJbQueue_Ins", new
        {
            Rule = 21,
            RulePriority = (byte)Priorities.ASAP,
            RuleMore = new { files = new[] { guidFile } }
        });

        TestJob(jobId);

        TestRule(20);

        //Assert.True(File.Exists(filename));

        //var text = File.ReadAllText(filename);

        //Assert.Equal(text, new String('1', 4096));
    }

    static void TestRule(int ruleId)
    {
        var jobId = DbHelper.Exec<int>("WJbRules_Test", ruleId);

        TestJob(jobId);
    }

    static void TestJob(int jobId)
    {
        var job = DbHelper.Exec<JobQueue>("WJbQueue_Start", jobId.ToString());

        if (job?.JobId > 0)
        {
            bool result = false;
            try
            {
                var action = job.CreateAction();

                if (action != null)
                {
                    result = action.ExecuteAsync().Result;

                    // action.NextAsync(result).Wait();
                }
            }
            catch
            {
                result = false;
                throw;
            }
            finally
            {
                DbHelper.ExecAsync("WJbQueue_Finish", new { JobId = jobId, JobStatus = result ? JobStatus.Completed : JobStatus.Failed }).Wait();
            }
        }
    }
}