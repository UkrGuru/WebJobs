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
/// 
/// </summary>
public static class UkrGuruWebJobsExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="connectionString"></param>
    /// <param name="logLevel"></param>
    /// <param name="nThreads"></param>
    public static void AddWebJobs(this IServiceCollection services, string? connectionString = null, DbLogLevel logLevel = DbLogLevel.Information, int nThreads = 4)
    {
        services.AddSqlJson(connectionString);

        services.AddSqlJsonExt(logLevel);

        Assembly.GetExecutingAssembly().InitDb();

        try { DbHelper.Exec($"WJbQueue_FinishAll"); } catch { }

        if (nThreads > 0)
        {
            services.AddHostedService<Scheduler>();

            for (int i = 0; i < nThreads; i++)
                services.AddSingleton<IHostedService, Worker>();
        }
    }
}
