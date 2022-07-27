// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text.Json;
using UkrGuru.WebJobs.Data;

namespace UkrGuru.WebJobs.Actions;

public class ParseTextAction : BaseAction
{
    public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var text = More.GetValue("text");
        ArgumentNullException.ThrowIfNull(text);

        var goals = JsonSerializer.Deserialize<ParsingGoal[]>(More.GetValue("goals") ?? "[]");
        ArgumentNullException.ThrowIfNull(goals);

        var result_name = More.GetValue("result_name") ?? "result";
        goals = goals.AppendRootNode(text);

        for(int i = 0; i < goals.Length; i++)
        {
            goals[i].Value = goals.ParseValue(goals[i]);
        }

        More[result_name] = goals.GetResult();

        await LogHelper.LogInformationAsync(nameof(SendEmailAction), new { jobId = JobId, result = ShortStr(More.GetValue(result_name), 200) }, cancellationToken);

        return true;
    }
}
