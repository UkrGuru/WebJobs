SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON


CREATE PROCEDURE [dbo].[WJbQueue_Finish] 
    @Data varchar(10)
AS
;WITH cte AS (
SELECT TOP 1 Q.Id, Q.[Priority], Q.Created, Q.RuleId, Q.Started, GETDATE() AS Finished, Q.MoreJson 
    FROM WJbQueue Q
    WHERE Q.Id = CAST(@Data as int) AND Q.Started IS NOT NULL)
DELETE cte 
OUTPUT deleted.* INTO WJbHistory

