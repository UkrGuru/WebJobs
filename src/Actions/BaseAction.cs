// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using UkrGuru.Extensions;
using UkrGuru.Extensions.Logging;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace UkrGuru.WebJobs.Actions;

/// <summary>
/// Represents the base class for actions that can be performed by a job.
/// </summary>
public class BaseAction
{
    private const string GOOD_RULE = "next";
    private const string FAIL_RULE = "fail";

    private const string GOOD_PREFIX = GOOD_RULE + "_";
    private const string FAIL_PREFIX = FAIL_RULE + "_";

    /// <summary>
    /// Gets or sets the ID of the job associated with this action.
    /// </summary>
    public int JobId { get; set; }

    /// <summary>
    /// Gets or sets additional information about the action.
    /// </summary>
    public More More { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseAction"/> class.
    /// </summary>
    public BaseAction() => More = new More();

    /// <summary>
    /// Initializes the action with the specified job.
    /// </summary>
    /// <param name="job">The job associated with this action.</param>
    public virtual void Init(Job job)
    {
        JobId = job.JobId;

        More.AddNew(job.JobMore);
        More.AddNew(job.RuleMore);
        More.AddNew(job.ActionMore);
    }

    /// <summary>
    /// Executes the action asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a value indicating whether the action was executed successfully.</returns>
    public virtual async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        await Task.Delay(100, cancellationToken);

        return true;
    }

    /// <summary>
    /// Determines the next action to execute based on the result of this action and schedules it for execution asynchronously.
    /// </summary>
    /// <param name="exec_result">A value indicating whether this action was executed successfully.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a value indicating whether the next action was scheduled successfully.</returns>
    public virtual async Task<bool> NextAsync(bool exec_result, CancellationToken cancellationToken = default)
    {
        var next_prefix = exec_result ? GOOD_PREFIX : FAIL_PREFIX;

        var next_rule = More.GetValue(exec_result ? GOOD_RULE : FAIL_RULE);
        if (string.IsNullOrEmpty(next_rule)) return false;

        var next_more = new More();

        foreach (var more in More.Where(item => item.Key.StartsWith(next_prefix)))
            next_more.Add(more.Key[next_prefix.Length..], more.Value);

        await DbLogHelper.LogDebugAsync("NextAsync", new { jobId = JobId, next_rule, next_more }, cancellationToken);

        var next_jobId = await DbHelper.ExecAsync<int?>("WJbQueue_Ins",
            new { Rule = next_rule, RulePriority = (byte)Priorities.ASAP, RuleMore = next_more },
            cancellationToken: cancellationToken);

        await DbLogHelper.LogInformationAsync("NextAsync", new { jobId = JobId, result = "OK", next_jobId }, cancellationToken);

        return true;
    }

    /// <summary>
    /// Truncates a string to a specified maximum length and adds an ellipsis if necessary.
    /// </summary>
    /// <param name="text">The text to truncate.</param>
    /// <param name="maxLength">The maximum length of the truncated text.</param>
    /// <returns>The truncated text.</returns>
    public static string? ShortStr(string? text, int maxLength) => (!string.IsNullOrEmpty(text)
        && text.Length > maxLength) ? string.Concat(text.AsSpan(0, maxLength), "...") : text;

    ///<summary> 
    //Returns a local file name for a given file name by appending it with JobId. 
    ///</summary> 
    ///<param name="fileName">The file name to append with JobId</param> 
    ///<returns>A local file name appended with JobId</returns> 
    public virtual string GetLocalFileName(string fileName) => $"#{this.JobId}-{fileName}";
}
