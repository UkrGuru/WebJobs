// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Reflection;
using UkrGuru.Extensions;
using UkrGuru.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;

public static class WebJobsServiceCollectionExtensions
{
    public static void AddWebJobsDemo(this IServiceCollection services, string connectionString, DbLogLevel logLevel = DbLogLevel.Debug, int nThreads = 4)
    {
        services.AddWebJobs(connectionString, logLevel, nThreads);

        Assembly.GetExecutingAssembly().InitDb();
    }
}