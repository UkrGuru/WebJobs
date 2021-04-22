SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON


/*
EXEC dbo.WJbSettings_Set N'{"Name" : "version", "Value" : "1.0.0" }'
*/
CREATE PROCEDURE [dbo].[WJbSettings_Set]
	@Data nvarchar(1000)
AS
UPDATE dbo.WJbSettings
SET Value = JSON_VALUE(@Data, '$.Value')
WHERE (Name = JSON_VALUE(@Data, '$.Name'))

IF @@ROWCOUNT = 0 BEGIN
    INSERT INTO dbo.WJbSettings (Name, Value)
    VALUES (JSON_VALUE(@Data, '$.Name'), JSON_VALUE(@Data, '$.Value'))
END

