// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UkrGuru.SqlJson;
using UkrGuru.SqlJson.Extensions;

namespace UkrGuru.WebJobs;

/// <summary>
/// The Scheduler class is a BackgroundService that schedules and executes Cron jobs.
/// </summary>
public class Scheduler : BackgroundService
{
    /// <summary>
    /// The logger instance used to log messages and errors.
    /// </summary>
    private readonly ILogger<Scheduler> _logger;

    /// <summary>
    /// Initializes a new instance of the Scheduler class with the specified logger.
    /// </summary>
    /// <param name="logger">The logger instance to use.</param>
    public Scheduler(ILogger<Scheduler> logger) => _logger = logger;

    /// <summary>
    /// Executes the background service, scheduling and executing Cron jobs until stopped.
    /// </summary>
    /// <param name="stoppingToken">The cancellation token used to stop the service.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken = default)
    {
        await WaitForNewMinute(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Run(async () => await CreateCronJobs(stoppingToken), cancellationToken: stoppingToken);

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    /// <summary>
    /// Creates Cron jobs by calling the WJbQueue_InsCron stored procedure.
    /// </summary>
    /// <param name="stoppingToken">The cancellation token used to stop the operation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    protected virtual async Task CreateCronJobs(CancellationToken stoppingToken = default)
    {
        try
        {
            await WJbDbHelper.WJbQueue_InsCronAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateCronJobs Error", nameof(CreateCronJobs));

            await DbLogHelper.LogErrorAsync(nameof(CreateCronJobs), new { errMsg = ex.Message }, stoppingToken);
        }
    }

    /// <summary>
    /// Waits for the start of a new minute by calling a SQL WAITFOR statement.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private async Task WaitForNewMinute(CancellationToken stoppingToken = default)
    {
        try
        {
            await WJbDbHelper.DelayAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WaitForMinuteEnd Error", nameof(WaitForNewMinute));
        }
    }
}