SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE PROCEDURE [dbo].[WJbQueue_FinishAll] 
AS
;WITH cte AS (
SELECT Q.Id, Q.[Priority], Q.Created, Q.RuleId, Q.Started, GETDATE() AS Finished, Q.MoreJson 
    FROM WJbQueue Q
    WHERE Q.Started IS NOT NULL)
DELETE cte 
OUTPUT deleted.* INTO WJbHistory


