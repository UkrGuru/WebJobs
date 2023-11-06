// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UkrGuru.SqlJson;
using UkrGuru.SqlJson.Extensions;
using UkrGuru.WebJobs.Data;

namespace UkrGuru.WebJobs;

/// <summary>
/// The Worker class is a BackgroundService that executes jobs from a job queue.
/// </summary>
public class Worker : BackgroundService
{
    private const int NO_DELAY = 0;
    private const int MIN_DELAY = 100;
    private const int NEW_DELAY = 1000;
    private const int MAX_DELAY = 20000;

    /// <summary>
    /// The logger instance used to log messages and errors.
    /// </summary>
    private readonly ILogger<Worker> _logger;

    /// <summary>
    /// Initializes a new instance of the Worker class with the specified logger.
    /// </summary>
    /// <param name="logger">The logger instance to use.</param>
    public Worker(ILogger<Worker> logger) => _logger = logger;

    /// <summary>
    /// Executes the background service, processing jobs from the job queue until stopped.
    /// </summary>
    /// <param name="stoppingToken">The cancellation token used to stop the service.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        int delay = MIN_DELAY;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var job = await WJbDbHelper.WJbQueue_Start1stAsync(stoppingToken);
                if (job?.JobId > 0)
                {
                    bool exec_result = false, next_result = false;
                    try
                    {
                        var action = job.CreateAction();

                        exec_result = await action.ExecuteAsync(stoppingToken);

                        next_result = await action.NextAsync(exec_result, stoppingToken);

                        delay = next_result ? NO_DELAY : MIN_DELAY;
                    }
                    catch (Exception ex)
                    {
                        exec_result = false;

                        delay += NEW_DELAY;

                        _logger.LogError(ex, $"Job #{job.JobId} crashed.", nameof(ExecuteAsync));

                        await DbLogHelper.LogErrorAsync(job.ActionType ?? nameof(Worker), 
                            new { jobId = job.JobId, errMsg = ex.Message }, stoppingToken);
                    }
                    finally
                    {
                        await WJbDbHelper.WJbQueue_FinishAsync(job.JobId, exec_result, stoppingToken);
                    }
                }
                else
                {
                    if (delay < MAX_DELAY) delay += NEW_DELAY;
                }
            }
            catch (Exception ex)
            {
                delay += NEW_DELAY;

                _logger.LogError(ex, "Worker.ExecuteAsync Error", nameof(ExecuteAsync));
            }

            if (delay > 0) 
                await Task.Delay(delay, stoppingToken);
            else
                delay = MIN_DELAY;
        }
    }
}
