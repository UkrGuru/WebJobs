EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbRules_Test]
	@Data int 
AS
INSERT INTO WJbQueue (RuleId) VALUES (@Data)

SELECT CAST(SCOPE_IDENTITY() as varchar)
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbQueue_Start]
    @Data int
AS
UPDATE WJbQueue
SET Started = GETDATE()
WHERE JobId = @Data

EXEC WJbQueue_Get @Data
';