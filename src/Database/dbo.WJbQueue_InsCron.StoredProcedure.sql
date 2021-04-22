SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON


CREATE PROCEDURE [dbo].[WJbQueue_InsCron]
AS
INSERT INTO WJbQueue (RuleId, Priority)
SELECT R.Id, R.Priority
FROM WJbRules R
WHERE R.Disabled = 0 
AND NOT JSON_VALUE(MoreJson, '$.cron') IS NULL
AND NOT EXISTS (SELECT 1 FROM WJbQueue WHERE RuleId = R.Id)
AND dbo.CronValidate(JSON_VALUE(MoreJson, '$.cron'), GETDATE()) = 1

