EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbActions_Del_Demo]
    @Data int
AS
DELETE FROM WJbActions
WHERE (ActionId = @Data)
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbActions_Get_Demo]
    @Data int
AS
SELECT ActionId, ActionName, ActionType, ActionMore
FROM WJbActions 
WHERE ActionId = @Data
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbActions_Grd_Demo]
AS
SELECT ActionId, ActionName, ActionType, ActionMore
FROM WJbActions
WHERE (Disabled = 0)
FOR JSON PATH
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbActions_Ins_Demo]
	@Data nvarchar(max) 
AS
INSERT INTO WJbActions (ActionName, ActionType, ActionMore)
SELECT ActionName, ActionType, ActionMore 
FROM OPENJSON(@Data) 
WITH (ActionName nvarchar(100), ActionType nvarchar(255), ActionMore nvarchar(max))

DECLARE @ActionId int = SCOPE_IDENTITY()

EXEC WJbActions_Get_Demo @ActionId
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbActions_Lst_Demo]
AS
SELECT ActionId, ActionName
FROM WJbActions
WHERE (Disabled = 0)
FOR JSON PATH
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbActions_Upd_Demo]
	@Data nvarchar(max) 
AS
DECLARE @ActionId int = JSON_VALUE(@Data,''$.ActionId'');

UPDATE R
SET ActionName = D.ActionName, ActionType = D.ActionType, ActionMore = D.ActionMore
FROM WJbActions R
CROSS JOIN (SELECT * FROM OPENJSON(@Data) 
    WITH (ActionName nvarchar(100), ActionType nvarchar(255), ActionMore nvarchar(max))) D
WHERE R.ActionId = @ActionId

EXEC WJbActions_Get_Demo @ActionId
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbHistory_Get_Demo]
    @Data int
AS
SELECT H.*, R.RuleName 
FROM WJbHistory H
INNER JOIN WJbRules AS R ON H.RuleId = R.RuleId
WHERE H.JobId = @Data
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbHistory_Grd_Demo]
    @Data varchar(10)
AS
DECLARE @Date datetime = CAST(@Data AS date)

SELECT H.*, R.RuleName
FROM (
    SELECT JobId, JobPriority, Created, RuleId, Started, Finished, LEFT(JobMore, 200) JobMore, JobStatus
    FROM WJbHistory
    WHERE Created >= @Date AND Created < DATEADD(DAY, 1, @Date)
    UNION ALL 
    SELECT JobId, JobPriority, Created, RuleId, Started, Finished, LEFT(JobMore, 200) JobMore, JobStatus
    FROM WJbQueue
    --WHERE Created >= @Date AND Created < DATEADD(DAY, 1, @Date)
    ) AS H 
INNER JOIN WJbRules AS R ON H.RuleId = R.RuleId
ORDER BY H.JobId ASC
FOR JSON PATH
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbRules_Del_Demo]
    @Data int
AS
DELETE WJbRules
WHERE (RuleId = @Data)
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbRules_Get_Demo]
    @Data int
AS
SELECT R.*, A.ActionName, A.ActionMore
FROM WJbRules R
INNER JOIN WJbActions A ON R.ActionId = A.ActionId
WHERE (RuleId = @Data)
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbRules_Grd_Demo]
AS
SELECT R.*, A.ActionName
FROM WJbRules R
INNER JOIN WJbActions A ON R.ActionId = A.ActionId
FOR JSON PATH
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbRules_Ins_Demo]
	@Data nvarchar(max) 
AS
INSERT INTO WJbRules (RuleName, RulePriority, ActionId, RuleMore, Disabled)
SELECT RuleName, RulePriority, ActionId, RuleMore, ISNULL(Disabled, 0) Disabled
FROM OPENJSON(@Data) 
WITH (RuleName nvarchar(100), RulePriority tinyint, ActionId int, RuleMore nvarchar(max), Disabled bit)

DECLARE @RuleId int = SCOPE_IDENTITY()

EXEC WJbRules_Get_Demo @RuleId
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[WJbRules_Upd_Demo]
	@Data nvarchar(max) 
AS
DECLARE @RuleId int = JSON_VALUE(@Data,''$.RuleId'');

UPDATE R
SET RuleName = D.RuleName
    ,RulePriority = D.RulePriority
    ,ActionId = D.ActionId
    ,RuleMore = D.RuleMore
    ,Disabled = D.Disabled
FROM WJbRules R
CROSS JOIN (SELECT * FROM OPENJSON(@Data) 
    WITH (RuleName nvarchar(100), RulePriority tinyint, ActionId int, RuleMore nvarchar(max), Disabled bit)) D
WHERE R.RuleId = @RuleId

EXEC WJbRules_Get_Demo @RuleId
';
