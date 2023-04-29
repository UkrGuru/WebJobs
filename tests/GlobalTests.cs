// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

global using Xunit;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Reflection;
using UkrGuru.Extensions;
using UkrGuru.SqlJson;

namespace UkrGuru.WebJobs;

public class GlobalTests
{
    public const string DbName = "WebJobsTest5";

    public const string ConnectionString = $"Server=(localdb)\\mssqllocaldb;Database={DbName};Trusted_Connection=True";

    public static bool DbOk { get; set; }

    public static IConfiguration? Configuration { get; set; }

    public GlobalTests()
    {
        if (DbOk) return;

        DbHelper.ConnectionString = ConnectionString.Replace(DbName, "master");

        DbHelper.Exec($"IF DB_ID('{DbName}') IS NULL CREATE DATABASE {DbName};");

        DbHelper.ConnectionString = ConnectionString;

        Assembly.GetAssembly(typeof(UkrGuru.SqlJson.DbHelper)).InitDb();

        Assembly.GetAssembly(typeof(UkrGuru.WebJobs.Worker)).InitDb();

        //Assembly.GetExecutingAssembly().InitDb();

        //var inMemorySettings = new Dictionary<string, string?>() {
        //    { "ConnectionStrings:DefaultConnection", ConnectionString },
        //    { "Logging:LogLevel:UkrGuru.SqlJson", "Debug" },
        //    { "WJbSettings:InitDb", "true" },
        //    { "WJbSettings:NThreads", "0" }
        //};

        //Configuration = new ConfigurationBuilder()
        //    .AddInMemoryCollection(inMemorySettings)
        //    .Build();

        DbOk = true;
    }

    [Fact]
    public void CanInitDbs() { }
}