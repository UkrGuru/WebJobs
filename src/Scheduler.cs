// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UkrGuru.Extensions;
using UkrGuru.SqlJson;

namespace UkrGuru.WebJobs;

public class Scheduler : BackgroundService
{
    private readonly ILogger<Scheduler> _logger;
    public Scheduler(ILogger<Scheduler> logger) => _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await DbHelper.ExecCommandAsync("DECLARE @Delay varchar(10) = '00:00:' + FORMAT(60 - DATEPART(SECOND, GETDATE()), '00'); WAITFOR DELAY @Delay;", timeout: 100);

        while (!stoppingToken.IsCancellationRequested)
        {
            _ = Task.Run(async () => await CreateCronJobs(stoppingToken));

            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }

    protected virtual async Task CreateCronJobs(CancellationToken stoppingToken)
    {
        try
        {
            await DbHelper.ExecProcAsync("WJbQueue_InsCron", cancellationToken: stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateCronJobs Error", nameof(CreateCronJobs));
            await WJbLogHelper.LogErrorAsync("CreateCronJobs Error", new { errMsg = ex.Message }, stoppingToken);
        }
    }
}
