// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

global using Xunit;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Reflection;
using UkrGuru.SqlJson;
using UkrGuru.SqlJson.Extensions;

namespace UkrGuru.WebJobs;

public class GlobalTests
{
    public const string DbName = "WebJobsTest5";

    private static string? _connectionString;

    public static string ConnectionString
    {
        get
        {
            _connectionString ??= $"Server=(localdb)\\mssqllocaldb;Database={DbName};Trusted_Connection=True";

            return _connectionString;
        }
    }

    private static IConfiguration? _configuration;

    public static IConfiguration Configuration
    {
        get
        {
            if (_configuration == null)
            {
                var inMemorySettings = new Dictionary<string, string?>() {
                    { "ConnectionStrings:DefaultConnection", ConnectionString},
                    { "Logging:LogLevel:UkrGuru.SqlJson", "Information" },
                    { "AppSettings:WJbInitDb", "true" },
                    { "AppSettings:WJbNThreads", "0" }
                };

                _configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(inMemorySettings)
                    .Build();
            }

            return _configuration;
        }
    }

    public GlobalTests()
    {
    }

    [Fact]
    public void CanInitDbs()
    {
        DbHelper.ConnectionString = ConnectionString;

        Assembly.GetAssembly(typeof(DbHelper)).InitDb();

        Assembly.GetAssembly(typeof(Worker)).InitDb();

        //Assembly.GetExecutingAssembly().InitDb();
    }
}