SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON



/*
EXEC WJbSettings_Value '{"Name" : "LmsRicoApi" }'
*/
CREATE PROCEDURE [dbo].[WJbSettings_Get]
	@Data nvarchar(1000)
AS
SELECT TOP 1 [Value]
FROM WJbSettings S
WHERE S.Name = JSON_VALUE(@Data, '$.Name')

