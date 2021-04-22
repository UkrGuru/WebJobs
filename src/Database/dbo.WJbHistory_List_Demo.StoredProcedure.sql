SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE   PROCEDURE [dbo].[WJbHistory_List_Demo]
    @Data varchar(100)
AS
DECLARE @Date datetime = CAST(JSON_VALUE(@Data, '$.Date') AS date)

SELECT H.*, R.Name RuleName
FROM (
    SELECT Id, Priority, Created, RuleId, Started, Finished, LEFT(MoreJson, 200) MoreJson
    FROM WJbHistory
    WHERE Created >= @Date AND Created < DATEADD(DAY, 1, @Date)
    UNION ALL 
    SELECT Id, Priority, Created, RuleId, Started, Finished, LEFT(MoreJson, 200) MoreJson
    FROM WJbQueue
    --WHERE Created >= @Date AND Created < DATEADD(DAY, 1, @Date)
    ) AS H 
INNER JOIN WJbRules AS R ON H.RuleId = R.Id
ORDER BY H.Id ASC
FOR JSON PATH

