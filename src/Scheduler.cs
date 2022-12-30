// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UkrGuru.Extensions;
using UkrGuru.Extensions.Logging;
using UkrGuru.SqlJson;

namespace UkrGuru.WebJobs;
/// <summary>
/// 
/// </summary>
public class Scheduler : BackgroundService
{
    private readonly ILogger<Scheduler> _logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    public Scheduler(ILogger<Scheduler> logger) => _logger = logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await DbHelper.ExecAsync("DECLARE @Delay varchar(10) = '00:00:' + FORMAT(60 - DATEPART(SECOND, GETDATE()), '00'); WAITFOR DELAY @Delay;", timeout: 100);

        while (!stoppingToken.IsCancellationRequested)
        {
            _ = Task.Run(async () => await CreateCronJobs(stoppingToken));

            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected virtual async Task CreateCronJobs(CancellationToken stoppingToken)
    {
        try
        {
            await DbHelper.ExecAsync("WJbQueue_InsCron", cancellationToken: stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateCronJobs Error", nameof(CreateCronJobs));
            await DbLogHelper.LogErrorAsync("CreateCronJobs Error", new { errMsg = ex.Message }, stoppingToken);
        }
    }
}
