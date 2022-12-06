// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UkrGuru.Extensions;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace UkrGuru.WebJobs;

/// <summary>
/// 
/// </summary>
public class Worker : BackgroundService
{
    private const int NO_DELAY = 0;
    private const int MIN_DELAY = 100;
    private const int ADD_DELAY = 1000;
    private const int MAX_DELAY = MIN_DELAY * 16;

    private int _delay = MIN_DELAY;

    private readonly ILogger<Worker> _logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    public Worker(ILogger<Worker> logger) => _logger = logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var job = await DbHelper.FromProcAsync<JobQueue>("WJbQueue_Start1st", cancellationToken: stoppingToken);
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
                        await WJbLogHelper.LogErrorAsync($"Job #{jobId} crashed.", new { jobId, errMsg = ex.Message }, stoppingToken);
                    }
                    finally
                    {
                        _ = await DbHelper.ExecProcAsync("WJbQueue_Finish", new { JobId = jobId, 
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
                await WJbLogHelper.LogErrorAsync("Worker.ExecuteAsync Error", new { errMsg = ex.Message }, stoppingToken);
            }

            if (_delay > 0) 
                await Task.Delay(_delay, stoppingToken);
            else
                _delay = MIN_DELAY;
        }
    }
}
