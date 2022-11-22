// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Hosting;
using System.Reflection;
using UkrGuru.Extensions;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs;
using LogLevel = UkrGuru.Extensions.WJbLog.Level;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddWebJobs(this IServiceCollection services, string connectionString, LogLevel logLevel = LogLevel.Debug, int nThreads = 4)
    {
        services.AddUkrGuruSqlJsonExt(connectionString, logLevel);

        Assembly.GetExecutingAssembly().InitDb();

        try { DbHelper.ExecProc($"WJbQueue_FinishAll"); } catch { }

        if (nThreads > 0)
        {
            services.AddHostedService<Scheduler>();

            for (int i = 0; i < nThreads; i++)
                services.AddSingleton<IHostedService, Worker>();
        }
    }
}
