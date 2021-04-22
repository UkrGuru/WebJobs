SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON


CREATE PROCEDURE [dbo].[WJbQueue_Item]
	@Data varchar(10) 
AS
SELECT TOP (1) Q.*, R.MoreJson RuleMoreJson, A.Name ActionName, A.Type ActionType, A.MoreJson ActionMoreJson
FROM WJbQueue Q
INNER JOIN WJbRules R ON Q.RuleId = R.Id 
INNER JOIN WJbActions A ON R.ActionId = A.Id
WHERE Q.Id = CAST(@Data as int)
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER

