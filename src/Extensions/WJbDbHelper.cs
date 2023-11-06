using UkrGuru.SqlJson.Extensions;
using UkrGuru.WebJobs.Data;

namespace UkrGuru.SqlJson;

/// <summary>
/// Provides helper methods for working with the WJb database.
/// </summary>
public class WJbDbHelper : DbHelper
{
    /// <summary>
    /// Delays the execution of the current method for a specified time.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the delay.</param>
    /// <returns>A task that represents the asynchronous delay operation.</returns>
    public static async Task DelayAsync(CancellationToken cancellationToken = default)
        => await ExecAsync("DECLARE @Delay varchar(10) = '00:00:' + FORMAT(60 - DATEPART(SECOND, GETDATE()), '00'); WAITFOR DELAY @Delay;",
            timeout: 100, cancellationToken: cancellationToken);

    /// <summary>
    /// Finishes the job with the specified ID and updates its status.
    /// </summary>
    /// <param name="jobId">The ID of the job to finish.</param>
    /// <param name="exec_result">The result of the job execution.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task WJbQueue_FinishAsync(int? jobId, bool exec_result, CancellationToken cancellationToken = default)
        => await UpdateAsync("WJbQueue_Finish", new { JobId = jobId, JobStatus = exec_result ? JobStatus.Completed : JobStatus.Failed }, cancellationToken: cancellationToken);

    /// <summary>
    /// Retrieves the job with the specified ID.
    /// </summary>
    /// <param name="jobId">The ID of the job to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the job with the specified ID, or null if the job was not found.</returns>
    public static async Task<Job?> WJbQueue_GetAsync(int? jobId, CancellationToken cancellationToken = default)
        => await CreateAsync<Job?>("WJbQueue_Get", jobId, cancellationToken: cancellationToken);

    /// <summary>
    /// Inserts a new job into the job queue with the specified rule, more information, and priority.
    /// </summary>
    /// <param name="rule">The rule for the new job.</param>
    /// <param name="more">Additional information for the new job.</param>
    /// <param name="priority">The priority of the new job. The default value is Normal.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the ID of the new job, or null if the job could not be created.</returns>
    public static async Task<int?> WJbQueue_InsAsync(string? rule, More? more, Priority priority = Priority.Normal, CancellationToken cancellationToken = default)
        => await CreateAsync<int?>("WJbQueue_Ins", new { Rule = rule, RulePriority = priority, RuleMore = more }, cancellationToken: cancellationToken);

    /// <summary>
    /// Inserts a new job into the job queue by specified cron expression.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task WJbQueue_InsCronAsync(CancellationToken cancellationToken = default)
        => await DbHelper.ExecAsync("WJbQueue_InsCron", cancellationToken: cancellationToken);

    /// <summary>
    /// Retrieves the first job in the job queue and starts its execution.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the first job in the queue, or null if the queue is empty.</returns>
    public static async Task<JobQueue?> WJbQueue_Start1stAsync(CancellationToken cancellationToken = default)
        => await ReadAsync<JobQueue?>("WJbQueue_Start1st", cancellationToken: cancellationToken);

    /// <summary>
    /// Retrieves the value of the setting with the specified name.
    /// </summary>
    /// <typeparam name="T">The type of the setting value.</typeparam>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the value of the setting with the specified name, or null if the setting was not found.</returns>
    public static async Task<T?> WJbSettings_GetAsync<T>(string? name, CancellationToken cancellationToken = default)
        => await ReadAsync<T?>("WJbSettings_Get", name, cancellationToken: cancellationToken);
}