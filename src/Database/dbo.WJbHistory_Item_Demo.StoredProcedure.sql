SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE   PROCEDURE [dbo].[WJbHistory_Item_Demo]
    @Data varchar(10)
AS
SELECT H.*, R.Name RuleName 
FROM WJbHistory H
INNER JOIN WJbRules AS R ON H.RuleId = R.Id
WHERE H.Id = CAST(@Data as int)
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER

