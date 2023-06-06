// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UkrGuru.Extensions.Logging;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace UkrGuru.WebJobs;

/// <summary>
/// The Worker class is a BackgroundService that executes jobs from a job queue.
/// </summary>
public class Worker : BackgroundService
{
    private const int NO_DELAY = 0;
    private const int MIN_DELAY = 100;
    private const int ADD_DELAY = 1000;
    private const int MAX_DELAY = 20000;

    private int _delay = MIN_DELAY;

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
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var job = await DbHelper.ExecAsync<JobQueue>("WJbQueue_Start1st", cancellationToken: stoppingToken);
                if (job?.JobId > 0)
                {
                    var jobId = job.JobId; bool exec_result = false, next_result = false;
                    try
                    {
                        var action = job.CreateAction();

                        if (action != null)
                        {
                            exec_result = await action.ExecuteAsync(stoppingToken);

                            next_result = await action.NextAsync(exec_result, stoppingToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        exec_result = false;

                        _logger.LogError(ex, $"Job #{jobId} crashed.", nameof(ExecuteAsync));
                        await DbLogHelper.LogErrorAsync($"Job #{jobId} crashed.", new { jobId, errMsg = ex.Message }, stoppingToken);
                    }
                    finally
                    {
                        _ = await DbHelper.ExecAsync("WJbQueue_Finish", new { JobId = jobId, 
                                JobStatus = exec_result ? JobStatus.Completed : JobStatus.Failed }, 
                                cancellationToken: stoppingToken);
                    }

                    _delay = next_result ? NO_DELAY : MIN_DELAY;
                }
                else
                {
                    if (_delay < MAX_DELAY) _delay += ADD_DELAY;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Worker.ExecuteAsync Error", nameof(ExecuteAsync));
            }

            if (_delay > 0) 
                await Task.Delay(_delay, stoppingToken);
            else
                _delay = MIN_DELAY;
        }
    }
}
