using System.Reflection;
using UkrGuru.SqlJson;
using Data = UkrGuru.WebJobs.Data;
using UkrGuru.WebJobs.Data;
using Xunit;
using UkrGuru.Extensions;

namespace ClosedXMLTests
{
    public class ImportFileTests
    {
        private readonly bool dbOK = false;

        public ImportFileTests()
        {
            var dbName = "ClosedXMLTest";

            var connectionString = $"Server=(localdb)\\mssqllocaldb;Database={dbName};Trusted_Connection=True";

            DbHelper.ConnectionString = connectionString.Replace(dbName, "master");

            DbHelper.ExecCommand($"IF DB_ID('{dbName}') IS NULL CREATE DATABASE {dbName};");

            DbHelper.ConnectionString = connectionString;

            if (dbOK) return;

            var assembly1 = Assembly.GetAssembly(typeof(UkrGuru.WebJobs.Actions.BaseAction));
            ArgumentNullException.ThrowIfNull(assembly1);
            dbOK = assembly1.InitDb();

            var assembly2 = Assembly.GetAssembly(typeof(UkrGuru.WebJobs.Actions.ClosedXML.ImportFileAction));
            ArgumentNullException.ThrowIfNull(assembly2);
            dbOK &= assembly2.InitDb();

            var assembly3 = Assembly.GetAssembly(typeof(ImportFileTests));
            ArgumentNullException.ThrowIfNull(assembly3);
            dbOK &= assembly3.InitDb();
        }

        [Fact]
        public void InitDbTest()
        {
            Assert.True(dbOK);
        }

        [Fact]
        public async Task ImportFileTestAsync()
        {
            var bytes = await System.IO.File.ReadAllBytesAsync("data.xlsx");

            var wjbFile = new WJbFile() { FileName = "customers.csv", FileContent = bytes };

            var guidFile = await wjbFile.SetAsync();

            Assert.NotNull(guidFile);

            var jobId = await DbHelper.FromProcAsync<int>("WJbQueue_Ins", new
            {
                Rule = 50,  /* Import Xlsx File */
                RulePriority = (byte)Priorities.ASAP,
                RuleMore = new { file = guidFile }
            });

            TestJob(jobId);

            Assert.True(true);
        }

        static void TestRule(int ruleId)
        {
            var jobId = DbHelper.FromProc<int>("WJbRules_Test", ruleId);

            TestJob(jobId);
        }

        static void TestJob(int jobId)
        {
            var job = DbHelper.FromProc<JobQueue>("WJbQueue_Start", jobId.ToString());

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
                    DbHelper.ExecProc("WJbQueue_Finish", new { JobId = jobId, JobStatus = result ? JobStatus.Completed : JobStatus.Failed });
                }
            }
        }
    }
}