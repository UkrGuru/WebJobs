// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Hosting;
using System.Reflection;
using UkrGuru.Extensions;
using UkrGuru.Extensions.Logging;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for adding WebJobs services to an IServiceCollection.
/// </summary>
public static class WJbServiceCollectionExtensions
{
    /// <summary>
    /// Adds WebJobs services to the specified IServiceCollection using the specified connection string, log level, number of threads, and database initialization option.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="connectionString">The connection string to use for the database.</param>
    /// <param name="logLevel">The log level to use for logging.</param>
    /// <param name="nThreads">The number of worker threads to use.</param>
    /// <param name="initDb">A value indicating whether to initialize the database.</param>
    public static void AddWebJobs(this IServiceCollection services, string? connectionString = null, DbLogLevel logLevel = DbLogLevel.Information, int nThreads = 4, bool initDb = true)
    {
        services.AddSqlJson(connectionString);

        services.AddSqlJsonExt(logLevel, initDb);

        services.AddWebJobs(nThreads, initDb);
    }

    /// <summary>
    /// Adds WebJobs services to the specified IServiceCollection using the specified number of threads and database initialization option.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="nThreads">The number of worker threads to use.</param>
    /// <param name="initDb">A value indicating whether to initialize the database.</param>
    public static void AddWebJobs(this IServiceCollection services, int nThreads = 4, bool initDb = true)
    {
        if (initDb) Assembly.GetExecutingAssembly().InitDb();

        try { DbHelper.Exec("WJbQueue_FinishAll"); } catch { }

        if (nThreads > 0)
        {
            services.AddHostedService<Scheduler>();

            for (int i = 0; i < nThreads; i++)
                services.AddSingleton<IHostedService, Worker>();
        }
    }
}